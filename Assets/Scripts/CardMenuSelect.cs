using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMenuSelect : MonoBehaviour, IPointerDownHandler {

	public MenuItem ItemType;

	private Card _representedCard;
	private ClientGame _client;

	// Use this for initialization
	void Start () {

		_client = GameObject.FindObjectOfType<ClientGame> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (ItemType == MenuItem.CardUnlock) {
			this.GetComponentInChildren<Image>().sprite = Resources.Load (string.Format ("Cards/Stats/num_{0}", _client.getSacrPower()), typeof(Sprite)) as Sprite;
		}
	}

	public void setCard(Card c) {

		_representedCard = c;
		foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(c.CardInfo);
	}

	public void OnPointerDown(PointerEventData eventData) {


		switch (eventData.button) {

		case (PointerEventData.InputButton.Left):

			switch (ItemType) {

			case (MenuItem.CardSelect):
				if (!_representedCard)
					return;
				if (!_client.isCardSelected(_representedCard)) {
					_client.selectCard(_representedCard);
					this.GetComponent<Image>().color = new Color(0.7f, 0.16f, 0.16f, 0.46f);
				} else {
					_client.deselectCard(_representedCard);
					this.GetComponent<Image>().color = new Color(0.06f, 0.06f, 0.06f, 0.46f);
				}
				break;
			case (MenuItem.CardUnlock):
				_client.awakenCard();
				break;
			}
			break;
		}
	}
	
	public void OnDisable() {
		
		if (ItemType == MenuItem.CardSelect)
			GameObject.Destroy(this.gameObject);
	}
}
