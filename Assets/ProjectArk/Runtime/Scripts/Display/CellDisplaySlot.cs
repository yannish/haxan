using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class CellDisplaySlot : MonoBehaviour
{
	public Image cellIcon;
	public TextMeshProUGUI cellText;

	public void DisplayCellConfig(CellConfig config)
	{
		//Debug.Log("... displaying cell config : " + config.name, config);

		gameObject.SetActive(true);

		if(config.icon)
			cellIcon.sprite = config.icon;
		else
		{
			Debug.Log("... cell icon is null!");
		}

		cellText.SetText(config.name.ToUpper());
	}

	public void Clear()
	{
		//Debug.Log("... clearing cell config : ");

		if (gameObject)
			gameObject.SetActive(false);
	}
}
