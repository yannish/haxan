using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingMarker : MonoBehaviour
{
	public Transform arrowHeadPivot;
	public Transform arrowCenterPivot;

	public HexDirectionFT startDir;
	public HexDirectionFT endDir;

	[ReadOnly] public int distClockwise;
	[ReadOnly] public int distCounterClockwise;

	[ReadOnly] public float angleIncrement;

	private void OnValidate()
	{
		distClockwise = startDir.ClockwiseTo(endDir);
		distCounterClockwise = startDir.CounterClockwiseTo(endDir);
	}

	[Range(0f, 1f)]
	public float arcPadding = 0f;


	[Range(0, 5)]
	public int offset;

	public bool clockwise;
    
	public MaterialBlockHandle matBlockHandle;

    public MaterialBlockColor colorBlock;

    public MaterialBlockFloat angleBlock;

    public MaterialBlockFloat directionBlock;

	public MaterialBlockFloat rotationBlock;


	[Range(0f, 1f)]
	public float debugAngle;
	[Range(0f, 1f)]
	public float debugRotation;
	public Color debugColor;
	public void Update()
	{
		float dir = clockwise ? 1f : -1f;
		float angle = endDir.ToAngle() - arcPadding * 360f * dir;
		arrowCenterPivot.rotation = Quaternion.AngleAxis(angle, Vector3.up);
		Vector3 offset = Vector3.forward;
		Vector3 offsetRotatedBack = Quaternion.AngleAxis(angle - 1f * dir, Vector3.up) * offset;
		Vector3 offsetRotatedForward = Quaternion.AngleAxis(angle + 1f * dir, Vector3.up) * offset;
		Vector3 toDirection = offsetRotatedBack.To(offsetRotatedForward);

		arrowHeadPivot.rotation = Quaternion.LookRotation(toDirection);

		//angleBlock.floatValue = debugAngle;
		//float offset = ((float)(startDir - 3) * 60f;
		

		angleIncrement = (float)(clockwise ? startDir.ClockwiseTo(endDir) : startDir.CounterClockwiseTo(endDir));
		angleBlock.floatValue = (angleIncrement * 60f) / 360f + arcPadding * dir;
		//angleBlock.floatValue = debugAngle;

		rotationBlock.floatValue = (((int)startDir) * 60f) / 360f;
		//rotationBlock.floatValue = debugRotation;
		//colorBlock.colorValue = debugColor;
		//matBlockHandle.RecordChange(colorBlock);

		matBlockHandle.RecordChange(angleBlock);
		matBlockHandle.RecordChange(rotationBlock);

		directionBlock.floatValue = clockwise ? -1f : 1f;
		matBlockHandle.RecordChange(directionBlock);
	}

	//public HexDirectionFT testDir;
	//public EditorButton incrementTestDir = new EditorButton("Increment", false);
	//public void Increment()
	//{
	//	testDir++;
	//}
}
