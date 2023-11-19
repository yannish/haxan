using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardState", menuName = "Haxan/BoardState")]
public class BoardStateVariable : ScriptableObject
{
	//... states of all units:
	public BoardState state;

	//... how they got there:
	public BoardHistory history;
}
