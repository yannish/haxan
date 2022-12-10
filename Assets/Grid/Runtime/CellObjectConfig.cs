using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObjectConfig : ScriptableObject
{
    public Sprite icon;
}

public interface IDeflectionHandler 
{
    CellDeflectionProfile GetDeflectionProfile();
    HexDirection Deflect(HexDirection inDir); 
}
