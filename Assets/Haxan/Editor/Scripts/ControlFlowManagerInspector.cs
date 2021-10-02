using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ControlFlowManager))]
public class ControlFlowManagerInspector : Editor
{
	public void OnEnable()
	{
		//Debug.Log("Enabled Control flow manager inspector");
		EditorApplication.update += DoRepaint;
	}

	public void OnDisable()
	{
		//Debug.Log("Disabled Control flow manager inspector");
		EditorApplication.update -= DoRepaint;
	}

	void DoRepaint()
	{
		//Debug.Log("Repainting");
		var flowManager = target as ControlFlowManager;
		flowManager.ShowCurrentController();
		this.Repaint();
	}
}
