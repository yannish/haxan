using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2Int NumTiles = new Vector2Int(4, 4);

    void OnDrawGizmos()
    {
        Vector2 size = new Vector2(
            CellV2.InnerRadius * 2f * (NumTiles.x + 0.5f),
            CellV2.OuterRadius * (2f + (NumTiles.y - 1) * 1.5f)
        );
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(size.x, 0f, 0f));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0f, 0f, size.y));
        Gizmos.DrawLine(transform.position + new Vector3(size.x, 0f, 0f), transform.position + new Vector3(size.x, 0f, size.y));
        Gizmos.DrawLine(transform.position + new Vector3(0f, 0f, size.y), transform.position + new Vector3(size.x, 0f, size.y));
    }
}
