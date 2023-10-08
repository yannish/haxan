using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeflectionFacet
{
    public HexDirection inFace;
    public HexDirection outFace;
}

[CreateAssetMenu(fileName = "CellDeflectionProfile", menuName = "Grids/CellDeflectionProfile")]
public class CellDeflectionProfile : ScriptableObject
{
    public List<DeflectionFacet> deflectionFacets = new List<DeflectionFacet>();

    public bool TryDeflect(CellObject cellObject, HexDirection inDir, out HexDirection outDir)
    {
        outDir = HexDirection.NE;

        HexDirection relativeInDir = inDir.Rotate(-(int)cellObject.facing);

        foreach (var facet in deflectionFacets)
		{
            if(facet.inFace == relativeInDir)
			{
                outDir = facet.outFace.Rotate((int)cellObject.facing);
                return true;
			}
		}

        return false;
    }

 //   public HexDirection Deflect(HexDirection inDir, CellObject cellObject)
	//{
 //       HexDirection outDir = HexDirection.NE;

 //       HexDirection relativeInDir = inDir.Rotate((int)cellObject.facing);

 //       return outDir;
	//}
}
