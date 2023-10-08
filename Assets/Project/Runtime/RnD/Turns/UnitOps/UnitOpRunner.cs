using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitOpRunner : MonoBehaviour
{
	public OpData quickOpData;

	public const int MAX_OPS = 10;

    public IOperable[] allOps = new IOperable[MAX_OPS];

	public List<string> jsonOps = new List<string>();
	public List<string> opTypes = new List<string>();

	//public Unit dummyUnit;
	//public BoardHistory boardHistory = new BoardHistory();

	[TextArea]
	public string jsonBlob;

	private void Start()
	{
		//allOps[0] = new QuickOpA(4);
		//allOps[1] = new QuickOpB(0.3f);

		//boardHistory.ops.Add((GroundMoveOp)allOps[0]);
		//boardHistory.ops.Add((GroundMoveOp)allOps[1]);
	}

	public void SetOps()
	{
		allOps[0] = new QuickOpA(4);
		allOps[1] = new QuickOpB(0.3f);
	}

	public void ClearOps()
	{
		allOps[0] = null;
		allOps[1] = null;
	}

	public void SaveToJSON()
	{
		opTypes.Clear();

		for (int i = 0; i < 2; i++)
		{
			var op = allOps[i];
			if (op == null)
				continue;

			var foundType = op.GetType().ToString();
			opTypes.Add(foundType);

			jsonOps.Add(JsonUtility.ToJson(op));
		}
	}

	public void LoadFromJSON()
	{
		for (int i = 0; i < 2; i++)
		{
			Type opType = Type.GetType(opTypes[i]);
			allOps[i] = (IOperable)JsonUtility.FromJson(jsonOps[i], opType);
		}
	}
}
