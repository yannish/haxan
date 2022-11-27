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

		CellObjFlowController.OnHoverPeeked += DisplayCellObject;
		CellObjFlowController.OnHoverUnpeeked += ClearCellObject;

		CellObjFlowController.OnEntered += DisplayCellObject;
		CellObjFlowController.OnExited += ClearCellObject;

		//CellObjFlowController.OnObjectEnabled += BrightenCellDisplay;
		//CellObjFlowController.OnObjectDisabled += DarkenCellDisplay;

		UpdateDisplay();

    }

    void BrightenCellDisplay(CellObjFlowController cellObjFlow)
	{
        veil.SetActive(false);
	}

    void DarkenCellDisplay(CellObjFlowController cellObjFlow)
	{
        veil.SetActive(true);
    }


    void DisplayCellObject(CellObjFlowController cellObjFlow)
    {
        if (cellObjFlow.cellObject.icon)
            iconSlot.sprite = cellObjFlow.cellObject.icon;

        nameSlot.SetText(cellObjFlow.cellObject.name.ToUpper());

        displayCount++;

        UpdateDisplay();
    }

    void ClearCellObject(CellObjFlowController cellObjFlow = null)
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
