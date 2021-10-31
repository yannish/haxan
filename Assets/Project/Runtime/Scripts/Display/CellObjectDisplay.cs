using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellObjectDisplay : MonoBehaviour
{
    public Image iconSlot;
    public TextMeshProUGUI nameSlot;

    public GameObject veil;

    void Start()
    {
        iconSlot = GetComponentInChildren<Image>();
        nameSlot = GetComponentInChildren<TextMeshProUGUI>();

        CellObjFlowController.OnObjectHovered += DisplayCellObject;
        CellObjFlowController.OnObjetUnhovered += ClearCellObject;
		
        CellObjFlowController.OnObjectSelected += DisplayCellObject;
        CellObjFlowController.OnObjectDeselected += ClearCellObject;

        CellObjFlowController.OnObjectEnabled += BrightenCellDisplay;
        CellObjFlowController.OnObjectDisabled += DarkenCellDisplay;

        UpdateDisplay();

    }

    void BrightenCellDisplay(CellObject cellObject)
	{
        veil.SetActive(false);
	}

    void DarkenCellDisplay(CellObject cellObject)
	{
        veil.SetActive(true);
    }


    void DisplayCellObject(CellObject cellObject)
    {
        if (cellObject.icon)
            iconSlot.sprite = cellObject.icon;

        nameSlot.SetText(cellObject.name.ToUpper());

        displayCount++;

        UpdateDisplay();
    }

    void ClearCellObject(CellObject cellObject = null)
    {
        displayCount--;

        UpdateDisplay();
    }

    [ReadOnly] public int displayCount;
    void UpdateDisplay()
	{
        //Debug.LogWarning("updating display");

        if(displayCount == 0)
		{
            if (gameObject)
                gameObject.SetActive(false);
        }
		else
		{
            gameObject.SetActive(true);
        }
    }
}
