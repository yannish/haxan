using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitDeflectionFacet
{
    public HexDirectionFT inFace;
    public HexDirectionFT outFace;
}

[CreateAssetMenu(fileName = "UnitDeflectionProfile", menuName = "Units/UnitDeflectionProfile")]
public class UnitDeflectionProfile : ScriptableObject
{
    public List<UnitDeflectionFacet> deflectionFacets = new List<UnitDeflectionFacet>();


}
