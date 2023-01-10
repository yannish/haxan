using UnityEngine;
using Unity.Mathematics;

public class CellV2 : MonoBehaviour
{
    public static readonly float OuterRadius = 2f;
    public static readonly float InnerRadius = OuterRadius * 0.866025404f;

    void OnDrawGizmosSelected()
    {
        if (transform.hasChanged)
        {
            float3 cartesian = new float3(transform.localPosition.x, transform.localPosition.z, 1f);
            float2 hex = math.mul(Board.CartesianToHex, cartesian).xy;
            hex = math.round(hex);
            float2 roundedCartesian = math.mul(Board.HexToCartesian, new float3(hex.x, hex.y, 1f)).xy;

            Vector3 pos = new Vector3(
                roundedCartesian.x,
                0f,
                roundedCartesian.y
            );

            transform.localPosition = pos;
            transform.hasChanged = false;
        }
    }
}
