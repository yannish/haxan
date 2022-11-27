using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ControlFlowManager))]
public class ControlFlowManagerInspector : Editor
{
	public void OnEnable() => EditorApplication.update += DoRepaint;

	public void OnDisable() => EditorApplication.update -= DoRepaint;

	void DoRepaint()
	{
		//Debug.Log("Repainting");
		var flowManager = target as ControlFlowManager;
		flowManager.ShowCurrentController();
		this.Repaint();
	}
}
