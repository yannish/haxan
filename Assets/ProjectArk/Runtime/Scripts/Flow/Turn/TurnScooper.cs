using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnScooper : MonoBehaviour
{
    public TurnCreator creator;

    //[ExposedScriptableObject]
    public Turn_OLD inputTurn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(creator.TryGetTurn(ref inputTurn))
		{
            Debug.LogWarning("GOT A TURN!");
		}
    }
}
