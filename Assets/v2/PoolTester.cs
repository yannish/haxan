using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTester : MonoBehaviour
{
	public FloatReference floatRef;

	public PooledMonoBehaviour poolPrefab;

	public Vector3 spot;
	public Vector3 normal = Vector3.forward;

	public float bump = 1f;

	public EditorButton doIt = new EditorButton("DoIt", true);
	public void DoIt()
	{
		if (poolPrefab == null)
			return;

		poolPrefab.GetAndPlay(
			transform.position + spot,
			normal
			);

		spot += Vector3.right * bump;
	}


	public PooledMonoBehaviour poolCell;
	public EditorButton doCell = new EditorButton("DoCell", true);
	public void DoCell()
	{
		var newCell = poolCell.GetAndPlay(
			transform.position + spot,
			Quaternion.identity
			);

		var visuals = newCell.GetComponentInChildren<PooledCellVisuals>();
		visuals.SetTrigger(CellState.hover);

		spot += Vector3.right * bump;
	}

	public PooledMonoBehaviour poolAbilityMarker;
	public EditorButton doAbilityMarker = new EditorButton("DoAbilityMarker", true);
	List<CellMarker> allAbilityMarkers = new List<CellMarker>();
	public void DoAbilityMarker()
	{
		var newMarker = poolAbilityMarker.GetAndPlay(
			transform.position + spot,
			Quaternion.identity
			);

		spot += Vector3.right * bump;

		var abilityMarker = newMarker.GetComponentInChildren<CellMarker>();
		abilityMarker.Mark();
		allAbilityMarkers.Add(abilityMarker);
	}

	public EditorButton releaseAbilityMarkers = new EditorButton("ReleaseAbilityMarkers", true);
	public void ReleaseAbilityMarkers()
	{
		foreach(var marker in allAbilityMarkers)
		{
			marker.Unmark();
		}

		allAbilityMarkers.Clear();
	}
}
