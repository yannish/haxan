using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "QuickScrObj", menuName = "Grids/QuickScrObj")]
public class QuickScrObj : InlineScriptableObject
{
    public float someFloat;
    public int someInt;
    public string someString;

    public override bool Creatable() => true;
}
