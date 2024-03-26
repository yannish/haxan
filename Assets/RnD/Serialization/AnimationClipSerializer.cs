using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DummyOpClipData
{
	public AnimationCurve blendCurve;
	public AnimationClip clip;
	public float weight;
}

[Serializable]
public class DummyOp
{
	public DummyOpClipData[] opClipDatas;
}

public class AnimationClipSerializer : MonoBehaviour
{
	[Multiline(30)]
	public string jsonBlorb;

	public DummyOp dummyOp;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
			Save();

		if (Input.GetKeyDown(KeyCode.L))
			Load();
	}

	public EditorButton loadBtn = new EditorButton("Load");
	private void Load() => dummyOp = JsonUtility.FromJson<DummyOp>(jsonBlorb);

	public EditorButton saveBtn = new EditorButton("Save");
	private void Save() => jsonBlorb = JsonUtility.ToJson(dummyOp);

	public EditorButton clearBtn = new EditorButton("Clear");
	private void Clear() => dummyOp = null;
}
