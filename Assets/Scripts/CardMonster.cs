using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CardMonster : Card {

	public GameObject locked, awakenMenuItem, attackInfo, shield;
	protected bool cardLocked;
	protected bool defending;
	public CardMonster PairCard { get; private set; }
	public Card Target { get; private set; }

	public override void Start() {
		base.Start ();

		attackInfo.SetActive (false);
	}

	public override void Update() {

		if (Input.GetKeyDown(KeyCode.Escape) && this.State == States.INFO)
			shield.SetActive (defending);

		base.Update ();
	}

	public override bool canTarget(Card target) {
		return (target is ECardMonster);
	}

	public override void assignTarget(Card target) {

		this.Target = target;
		this.attackMonster (this.Target);
	}
	
	public override void changeCard(CardInfo c) {

		CardI = c as MonsterInfo;
		if (CardI != null) {
			var s = Resources.Load ("Cards/"+CardI.ID, typeof(Sprite)) as Sprite;
			if (s != null)
				this.GetComponent<SpriteRenderer> ().sprite = s;

			if ((CardI as MonsterInfo).Level < 5) {
				locked.SetActive(false);
				awakenMenuItem.SetActive(false);
			} else
				cardLocked = true;
		}
		
		foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(c);
	}
	
	public void attackMonster(Card target) {

		if (target != null) {
			this.setDefending (false);
			this.attackInfo.SetActive (true);
			this.attackInfo.GetComponentInChildren<CardStatComponent> ().changeStat (target.CardI as MonsterInfo);
		} else {
//			this.setDefending(true);
			this.attackInfo.SetActive(false);
		}
	}

	public override void sendCardToGraveyard() {
		base.sendCardToGraveyard ();

		if (defending)
			setDefending (false);
		if (this.Target != null)
			this.clearTarget ();

	}

	public bool canSacrifice(bool leviathan) {

		if (leviathan && (CardI as MonsterInfo).Level < 5)
			return false;
		
		return !(cardLocked || client.cardAwakening == this);
	}	

	public bool isLocked() {
		return cardLocked;
	}
	
	public bool isDefending() {
		return defending;
	}
	
	public void setDefending(bool b) {
		defending = b;
		shield.SetActive (defending);
		client.game.SendDefenseToggleEv (this.UID, this.isDefending ());
	}
	
	public bool awakenCard() {
		
		this.cardLocked = false;
		locked.SetActive(false);
		awakenMenuItem.SetActive(false);
		return true;
	}

	public bool hasPair() {

		return (this.CardI.AssoCardInfo.ContainsKey (CardRelation.LPAIR) || this.CardI.AssoCardInfo.ContainsKey (CardRelation.RPAIR));
	}

	public MonsterInfo getPair() {
		
		CardInfo ci;
		if (!this.CardI.AssoCardInfo.TryGetValue (CardRelation.LPAIR, out ci))
			this.CardI.AssoCardInfo.TryGetValue (CardRelation.RPAIR, out ci);
		return (ci as MonsterInfo);
	}

	public override bool dragPass() {

		if (!base.dragPass ())
			return false;

		if (this.isLocked())
			return false;

		if (this.hasPair () && !client.dragging) {
			Card cCheck;
			if (!client.isCardInHand(this.getPair (), out cCheck)) {
				this.PairCard = null;
				return false;
			} else if (this.PairCard == null)
				this.PairCard = cCheck as CardMonster;
		}

		return true;
	}

	public override void OnBeginDrag(PointerEventData eventData) {
		base.OnBeginDrag (eventData);

		if (this.PairCard != null)
			PairCard.prepareDrag();
	}

	public override void OnDrag(PointerEventData eventData) {
		base.OnDrag (eventData);
		
		if (this.PairCard != null) {
			PairCard.transform.position = 
				Camera.main.ScreenToWorldPoint (eventData.position) - new Vector3 ((this.CardI.AssoCardInfo.ContainsKey (CardRelation.LPAIR) ? 1.0f : -1.0f) * 2.22f, 0.0f, -10.0f);
		}
	}

	public override void OnEndDrag(PointerEventData eventData) {
		base.OnEndDrag (eventData);
		
		if (this.PairCard != null)
			PairCard.endDrag();
	}

	public override void OnPointerClick(PointerEventData eventData) {
		base.OnPointerClick (eventData);

		switch (eventData.button) {

		case (PointerEventData.InputButton.Right):
			shield.SetActive(false);
			break;
		case (PointerEventData.InputButton.Left):
			if (!client.game.canTakeAction(Actions.MENU))
				break;
			awakenMenuItem.SetActive (client.game.canTakeAction (Actions.AWAKEN) && cardLocked == true);
			break;
		}
	}
}
