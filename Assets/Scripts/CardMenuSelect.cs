using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMenuSelect : MonoBehaviour, IPointerDownHandler {

	public MenuItem ItemType;

    public bool AllowMultipleSelections { get; set; }

	private Card _representedCard;
	private TargetCardMenu _targetCardMenu;

	// Use this for initialization
	void Awake ()
	{

	    _targetCardMenu = FindObjectOfType<TargetCardMenu>();
	}
	
	// Update is called once per frame
	void Update () {

		if (ItemType == MenuItem.CardUnlock)
		{
			this.GetComponentInChildren<Image>().sprite = Resources.Load (string.Format ("Cards/Stats/num_{0}", _targetCardMenu.GetTotalLevels()), typeof(Sprite)) as Sprite;
		}
		else if (ItemType == MenuItem.CardSelect)
		{
		    GetComponent<Image>().color =
		        _targetCardMenu.SelectedCards.Contains(_representedCard) ? new Color(0.7f, 0.16f, 0.16f, 0.46f) :
		            GetComponent<Image>().color = new Color(0.06f, 0.06f, 0.06f, 0.46f);
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
				if (!_targetCardMenu.SelectedCards.Contains(_representedCard)) {
					//_targetCardMenu.selectCard(_representedCard, !AllowMultipleSelections);
				    _targetCardMenu.SelectCard(_representedCard);
				} else {
					//_targetCardMenu.deselectCard(_representedCard);
				    _targetCardMenu.DeselectCard(_representedCard);
				}
				break;
			case (MenuItem.CardUnlock):
                FindObjectOfType<TargetCardMenu>().TakeAction();
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
