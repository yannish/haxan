using UnityEngine;
using Unity.Mathematics;

public class Board
{
    public static readonly float3x3 CartesianToHex = math.mul(
        new float3x3(
            Mathf.Sqrt(3f) / (3f * CellV2.OuterRadius), -1f / (3f * CellV2.OuterRadius), 0f,
            0f, 2f / (3f * CellV2.OuterRadius), 0f,
            0f, 0f, 1f
        ),
        new float3x3(
            1f, 0f, -CellV2.InnerRadius,
            0f, 1f, -CellV2.OuterRadius,
            0f, 0f, 1f
        )
    );
    public static readonly float3x3 HexToCartesian = math.mul(
        new float3x3(
            1f, 0f, CellV2.InnerRadius,
            0f, 1f, CellV2.OuterRadius,
            0f, 0f, 1f
        ),
        new float3x3(
            CellV2.InnerRadius * 2f, CellV2.InnerRadius, 0f,
            0f, 1.5f * CellV2.OuterRadius, 0f,
            0f, 0f, 1f
        )
    );
}
