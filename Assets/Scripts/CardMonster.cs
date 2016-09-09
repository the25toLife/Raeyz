﻿using UnityEngine;
using UnityEngine.EventSystems;

public class CardMonster : Card {

    // Game object references set in Unity's inspector; must remain public
	public GameObject Locked, AwakenMenuItem, AttackInfo, Shield;

	private bool _defending, _cardLocked;
	public CardMonster PairCard { get; private set; }
	public Card Target { get; private set; }

	public override void Start() {
		base.Start ();

		AttackInfo.SetActive (false);
	}

	public override void Update() {

		if (Input.GetKeyDown(KeyCode.Escape) && State == States.INFO)
			Shield.SetActive (_defending);

		base.Update ();
	}

	public override bool canTarget(Card target) {
		return (target is CardMonster && target.IsEnemyCard);
	}

	public override void assignTarget(Card target) {

		this.Target = target;
		this.attackMonster (this.Target);
	}

    public override void changeCard(CardInfo c) {

		CardI = c as MonsterInfo;
		if (CardI != null) {
			var s = Resources.Load ("Cards/"+CardI.GetId(), typeof(Sprite)) as Sprite;
			if (s != null)
				this.GetComponent<SpriteRenderer> ().sprite = s;

			if ((CardI as MonsterInfo).GetLevel() < 15) {
				Locked.SetActive(false);
				AwakenMenuItem.SetActive(false);
			} else
				_cardLocked = true;
		}
		
		foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(c);
	}
	
	public void attackMonster(Card target) {

		if (target != null) {
			this.setDefending (false);
			this.AttackInfo.SetActive (true);
			this.AttackInfo.GetComponentInChildren<CardStatComponent> ().changeStat (target.CardI as MonsterInfo);
		} else {
//			this.setDefending(true);
			this.AttackInfo.SetActive(false);
		}
	}

	public override void sendCardToGraveyard() {
		base.sendCardToGraveyard ();

		if (_defending)
			setDefending (false);
		if (this.Target != null)
			this.clearTarget ();

	}

	public bool canSacrifice(bool leviathan) {

		if (leviathan && (CardI as MonsterInfo).GetLevel() < 5)
			return false;
		
		return !(_cardLocked || client.cardAwakening == this);
	}	

	public bool isLocked() {
		return _cardLocked;
	}
	
	public bool isDefending() {
		return _defending;
	}
	
	public void setDefending(bool b) {
		_defending = b;
		Shield.SetActive (_defending);
		if (!IsEnemyCard) client.game.SendDefenseToggleEv (this.UID, this.isDefending ());
	}
	
	public bool awakenCard() {
		
		this._cardLocked = false;
		Locked.SetActive(false);
		AwakenMenuItem.SetActive(false);
		return true;
	}

	public bool hasPair() {

		return (this.CardI.AssoCardInfo.ContainsKey (CardRelation.PairL) || this.CardI.AssoCardInfo.ContainsKey (CardRelation.PairR));
	}

	public MonsterInfo getPair() {
		
		CardInfo ci;
		if (!this.CardI.AssoCardInfo.TryGetValue (CardRelation.PairL, out ci))
			this.CardI.AssoCardInfo.TryGetValue (CardRelation.PairR, out ci);
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
				Camera.main.ScreenToWorldPoint (eventData.position) - new Vector3 ((this.CardI.AssoCardInfo.ContainsKey (CardRelation.PairL) ? 1.0f : -1.0f) * 2.22f, 0.0f, -10.0f);
		}
	}

	public override void OnEndDrag(PointerEventData eventData) {
		base.OnEndDrag (eventData);
		
		if (this.PairCard != null)
			PairCard.endDrag();
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick (eventData);

		switch (eventData.button) {

		case (PointerEventData.InputButton.Right):
			Shield.SetActive(false);
			break;
		case (PointerEventData.InputButton.Left):
		        if (IsEnemyCard)
		        {
		            if (State == States.INPLAY && client.isACardSelected())
		            {

		                client.selectEnemyCard(this);
		            }
		        }
			if (!client.game.canTakeAction(Actions.MENU))
				break;
			AwakenMenuItem.SetActive (client.game.canTakeAction (Actions.AWAKEN) && _cardLocked);
			break;
		}
	}
}
