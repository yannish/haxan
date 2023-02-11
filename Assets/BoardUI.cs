using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BoardUI : MonoBehaviour
{
    enum Mode
    {
        Neutral,
        UnitSelected,
        AbilitySelected,

        ProcessingCommands
    }

    [ReadOnly, SerializeField] private Mode mode;


    Vector2Int mouseDownPos;
    Vector2Int mouseUpPos;
    GameObject gizmos;

    GameObject waypointPrefab;
    Vector2Int[] waypointPositions; // In offset coordinates
    
    Vector2Int hoveredCellPos; // In offset coordinates
    bool isPointerInUI;

    public void Init()
    {
        portrait = transform.Find("portrait").gameObject;
        unitNameDisplay = portrait.GetComponentInChildren<TextMeshProUGUI>();
        portrait.SetActive(false);
        gizmos = new GameObject("Gizmos");

        waypointPrefab = (GameObject)Resources.Load("Prefabs/BoardUI/Waypoint");


        // Generate one button per unit
        var prefab = Resources.Load("Prefabs/UnitButton");

        for (int i = 0; i < Board.Units.Length; i++)
        {
            Unit unit = Board.Units[i];
            if (!(unit is PlayerUnit))
                continue;

            GameObject go = (GameObject)Instantiate(prefab, transform);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(0f, 110f * i);
            UnitButton btn = go.GetComponent<UnitButton>();
            btn.Init(this, unit.GetInstanceID());
        }

        abilityDisplay = portrait.transform.Find("abilities").gameObject;
        abilityButtons = portrait.GetComponentsInChildren<AbilityButton>();
        foreach (var abilityButton in abilityButtons)
            abilityButton.gameObject.SetActive(false);
        abilityDisplay.SetActive(false);

        Pool.GetPool(hoverCellMarker);
        Pool.GetPool(abilityValidMarker);

        Application.targetFrameRate = 60;
    }

    void Update()
    {
        mousePos = MouseToOffsetPos();

        if (mode == Mode.Neutral)
        {
            HandleNeutralMode();
        }
        else if (mode == Mode.UnitSelected)
        {
            HandleUnitSelectedMode();
        }
        else if (mode == Mode.AbilitySelected)
        {
            HandleAbilitySelectedMode();
        }
        else if(mode == Mode.ProcessingCommands)
		{
            HandleCommandProcessing();
		}
    }

	private void HandleCommandProcessing()
	{
		
	}

	Vector2Int mousePos;
    Vector2Int MouseToOffsetPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        plane.Raycast(ray, out float dist);
        Vector3 worldPos = ray.GetPoint(dist);
        Vector2Int offset = Board.WorldToOffset(worldPos);
        return offset;
    }

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


            CellV2 hoveredCell = Board.TryGetCellAtPos(mousePos);
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


    Vector2Int mousePosLastFrame;
    bool hoveredWaypointLastFrame;
    void HandleUnitSelectedMode()
    {
        // ^ We're in the mode where a unit is selected

        Vector2Int mousePos = MouseToOffsetPos();
        bool hoveredWaypoint = false;
        bool mouseMoved = (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f);

        //bool hovereNewCell = mousePos != mousePosLastFrame;

        if (
            !isPointerInUI 
            && mouseMoved
            )
        {
			Unit hoveredUnit = Board.GetUnitAtPos(mousePos);
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
                CellV2 hoveredCell = Board.TryGetCellAtPos(mousePos);
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


        if (
            !isPointerInUI
            && mouseMoved
            )
        {
            //bool hoveredWaypoint = false;

            if(waypointPositions != null)
                foreach (Vector2Int pos in waypointPositions)
                {
                    if (pos == mousePos)
                    {
                        hoveredWaypoint = true;
                        break;
                    }
                }

            if (hoveredWaypoint && mousePosLastFrame != mousePos)
            {
                // ^ The mouse is hovering over a waypoint, and it is not the
                // current path's destination. The second check is done to avoid
                // running the A* algorithm and regenerating pathing markers
                // when the mouse is moved within the same cell.
                hoveredCellPos = mousePos;
                DestroyPathGizmos(selectedUnit);

                // Find the shortest path
                Vector2Int[] path = Board.FindPath(selectedUnit.OffsetPos, hoveredCellPos);

                // Create new path gizmos
                var prefab = Resources.Load("Prefabs/PathQuad");
                string pathName = $"Path{selectedUnit.GetInstanceID()}";
                for (int i = 0; i < path.Length; i++)
                {
                    Vector2Int from = (i == 0) ? selectedUnit.OffsetPos : path[i - 1];
                    Vector2Int to = path[i];
                    GameObject pathQuad = (GameObject)Instantiate(prefab, gizmos.transform);
                    pathQuad.name = pathName;
                    pathQuad.transform.position = Board.OffsetToWorld(from);
                    // Rotate the path by locating its index in the neighbor
                    // look-up table
                    float degrees = 0f;
                    {
                        int parity = from.x & 1;
                        Vector2Int delta = to - from;
                        int j;
                        for (j = 0; j < 6; j++)
                        {
                            if (Board.neighborLut[parity, j] == delta)
                            {
                                break;
                            }
                        }
                        degrees = (1 + j) * 60f;
                    }
                    pathQuad.transform.rotation = Quaternion.Euler(0, degrees, 0);
                }
            }

            if (!hoveredWaypoint && hoveredWaypointLastFrame)
            {
                DestroyPathGizmos(selectedUnit);
            }
        }

        if (!isPointerInUI && Input.GetMouseButtonDown(0))
        {
            mouseDownPos = MouseToOffsetPos();
        }

        if (!isPointerInUI && Input.GetMouseButtonUp(0))
        {
            Vector2Int mouseUpPos = MouseToOffsetPos();
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



    [Header("HOVER CELLS:")]
    public PooledCellVisuals hoverCellMarker;
    Dictionary<Vector2Int, PooledCellVisuals> coordToCellMarkerLookup = new Dictionary<Vector2Int, PooledCellVisuals>();
    CellV2 currHoveredEmptyCell;

	//... BARE GRID:
	void HoverEmptyCell(CellV2 cell)
    {
        currHoveredEmptyCell = cell;

        Vector2Int offsetCellCoord = Board.WorldToOffset(cell.transform.position);
        if (coordToCellMarkerLookup.TryGetValue(offsetCellCoord, out var foundCellMarker))
        {
            foundCellMarker.Hover();
        }
        else
        {
            var newCellMarker = hoverCellMarker.GetAndPlay(cell.transform.position, Quaternion.identity, play: false);
            newCellMarker.Hover();
            newCellMarker.OnReturnedToPool += () =>
            {
                coordToCellMarkerLookup.Remove(offsetCellCoord);
            };
            coordToCellMarkerLookup.Add(offsetCellCoord, newCellMarker);
        }
    }

    void UnhoverEmptyCell()
    {
        if (currHoveredEmptyCell != null)
        {
            Vector2Int offsetCellCoord = Board.WorldToOffset(currHoveredEmptyCell.transform.position);
            if (coordToCellMarkerLookup.TryGetValue(offsetCellCoord, out var foundCellMarker))
                foundCellMarker.Unhover();
        }

        currHoveredEmptyCell = null;
    }



    //... UNIT:
    GameObject portrait;
    public TextMeshProUGUI unitNameDisplay;

    //Text
    Unit currHoveredUnit;
    Unit selectedUnit;

    void HoverUnitFromNeutral(Unit unit)
    {
        currHoveredUnit = unit;
        portrait.SetActive(true);
        unitNameDisplay.SetText(unit.name.ToUpper());

		abilityDisplay.SetActive(true);
        for (int i = 0, j = 0; i < unit.Abilities.Count && j < 4; i++, j++)
        {
            abilityButtons[i].gameObject.SetActive(true);
            abilityButtons[i].Init(this, unit.Abilities[i]);
        }

		if (coordToCellMarkerLookup.TryGetValue(unit.OffsetPos, out var foundCellMarker))
		{
			foundCellMarker.Clickable();
		}
		else
		{
			var newCellMarker = hoverCellMarker.GetAndPlay(unit.transform.position, Quaternion.identity, playParams: (int)CellStateV2.HOVERED);
			newCellMarker.Hover();
            newCellMarker.Clickable();
			newCellMarker.OnReturnedToPool += () =>
			{
				coordToCellMarkerLookup.Remove(unit.OffsetPos);
			};
			coordToCellMarkerLookup.Add(unit.OffsetPos, newCellMarker);
		}
	}

    void UnhoverUnitFromNeutral()
    {
        if (currHoveredUnit == null)
            return;

		Vector2Int offsetCellCoord = Board.WorldToOffset(currHoveredUnit.transform.position);
		if (coordToCellMarkerLookup.TryGetValue(offsetCellCoord, out var foundCellMarker))
		{
            foundCellMarker.Unhover();
            foundCellMarker.Unclickable();
		}

		currHoveredUnit = null;

		portrait.SetActive(false);

		foreach (var abilityButton in abilityButtons)
			abilityButton.gameObject.SetActive(false);

		abilityDisplay.SetActive(false);
	}

    void HoverUnitFromSelected(Unit unit)
	{
        if (coordToCellMarkerLookup.TryGetValue(unit.OffsetPos, out var foundCellMarker))
        {
            foundCellMarker.Clickable();
        }
        else
        {
            var newCellMarker = hoverCellMarker.GetAndPlay(unit.transform.position, Quaternion.identity, playParams: (int)CellStateV2.HOVERED);
            newCellMarker.Hover();
            newCellMarker.Clickable();
            newCellMarker.OnReturnedToPool += () =>
            {
                coordToCellMarkerLookup.Remove(unit.OffsetPos);
            };
            coordToCellMarkerLookup.Add(unit.OffsetPos, newCellMarker);
        }
    }

    void UnhoverUnitFromSelected()
	{
        if (currHoveredUnit == null)
            return;

        Vector2Int offsetCellCoord = Board.WorldToOffset(currHoveredUnit.transform.position);
        if (coordToCellMarkerLookup.TryGetValue(offsetCellCoord, out var foundCellMarker))
        {
            foundCellMarker.Unhover();
            foundCellMarker.Unclickable();
        }

        currHoveredUnit = null;
    }

    void SelectUnit(Unit unit)
    {
        // ^ A unit was clicked
        mode = Mode.UnitSelected;
        selectedUnit = unit;
        portrait.SetActive(true);

        if(selectedUnit is PlayerUnit)
            ShowNavigableTiles(unit);

        if (coordToCellMarkerLookup.TryGetValue(unit.OffsetPos, out var foundCellMarker))
        {
            foundCellMarker.Select();
        }
    }

    void DeselectUnit()
    {
        mode = Mode.Neutral;
        DestroyPathGizmos(selectedUnit);
        waypointPositions = null;
        portrait.SetActive(false);

        HideNavigableTiles();

        if (coordToCellMarkerLookup.TryGetValue(selectedUnit.OffsetPos, out var foundCellMarker))
        {
            foundCellMarker.Deselect();

            if(mousePos != selectedUnit.OffsetPos)
			{
                foundCellMarker.Unhover();
                foundCellMarker.Unclickable();
			}
        }

        selectedUnit = null;
    }

    void ShowNavigableTiles(Unit unit)
    {
        //var prefab = Resources.Load("Prefabs/BoardUI/Waypoint");
        waypointPositions = Board.GetNavigableTiles(unit);

        foreach (Vector2Int pos in waypointPositions)
        {
            GameObject waypt = (GameObject)Instantiate(waypointPrefab, gizmos.transform);
            waypt.name = "Waypoint";
            waypt.transform.position = Board.OffsetToWorld(pos) + new Vector3(0, 0.1f, 0);
            waypt.transform.localScale = Vector3.one * 0.5f;
            //waypt.transform.DOScale(0.3f, 0.1f);
        }
    }

    void HideNavigableTiles()
    {
        foreach (Transform child in gizmos.transform)
        {
            if (child.name == "Waypoint")
            {
                Destroy(child.gameObject);
            }
        }
    }

    void DestroyPathGizmos(Unit unit)
    {
        string pathName = $"Path{unit.GetInstanceID()}";
        foreach (Transform child in gizmos.transform)
        {
            if (child.name == pathName)
            {
                Destroy(child.gameObject);
            }
        }
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

    public void OnPointerEnterUnitButton(/*int unitGuid*/)
    {
        isPointerInUI = true;
        if (mode == Mode.Neutral)
            portrait.SetActive(true);
    }

    public void OnPointerExitUnitButton()
    {
        isPointerInUI = false;
        if (mode == Mode.Neutral)
            portrait.SetActive(false);
    }

    public void OnPointerClickUnitButton(int unitGuid)
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
    }


    //... ABILITY:

    [Header("ABILITIES:")]
    [ReadOnly] public AbilityButton[] abilityButtons;
    [ReadOnly] public AbilityV2 hoveredAbility;
    GameObject abilityDisplay;
    AbilityV2 selectedAbility;
    List<Vector2Int> validAbilityCoords;
    List<GameObject> validMoveMarkers;

    internal void OnPointerEnteredAbilityButton(AbilityV2 ability)
    {
        isPointerInUI = true;

        if (mode != Mode.UnitSelected)
            return;

        HoverAbility(ability);
    }

    internal void OnPointerExitedAbilityButton(AbilityV2 ability)
    {
        isPointerInUI = false;

        if (mode != Mode.UnitSelected)
            return;

        UnhoverAbility();
    }

    internal void OnPointerClickedAbilityButton(AbilityV2 ability)
    {
        switch (mode)
        {
            case Mode.Neutral:
                break;
            case Mode.UnitSelected:
                SelectAbility(ability);
                break;
            case Mode.AbilitySelected:
                DeselectAbility();
                SelectAbility(ability);
                break;
            default:
                break;
        }
    }


    Dictionary<Vector2Int, PooledMonoBehaviour> coordToPreviewLookup = new Dictionary<Vector2Int, PooledMonoBehaviour>();
    public PooledMonoBehaviour abilityPreviewMarker;
    Vector2Int hoveredValidAbilityCoord;
    bool hoveredValidMoveLastFrame;

    Dictionary<Vector2Int, PooledMonoBehaviour> abilityValidLookup = new Dictionary<Vector2Int, PooledMonoBehaviour>();
    public PooledMonoBehaviour abilityValidMarker;
    Vector2Int currValidMoveCoord;
    Vector2Int prevValidMoveCoord;
    [ReadOnly] public bool hoveredValidMove;

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
			    HoverValidAbilityMove(currValidMoveCoord);
			}                
		}
		else
		{
            if (hoveredValidMoveLastFrame)
                UnhoverValidAbilityMove();
		}

        if (!isPointerInUI && Input.GetMouseButtonDown(0))
        {
            mouseDownPos = MouseToOffsetPos();
        }

        if (!isPointerInUI && Input.GetMouseButtonDown(0))
        {
            mouseUpPos = MouseToOffsetPos();
        }


        foreach (var coord in validAbilityCoords)
		{
			if (coord == mouseUpPos)
			{
                var newCommandQueue = selectedAbility.FetchCommandChain(currValidMoveCoord, selectedUnit);

			    //... clicked a valid move, time to process it.
			    mode = Mode.ProcessingCommands;
                return;
			}
		}

        if (
            !hoveredValidMove
            && !isPointerInUI 
            && Input.GetMouseButtonUp(0)
            )
        {
            Vector2Int mouseUpPos = MouseToOffsetPos();
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

    void HoverValidAbilityMove(Vector2Int hoveredCoord)
    {
        Debug.LogWarning("hovered a valid ability move");

        var affectedCoords = selectedAbility.GetAffectedCells(selectedUnit.OffsetPos, currValidMoveCoord);

        foreach(var affectedCoord in affectedCoords)
		{
            if(coordToPreviewLookup.TryGetValue(affectedCoord, out var previewMarker))
			{
                previewMarker.Play();
			}
			else
			{
                var newPreviewMarker = selectedAbility.PreviewAffectedCell(selectedUnit.OffsetPos, affectedCoord);
                if (newPreviewMarker == null)
                    continue;

                newPreviewMarker.OnReturnedToPool += () =>
                {
                    coordToPreviewLookup.Remove(affectedCoord);
                };

                coordToPreviewLookup.Add(affectedCoord, newPreviewMarker);
			}
		}

        if(coordToCellMarkerLookup.TryGetValue(hoveredCoord, out var foundCellMarker))
		{
            foundCellMarker.Hover();
            foundCellMarker.Clickable();
		}
		else
		{
            var newCellMarker = hoverCellMarker.GetAndPlay(Board.OffsetToWorld(hoveredCoord), Quaternion.identity);
            newCellMarker.Hover();
            newCellMarker.Clickable();

            newCellMarker.OnReturnedToPool += () =>
            {
                coordToCellMarkerLookup.Remove(hoveredCoord);
            };

            coordToCellMarkerLookup.Add(hoveredCoord, newCellMarker);
		}
    }

    void UnhoverValidAbilityMove()
    {
        if(coordToCellMarkerLookup.TryGetValue(currValidMoveCoord, out var foundCellMarker))
		{
            foundCellMarker.Unhover();
            foundCellMarker.Unclickable();
		}

		foreach (var kvp in coordToPreviewLookup)
        {
            var abilityMarker = kvp.Value;
            abilityMarker.Stop();
        }
    }

    void HoverAbility(AbilityV2 ability)
	{
        HideNavigableTiles();

        validAbilityCoords = ability.GetValidMoves(
            Board.WorldToOffset(selectedUnit.transform.position),
            selectedUnit
            );

        if (validAbilityCoords.IsNullOrEmpty())
            return;

        foreach(var coord in validAbilityCoords)
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
        ShowNavigableTiles(selectedUnit);

        foreach(var kvp in abilityValidLookup)
		{
            var abilityMarker = kvp.Value;
            abilityMarker.Stop();
		}
    }

    void HighlightValidAbilityCells(List<Vector2Int> coords)
	{

	}

    void UnhighlightValidAbilityCells()
	{

	}

	void SelectAbility(AbilityV2 ability)
	{
		mode = Mode.AbilitySelected;
        selectedAbility = ability;
	}

	void DeselectAbility()
	{
        UnhoverAbility();
        SelectUnit(selectedUnit);
	}
}
