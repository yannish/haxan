using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

[SelectionBase]
public class Unit : MonoBehaviour
{
	[Header("STATE:")]
	public HexDirectionFT Facing;

    [Header("CONFIG:")]
    public int groundedMovementRange;
    public UnitPreset preset;

    [Header("ABILITIES:")]
    public AbilityV2 MovementAbility;
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

    //public HexDirectionFT offsetDir;
    public int parity;
    void OnDrawGizmos()
    {
        if (transform.hasChanged)
		{
            transform.SnapToGrid();
            OffsetPos = Board.WorldToOffset(transform.position);
            //Debug.LogWarning("snapped to : " + OffsetPos.ToString());
        }

        //parity = this.OffsetPos.x & 1;

        //Vector2Int offset = Board.neighborLut[parity, (int)offsetDir];
        //Vector2Int offsetPos = this.OffsetPos + offset;
        //Vector3 worldPos = Board.OffsetToWorld(offsetPos);

        //Gizmos.DrawSphere(worldPos, 1f);
    }
}

public static class UnitExtensions
{
 //   public static void SetFacing(this GridSnap unit, HexDirectionFT dir)
	//{
 //       if (unit.pivot == null)
	//	{
 //           Debug.LogWarning($"unit {unit.name} is missing its pivot.");
 //           return;
	//	}

 //       var newFacingDir = dir.ToVector();
 //       unit.pivot.rotation = Quaternion.LookRotation(newFacingDir);
 //       unit.Facing = dir;
	//}

    public static void SetFacing(this Transform pivot, HexDirectionFT dir)
	{
        var newFacingDir = dir.ToVector();
        pivot.rotation = Quaternion.LookRotation(newFacingDir);
        //unit.Facing = dir;
    }

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