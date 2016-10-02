using UnityEngine;
using UnityEngine.EventSystems;

public class CardMenuElement : MonoBehaviour, IPointerDownHandler {
	
	public MenuItem itemType;
	private GameObject _lcMenu;

	// Use this for initialization
	void Start () {

		_lcMenu = this.transform.parent.gameObject;
	}

	public void OnPointerDown(PointerEventData eventData) {

		switch (itemType)
		{
            case MenuItem.Activate:
                FindObjectOfType<TargetCardMenu>().OpenMenu(GetComponentInParent<Card>(), false);
		        break;
            case MenuItem.Awaken:
		        FindObjectOfType<TargetCardMenu>().OpenMenu(GetComponentInParent<Card>(), true);
                //FindObjectOfType<ClientGame>().openAwakenCardMenu(GetComponentInParent<CardMonster>());
                _lcMenu.SetActive(false);
                break;
            case MenuItem.Discard:
                foreach(CardMenuElement cme in _lcMenu.GetComponentsInChildren<CardMenuElement>(true)) {

                    if (cme.itemType == MenuItem.Confirm)
                        cme.gameObject.SetActive(true);
                }
                gameObject.SetActive(false);
                break;
            case MenuItem.Confirm:
                FindObjectOfType<ClientGame>().Game.SendCardToGraveyard(GetComponentInParent<Card> ());
                break;
		}
	}

	public void OnDisable() {

		if (itemType != MenuItem.Confirm)
			return;
		foreach(CardMenuElement cme in _lcMenu.GetComponentsInChildren<CardMenuElement>(true)) {
			
			if (cme.itemType == MenuItem.Confirm)
				cme.gameObject.SetActive(false);
			else if (cme.itemType == MenuItem.Discard)
				cme.gameObject.SetActive(true);
		}
	}
}
