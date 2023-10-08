using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTween : MonoBehaviour
{
    public Transform targetPivot;

    public float maxHeight = 1f;
	public float duration = 1f;
	public float settleDuration = 2f;

	[ReadOnly] public float currHeight = 0f;


    private float initHeight;
	public Vector3 initPos;

	protected Sequence seq;

	public Ease ease;

	[Header("SPRING:")]
	public float damping;
	public float frequency;
	[ReadOnly] public Vector3 currTargetPos;
	[ReadOnly] public Vector3 currPos;
	[ReadOnly] public Vector3 currVelocity;

	


	public EditorButton toMax = new EditorButton("ToMax", true);
	void ToMax()
	{
		SetTween(maxHeight);
	}

	public EditorButton toBottom = new EditorButton("ToBottom", true);
	void ToBottom()
	{
		SetTween(0f);
	}


	public void SetTween(float newHeightTarget)
	{
		//if (seq.IsActive())
			seq.Kill();

		seq = DOTween.Sequence();
		seq.SetAutoKill();

		currTargetPos = initPos + Vector3.up * newHeightTarget;

		seq.Append(DOTween.To(() => currHeight, x => currHeight = x, newHeightTarget, duration))
			.OnUpdate(() =>
			{
				Springy.Spring(ref currPos, ref currVelocity, currTargetPos, damping, frequency, Time.deltaTime);
				targetPivot.position = initPos + currPos;
			});
	}


	private void Awake()
	{
		//initPos = targetPivot.position;
		cachedPos = targetRect.anchoredPosition;
		//cachedPos = targetRect.rect.position;
	}

	public RectTransform targetRect;
	public Vector2 cachedPos;
	public float offset;
	bool flipped;
	private void Update()
	{
		if (targetRect == null)
			return;

		targetRect.anchoredPosition = cachedPos + new Vector2(0f, offset);

		//targetRect.position = cachedPos + new Vector3(0f, offset, 0f);
		//targetRect.localPosition = cachedLocalPos + new Vector3(0f, offset, 0f);


		//if(Input.GetKeyDown(KeyCode.T))
		//{
		//	if (flipped)
		//	{
		//		SetTween(0f);
		//	}
		//	else
		//	{
		//		SetTween(maxHeight);
		//	}

		//	flipped = !flipped;
		//}
	}
}
