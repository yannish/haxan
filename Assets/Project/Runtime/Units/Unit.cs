using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using System;
using BOG;

[SelectionBase]
public class Unit : MonoBehaviour
{
    public bool logDebug;

    public static Action<Unit> OnUnitChanged;

    [ReadOnly] public string templatePath = "";

    public UnitType type;

    public UnitState state;

    public Ability MovementAbility;
    
    public List<Ability> Abilities = new List<Ability>();
    
    public List<Item> startingInventory = new List<Item>();
    
    public List<Item> inventory = new List<Item>();


    [Header("STATE:")]
	public HexDirectionFT Facing;
    [ReadOnly] public int currMove = -1;
    [ReadOnly] public int currReactions = -1;


	private void OnEnable()
	{
        Debug.LogWarning($"UNIT ENABLED: {this.gameObject.name}");

        //if (GameContext.I == null)
        //    return;

        Haxan.activeUnits.Add(this);
	}

	private void OnDisable()
	{
        //if (GameContext.I == null)
        //    return;

        Haxan.activeUnits.Remove(this);
    }

	//public void OnSaveStart()
	//{
 //       state.name = this.name;
 //       state.id = this.gameObject.GetInstanceID();
 //       state.offsetPos = this.OffsetPos;
 //       state.facing = this.Facing;
 //       //unitState.rot = this.transform.rotation;
 //       GameContext.board.state.unitStates.Add(state);
	//}

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

    [Header("BITS:")]
    public Transform pivot;

    // Position in offset coordinate space
    //[ReadOnly, System.NonSerialized]
    public Vector2Int OffsetPos; //TODO: why did i set this non-serialized
    //.. was it breaking? i want it to be drawn.
    public int parity;

    public virtual void Start()
    {
        OffsetPos = Board.WorldToOffset(transform.position);
    }

	private void OnValidate()
	{
        this.SetFacing(Facing);
	}

    private void Reset()
	{
        Debug.LogWarning($"RESETTING UNIT: {this.name}");
	}

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
    public static void SetState(this Unit unit, UnitState state)
	{
        unit.state = state;
        unit.MoveTo(state.offsetPos);
        unit.SetFacing(state.facing);
	}

    public static void SetVisualPos(this Unit unit, Vector3 newPos, bool inLocalSpace = false)
	{
        if (unit.pivot == null)
            return;

        if (inLocalSpace)
            unit.pivot.localPosition = newPos;
        else
            unit.pivot.position = newPos;
	}


    public static void MoveTo(this Unit unit, Vector2Int coord)
	{
        unit.transform.position = Board.OffsetToWorld(coord);
        unit.OffsetPos = coord;
        if (unit.logDebug)
            Debog.logGameflow($"moved unit to : {coord.x},{coord.y}");
	}

    public static void SetFacing(this Transform pivot, HexDirectionFT dir)
	{
        var newFacingDir = dir.ToVector();
        pivot.rotation = Quaternion.LookRotation(newFacingDir);
    }

    /// <summary>
    /// Set the unit's cardinal facing direction.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="dir"></param>
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

    /// <summary>
    /// Directly set the unit's pivot rotation.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="dir"></param>
    public static void SetDirectFacing(this Unit unit, Vector3 dir)
    {
        if (unit.pivot == null)
            return;

        unit.pivot.rotation = Quaternion.LookRotation(dir);
    }
}