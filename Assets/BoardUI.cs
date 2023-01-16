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

    void Awake()
    {
        portrait = transform.Find("portrait").gameObject;
        portrait.SetActive(false);
        waypoints = new GameObject("Waypoints");
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
        if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
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

        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = MouseToOffsetPos();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2Int mouseUpPos = MouseToOffsetPos();
            if (mouseUpPos == mouseDownPos)
            {
                // ^ The mouse was pressed and released on the same tile
                Vector2Int offset = MouseToOffsetPos();
                var unit = Board.GetUnitAtPos(offset);
                if (unit)
                {
                    // ^ A unit was clicked
                    mode = Mode.UnitSelected;
                    selectedUnit = unit;
                    // Show navigable tiles
                    var prefab = Resources.Load("Prefabs/Waypoint");
                    Vector2Int[] positions = Board.GetNavigableTiles(offset);
                    foreach (Vector2Int pos in positions)
                    {
                        GameObject waypt = (GameObject)Instantiate(prefab, waypoints.transform);
                        waypt.transform.position = Board.OffsetToWorld(pos)
                            + new Vector3(0, 0.1f, 0);
                        waypt.transform.localScale = Vector3.zero;
                        waypt.transform.DOScale(0.3f, 0.05f);
                    }
                }
            }
        }
    }

    void HandleUnitSelectedMode()
    {
        // ^ We're in the mode where a unit is selected
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = MouseToOffsetPos();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2Int mouseUpPos = MouseToOffsetPos();
            if (mouseUpPos == mouseDownPos)
            {
                // ^ The mouse was pressed and released on the same tile
                Vector2Int offset = MouseToOffsetPos();
                var unit = Board.GetUnitAtPos(offset);
                if (unit && unit != selectedUnit)
                {
                    // ^ A unit was clicked, and it is not already selected
                }
                else if (unit == null)
                {
                    // ^ A cell without a unit on it was clicked
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
}
