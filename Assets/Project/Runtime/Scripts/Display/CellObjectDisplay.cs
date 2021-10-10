using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellObjectDisplay : MonoBehaviour
{
    public Image iconSlot;
    public TextMeshProUGUI nameSlot;

    void Start()
    {
        iconSlot = GetComponentInChildren<Image>();
        nameSlot = GetComponentInChildren<TextMeshProUGUI>();

        CellObjFlowController.OnObjectHovered += DisplayCellObject;
        CellObjFlowController.OnObjetUnhovered += ClearCellObject;
		
        CellObjFlowController.OnObjectSelected += DisplayCellObject;
        CellObjFlowController.OnObjectDeselected += ClearCellObject;

        ClearCellObject();
    }

    void DisplayCellObject(CellObject cellObject)
    {
        Debug.Log("... displaying cellObject");

        gameObject.SetActive(true);

        if(cellObject.icon)
            iconSlot.sprite = cellObject.icon;
        
        nameSlot.SetText(cellObject.name.ToUpper());
    }

    void ClearCellObject(CellObject cellObject = null)
    {
        Debug.Log("... clearing cellObject");

        if (gameObject)
            gameObject.SetActive(false);
    }
}
