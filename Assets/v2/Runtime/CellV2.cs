using UnityEngine;
using Unity.Mathematics;

public class CellV2 : MonoBehaviour
{
    public static readonly float OuterRadius = 2f;
    public static readonly float InnerRadius = OuterRadius * 0.866025404f;

    void OnDrawGizmosSelected()
    {
        float3x3 cartesianToHex = math.mul(
            new float3x3(
                Mathf.Sqrt(3f) / (3f * OuterRadius), -1f / (3f * OuterRadius), 0f,
                0f, 2f / (3f * OuterRadius), 0f,
                0f, 0f, 1f
            ),
            new float3x3(
                1f, 0f, -InnerRadius,
                0f, 1f, -OuterRadius,
                0f, 0f, 1f
            )
        );
        float3 cartesian = new float3(transform.position.x, transform.position.z, 1f);
        float2 hex = math.mul(cartesianToHex, cartesian).xy;
        hex = math.round(hex);
        float3x3 hexToCartesian = math.mul(
            new float3x3(
                1f, 0f, InnerRadius,
                0f, 1f, OuterRadius,
                0f, 0f, 1f
            ),
            new float3x3(
                InnerRadius * 2f, InnerRadius, 0f,
                0f, 1.5f * OuterRadius, 0f,
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
    }
}
