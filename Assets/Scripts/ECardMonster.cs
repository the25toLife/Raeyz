using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ECardMonster : Card {

	public GameObject shield;
	private bool defending;

	// Update is called once per frame
	public override void Update () {

		if (State == States.DISABLED || client.game.CurrentStage == GameStage.SETUP)
			setCardFOW (true);
		else
			setCardFOW (false);

		this.GetComponentInChildren<Canvas> ().sortingOrder = this.GetComponent<SpriteRenderer> ().sortingOrder + 1;
		
		if (Input.GetKeyDown (KeyCode.Escape) && this.State == States.INFO) {
			
			this.transform.localScale = Vector3.one;
			fullInfoCanvas.SetActive (false);
			statOverlay.SetActive (true);
			this.transform.SetParent(parentToReturnTo);
			this.GetComponent<SpriteRenderer> ().sortingOrder -= 100;
			this.transform.localPosition = Vector3.zero;
			
			shield.SetActive (defending);
			
			this.State = (stateToReturnTo == States.EXPANDINHAND ? States.INHAND : stateToReturnTo);
			
			GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = false;
		}
	}

	public override void changeCard(CardInfo c) {
		
		CardI = c as MonsterInfo;
		
		foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(c);
	}

	public void informDefenseToggle() {
		client.game.SendDefenseToggleEv (CardI.GetId(), isDefending ());
	}	

	public bool hasPair() {
		
		return (this.CardI.AssoCardInfo.ContainsKey (CardRelation.PairL) || this.CardI.AssoCardInfo.ContainsKey (CardRelation.PairR));
	}

	public bool isDefending() {
		return defending;
	}
	
	public void setDefending(bool b) {
		defending = b;
		shield.SetActive (defending);
	}

	public override void sendCardToGraveyard () {

	    FindObjectOfType<FieldManager>().RemoveCardFromField(this);
	    if (defending)
			setDefending (false);
		if (State == States.INPLAY)
			this.transform.SetParent (null);
		if (State != States.DISABLED)
			deathHandler.SetActive (true);
		else
			GameObject.Destroy (this.gameObject);
		//client.game.sendCardToGraveyard (this);
	}

	public void setCardFOW(bool fow) {

		if (CardI != null) {
			if (!fow) {
				
				statOverlay.SetActive (true);
				Sprite s = Resources.Load ("Cards/"+CardI.GetId(), typeof(Sprite)) as Sprite;
				if (s != null)
					this.GetComponent<SpriteRenderer> ().sprite = s;
				else
					this.GetComponent<SpriteRenderer> ().sprite = Resources.Load ("Cards/_empty", typeof(Sprite)) as Sprite;
			} else {

				//statOverlay.SetActive (false);
				this.GetComponent<SpriteRenderer> ().sprite = Resources.Load ("Cards/cardBackRed", typeof(Sprite)) as Sprite;;
			}
		}
	}	


	public override void OnBeginDrag(PointerEventData eventData) {

	}
	
	public override void OnDrag(PointerEventData eventData) {

	}
	
	public override void OnEndDrag(PointerEventData eventData) {

	}
	
	public override void OnPointerClick(PointerEventData eventData) {

		if (deathHandler.activeSelf)
			return;

		switch (eventData.button) {
			
		case (PointerEventData.InputButton.Right):

			if (State == States.DISABLED) 
				return;

			shield.SetActive(false);

			this.changeReturnParent(this.transform.parent);
			this.transform.SetParent(null);
			this.transform.localScale = new Vector3(3.0f, 3.0f, 1.0f);
			this.GetComponent<SpriteRenderer> ().sortingOrder += 100;
			statOverlay.SetActive (false);
			fullInfoCanvas.SetActive (true);
			this.transform.localPosition = new Vector3(-3.0f, 0.0f, 0.0f);
			stateToReturnTo = State;
			State = States.INFO;
			
			GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = true;
			
			break;
		case (PointerEventData.InputButton.Left):

			if (State == States.INPLAY && client.isACardSelected()) {

				//client.selectEnemyCard(this);
			}		
			
			break;
		}
	}

	public override bool canTarget(Card target) {
		return false;
	}	
	public override void assignTarget(Card target) {
	}
		
	public override void OnPointerEnter(PointerEventData eventData) {

	}
	
	public override void OnPointerExit(PointerEventData eventData) {

	}

}
