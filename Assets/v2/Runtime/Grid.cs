using Unity.Mathematics;
using UnityEngine;

public class Grid : MonoBehaviour
{
    void OnDrawGizmosSelected()
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
        // Draw rect around child extents
        {
            var cells = GetComponentsInChildren<CellV2>();
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(-float.MaxValue, -float.MaxValue);
            foreach (CellV2 cell in cells)
            {
                min = new Vector2(Mathf.Min(min.x, cell.transform.position.x), Mathf.Min(min.y, cell.transform.position.z));
                max = new Vector2(Mathf.Max(max.x, cell.transform.position.x), Mathf.Max(max.y, cell.transform.position.z));
            }
            // Min and max contain the bounds of the tile centers.
            // Now offset by radii of the tiles to get the bounding min and max.
            Vector2 bmin = min - new Vector2(CellV2.InnerRadius, CellV2.OuterRadius);
            Vector2 bmax = max + new Vector2(CellV2.InnerRadius, CellV2.OuterRadius);

            Gizmos.DrawLine(new Vector3(bmin.x, 0f, bmin.y), new Vector3(bmin.x, 0f, bmax.y));
            Gizmos.DrawLine(new Vector3(bmin.x, 0f, bmin.y), new Vector3(bmax.x, 0f, bmin.y));
            Gizmos.DrawLine(new Vector3(bmin.x, 0f, bmax.y), new Vector3(bmax.x, 0f, bmax.y));
            Gizmos.DrawLine(new Vector3(bmax.x, 0f, bmin.y), new Vector3(bmax.x, 0f, bmax.y));
        }
    }
}
