using UnityEngine;
using Unity.Mathematics;

public class Unit : MonoBehaviour
{
    // Position in offset coordinate space
    [HideInInspector, System.NonSerialized]
    public Vector2Int OffsetPos;

    void Start()
    {
        OffsetPos = Board.WorldToOffset(transform.position);
    }

    void OnDrawGizmos()
    {
        if (transform.hasChanged)
        {
            float3 cartesian = new float3(transform.position.x, transform.position.z, 1f);
            float2 axial = math.mul(Board.CartesianToAxial, cartesian).xy;
            axial = math.round(axial);
            float2 roundedCartesian = math.mul(Board.AxialToCartesian, new float3(axial.x, axial.y, 1f)).xy;

            Vector3 pos = new Vector3(
                roundedCartesian.x,
                0f,
                roundedCartesian.y
            );

            transform.position = pos;
            transform.hasChanged = false;
        }
    }
}
