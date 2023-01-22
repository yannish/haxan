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
    GameObject waypoints;
    bool isPointerInUI;

    public void Init()
    {
        portrait = transform.Find("portrait").gameObject;
        portrait.SetActive(false);
        waypoints = new GameObject("Waypoints");
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
                    if(unit != selectedUnit)
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
        Vector2Int[] positions = Board.GetNavigableTiles(unit);
        foreach (Vector2Int pos in positions)
        {
            GameObject waypt = (GameObject)Instantiate(prefab, waypoints.transform);
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
        portrait.SetActive(false);
        foreach (Transform child in waypoints.transform)
        {
            child.transform
                .DOScale(0f, 0.05f)
                .OnComplete(() => Destroy(child.gameObject));
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
