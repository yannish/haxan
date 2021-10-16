using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
	public FlowController _flow;
	public FlowController flow
	{
		get
		{
			if (_flow == null)
				_flow = GetComponent<FlowController>();
			return _flow;
		}
	}

	public virtual List<Cell> GetValidMoves(Cell cell, CharacterFlow flow) => null;

	public virtual List<CharacterCommand> FetchCommandChain(Cell targetCell, CharacterFlow flow) => null;
}
