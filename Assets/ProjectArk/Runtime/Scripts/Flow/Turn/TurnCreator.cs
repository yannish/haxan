using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCreator : MonoBehaviour
{
    //[ExposedScriptableObject]
    //public QuickScrObj quickScrObj;

    //[ExposedScriptableObject]
    public CellDeflectionProfile deflectProfile;

	[ExposedScriptableObject]
	public Turn_OLD createdTurn;

    public CellObject owner;

    public bool createdTurnThisFrame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.A))
		{
            //quickScrObj = ScriptableObject.CreateInstance<QuickScrObj>();
            //quickScrObj.someFloat = UnityEngine.Random.Range(0f, 1f);
            //quickScrObj.hideFlags = HideFlags.DontSaveInEditor;
        }

        if(Input.GetKeyDown(KeyCode.C))
		{
            //quickScrObj = null;
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
            Debug.LogWarning("PRESSED T");
            Turn_OLD newTurn = new Turn_OLD();
            newTurn.numba = UnityEngine.Random.Range(0f, 1f);
            createdTurn = newTurn;
            createdTurnThisFrame = true;
		}
    }

	public bool TryGetTurn(ref Turn_OLD newTurn)
	{
        if (createdTurnThisFrame)
            Debug.LogWarning("tryin to fetch a turn on the frame it was made");

        createdTurnThisFrame = false;

        if (createdTurn != null)
		{
            newTurn = createdTurn;
            createdTurn = null;
            return true;
		}

        return false;
	}
}
