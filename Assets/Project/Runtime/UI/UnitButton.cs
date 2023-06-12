using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
{
    BoardUI ui;
    int unitGuid;

    public Image icon;
    public Image background;

    public Color hoverColor;
    public Color selectedColor;


    public void Init(BoardUI ui, int unitGuid, UnitPreset unitPreset)
    {
        this.ui = ui;
        this.unitGuid = unitGuid;
        this.icon.sprite = unitPreset.icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.OnPointerEnterUnitButton(unitGuid, this);
        //background.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.OnPointerExitUnitButton(unitGuid, this);
        //background.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ui.OnPointerClickUnitButton(unitGuid, this);
    }

}