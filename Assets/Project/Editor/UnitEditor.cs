using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Unit), true)]
public class UnitEditor : Editor
{
	//GUIStyle headerStyle;
	//private void OnEnable()
	//{
	//	//headerStyle = EditorStyles.boldLabel;
	//}

	//public override void OnInspectorGUI()
	//{
	//	DrawDefaultInspector();
	//}

	private void OnSceneGUI()
	{
		Unit unit = target as Unit;

		Vector2Int unitCoord = Board.WorldToOffset(unit.transform.position);

		if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			plane.Raycast(ray, out float dist);
			
			Vector3 worldPos = ray.GetPoint(dist);
			Vector2Int targetCoord = Board.WorldToOffset(worldPos);
			Vector2Int axialCoord = Board.OffsetToAxial(targetCoord);
			Vector3Int fullAxialCoord = new Vector3Int(axialCoord.x, axialCoord.y, -axialCoord.x - axialCoord.y);

			Debug.LogWarning("... is neighbour: " + unitCoord.IsNeighbourOf(targetCoord));

			HexDirectionFT toTarget = unitCoord.ToNeighbour(targetCoord);

			Debug.LogWarning("to target: " + toTarget.ToString());

			//Vector3Int unitCubicCoord = Board.OffsetToCubic(unitCoord);
			//Vector3Int targetCubicCoord = Board.OffsetToCubic(targetCoord);
			//Vector3Int deltaCubicCoord = unitCubicCoord - targetCubicCoord;

			//Debug.LogWarning("targetOffset: " + targetCoord);
			//Debug.LogWarning("targetCubicCoord: " + targetCubicCoord);
			//Debug.LogWarning("fullAxialCoord: " + fullAxialCoord);

			//Debug.LogWarning("unitCubic: " + unitCubicCoord);
			//Debug.LogWarning("deltaCubicCoord: " + deltaCubicCoord);

			//Vector3Int weirdCoord = new Vector3Int(
			//	deltaCubicCoord.x - deltaCubicCoord.y,
			//	deltaCubicCoord.y - deltaCubicCoord.z,
			//	deltaCubicCoord.z - deltaCubicCoord.x
			//	);
		}

		if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.RightArrow)
		{
			unit.SetFacing(unit.Facing.Next());
			//unit.transform.hasChanged = true;
			Event.current.Use();
		}

		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftArrow)
		{
			unit.SetFacing(unit.Facing.Previous());
			//unit.transform.hasChanged = true;
			Event.current.Use();
		}
	}
}
