using Unity.Mathematics;
using UnityEngine;

public class Grid : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        if (transform.hasChanged)
        {
            // An offset of half a cell size is applied to the matrices because
            // unlike the cells themselves, the grids need to snap to the cell
            // boundaries, and not the cell centers.
            float3x3 offsetCartesianToAxial = math.mul(
                Board.CartesianToAxial,
                new float3x3(
                    1f, 0f, CellV2.InnerRadius,
                    0f, 1f, CellV2.OuterRadius,
                    0f, 0f, 1f
                )
            );
            float3x3 offsetAxialToCartesian = math.mul(
                new float3x3(
                    1f, 0f, -CellV2.InnerRadius,
                    0f, 1f, -CellV2.OuterRadius,
                    0f, 0f, 1f
                ),
                Board.AxialToCartesian
            );

            float3 cartesian = new float3(transform.position.x, transform.position.z, 1f);
            float2 axial = math.mul(offsetCartesianToAxial, cartesian).xy;
            axial = math.round(axial);
            float2 roundedCartesian = math.mul(offsetAxialToCartesian, new float3(axial.x, axial.y, 1f)).xy;

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

    void Awake()
    {
        Board.AddGrid(this);
    }

    void Start()
    {
        if (Board.Cells == null)
        {
            // ^ The board has not been built yet. We only want to do this once
            // across the whole game's startup, not once per Grid.
            Board.Build();
        }
    }
}
