using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardUI : MonoBehaviour
{
    enum Mode
    {
        Neutral,
        UnitSelected,
        AbilitySelected
    }


    Mode mode;
    Vector2Int mouseDownPos;
    GameObject gizmos;
    Vector2Int[] waypointPositions; // In offset coordinates
    Vector2Int hoveredCellPos; // In offset coordinates
    bool isPointerInUI;
    public void Init()
    {
        portrait = transform.Find("portrait").gameObject;
        portrait.SetActive(false);
        gizmos = new GameObject("Gizmos");
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
    }


	void Update()
    {
        if (mode == Mode.Neutral)
        {
            HandleNeutralMode();
        }
        else if (mode == Mode.UnitSelected)
        {
            HandleUnitSelectedMode();
        }
    }

    void HandleNeutralMode()
    {
        // ^ We're in neutral mode, where no unit is selected
        if (!isPointerInUI && (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f))
        {
            // ^ The mouse has moved
            Vector2Int offset = MouseToOffsetPos();
            Unit hoveredUnit = Board.GetUnitAtPos(offset);
            if (hoveredUnit)
            {
                if(hoveredUnit != currHoveredUnit)
				{
                    UnhoverUnit();
                    HoverUnit(hoveredUnit);
				}
            }
            else
            {
                UnhoverUnit();
            }

            CellV2 hoveredCell = Board.TryGetCellAtPos(offset);
            if(hoveredCell != null)
			{
                if(hoveredCell != currHoveredCell)
				{
                    UnhoverEmptyCell();
                    HoverEmptyCell(hoveredCell);

                    //int parity = offset.x & 1;
                    //Debug.LogWarning($"offset.x: {offset.x} , parity: {parity}");
                }

                //Debug.LogWarning($"hit a cell at {offset.x}, {offset.y}.");
            }
			else
			{
                if (currHoveredCell != null)
                    UnhoverEmptyCell();

                //Debug.LogWarning($"didn't hit a cell at {offset.x}, {offset.y}.");
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
                Vector2Int offset = MouseToOffsetPos();
                var unit = Board.GetUnitAtPos(offset);
                if (unit)
                {
                    SelectUnit(unit);
                }
            }
        }
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
            && (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
            )
        {
            // ^ The mouse has moved

            //bool hoveredWaypoint = false;
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

            if(!hoveredWaypoint && hoveredWaypointLastFrame)
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

        //if (
        //    !hoveredWaypoint && hoveredWaypointLastFrame
        //    //&& mousePos != mousePosLastFrame
        //    )
        //{
        //    DestroyPathGizmos(selectedUnit);
        //}

        mousePosLastFrame = mousePos;
        hoveredWaypointLastFrame = hoveredWaypoint;
    }

    Vector2Int MouseToOffsetPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        plane.Raycast(ray, out float dist);
        Vector3 worldPos = ray.GetPoint(dist);
        Vector2Int offset = Board.WorldToOffset(worldPos);
        return offset;
    }


    CellV2 currHoveredCell;
    void HoverEmptyCell(CellV2 cell)
    {
        currHoveredCell = cell;

        var hoverPrefab = Resources.Load("Prefabs/HoveredCell");
        var newHoverRing = (GameObject)Instantiate(hoverPrefab, gizmos.transform);
        newHoverRing.name = "HoveredCell";
        newHoverRing.transform.position = cell.transform.position;
        //newHoverRing.transform.localScale = Vector3.zero;
        //newHoverRing.transform.DOScale(1f, 0.05f);
    }

    void UnhoverEmptyCell()
    {
        currHoveredCell = null;

        foreach (Transform child in gizmos.transform)
        {
            if (child.name == "HoveredCell")
            {
                Destroy(child.gameObject);
                //child.transform
                //    .DOScale(0f, 0.05f)
                //    .OnComplete(() => Destroy(child.gameObject));
            }
        }
    }


    //... UNIT:
    GameObject portrait;
    Unit currHoveredUnit;
    Unit selectedUnit;

    void HoverUnit(Unit unit)
	{
        currHoveredUnit = unit;
        portrait.SetActive(true);
        abilityDisplay.SetActive(true);
        for (int i = 0, j = 0; i < unit.Abilities.Count && j < 4; i++, j++)
        {
            abilityButtons[i].gameObject.SetActive(true);
            abilityButtons[i].Init(this, unit.Abilities[i]);
        }
    }

    void UnhoverUnit()
	{
        currHoveredUnit = null;
        portrait.SetActive(false);
        foreach (var abilityButton in abilityButtons)
            abilityButton.gameObject.SetActive(false);
        abilityDisplay.SetActive(false);
    }

    void SelectUnit(Unit unit)
    {
        // ^ A unit was clicked
        mode = Mode.UnitSelected;
        selectedUnit = unit;
        portrait.SetActive(true);

        // Show navigable tiles
        var prefab = Resources.Load("Prefabs/Waypoint");
        waypointPositions = Board.GetNavigableTiles(unit);
        foreach (Vector2Int pos in waypointPositions)
        {
            GameObject waypt = (GameObject)Instantiate(prefab, gizmos.transform);
            waypt.name = "Waypoint";
            waypt.transform.position = Board.OffsetToWorld(pos) + new Vector3(0, 0.1f, 0);
            waypt.transform.localScale = Vector3.zero;
            waypt.transform.DOScale(0.3f, 0.1f);
        }
    }

    void DeselectUnit()
    {
        mode = Mode.Neutral;
        DestroyPathGizmos(selectedUnit);
        selectedUnit = null;
        waypointPositions = null;
        portrait.SetActive(false);
        foreach (Transform child in gizmos.transform)
        {
            if (child.name == "Waypoint")
            {
                child.transform
                    .DOScale(0f, 0.05f)
                    .OnComplete(() => Destroy(child.gameObject));
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
    [ReadOnly] public AbilityButton[] abilityButtons;
    [ReadOnly] public AbilityV2 hoveredAbility;
    GameObject abilityDisplay;
    AbilityV2 selectedAbility;
    List<Vector2Int> validAbilityCoords;
    List<GameObject> validMoveMarkers;

    internal void OnPointerEnteredAbilityButton(AbilityV2 ability)
    {
        //Debug.LogWarning("HOVERING ABILITY: " + ability.name);
        isPointerInUI = true;
        HoverAbility(ability);
    }

    internal void OnPointerExitedAbilityButton(AbilityV2 ability)
    {
        //Debug.LogWarning("UNHOVERING ABILITY: " + ability.name);
        isPointerInUI = false;
        UnhoverAbility();
    }

    internal void OnPointerClickedAbilityButton(AbilityV2 ability)
    {
        //Debug.LogWarning("SELECTED ABILITY: " + ability.name);
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

    void HoverAbility(AbilityV2 ability)
	{
        validAbilityCoords = ability.GetValidMoves(
            Board.WorldToOffset(selectedUnit.transform.position),
            selectedUnit
            );

        if (validAbilityCoords.IsNullOrEmpty())
            return;

        foreach(var coord in validAbilityCoords)
		{
            Vector3 worldPos = Board.OffsetToWorld(coord);
            //var newMarker = 

            var hoverPrefab = Resources.Load("Prefabs/HoveredCell");
            var newHoverRing = (GameObject)Instantiate(hoverPrefab, gizmos.transform);
            newHoverRing.name = "ValidMove";
            newHoverRing.transform.position = worldPos + new Vector3(0, 0.1f, 0);
            newHoverRing.transform.SetParent(gizmos.transform);
        }
	}

    void UnhoverAbility()
	{
        foreach (Transform child in gizmos.transform)
        {
            if (child.name == "ValidMove")
            {
                Destroy(child.gameObject);
                //child.transform
                //    .DOScale(0f, 0.05f)
                //    .OnComplete(() => Destroy(child.gameObject));
            }
        }
    }

	void SelectAbility(AbilityV2 ability)
	{
		mode = Mode.AbilitySelected;
        selectedAbility = ability;
	}

	void DeselectAbility()
	{
		
	}
}
