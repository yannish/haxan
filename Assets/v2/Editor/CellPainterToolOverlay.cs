using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

[Overlay(typeof(SceneView), "Cell Painter")]
[Icon(iconPath + "CellPainterIcon.png")]
public class CellPainterToolOverlay : ToolbarOverlay
{
	const string iconPath = "Assets/Editor/ToolIcons/";

	CellPainterToolOverlay() : base(PaintBrushButton.id) { }

	[EditorToolbarElement(id, typeof(SceneView))]
	class PaintBrushButton : EditorToolbarButton
	{
		public const string id = "CellPainter/Brush";

		public PaintBrushButton()
		{
			this.text = "PaintBrush";
			this.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath + "PaintBrush.png");
			this.tooltip = "Paint swatches to cells";
			this.clicked += OnClick;
		}

		private void OnClick()
		{
			Debug.LogWarning("CLICKED PAINT BRUSH!");
		}
	}
}