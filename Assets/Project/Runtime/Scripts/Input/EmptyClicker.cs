using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmptyClicker : MonoBehaviour 
{
	private void Update()
	{
		if (
			Input.GetMouseButtonDown(0) 
			)
		{
			if(EventSystem.current.IsPointerOverGameObject())
				Debug.Log("over something");
			else
				Debug.Log("over nothing");
		}
	}
}
