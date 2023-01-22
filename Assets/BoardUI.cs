using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardUI : MonoBehaviour
{
    enum Mode
    {
        Neutral,
        UnitSelected
    }

    GameObject portrait;
    Mode mode;
    Vector2Int mouseDownPos;
    Unit selectedUnit;
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
            GameObject go = (GameObject)Instantiate(prefab, transform);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(0f, 110f * i);
            UnitButton btn = go.GetComponent<UnitButton>();
            btn.Init(this, unit.GetInstanceID());
        }
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
                portrait.SetActive(true);
            }
            else
            {
                portrait.SetActive(false);
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

    void HandleUnitSelectedMode()
    {
        // ^ We're in the mode where a unit is selected
        if (!isPointerInUI && (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f))
        {
            // ^ The mouse has moved
            Vector2Int mousePos = MouseToOffsetPos();

            bool hoveredWaypoint = false;
            foreach (Vector2Int pos in waypointPositions)
            {
                if (pos == mousePos)
                {
                    hoveredWaypoint = true;
                    break;
                }
            }
            if (hoveredWaypoint && hoveredCellPos != mousePos)
            {
                // ^ The mouse is hovering over a waypoint, and it is not the
                // current path's destination. The second check is done to avoid
                // running the A* algorithm and regenerating pathing markers
                // when the mouse is moved within the same cell.
                hoveredCellPos = mousePos;
                // Destroy existing path gizmos
                string name = $"Path{selectedUnit.GetInstanceID()}";
                foreach (Transform child in gizmos.transform)
                {
                    if (child.name == name)
                    {
                        Destroy(child.gameObject);
                    }
                }
                // Find the shortest path
                Vector2Int[] path = Board.FindPath(selectedUnit.OffsetPos, hoveredCellPos);
                // Create new path gizmos
                var prefab = Resources.Load("Prefabs/PathQuad");
                for (int i = 0; i < path.Length; i++)
                {
                    Vector2Int from = (i == 0) ? selectedUnit.OffsetPos : path[i - 1];
                    Vector2Int to = path[i];
                    GameObject pathQuad = (GameObject)Instantiate(prefab, gizmos.transform);
                    pathQuad.name = name;
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
            waypt.transform.position = Board.OffsetToWorld(pos)
                + new Vector3(0, 0.1f, 0);
            waypt.transform.localScale = Vector3.zero;
            waypt.transform.DOScale(0.3f, 0.05f);
        }
    }

    void DeselectUnit()
    {
        mode = Mode.Neutral;
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
        {
            portrait.SetActive(true);
        }
    }

    public void OnPointerExitUnitButton()
    {
        isPointerInUI = false;

        if (mode == Mode.Neutral)
        {
            portrait.SetActive(false);
        }
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
}
