using UnityEngine;
using UnityEngine.EventSystems;

public class TakeActionButtom : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        FindObjectOfType<TargetCardMenu>().TakeAction();
    }
}
