using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsHandler : MonoBehaviour
{
	public List<Renderer> renderers;
	public Bounds enclosingBounds;
	public EditorButton calculateBtn = new EditorButton("CalculateBounds", false);
	public void CalculateBounds()
	{
		enclosingBounds.center = this.transform.position;
		enclosingBounds.size = Vector3.zero;
		renderers = GetComponentsInChildren<Renderer>().ToList();
		foreach(var rend in renderers)
		{
			enclosingBounds.Encapsulate(rend.bounds);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(enclosingBounds.center, enclosingBounds.size);
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
