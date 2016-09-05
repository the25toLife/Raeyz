using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CardMenuElement : MonoBehaviour, IPointerDownHandler {
	
	public MenuItem itemType;
	private GameObject lcMenu;

	// Use this for initialization
	void Start () {

		lcMenu = this.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerDown(PointerEventData eventData) {

		switch (itemType) {

		case MenuItem.AWAKEN:
//			foreach(CardMenuElement cme in lcMenu.GetComponentsInChildren<CardMenuElement>(true)) {
//
//				if (cme.itemType == MenuItem.CANCEL)
//					cme.gameObject.SetActive(true);
//				else
//					cme.gameObject.SetActive(false);
			//			}
			GameObject.FindObjectOfType<ClientGame>().openAwakenCardMenu(this.GetComponentInParent<CardMonster>());
			lcMenu.SetActive(false);
			break;
		case MenuItem.DISCARD:
			foreach(CardMenuElement cme in lcMenu.GetComponentsInChildren<CardMenuElement>(true)) {

				if (cme.itemType == MenuItem.CONFIRM)
					cme.gameObject.SetActive(true);
			}
			this.gameObject.SetActive(false);
			break;
		case MenuItem.CONFIRM:
			GameObject.FindObjectOfType<ClientGame>().game.sendCardToGraveyard(this.GetComponentInParent<Card> ());
			break;
		}
	}

	public void OnDisable() {

		if (itemType != MenuItem.CONFIRM)
			return;
		foreach(CardMenuElement cme in lcMenu.GetComponentsInChildren<CardMenuElement>(true)) {
			
			if (cme.itemType == MenuItem.CONFIRM)
				cme.gameObject.SetActive(false);
			else if (cme.itemType == MenuItem.DISCARD)
				cme.gameObject.SetActive(true);
		}
	}
}
