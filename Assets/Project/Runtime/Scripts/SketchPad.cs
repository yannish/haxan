using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchPad : MonoBehaviour
{
    Sequence seq;

    TweenCallback currCallback;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Debug.LogWarning("Startin sketchpad");
    }



    public float easeTime;
    [ReadOnly] public float currTime;

    [ContextMenu("Go Forward")]
    void GoForward()
	{
        seq = DOTween.Sequence();
        seq.SetAutoKill();
        seq
            .Append(DOTween.To(() => currTime, x => currTime = x, 1f, easeTime))
            .OnUpdate(currCallback)
            .SetEase(Ease.OutQuint);


		//seq.Kill(true);
	}


    [ContextMenu("Go Backward")]
    void GoBackward()
	{
        seq = DOTween.Sequence();
        seq.SetAutoKill();
        seq
            .Append(DOTween.To(() => currTime, x => currTime = x, 0f, easeTime))
            .OnUpdate(currCallback)
            .SetEase(Ease.OutQuint);

        //seq.Kill(true);
    }

    [ContextMenu("Set Red")]
    void SetCallbackRed()
	{
        currCallback = () => Debug.Log("RED");
	}

    [ContextMenu("Set Blue")]
    void SetCallbackBlue()
    {
        currCallback = () => Debug.Log("BLUE");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
