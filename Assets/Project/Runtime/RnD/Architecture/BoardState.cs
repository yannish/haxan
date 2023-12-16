using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "BoardState", menuName = "Haxan/BoardState")]

[Serializable]
public class BoardState
{
	//... states of all units:
	public BoardLayout layout;

	//... how they got there:
	public BoardHistory history;
}
