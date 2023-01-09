using Unity.Mathematics;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2Int NumTiles = new Vector2Int(4, 4);

    void OnDrawGizmos()
    {
        if (transform.hasChanged)
        {
            float3x3 cartesianToHex = math.mul(
                new float3x3(
                    Mathf.Sqrt(3f) / (3f * CellV2.OuterRadius), -1f / (3f * CellV2.OuterRadius), 0f,
                    0f, 2f / (3f * CellV2.OuterRadius), 0f,
                    0f, 0f, 1f
                ),
                new float3x3(
                    1f, 0f, 0f,
                    0f, 1f, 0f,
                    0f, 0f, 1f
                )
            );
            float3 cartesian = new float3(transform.position.x, transform.position.z, 1f);
            float2 hex = math.mul(cartesianToHex, cartesian).xy;
            hex = math.round(hex);
            float3x3 hexToCartesian = math.mul(
                new float3x3(
                    1f, 0f, 0f,
                    0f, 1f, 0f,
                    0f, 0f, 1f
                ),
                new float3x3(
                    CellV2.InnerRadius * 2f, CellV2.InnerRadius, 0f,
                    0f, 1.5f * CellV2.OuterRadius, 0f,
                    0f, 0f, 1f
                )
            );
            float2 roundedCartesian = math.mul(hexToCartesian, new float3(hex.x, hex.y, 1f)).xy;

            Vector3 pos = new Vector3(
                roundedCartesian.x,
                0f,
                roundedCartesian.y
            );
            transform.position = pos;
            transform.hasChanged = false;
        }
        // Draw the bounds gizmo
        {
            Vector3 pos = transform.position;
            Vector2 size = new Vector2(
                CellV2.InnerRadius * 2f * (NumTiles.x + 0.5f),
                CellV2.OuterRadius * (2f + (NumTiles.y - 1) * 1.5f)
            );

            Gizmos.DrawLine(pos, pos + new Vector3(size.x, 0f, 0f));
            Gizmos.DrawLine(pos, pos + new Vector3(0f, 0f, size.y));
            Gizmos.DrawLine(pos + new Vector3(size.x, 0f, 0f), pos + new Vector3(size.x, 0f, size.y));
            Gizmos.DrawLine(pos + new Vector3(0f, 0f, size.y), pos + new Vector3(size.x, 0f, size.y));
        }
    }
}
