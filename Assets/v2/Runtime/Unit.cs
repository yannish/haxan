using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

[SelectionBase]
public class Unit : MonoBehaviour
{
	[Header("STATE:")]
	public HexDirectionFT Facing;

    [Header("CONFIG:")]
    public List<AbilityV2> Abilities = new List<AbilityV2>();

    [Header("BITS:")]
    public Transform pivot;

    // Position in offset coordinate space
    [HideInInspector, System.NonSerialized]
    public Vector2Int OffsetPos;

    void Start()
    {
        OffsetPos = Board.WorldToOffset(transform.position);
    }

	private void OnValidate()
	{
        this.SetFacing(Facing);
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

public static class UnitExtensions
{
    public static void SetFacing(this Unit unit, HexDirectionFT dir)
	{
        if (unit.pivot == null)
		{
            Debug.LogWarning($"unit {unit.name} is missing its pivot.");
            return;
		}

        var newFacingDir = dir.ToVector();
        unit.pivot.rotation = Quaternion.LookRotation(newFacingDir);
        unit.Facing = dir;
	}
}