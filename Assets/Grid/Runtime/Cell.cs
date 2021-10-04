using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase, RequireComponent(typeof(CellFlowController))]
public class Cell : MonoBehaviour, 
	IFlowable
{

	public static event Action<Cell> OnCellHovered = delegate { };
	public static event Action OnCellUnhovered = delegate { };

    //public void CellHovered() { OnCellHovered(this); }
    //public void CellUnhovered() { OnCellUnhovered(); }


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


    //... coords:
    public OffsetCoordinates offsetCoords;
	public HexCoordinates coords;

	[ReadOnly] public CellFlowController cellFlow;
	[ReadOnly] public Cell[] neighbours = new Cell[6];
	public Cell[] validNeighbours { get { return neighbours.Where(t => t != null && t.IsPassable).ToArray(); } }


	//... assigned components:
	public CellPreset preset;
	public Transform occupantPivot;
	public MeshRenderer baseMeshRenderer;


	void Awake()
	{
		cellFlow = GetComponent<CellFlowController>();
        this.Unbind();
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