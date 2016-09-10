using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMenuSelect : MonoBehaviour, IPointerDownHandler {

	public MenuItem itemType;

	private Card representedCard;
	private ClientGame client;

	// Use this for initialization
	void Start () {

		client = GameObject.FindObjectOfType<ClientGame> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (itemType == MenuItem.CardUnlock) {
			this.GetComponentInChildren<Image>().sprite = Resources.Load (string.Format ("Cards/Stats/num_{0}", client.getSacrPower()), typeof(Sprite)) as Sprite;
		}
	}

	public void setCard(Card c) {

		representedCard = c;
		foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(c.CardInfo);
	}

	public void OnPointerDown(PointerEventData eventData) {


		switch (eventData.button) {

		case (PointerEventData.InputButton.Left):

			switch (itemType) {

			case (MenuItem.CardSelect):
				if (!representedCard)
					return;
				if (!client.isCardSelected(representedCard)) {
					client.selectCard(representedCard);
					this.GetComponent<Image>().color = new Color(0.7f, 0.16f, 0.16f, 0.46f);
				} else {
					client.deselectCard(representedCard);
					this.GetComponent<Image>().color = new Color(0.06f, 0.06f, 0.06f, 0.46f);
				}
				break;
			case (MenuItem.CardUnlock):
				client.awakenCard();
				break;
			}
			break;
		}
	}
	
	public void OnDisable() {
		
		if (itemType == MenuItem.CardSelect)
			GameObject.Destroy(this.gameObject);
	}
}
