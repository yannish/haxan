using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMarker : MonoBehaviour
    , IPoolable
{
	public bool IsProcessing() => true;

	public void Play(Vector3 pos, Vector3 normal) { }

	public void Return() { }

	public void Tick() { }
}
