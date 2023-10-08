using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ResourceProber : MonoBehaviour
{
    [ReadOnly] public Object assetObject;

    [ReadOnly] public int cachedId = -1;
    
    public EditorButton reportBtn = new EditorButton("ReportID", false);
    public void ReportID()
	{
        if (assetObject == null)
            return;
        Debug.LogWarning($"prefab id: {assetObject.GetInstanceID()}");
        cachedId = assetObject.GetInstanceID();
        //prefab.id
	}

    public EditorButton fetchBtn = new EditorButton("Fetch", false);
    public void Fetch()
    {
        var fetched = Resources.InstanceIDToObject(cachedId);
        if (fetched == null)
            return;

        var instance = Object.Instantiate(fetched, this.transform);
    }

    public GameObject sceneInstance;
    public EditorButton prefabRootCheck = new EditorButton("PrefabRootCheck", false);
    [ReadOnly] public string prefabRootPath;
    public void PrefabRootCheck()
	{
        prefabRootPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(sceneInstance);
	}


    public string dummyString;
    public string splitString;
    [ReadOnly] public string splitStringResult;
    [ReadOnly] public Object foundThing;
    public EditorButton splitStringTest = new EditorButton("SplitString");
    public void SplitString()
	{
        splitStringResult = dummyString.Split(splitString)[0];
        Debug.LogWarning(splitStringResult);
        foundThing = Resources.Load<GameObject>(splitStringResult);
	}

    public string dummyGrabString;
    [ReadOnly] public Object dummyGrabbedObj;
    public EditorButton grabTestBtn = new EditorButton("Grab");
    public void Grab()
    {
		//dummyGrabbedObj = Resources.InstanceIDToObject(cachedId);
		dummyGrabbedObj = Resources.Load<Object>(dummyGrabString);
	}

    public GameObject dummySceneObject;
    public void ProbeSceneObject()
	{
        
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
