using UnityEngine;

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

    void Awake()
    {
        portrait = transform.Find("portrait").gameObject;
        portrait.SetActive(false);
    }

    void Update()
    {
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
                    // TODO: Show navigable tiles
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
