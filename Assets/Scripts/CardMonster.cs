using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMonster : Card {

    // Game object references set in Unity's inspector; must remain public
	public GameObject Locked, AwakenMenuItem, AttackInfo, Shield;

	private bool _defending, _cardLocked;
	public CardMonster PairCard { get; private set; }
	public CardMonster Target { get; set; }
    public int Kills { get; set; }

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
		return CardInfo.TargetCriteria.Matches(target);
	}

	public override void assignTarget(Card target) {

	    if (target == null && Target != null)
	    {
	        Target.Target = null;
	        foreach (var statusEffect in Target.StatusEffects)
	            if (statusEffect.AppliesAgainstCriteria != null) statusEffect.Apply();
	    }
	    Target = target as CardMonster;
	    foreach (var statusEffect in StatusEffects)
	        if (statusEffect.AppliesAgainstCriteria != null) statusEffect.Apply();

	    if (Target != null)
	    {
	        Target.Target = this;
	        foreach (var statusEffect in Target.StatusEffects)
	            if (statusEffect.AppliesAgainstCriteria != null) statusEffect.Apply();
	    }

	    attackMonster (Target);
	}

    public override void changeCard(CardInfo c) {

		CardInfo = c as MonsterInfo;
		if (CardInfo != null) {
		    if (!FowActive)
		    {
		        var s = Resources.Load("Cards/" + CardInfo.GetId(), typeof(Sprite)) as Sprite;
		        if (s != null)
		        {
		            Image image = transform.Find("CardImage").GetComponent<Image>();
		            image.sprite = s;
		        }
		    }
		    if ((CardInfo as MonsterInfo).GetLevel() < 5 || IsEnemyCard) {
				Locked.SetActive(false);
				AwakenMenuItem.SetActive(false);
			} else
				_cardLocked = true;
		}

        foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
        {
            if (!csc.transform.parent.name.Equals("attackInfo")) csc.changeStat(c);
        }
    }
	
	public void attackMonster(Card target) {

		if (target != null) {
			this.setDefending (false);
			this.AttackInfo.SetActive (true);
			this.AttackInfo.GetComponentInChildren<CardStatComponent> ().changeStat (target.CardInfo as MonsterInfo);
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

	public bool canSacrifice(bool leviathan)
	{

	    if (IsEnemyCard) return false;
		if (leviathan && (CardInfo as MonsterInfo).GetLevel() < 5)
			return false;
		
		return !(_cardLocked || Client.CardAwakening == this);
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
		if (!IsEnemyCard) Client.Game.SendDefenseToggleEv (this.UID, this.isDefending ());
	}
	
	public bool SetAwake(bool awake) {
		
		_cardLocked = !awake;
		Locked.SetActive(!awake);
		AwakenMenuItem.SetActive(!awake);
	    return true;
	}

	public bool hasPair() {

		return (this.CardInfo.AssoCardInfo.ContainsKey (CardRelation.PairL) || this.CardInfo.AssoCardInfo.ContainsKey (CardRelation.PairR));
	}

	public MonsterInfo getPair() {
		
		CardInfo ci;
		if (!this.CardInfo.AssoCardInfo.TryGetValue (CardRelation.PairL, out ci))
			this.CardInfo.AssoCardInfo.TryGetValue (CardRelation.PairR, out ci);
		return (ci as MonsterInfo);
	}

	public override bool dragPass() {

		if (!base.dragPass ())
			return false;

		if (this.isLocked())
			return false;

		if (this.hasPair () && !Client.Dragging) {
			Card cCheck;
			if (!Client.isCardInHand(this.getPair (), out cCheck)) {
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
				Camera.main.ScreenToWorldPoint (eventData.position) - new Vector3 ((this.CardInfo.AssoCardInfo.ContainsKey (CardRelation.PairL) ? 1.0f : -1.0f) * 2.22f, 0.0f, -10.0f);
		}
	}

	public override void OnEndDrag(PointerEventData eventData) {
		base.OnEndDrag (eventData);
		
		if (this.PairCard != null)
			PairCard.endDrag();
	}

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        if (eventData.button == PointerEventData.InputButton.Right) return;

        // Allows auxiliary cards to be dropped on a monster card in order to play it
        Card card = eventData.pointerDrag.GetComponent<Card>();
        if (!card.dragPass()) return;
        if (card is CardAuxiliary)
        {
            CardAuxiliary cardAuxiliary = (CardAuxiliary) card;
            if (cardAuxiliary.canTarget(this))
            {
                Slot slot = GetComponentInParent<Slot>();
                if (slot == null) return;
                Slot auxiliarySlot = GameObject.Find(String.Format("playerAux{0}", slot.SlotID)).GetComponent<Slot>();
                if (auxiliarySlot != null && auxiliarySlot.CurrentCard == null) auxiliarySlot.setCard(cardAuxiliary);
            }
        }
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
		            if (State == States.INPLAY && Client.isACardSelected())
		            {

		                Client.selectEnemyCard(this);
		            }
		        }
			if (!Client.Game.canTakeAction(Actions.MENU))
				break;
			AwakenMenuItem.SetActive (Client.Game.canTakeAction (Actions.AWAKEN) && _cardLocked);
			break;
		}
	}

    public override void OnPlay()
    {
        base.OnPlay();
        if (IsEnemyCard) SetAwake(true);
    }
}
