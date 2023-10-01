using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardState", menuName = "Haxan/BoardState")]
public class BoardStateVariable : ScriptableObject
{
    //public List<UnitState> unitStates = new List<UnitState>();

	//[SerializeField]
	public BoardState state;
}
