using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;

public class BoardUI : MonoBehaviour
{
    enum Mode
    {
        Neutral,
        UnitSelected,
        AbilitySelected,

        ProcessingCommands
    }

    [Header("DEBUG:")]
    public bool drawCellCoords;


    [Header("STATE:")]
    [SerializeField] private Mode mode;
    [SerializeField] private TurnPlaybackState playbackState;
    [SerializeField] private int currTimeStep;

    Vector2Int mousePos;
    Vector2Int mousePosLastFrame;
    Vector2Int mouseDownPos;
    Vector2Int mouseUpPos;
    bool mouseMoveAcrossCells;
    bool mouseMoved;
    Vector2Int MouseToOffsetPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        plane.Raycast(ray, out float dist);
        Vector3 worldPos = ray.GetPoint(dist);
        Vector2Int offset = Board.WorldToOffset(worldPos);
        return offset;
    }

	GameObject waypointPrefab;
    GameObject pathQuadPrefab;
    Vector2Int[] waypointPositions; // In offset coordinates
    
    GameObject gizmos;
    Vector2Int hoveredCellPos; // In offset coordinates
    bool isPointerInUI;


    public void Init()
    {
        portraitDisplay = transform.Find("portrait").gameObject;
        portrait = portraitDisplay.GetComponentInChildren<Image>();
        unitNameDisplay = portraitDisplay.GetComponentInChildren<TextMeshProUGUI>();
        portraitDisplay.SetActive(false);
        gizmos = new GameObject("Gizmos");

        waypointPrefab = (GameObject)Resources.Load("Prefabs/BoardUI/Waypoint");
        pathQuadPrefab = (GameObject)Resources.Load("Prefabs/BoardUI/PathQuad");


        //... init units:
        unitDisplay = transform.Find("unitDisplay").gameObject;
        unitButtons = unitDisplay.GetComponentsInChildren<UnitButton>();
        foreach (var unitButton in unitButtons)
            unitButton.gameObject.SetActive(false);

        int j = 0;
        for (int i = 0; i < Board.Units.Length && j < unitButtons.Length; i++)
		{
            Unit unit = Board.Units[i];
            if (!(unit is PlayerUnit))
                continue;

            UnitButton unitBtn = unitButtons[j];
            unitBtn.gameObject.SetActive(true);
            unitBtn.Init(this, unit.GetInstanceID(), unit.preset);
            j++;
        }


        //... init abilities:
        abilityDisplay = portraitDisplay.transform.Find("abilities").gameObject;
        abilityButtons = portraitDisplay.GetComponentsInChildren<AbilityButton>();
        foreach (var abilityButton in abilityButtons)
            abilityButton.gameObject.SetActive(false);
        abilityDisplay.SetActive(false);

        Pool.GetPool(hoverCellMarker);
        Pool.GetPool(abilityValidMarker);

        currCommandHistory = new Stack<UnitCommand>();
        commandsToProcess = new Queue<UnitCommand>();
        turnHistory = new Stack<TurnV2>();
        //Application.targetFrameRate = 60;
    }

    void Update()
    {
        mousePos = MouseToOffsetPos();
        mouseMoved = (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f);
        mouseMoveAcrossCells = mousePos != mousePosLastFrame;

		switch (mode)
		{
			case Mode.Neutral:
                HandleNeutralMode();
                break;
			case Mode.UnitSelected:
                HandleUnitSelectedMode();
                break;
			case Mode.AbilitySelected:
                HandleAbilitySelectedMode();
                break;
			case Mode.ProcessingCommands:
                HandleCommandProcessing();
                break;
			default:
				break;
		}

        mousePosLastFrame = mousePos;
    }


    [Header("HOVER CELLS:")]
    public PooledCellVisuals hoverCellMarker;
    public Dictionary<Vector2Int, PooledCellVisuals> coordToCellMarkerLookup = new Dictionary<Vector2Int, PooledCellVisuals>();
    Cell currHoveredEmptyCell;


    [Header("UNITS:")]
    [ReadOnly] public UnitButton[] unitButtons;
    GameObject portraitDisplay;
    Image portrait;
    TextMeshProUGUI unitNameDisplay;
    [ReadOnly] public Unit currHoveredUnit;
    [ReadOnly] public Unit selectedUnit;
    [ReadOnly] public Unit lastSelectedUnit;
    //[ReadOnly] public Unit ;
    bool hoveredWaypointLastFrame;
    List<Vector2Int> validMoveCoords;


    [Header("ABILITIES:")]
    Dictionary<Vector2Int, PooledMonoBehaviour> abilityValidLookup = new Dictionary<Vector2Int, PooledMonoBehaviour>();
    public PooledMonoBehaviour abilityValidMarker;

    Dictionary<Vector2Int, PooledMonoBehaviour> coordToPreviewLookup = new Dictionary<Vector2Int, PooledMonoBehaviour>();
    Vector2Int hoveredValidAbilityCoord;
    bool hoveredValidMoveLastFrame;

    Vector2Int currValidMoveCoord;
    Vector2Int prevValidMoveCoord;
    [ReadOnly] public bool hoveredValidMove;
    [ReadOnly] public AbilityButton[] abilityButtons;
    [ReadOnly] public AbilityV2 hoveredAbility;
    GameObject abilityDisplay;
    GameObject unitDisplay;
    AbilityV2 selectedAbility;
    AbilityButton selectedAbillityButton;
    List<Vector2Int> validAbilityCoords;
    List<GameObject> validMoveMarkers;



    [Header("PATHING:")]
    Dictionary<Vector2Int, PooledMonoBehaviour> coordToPathableCellLookup = new Dictionary<Vector2Int, PooledMonoBehaviour>();
    public PooledMonoBehaviour pathValidMarker;


    //... NEUTRAL:
    void HandleNeutralMode()
    {
        // ^ We're in neutral mode, where no unit is selected

        bool mouseMoved = mousePos != mousePosLastFrame;
        //bool mouseMoved = (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f);

        if (
            !isPointerInUI
            && mouseMoved
            )
        {
            // ^ The mouse has moved
            Unit hoveredUnit = Board.GetUnitAtPos(mousePos);
            if (hoveredUnit)
            {
                if (hoveredUnit != currHoveredUnit)
                {
                    UnhoverUnitFromNeutral();
                    HoverUnitFromNeutral(hoveredUnit);
                }
            }
            else
            {
                //Debug.LogWarning("UNHOVERING IN NEUTRAL");
                UnhoverUnitFromNeutral();
            }


            Cell hoveredCell = Board.TryGetCellAtPos(mousePos);
            if (hoveredCell != null)
            {
                if (hoveredCell != currHoveredEmptyCell)
                {
                    UnhoverEmptyCell();
                    HoverEmptyCell(hoveredCell);
                }
            }
            else
            {
                if (currHoveredEmptyCell != null)
                    UnhoverEmptyCell();
            }

            if (mousePos != mousePosLastFrame)
            {

            }
        }

        if (!isPointerInUI && Input.GetMouseButtonDown(0))
        {
            mouseDownPos = MouseToOffsetPos();
        }

        if (!isPointerInUI && Input.GetMouseButtonUp(0))
        {
            mouseUpPos = MouseToOffsetPos();
            if (mouseUpPos == mouseDownPos)
            {
                // ^ The mouse was pressed and released on the same tile
                Vector2Int offset = MouseToOffsetPos();
                var unit = Board.GetUnitAtPos(offset);
                if (unit)
                {
                    SelectUnit(unit);
                }
            }
        }

        mousePosLastFrame = mousePos;
    }

    PooledCellVisuals GetCellMarker(Vector2Int cellCoord)
	{
        if (coordToCellMarkerLookup.TryGetValue(cellCoord, out var foundCellMarker))
        {
            return foundCellMarker;
        }
        else
		{
            Vector3 cellPos = Board.OffsetToWorld(cellCoord);
            var newCellMarker = hoverCellMarker.GetAndPlay(cellPos, Quaternion.identity, play: false);
			newCellMarker.OnReturnedToPool += () =>
			{
				coordToCellMarkerLookup.Remove(cellCoord);
			};
			coordToCellMarkerLookup.Add(cellCoord, newCellMarker);
            return newCellMarker;
        }
    }

    void HoverEmptyCell(Cell cell)
    {
        currHoveredEmptyCell = cell;

        Vector2Int offsetCellCoord = Board.WorldToOffset(cell.transform.position);
        var cellMarker = GetCellMarker(offsetCellCoord);
        cellMarker.Hover();

        //if (coordToCellMarkerLookup.TryGetValue(offsetCellCoord, out var foundCellMarker))
        //{
        //    foundCellMarker.Hover();
        //}
        //else
        //{
        //    var newCellMarker = hoverCellMarker.GetAndPlay(cell.transform.position, Quaternion.identity, play: false);
        //    newCellMarker.Hover();
        //    newCellMarker.OnReturnedToPool += () =>
        //    {
        //        coordToCellMarkerLookup.Remove(offsetCellCoord);
        //    };
        //    coordToCellMarkerLookup.Add(offsetCellCoord, newCellMarker);
        //}
    }

    void UnhoverEmptyCell()
    {
        if (currHoveredEmptyCell != null)
        {
            Vector2Int cellCoord = Board.WorldToOffset(currHoveredEmptyCell.transform.position);
            var cellMarker = GetCellMarker(cellCoord);
            cellMarker.Unhover();

            //if (coordToCellMarkerLookup.TryGetValue(cellCoord, out var foundCellMarker))
                //foundCellMarker.Unhover();
        }

        currHoveredEmptyCell = null;
    }

    List<Vector2Int> currPath;

    public int lookupOffset;


    //... UNIT:
    void HandleUnitSelectedMode()
    {
        // ^ We're in the mode where a unit is selected

        bool hoveredWaypoint = false;
        bool mouseMoved = (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f);

        Unit hoveredUnit = null;
        if (!isPointerInUI && mouseMoved)
        {
            hoveredUnit = Board.GetUnitAtPos(mousePos);
        }

        if (!isPointerInUI && mouseMoveAcrossCells)
        {
            //Unit hoveredUnit = Board.GetUnitAtPos(mousePos);
            if (hoveredUnit)
            {
                if (hoveredUnit != currHoveredUnit)
                {
                    UnhoverUnitFromSelected();
                    HoverUnitFromSelected(hoveredUnit);
                }
            }
            else
            {
                UnhoverUnitFromSelected();
            }

            if (mousePos != mousePosLastFrame)
            {
                Cell hoveredCell = Board.TryGetCellAtPos(mousePos);
                if (hoveredCell != null)
                {
                    if (hoveredCell != currHoveredEmptyCell)
                    {
                        UnhoverEmptyCell();
                        HoverEmptyCell(hoveredCell);
                    }
                }
                else
                {
                    if (currHoveredEmptyCell != null)
                        UnhoverEmptyCell();
                }
            }
        }

		if (!isPointerInUI)
		{
            if (!validMoveCoords.IsNullOrEmpty())
            {
                foreach (Vector2Int coord in validMoveCoords)
                {
                    if (coord == mousePos)
                    {
                        hoveredWaypoint = true;
                        break;
                    }
                }
            }
        }

        if(currPath != null)
            currPath.Clear();

        if (!isPointerInUI && mouseMoveAcrossCells)
        {
            if (hoveredWaypoint && mousePosLastFrame != mousePos)
            {
                // ^ The mouse is hovering over a waypoint, and it is not the
                // current path's destination. The second check is done to avoid
                // running the A* algorithm and regenerating pathing markers
                // when the mouse is moved within the same cell.
                if (selectedUnit.MovementAbility != null)
				{
                    hoveredCellPos = mousePos;
                    DestroyPathGizmos(selectedUnit);

                    selectedUnit.MovementAbility.GetAffectedCells(
                        selectedUnit.OffsetPos, 
                        hoveredCellPos, 
                        selectedUnit
                        );
                }
            }

            if (!hoveredWaypoint && hoveredWaypointLastFrame)
            {
                DestroyPathGizmos(selectedUnit);
            }
        }

        if (!isPointerInUI && Input.GetMouseButtonDown(0))
            mouseDownPos = MouseToOffsetPos();

        if (!isPointerInUI && Input.GetMouseButtonUp(0))
            mouseUpPos = MouseToOffsetPos();

        //... handle valid move:
        if (hoveredWaypoint && !isPointerInUI && Input.GetMouseButtonUp(0))
        {
            Debug.Log("CLICKED A VALID MOVE)");
            var fetchedCommands = selectedUnit.MovementAbility.FetchCommandChain(mouseUpPos, selectedUnit);
            StartProcessingCommands(fetchedCommands);

            mousePosLastFrame = mousePos;
            hoveredWaypointLastFrame = hoveredWaypoint;

            return;
        }

        if (!isPointerInUI && Input.GetMouseButtonUp(0))
        {
            if (mouseUpPos == mouseDownPos)
            {
                // ^ The mouse was pressed and released on the same tile
                var unit = Board.GetUnitAtPos(mouseUpPos);
                if (unit)
                {
                    if (unit != selectedUnit)
                    {
                        // ^ A unit was clicked, and it is not already selected
                        DeselectUnit();
                        SelectUnit(unit);
                    }
                    else
                    {
                        DeselectUnit();
                        HoverUnitFromNeutral(unit);

                    }
                }
                else if (unit == null)
                {
                    // ^ A cell without a unit on it was clicked
                    DeselectUnit();
                }
            }
        }

        mousePosLastFrame = mousePos;
        hoveredWaypointLastFrame = hoveredWaypoint;
    }

    void HoverUnitFromNeutral(Unit unit)
    {
        currHoveredUnit = unit;

        ShowUnitPortrait(unit);

        unitNameDisplay.SetText(unit.name.ToUpper());

		abilityDisplay.SetActive(true);
        for (int i = 0, j = 0; i < unit.Abilities.Count && j < 4; i++, j++)
        {
            abilityButtons[i].gameObject.SetActive(true);
            abilityButtons[i].Init(this, unit.Abilities[i]);
        }

        Vector2Int unitCoord = Board.WorldToOffset(unit.transform.position);
        var cellMarker = GetCellMarker(unitCoord);
        cellMarker.Hover();
        cellMarker.Clickable();

		//if (coordToCellMarkerLookup.TryGetValue(unit.OffsetPos, out var foundCellMarker))
		//{
		//	foundCellMarker.Clickable();
		//}
		//else
		//{
		//	var newCellMarker = hoverCellMarker.GetAndPlay(unit.transform.position, Quaternion.identity, playParams: (int)CellStateV2.HOVERED);
		//	newCellMarker.Hover();
  //          newCellMarker.Clickable();
		//	newCellMarker.OnReturnedToPool += () =>
		//	{
		//		coordToCellMarkerLookup.Remove(unit.OffsetPos);
		//	};
		//	coordToCellMarkerLookup.Add(unit.OffsetPos, newCellMarker);
		//}

        if (currHoveredUnit is PlayerUnit && currHoveredUnit.MovementAbility != null)
        {
            ShowValidMoves(currHoveredUnit);
        }
    }

    void UnhoverUnitFromNeutral()
    {
        if (currHoveredUnit == null)
            return;

		Vector2Int offsetCellCoord = Board.WorldToOffset(currHoveredUnit.transform.position);
        var cellMarker = GetCellMarker(offsetCellCoord);
        cellMarker.Unhover();
        cellMarker.Unclickable();

		//if (coordToCellMarkerLookup.TryGetValue(offsetCellCoord, out var foundCellMarker))
		//{
  //          foundCellMarker.Unhover();
  //          foundCellMarker.Unclickable();
		//}

		currHoveredUnit = null;

        HideUnitPortrait();
		//portraitDisplay.SetActive(false);

        HideAbilities();

		//foreach (var abilityButton in abilityButtons)
		//	abilityButton.gameObject.SetActive(false);

		//abilityDisplay.SetActive(false);

        HideValidMoves();
	}

    void HoverUnitFromSelected(Unit unit)
	{
        currHoveredUnit = unit;

        var cellMarker = GetCellMarker(Board.WorldToOffset(unit.transform.position));
        cellMarker.Hover();
        //... ^^ needed..?
        cellMarker.Clickable();

        //if (coordToCellMarkerLookup.TryGetValue(unit.OffsetPos, out var foundCellMarker))
        //{
        //    foundCellMarker.Clickable();
        //}
        //else
        //{
        //    var newCellMarker = hoverCellMarker.GetAndPlay(unit.transform.position, Quaternion.identity, playParams: (int)CellStateV2.HOVERED);
        //    newCellMarker.Hover();
        //    newCellMarker.Clickable();
        //    newCellMarker.OnReturnedToPool += () =>
        //    {
        //        coordToCellMarkerLookup.Remove(unit.OffsetPos);
        //    };
        //    coordToCellMarkerLookup.Add(unit.OffsetPos, newCellMarker);
        //}
    }

    void UnhoverUnitFromSelected()
	{
        //Debug.LogWarning("UNHOVERING UNIT FROM SELECTED");

        if (currHoveredUnit == null)
            return;

        Vector2Int offsetCellCoord = Board.WorldToOffset(currHoveredUnit.transform.position);
        var cellMarker = GetCellMarker(offsetCellCoord);
        cellMarker.Unhover();
        cellMarker.Unclickable();
        
        //if (coordToCellMarkerLookup.TryGetValue(offsetCellCoord, out var foundCellMarker))
        //{
        //    foundCellMarker.Unhover();
        //    foundCellMarker.Unclickable();
        //}

        currHoveredUnit = null;
    }

    void SelectUnit(Unit unit)
    {
        //Debug.LogWarning("Selecting unit.");
  //      if(selectedUnit != unit)
		//{

		//}

        // ^ A unit was clicked
        mode = Mode.UnitSelected;
        selectedUnit = unit;

        Debug.LogWarning("... selected unit: " + selectedUnit.OffsetPos);

        ShowUnitPortrait(unit);

        ShowAbilities(unit);

        if (selectedUnit is PlayerUnit && selectedUnit.MovementAbility != null)
		{
            HideValidMoves();
			ShowValidMoves(unit);
		}

        var cellMarker = GetCellMarker(Board.WorldToOffset(unit.transform.position));
        cellMarker.Select();

		//if (coordToCellMarkerLookup.TryGetValue(unit.OffsetPos, out var foundCellMarker))
  //      {
  //          foundCellMarker.Select();
  //      }
    }

    void DeselectUnit()
    {
        mode = Mode.Neutral;

        DestroyPathGizmos(selectedUnit);
        HideUnitPortrait();

        HideValidMoves();
        validMoveCoords.Clear();
        //^^ sometimes you've deselected, but are still hovering the unit.!

        UnhoverAbility();

        Vector2Int selectedUnitCoord = Board.WorldToOffset(selectedUnit.transform.position);
        var cellMarker = GetCellMarker(selectedUnitCoord);
        cellMarker.Deselect();
        if(mousePos != selectedUnitCoord)
		{
            cellMarker.Unhover();
            cellMarker.Unclickable();
        }

   //     if (coordToCellMarkerLookup.TryGetValue(selectedUnit.OffsetPos, out var foundCellMarker))
   //     {
   //         foundCellMarker.Deselect();

   //         if(mousePos != selectedUnit.OffsetPos)
			//{
   //             foundCellMarker.Unhover();
   //             foundCellMarker.Unclickable();
			//}
   //     }

        lastSelectedUnit = selectedUnit;
        selectedUnit = null;

        foreach (var button in abilityButtons)
            button.Hide();
    }


    //... PATHING:
    void ShowValidMoves(Unit unit)
    {
        if(unit.MovementAbility != null)
		{
            validMoveCoords = unit.MovementAbility.GetValidCoords(unit.OffsetPos, unit);
        }

        //Debug.LogWarning($"Showing {validMoveCoords.Count} moves.");

        foreach (var coord in validMoveCoords)
        {
            if (coordToPathableCellLookup.TryGetValue(coord, out var pathMarker))
            {
                pathMarker.Play();
                //pooledCellVisuals.Play((int)CellStateV2.PATHHINTED);
            }
            else
            {
                var newPathMarker = pathValidMarker.GetAndPlay(Board.OffsetToWorld(coord), Quaternion.identity);
                newPathMarker.Play();
                //newPathMarker.Play((int)CellStateV2.PATHHINTED);
                newPathMarker.OnReturnedToPool += () =>
                {
                    coordToPathableCellLookup.Remove(coord);
                };
                coordToPathableCellLookup.Add(coord, newPathMarker);
            }
        }

        //var prefab = Resources.Load("Prefabs/BoardUI/Waypoint");
        //waypointPositions = Board.GetNavigableTiles(unit);

        //foreach (Vector2Int pos in waypointPositions)
        //{
        //    GameObject waypt = (GameObject)Instantiate(waypointPrefab, gizmos.transform);
        //    waypt.name = "Waypoint";
        //    waypt.transform.position = Board.OffsetToWorld(pos) + new Vector3(0, 0.1f, 0);
        //    waypt.transform.localScale = Vector3.one * 0.5f;
            //waypt.transform.DOScale(0.3f, 0.1f);
        //}
    }

    void HideValidMoves()
    {
        if (validMoveCoords.IsNullOrEmpty())
            return;

        foreach(var coord in validMoveCoords)
		{
            if(coordToPathableCellLookup.TryGetValue(coord, out var foundPathMarker))
			{
                foundPathMarker.Stop();
                //foundPathMarker.Stop((int)CellStateV2.PATHHINTED);
            }
		}
    }

    void DestroyPathGizmos(Unit unit)
    {
        if(unit.MovementAbility != null)
		{
            unit.MovementAbility.HidePreview();
		}

        //string pathName = $"Path{unit.GetInstanceID()}";
        //foreach (Transform child in gizmos.transform)
        //{
        //    if (child.name == pathName)
        //    {
        //        Destroy(child.gameObject);
        //    }
        //}
    }

    Unit GetUnitByGuid(int unitGuid)
    {
        foreach (Unit unit in Board.Units)
        {
            if (unit.GetInstanceID() == unitGuid)
            {
                return unit;
            }
        }

        throw new System.ArgumentException("Unit with given guid not found");
    }

    void ShowUnitPortrait(Unit unit)
	{
        portraitDisplay.SetActive(true);
        portrait.sprite = unit.preset.icon;
        unitNameDisplay.SetText(unit.name.ToUpper());
    }

    void HideUnitPortrait()
	{
        portraitDisplay.SetActive(false);
    }

    public void OnPointerEnterUnitButton(int unitGuid, UnitButton button)
    {
        isPointerInUI = true;

        Unit unit = GetUnitByGuid(unitGuid);
        if (unit == null)
            return;

  //      if (mode == Mode.Neutral)
		//{
  //          portraitDisplay.SetActive(true);
  //          portrait.sprite = unit.preset.icon;
  //      }

        if (mode == Mode.Neutral)
        {
            HoverUnitFromNeutral(unit);
            //button.background.color = button.hoverColor;
        }
        else if(mode == Mode.UnitSelected)
		{
            HoverUnitFromSelected(unit);

   //         if (selectedUnit != null && selectedUnit.gameObject.GetInstanceID() == unitGuid)
   //         {
                
   //         }
			//else
			//{
   //             button.background.color = button.hoverColor;
			//}
        }
    }
    
    public void OnPointerExitUnitButton(int unitGuid, UnitButton button)
    {
        isPointerInUI = false;
        if (mode == Mode.Neutral)
            HideUnitPortrait();

        if (mode == Mode.Neutral)
        {
            UnhoverUnitFromNeutral();
            //button.background.color = Color.white.With(a : 0f);
        }
        else if (mode == Mode.UnitSelected)
        {
            UnhoverUnitFromSelected();
            //button.background.color = Color.white.With(a: 0f);
        }
    }

    public void OnPointerClickUnitButton(int unitGuid, UnitButton button)
    {
        if (mode == Mode.Neutral)
        {
            Unit unit = GetUnitByGuid(unitGuid);
            SelectUnit(unit);
        }
        else if (mode == Mode.UnitSelected)
        {
            DeselectUnit();
            Unit unit = GetUnitByGuid(unitGuid);
            SelectUnit(unit);
        }

  //      foreach(var unitButton in unitButtons)
		//{
  //          unitButton.background.color = Color.white.With(a: 0f);
		//}

  //      button.background.color = button.selectedColor;
    }


    //... ABILITY:
    void HandleAbilitySelectedMode()
    {
        Vector2Int mousePos = MouseToOffsetPos();
        bool mouseMoved = (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f);

        if (mousePos == selectedUnit.OffsetPos && Input.GetMouseButtonDown(0))
        {
            DeselectAbility();
            return;
        }

        hoveredValidMove = false;
        foreach (var coord in validAbilityCoords)
        {
            if (coord == mousePos)
            {
                hoveredValidMove = true;
                break;
            }
        }

        if (hoveredValidMove)
        {
            if (prevValidMoveCoord != mousePos)
            {
                selectedAbility.HidePreview();
				UnhoverValidAbilityMove();
			}
            else
            {
                //Debug.LogWarning("")
            }

            currValidMoveCoord = mousePos;

            if (prevValidMoveCoord != currValidMoveCoord || !hoveredValidMoveLastFrame)
            {
                Debug.LogWarning("hovering valid move at : " + currValidMoveCoord.ToString());
                selectedAbility.ShowPreview(currValidMoveCoord, selectedUnit);
				HoverValidAbilityMove(selectedAbility, currValidMoveCoord);
			}
        }
        else
        {
            if (hoveredValidMoveLastFrame)
                selectedAbility.HidePreview();
			UnhoverValidAbilityMove();
		}

        if (!isPointerInUI && Input.GetMouseButtonDown(0))
            mouseDownPos = MouseToOffsetPos();

        if (!isPointerInUI && Input.GetMouseButtonUp(0))
            mouseUpPos = MouseToOffsetPos();

        foreach (var coord in validAbilityCoords)
        {
            if (coord == mouseUpPos)
            {
                DeselectAbility();
                var fetchedCommands = selectedAbility.FetchCommandChain(currValidMoveCoord, selectedUnit);
                StartProcessingCommands(fetchedCommands);

                //if (!commandsToProcess.IsNullOrEmpty())
                //{
                //    currCommand = commandsToProcess.Dequeue();
                //    currCommand.OnBeginTick();
                //    currCommandHistory = new Stack<UnitCommand>();
                //}
                
                ////... clicked a valid move, time to process it.
                //mode = Mode.ProcessingCommands;
                //playbackState = TurnPlaybackState.PLAYING;

                //Debug.LogWarning("GOT A NEW COMMAND CHAIN!");

                return;
            }
        }

        if (
            !hoveredValidMove
            && !isPointerInUI
            && Input.GetMouseButtonUp(0)
            )
        {
            if (mouseUpPos == mouseDownPos)
            {
                // ^ The mouse was pressed and released on the same tile
                var unit = Board.GetUnitAtPos(mouseUpPos);
                if (unit)
                {
                    if (unit != selectedUnit)
                    {
                        // ^ A unit was clicked, and it is not already selected
                        DeselectAbility();
                        DeselectUnit();
                        SelectUnit(unit);
                    }
                    else
                    {
                        DeselectUnit();
                        // ^^ maybe weird, sometimes you'd be using ability on selected unit
                        //... guess that's caught above if it's a valid move.
                    }
                }
                else if (unit == null)
                {
                    // ^ A cell without a unit on it was clicked
                    DeselectUnit();
                }
            }
        }

        hoveredValidMoveLastFrame = hoveredValidMove;
        prevValidMoveCoord = currValidMoveCoord;

        mousePosLastFrame = mousePos;
    }

    void StartProcessingCommands(Queue<UnitCommand> commands)
	{
        currInstigator = selectedUnit;
        DeselectUnit();

        commandsToProcess = commands;
        currCommand = commandsToProcess.Dequeue();
        currCommand.OnBeginTick();
        currCommandHistory = new Stack<UnitCommand>();

        mode = Mode.ProcessingCommands;
        playbackState = TurnPlaybackState.PLAYING;

        Debug.Log("STARTED PROCESSING COMMANDS");
    }

    internal void OnPointerEnteredAbilityButton(AbilityV2 ability, AbilityButton button)
    {
        isPointerInUI = true;

        if (mode != Mode.UnitSelected)
            return;

        HideValidMoves();

        HoverAbility(ability);
    }

    internal void OnPointerExitedAbilityButton(AbilityV2 ability, AbilityButton button)
    {
        isPointerInUI = false;

        if (mode != Mode.UnitSelected)
            return;

        UnhoverAbility();

		if (selectedUnit != null)
			ShowValidMoves(selectedUnit);
	}

    internal void OnPointerClickedAbilityButton(AbilityV2 ability, AbilityButton button)
    {
        switch (mode)
        {
            case Mode.Neutral:
                break;
            case Mode.UnitSelected:
                SelectAbility(ability, button);
                button.Select();
                break;
            case Mode.AbilitySelected:
                if(ability == selectedAbility)
				{
                    DeselectAbility();
				}
				else
				{
                    DeselectAbility();
                    SelectAbility(ability, button);
				}
                break;
            default:
                break;
        }
    }

    void ShowAbilities(Unit unit)
    {
        abilityDisplay.SetActive(true);
        for (int i = 0, j = 0; i < unit.Abilities.Count && j < 4; i++, j++)
        {
            abilityButtons[i].gameObject.SetActive(true);
            abilityButtons[i].Init(this, unit.Abilities[i]);
        }
    }

    void HideAbilities()
    {
        foreach (var abilityButton in abilityButtons)
            abilityButton.gameObject.SetActive(false);

        abilityDisplay.SetActive(false);
    }

    void HoverAbility(AbilityV2 ability)
    {
        //HideValidMoves();

        validAbilityCoords = ability.GetValidCoords(
            Board.WorldToOffset(selectedUnit.transform.position),
            selectedUnit
            );

        if (validAbilityCoords.IsNullOrEmpty())
            return;

        foreach (var coord in validAbilityCoords)
        {
            if (abilityValidLookup.TryGetValue(coord, out var foundCellMarker))
            {
                foundCellMarker.Play();
            }
            else
            {
                var newAbilityMarker = abilityValidMarker.GetAndPlay(Board.OffsetToWorld(coord), Quaternion.identity);
                newAbilityMarker.OnReturnedToPool += () =>
                {
                    abilityValidLookup.Remove(coord);
                };
                abilityValidLookup.Add(coord, newAbilityMarker);
            }
        }
    }

    void UnhoverAbility()
    {
        //if (selectedUnit != null)
        //    ShowValidMoves(selectedUnit);

        foreach (var kvp in abilityValidLookup)
        {
            var abilityMarker = kvp.Value;
            abilityMarker.Stop();
        }
    }

    void HoverValidAbilityMove(AbilityV2 ability, Vector2Int hoveredCoord)
    {
        Debug.LogWarning("hovered a valid ability move");

        //      var affectedCoords = selectedAbility.GetAffectedCells(
        //          selectedUnit.OffsetPos, 
        //          currValidMoveCoord, 
        //          selectedUnit
        //          );

        //      foreach(var affectedCoord in affectedCoords)
        //{
        //          if(coordToPreviewLookup.TryGetValue(affectedCoord, out var previewMarker))
        //	{
        //              previewMarker.Play();
        //	}
        //	else
        //	{
        //              var newPreviewMarker = selectedAbility.PreviewAffectedCell(selectedUnit.OffsetPos, affectedCoord);
        //              if (newPreviewMarker == null)
        //                  continue;

        //              newPreviewMarker.OnReturnedToPool += () =>
        //              {
        //                  coordToPreviewLookup.Remove(affectedCoord);
        //              };

        //              coordToPreviewLookup.Add(affectedCoord, newPreviewMarker);
        //	}
        //}

        var cellMarker = GetCellMarker(hoveredCoord);
        cellMarker.Hover();
        cellMarker.Clickable();

  //      if(coordToCellMarkerLookup.TryGetValue(hoveredCoord, out var foundCellMarker))
		//{
  //          foundCellMarker.Hover();
  //          foundCellMarker.Clickable();
		//}
		//else
		//{
  //          var newCellMarker = hoverCellMarker.GetAndPlay(Board.OffsetToWorld(hoveredCoord), Quaternion.identity);
  //          newCellMarker.Hover();
  //          newCellMarker.Clickable();

  //          newCellMarker.OnReturnedToPool += () =>
  //          {
  //              coordToCellMarkerLookup.Remove(hoveredCoord);
  //          };

  //          coordToCellMarkerLookup.Add(hoveredCoord, newCellMarker);
		//}
    }

    void UnhoverValidAbilityMove()
    {
        if(coordToCellMarkerLookup.TryGetValue(currValidMoveCoord, out var foundCellMarker))
		{
            foundCellMarker.Unhover();
            foundCellMarker.Unclickable();
		}

		//foreach (var kvp in coordToPreviewLookup)
  //      {
  //          var abilityMarker = kvp.Value;
  //          abilityMarker.Stop();
  //      }
    }

    void HighlightValidAbilityCells(List<Vector2Int> coords)
	{

	}

    void UnhighlightValidAbilityCells()
	{

	}

	void SelectAbility(AbilityV2 ability, AbilityButton button)
	{
        //Debug.LogWarning("SELECTED ABILITY:");

		mode = Mode.AbilitySelected;
        selectedAbility = ability;
        selectedAbillityButton = button;
        selectedAbillityButton.Select();
	}

	void DeselectAbility()
	{
        //Debug.LogWarning("...DESELECTED ABILITY:");
        if (selectedAbility != null)
            selectedAbility.HidePreview();
        UnhoverAbility();
        SelectUnit(selectedUnit);
        selectedAbillityButton.Deselect();
        selectedAbillityButton = null;
	}


    //... TURNS:
    //[Header("TURNS:")]
    Unit currInstigator;
    UnitCommand currCommand;
    Queue<UnitCommand> commandsToProcess;
    public Stack<UnitCommand> currCommandHistory;
    public Stack<TurnV2> turnHistory;

	private void HandleCommandProcessing()
    {
		switch (playbackState)
		{
			case TurnPlaybackState.PAUSED:
				break;
			case TurnPlaybackState.PLAYING:
                HandleTurnForward();
                if (currCommand == null)
				{
                    Debug.LogWarning("DONE WITH TURN, BACK TO FLOW");
                    playbackState = TurnPlaybackState.PAUSED;
					//mode = Mode.UnitSelected;
					SelectUnit(lastSelectedUnit);
					currInstigator = null;
				}
				break;
			case TurnPlaybackState.REWINDING:
                HandleTurnBackward();
                break;
			default:
				break;
		}
	}

    private void HandleTurnForward()
    {
        if (currCommand == null)
            return;

		if (currCommand.Tick())
		{
            currCommand.OnCompleteTick();
            currCommand.Execute();
            currCommandHistory.Push(currCommand);
            if (currCommand.StepsTimeForward())
                currTimeStep++;
            currCommand = null;

			if (!commandsToProcess.IsNullOrEmpty())
			{
                currCommand = commandsToProcess.Dequeue();
                currCommand.OnBeginTick();
			}
			else
			{
                playbackState = TurnPlaybackState.PAUSED;

                TurnV2 recordedTurn = new();
                recordedTurn.instigator = currInstigator;
                recordedTurn.commandHistory = currCommandHistory;
                turnHistory.Push(recordedTurn);

				currCommandHistory = null;
			}
		}
    }

    public void Undo()
    {
        if (mode != Mode.UnitSelected)
            return;

        mode = Mode.ProcessingCommands;
        playbackState = TurnPlaybackState.REWINDING;

        TurnV2 turnToUndo = turnHistory.Pop();
        currCommandHistory = turnToUndo.commandHistory;
        currCommand = currCommandHistory.Pop();
    }

    private void HandleTurnBackward()
	{
        if (currCommand == null)
            return;

		if (currCommand.Tick(-1f))
		{
            currCommand.OnCompleteReverseTick();
            currCommand.Undo();
            currCommand = null;

			if (!currCommandHistory.IsNullOrEmpty())
			{
                currCommand = currCommandHistory.Pop();
                currCommand.OnBeginReverseTick();
			}
            else
			{
                playbackState = TurnPlaybackState.PAUSED;
                currCommandHistory = null;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (drawCellCoords)
		{
            //foreach()
		}
	}
}
