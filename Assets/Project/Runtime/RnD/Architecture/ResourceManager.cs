using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
	public Dictionary<UnitType, UnitDefinition> typeToUnitReference = new Dictionary<UnitType, UnitDefinition>();

	void OnAwake()
	{
		AssembleResources();
	}

    void AssembleResources()
	{
		var allUnitArchetypes = Resources.LoadAll<UnitDefinition>("UnitArchetypes").ToList();
		allUnitArchetypes.ToDictionary(r => r.type, r => r);
	}
}
