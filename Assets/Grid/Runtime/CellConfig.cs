using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellConfig : ScriptableObject
{
	public Sprite icon;
	public Material baseMat;
	//public Mesh baseMesh;

	public virtual void Paint(Cell cell)
	{
		if (baseMat != null)
			cell.baseMeshRenderer.sharedMaterial = baseMat;
	}
}

public interface IExitHandler { void OnExit(CellObject cellObj); }

public interface IEntryHandler { void OnEntry(CellObject cellObj); }

public interface INavHandler {	bool IsNavigable { get; } }

public interface IOcclusionHandler { bool IsOccluding { get; } }

public interface IPushableHandler {	bool IsPushable { get; } }