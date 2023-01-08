using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2Int NumTiles = new Vector2Int(4, 4);

    void OnDrawGizmos()
    {
        Vector2 tileSize = new Vector2(4f * 0.8660254f, 4f);
        Vector2 size = new Vector2(NumTiles.x * tileSize.x, NumTiles.y * tileSize.y);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(size.x, 0f, 0f));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0f, 0f, size.y));
        Gizmos.DrawLine(transform.position + new Vector3(size.x, 0f, 0f), transform.position + new Vector3(size.x, 0f, size.y));
        Gizmos.DrawLine(transform.position + new Vector3(0f, 0f, size.y), transform.position + new Vector3(size.x, 0f, size.y));
    }
}
