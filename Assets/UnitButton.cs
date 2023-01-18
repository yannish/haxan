using UnityEngine;
using UnityEngine.EventSystems;

public class UnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    BoardUI ui;
    int unitGuid;

    public void Init(BoardUI ui, int unitGuid)
    {
        this.ui = ui;
        this.unitGuid = unitGuid;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.OnPointerEnterUnitButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.OnPointerExitUnitButton();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ui.OnPointerClickUnitButton(unitGuid);
    }

}