using UnityEngine;
using UnityEngine.EventSystems;

public class UnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    BoardUI ui;
    // int unitGuid; // TODO

    public void Init(BoardUI ui/*, int unitGuid*/)
    {
        this.ui = ui;
        // this.unitGuid = unitGuid; // TODO
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.OnPointerEnterUnitButton(/*unitGuid*/);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.OnPointerExitUnitButton();
    }

}