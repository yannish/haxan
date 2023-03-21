using UnityEngine;
using Unity.Mathematics;
using System;

public class Cell : MonoBehaviour
{
    public CellSurfaceFlags surfaceFlags;
    public Transform pivot;

    public static readonly float OuterRadius = 2f;

    public static readonly float InnerRadius = OuterRadius * 0.866025404f;

    void OnDrawGizmos()
    {
        if (transform.hasChanged)
        {
            float3 cartesian = new float3(transform.localPosition.x, transform.localPosition.z, 1f);
            float2 axial = math.mul(Board.CartesianToAxial, cartesian).xy;
            axial = math.round(axial);
            float2 roundedCartesian = math.mul(Board.AxialToCartesian, new float3(axial.x, axial.y, 1f)).xy;

            Vector3 pos = new Vector3(
                roundedCartesian.x,
                0f,
                roundedCartesian.y
            );

            transform.localPosition = pos;
            transform.hasChanged = false;
        }

        Vector2Int offsetCoord = Board.WorldToOffset(transform.position);
        transform.position.DrawString($"{offsetCoord.x}, {offsetCoord.y}", Color.green, -40f);
    }
}
