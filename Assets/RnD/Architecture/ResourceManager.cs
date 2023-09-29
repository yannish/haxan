using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
	public Dictionary<UnitType, UnitReference> typeToUnitReference = new Dictionary<UnitType, UnitReference>();

	void OnAwake()
	{
		AssembleResources();
	}

    void AssembleResources()
	{
		var allUnitArchetypes = Resources.LoadAll<UnitReference>("UnitArchetypes").ToList();
		allUnitArchetypes.ToDictionary(r => r.type, r => r);
	}
}
