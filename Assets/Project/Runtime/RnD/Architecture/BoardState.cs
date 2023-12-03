using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardState", menuName = "Haxan/BoardState")]
public class BoardState : ScriptableObject
{
	//... states of all units:
	public BoardLayout layout;

	//... how they got there:
	public BoardHistory history;
}
