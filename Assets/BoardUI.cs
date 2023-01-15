using Unity.Mathematics;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    GameObject portrait;

    void Awake()
    {
        portrait = transform.Find("portrait").gameObject;
        portrait.SetActive(false);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            plane.Raycast(ray, out float dist);
            Vector3 worldPos = ray.GetPoint(dist);
            Vector2Int offset = Board.WorldToOffset(worldPos);

            bool hasUnit = false;

            foreach (Unit unit in Board.Units)
            {
                if (unit.OffsetPos == offset)
                {
                    // ^ Unit exists at this position
                    hasUnit = true;
                    portrait.SetActive(true);
                    break;
                }
            }

            if (!hasUnit)
            {
                portrait.SetActive(false);
            }
        }
    }
}
