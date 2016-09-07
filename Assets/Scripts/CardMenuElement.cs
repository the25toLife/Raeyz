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

		case MenuItem.Awaken:
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
		case MenuItem.Discard:
			foreach(CardMenuElement cme in lcMenu.GetComponentsInChildren<CardMenuElement>(true)) {

				if (cme.itemType == MenuItem.Confirm)
					cme.gameObject.SetActive(true);
			}
			this.gameObject.SetActive(false);
			break;
		case MenuItem.Confirm:
			GameObject.FindObjectOfType<ClientGame>().game.SendCardToGraveyard(this.GetComponentInParent<Card> ());
			break;
		}
	}

	public void OnDisable() {

		if (itemType != MenuItem.Confirm)
			return;
		foreach(CardMenuElement cme in lcMenu.GetComponentsInChildren<CardMenuElement>(true)) {
			
			if (cme.itemType == MenuItem.Confirm)
				cme.gameObject.SetActive(false);
			else if (cme.itemType == MenuItem.Discard)
				cme.gameObject.SetActive(true);
		}
	}
}
