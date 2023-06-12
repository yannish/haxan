using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMarker_OLD : MonoBehaviour
    , IPoolable
{
	public bool IsProcessing() => true;

	public void Play(int playParams = 0) { }

	public void Stop(int stopParams = 0) { }

	public void Return() { }

	public void Tick() { }

}
