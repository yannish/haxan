using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public interface IOperable
{
    void Tick();

    float Duration { get; }

	void DrawInspectorContent();
}

[Serializable]
public struct QuickOpA : IOperable
	//, ISerializationCallbackReceiver
{
	public int bashDistance;

	public float duration;

	public QuickOpA(int bashDistance)
	{
		this.bashDistance = bashDistance;
		duration = 1f;
	}

	public float Duration => duration;

	public void DrawInspectorContent()
	{
#if UNITY_EDITOR
		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.IntField("Bash Distance: ", bashDistance);
			EditorGUILayout.FloatField("Duration: ", duration);
		}
#endif
	}

	//public void OnAfterDeserialize()
	//{
	//	Debug.LogWarning("ON AFTER SERIALIZE QUICK A");	
	//}

	//public void OnBeforeSerialize()
	//{
	//	Debug.LogWarning("ON BEFORE SERIALIZE QUICK A");
	//}

	public void Tick()
	{
		
	}
}

[Serializable]
public struct QuickOpB : IOperable
	//, ISerializationCallbackReceiver
{
	public float timeScale;

	public float duration; 
	
	public QuickOpB(float timeScale)
	{
		this.timeScale = timeScale;
		duration = 1f;
	}

	public float Duration => duration;

	public void DrawInspectorContent()
	{
#if UNITY_EDITOR
		using (new GUILayout.VerticalScope(EditorStyles.helpBox))
		{
			EditorGUILayout.FloatField("Timescale: ", timeScale);
			EditorGUILayout.FloatField("Duration: ", duration);
		}
#endif
	}

	//public void OnAfterDeserialize()
	//{
	//	Debug.LogWarning("ON AFTER SERIALIZE QUICK B");
	//}

	//public void OnBeforeSerialize()
	//{
	//	Debug.LogWarning("ON BEFORE SERIALIZE QUICK B");
	//}

	public void Tick()
	{

	}
}

public class PolymorphicStructRunner : MonoBehaviour
	//, ISerializationCallbackReceiver
{
    const int MAX_OPS = 10;

	[SerializeField]
    public IOperable[] ops = new IOperable[MAX_OPS];
	public List<string> opTypes = new List<string>();
	public List<string> _jsonOps = new List<string>();

	public void SaveToJSON()
	//public void OnBeforeSerialize()
	{
		opTypes.Clear();
		for(int i = 0; i < 2; i++)
		{
			var op = ops[i];
			if (op == null)
				continue;
			var foundType = op.GetType().ToString();
			opTypes.Add(foundType);

			_jsonOps.Add(JsonUtility.ToJson(op));
		}
		//Debug.LogWarning("ON BEFORE SERIALIZE STRUCT RUNNER");
	}

	public void LoadFromJSON()
	//public void OnAfterDeserialize()
	{
		for (int i = 0; i < 2; i++)
		{
			Type opType = Type.GetType(opTypes[i]);
			ops[i] = (IOperable)JsonUtility.FromJson(_jsonOps[i], opType);
		}

		//Debug.LogWarning("ON AFTER SERIALIZE STRUCT RUNNER");
	}

	public EditorButton saveBtn = new EditorButton("Save");
	public void Save()
	{
		ops[0] = new QuickOpA(4);
		ops[1] = new QuickOpB(0.3f);
	}

	public EditorButton loadBtn = new EditorButton("Load");
	public void Load()
	{
		ops[0] = new QuickOpA(4);
		ops[1] = new QuickOpB(0.3f);
	}

	public EditorButton addBtn = new EditorButton("Add");
	public void Add()
	{
		ops[0] = new QuickOpA(4);
		ops[1] = new QuickOpB(0.3f);
	}

	private void Start()
	{
		ops[0] = new QuickOpA(4);
		ops[1] = new QuickOpB(0.3f);
	}
}
