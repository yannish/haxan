using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using System;

[SelectionBase]
public class Unit : MonoBehaviour
{
    public static Action<Unit> OnUnitChanged;


	[Header("STATE:")]
	public HexDirectionFT Facing;
    [ReadOnly] public int currMove = -1;

    public void DecrementMove()
	{
        currMove--;
        OnUnitChanged?.Invoke(this);
    }

    public void IncrementMove()
	{
        currMove++;
        OnUnitChanged?.Invoke(this);
	}


    [Header("CONFIG:")]
    public int groundedMovementRange;
    public UnitPreset preset;

    [Header("ABILITIES:")]
    public AbilityV2 MovementAbility;
    public List<AbilityV2> Abilities = new List<AbilityV2>();

    [Header("INVENTORY:")]
    public List<Item> Items = new List<Item>();

    [Header("BITS:")]
    public Transform pivot;

    // Position in offset coordinate space
    [ReadOnly, System.NonSerialized] public Vector2Int OffsetPos;
    //[HideInInspector, System.NonSerialized]

    void Start()
    {
        OffsetPos = Board.WorldToOffset(transform.position);
    }

	private void OnValidate()
	{
        this.SetFacing(Facing);
	}

    public int parity;
    void OnDrawGizmos()
    {
        if (transform.hasChanged)
		{
            transform.SnapToGrid();
            OffsetPos = Board.WorldToOffset(transform.position);
        }
    }
}

public static class UnitExtensions
{
    public static void SetVisualPos(this Unit unit, Vector3 newPos, bool inLocalSpace = false)
	{
        if (unit.pivot == null)
            return;

        if (inLocalSpace)
            unit.pivot.localPosition = newPos;
        else
            unit.pivot.position = newPos;
	}

    public static void SetFacing(this Transform pivot, HexDirectionFT dir)
	{
        var newFacingDir = dir.ToVector();
        pivot.rotation = Quaternion.LookRotation(newFacingDir);
        //unit.Facing = dir;
    }

    public static void MoveTo(this Unit unit, Vector2Int coord)
	{
        unit.transform.position = Board.OffsetToWorld(coord);
        unit.OffsetPos = coord;
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

    public static void SetDirectFacing(this Unit unit, Vector3 dir)
    {
        if (unit.pivot == null)
            return;

        unit.pivot.rotation = Quaternion.LookRotation(dir);
    }
}