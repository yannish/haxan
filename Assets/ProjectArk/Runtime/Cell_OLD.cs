using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct HexCoordinates
{
	[SerializeField]
	private int x;
	[SerializeField]
	private int z;

	public int X { get { return x; } }
	public int Z { get { return z; } }
	public int Y { get { return -X - Z; } }

	public HexCoordinates(int x, int z)
	{
		this.x = x;
		this.z = z;
	}

	public Vector3 WorldPos()
	{
		return new Vector3(
			(x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2f,
			0f,
			z * (HexMetrics.outerRadius * 1.5f)
			);
	}

	public static HexCoordinates FromOffsetCoordinates(int x, int z)
	{
		return new HexCoordinates(x - z / 2, z);
	}

	public override string ToString()
	{
		return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}
}

[SelectionBase, RequireComponent(typeof(CellFlowController))]
public class Cell_OLD : MonoBehaviour
	, IFlowable
{
	//... cells are the IFlowables of their UIElements component.
	public FlowController Flow
	{
		get
		{
			//Debug.Log("fetching flow on : " + gameObject.name);
			if (this.TryGetBoundCellObject(out CellObject foundCellObject))
			{
				if (foundCellObject.flowController != null)
					return foundCellObject.flowController;
			}

			//Debug.Log("... no bound object, getting base flow: " + gameObject.name);
			return cellFlow;
		}
	}

	public static event Action<Cell_OLD> OnCellHovered = delegate { };

	public static event Action OnCellUnhovered = delegate { };


	[Header("COORDS:")]
    //public OffsetCoordinates offsetCoords;
	public HexCoordinates coords;

	[ReadOnly] public PooledCellVisuals visuals;
	[ReadOnly] public CellFlowController cellFlow;
	[ReadOnly] public Cell_OLD[] neighbours = new Cell_OLD[6];
	public Cell_OLD[] validNeighbours { get { return neighbours.Where(t => t != null && t.IsPassable).ToArray(); } }


	//... assigned components:
	[Header("PAINT:")]
	public CellPreset preset;

	//[Header("Visuals")]
	public GameObject visualBase;
	[ReadOnly] public List<MeshRenderer> baseMeshRenderers;

	[Header("PIVOT:")]
	public Transform occupantPivot;


	void Awake()
	{
		visuals = GetComponentInChildren<PooledCellVisuals>();
		cellFlow = GetComponent<CellFlowController>();
        this.Unbind();
	}

	void OnValidate()
	{
		if (visualBase == null)
			return;

		baseMeshRenderers = visualBase.GetComponentsInChildren<MeshRenderer>().ToList();
	}




	public void Enter(CellObject cellObj)
	{
		if (preset == null || preset.configs.IsNullOrEmpty())
			return;

		foreach (var config in preset.configs)
		{
			if (config is IEntryHandler)
				(config as IEntryHandler).OnEntry(cellObj);
		}
	}

	public void Leave(CellObject cellObj)
	{
		if (preset == null || preset.configs.IsNullOrEmpty())
			return;

		foreach (var config in preset.configs)
		{
			if (config is IExitHandler)
				(config as IExitHandler).OnExit(cellObj);
		}
	}

	public bool IsPassable
	{
		get
		{
			if (this.IsBound())
				return false;

			if (preset == null)
				return true;

			foreach (var config in preset.configs)
				if (config is INavHandler)
					if (!(config as INavHandler).IsNavigable)
						return false;

			return true;
		}
	}
}