using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2Int NumTiles = new Vector2Int(4, 4);

    void OnDrawGizmos()
    {
        // Quantize the grid's position so that all grids line up with each other
        float q = (Mathf.Sqrt(3f) / 3f * transform.position.x - 1f / 3f * transform.position.z) / CellV2.OuterRadius;
        float r = (2f / 3f * transform.position.z) / CellV2.OuterRadius;
        q = Mathf.Round(q);
        r = Mathf.Round(r);

        Vector3 pos = new Vector3(
            CellV2.InnerRadius * (2f * q + r),
            0f,
            CellV2.OuterRadius * (1.5f * r)
        );

        Vector2 size = new Vector2(
            CellV2.InnerRadius * 2f * (NumTiles.x + 0.5f),
            CellV2.OuterRadius * (2f + (NumTiles.y - 1) * 1.5f)
        );
        Gizmos.DrawLine(pos, pos + new Vector3(size.x, 0f, 0f));
        Gizmos.DrawLine(pos, pos + new Vector3(0f, 0f, size.y));
        Gizmos.DrawLine(pos + new Vector3(size.x, 0f, 0f), pos + new Vector3(size.x, 0f, size.y));
        Gizmos.DrawLine(pos + new Vector3(0f, 0f, size.y), pos + new Vector3(size.x, 0f, size.y));

        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            Vector3 offset = pos - transform.position;
            CellV2[] cells = gameObject.GetComponentsInChildren<CellV2>();
            foreach (CellV2 cell in cells)
            {
                String[] tokens = cell.name.Split('_');
                Debug.Assert(tokens.Length == 3);
                int x = int.Parse(tokens[1]);
                int y = int.Parse(tokens[2]);
                cell.transform.localPosition = offset +
                new Vector3(
                    (0.5f + x + y * 0.5f - y / 2) * CellV2.InnerRadius * 2f,
                    0f,
                    CellV2.OuterRadius + y * (CellV2.OuterRadius * 1.5f)
                );
            }
        }
    }
}
