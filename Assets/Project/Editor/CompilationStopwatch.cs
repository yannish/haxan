using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

//[InitializeOnLoad]
public class CompilationStopwatch
{
 //   static CompilationStopwatch()
	//{
		//CompilationPipeline.compilationStarted += OnCompilationStarted;
		//CompilationPipeline.compilationFinished += OnCompliationFinished;
		//CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
		//CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
	//}


	//private static void OnAssemblyCompilationStarted(string obj)
	//{
	//	assemblyStartTime = EditorApplication.timeSinceStartup;
	//	UnityEditor.EditorApplication.update += OnUpdateTick;
	//}

	//private static void OnAssemblyCompilationFinished(string arg1, CompilerMessage[] arg2)
	//{
	//	var deltaTime = EditorApplication.timeSinceStartup - assemblyStartTime;
	//	Debug.LogWarning($"... deltaTime: {deltaTime}");
	//	UnityEditor.EditorApplication.update -= OnUpdateTick;
	//}




	private static void OnUpdateTick()
	{
		Debug.LogWarning($"OnUpdateTick! {EditorApplication.timeSinceStartup}");
		//EditorApplication.timeSinceStartup
	}

	static double assemblyStartTime;
	//static int startTime;
	private static void OnCompilationStarted(object obj)
	{
		assemblyStartTime = EditorApplication.timeSinceStartup;
		UnityEditor.EditorApplication.update += OnUpdateTick;
	}





	private static void OnCompliationFinished(object obj)
	{
		var deltaTime = EditorApplication.timeSinceStartup - assemblyStartTime;
		Debug.LogWarning($"... deltaTime: {deltaTime}");
		UnityEditor.EditorApplication.update -= OnUpdateTick;
	}
}
