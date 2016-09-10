using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {
	
	public int SlotID;
	public CardInfo.CardType SlotType;
	public Card CurrentCard { get; set; }
	public ClientGame Client;
	public GameObject MultiCardPrefab;
    public Slot MonsterSlot;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () {

	    if (Client != null && Client.Dragging)
	    {
	        if (canDrop(Client.CardDragged))
	            GetComponent<SpriteRenderer>().color = new Color(0.396f, 0.718f, 1.0f);
	        else
	        {
	            CardMonster cardMonster = Client.CardDragged as CardMonster;
	            if (cardMonster != null && cardMonster.hasPair() && canDrop(cardMonster.PairCard))
	                GetComponent<SpriteRenderer>().color = new Color(0.396f, 0.718f, 1.0f);
	        }
	    } else
			this.GetComponent<SpriteRenderer>().color = Color.white;
	}
	
	public bool canDrop(Card c) {
		if (CurrentCard != null || !Client.Game.canTakeAction (Actions.PLAY))
			return false;
	    if (c == null || c.CardInfo.GetCardType() != SlotType || !c.dragPass ())
	        return false;

	    switch (SlotType) {
            case CardInfo.CardType.Monster:
	            if (c.CardInfo.AssoCardInfo.ContainsKey(CardRelation.PairL))
	            {
	                Transform pairDropSlot = transform.parent.FindChild(string.Format("playerMonster{0}", SlotID - 1));
	                if (pairDropSlot == null) return false;
	                Slot pdsm = pairDropSlot.GetComponent<Slot>();
	                if (pdsm == null || pdsm.CurrentCard != null) return false;
	            } else if (c.CardInfo.AssoCardInfo.ContainsKey(CardRelation.PairR))
	            {
	                Transform pairDropSlot = transform.parent.FindChild(string.Format("playerMonster{0}", SlotID + 1));
	                if (pairDropSlot == null) return false;
	                Slot pdsm = pairDropSlot.GetComponent<Slot>();
	                if (pdsm == null || pdsm.CurrentCard != null) return false;
	            }
	            break;
	        case CardInfo.CardType.Auxiliary:
	            foreach (Slot s in FindObjectsOfType<Slot>()) {
	                if (s.SlotID == SlotID - 5 && s.CurrentCard == null)
	                    return false;
	            }
	            break;
	    }

	    return true;
	}

	public void OnDrop(PointerEventData eventData)
	{

	    if (eventData.button == PointerEventData.InputButton.Right) return;

		Card c = eventData.pointerDrag.GetComponent<Card> ();
		if (!canDrop (c))
			return;
		if (c is CardMonster && (c as CardMonster).hasPair())
			setMultiCard(c as CardMonster, (c.CardInfo.AssoCardInfo.ContainsKey (CardRelation.PairL) ? -1 : 1));
		else
			setCard (c);
	}	

	public void setCard(Card c) {
		CurrentCard = c;
		c.GetComponent<SpriteRenderer> ().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 1;
		c.changeReturnParent(transform);
		c.State = Card.States.INPLAY;

	    Client.Game.playCard (c, SlotID);

	    // Adds the card to the field manager.  Multipart cards are handled separately to prevent them from
	    // being added to the field manager twice.
	    if (!(c is CardMultiPart)) Client.Game.FieldManager.AddCardToField(c);

	    if (CurrentCard is CardAuxiliary)
	    {
            if (MonsterSlot != null && (MonsterSlot.CurrentCard as CardMonster) != null)
                ((CardAuxiliary) CurrentCard).OnPlay((CardMonster) MonsterSlot.CurrentCard);
	    }
	}
	
	public void setMultiCard(CardMonster c, int dir) {
		
		Transform pairDropSlot = this.transform.parent.FindChild(string.Format("playerMonster{0}", SlotID + dir));
		//		foreach (SlotMonster sm in this.transform.parent.GetComponentsInChildren<SlotMonster>()) {
		//			if (sm.gameObject.name.Equals(string.Format("playerMonster{0}", SlotID + dir)))
		//				pairDropSlot = sm;
		//		}
		if (pairDropSlot == null)
			return;
		Slot pdsm = pairDropSlot.GetComponent<Slot> ();
		if (pdsm != null && pdsm.canDrop (c.PairCard)) {
			
			GameObject o = GameObject.Instantiate(MultiCardPrefab);
			
			CardMultiPart mpc = o.GetComponent<CardMultiPart>();
			if (dir > 0) {
				mpc.changeCard((c.CardInfo as MonsterInfo) + c.getPair());
				mpc.createUID(c.PairCard.UID);
				pdsm.setCard(mpc);
				mpc.createUID(c.UID);
				setCard(mpc);
			} else {
				mpc.changeCard(c.getPair() + (c.CardInfo as MonsterInfo));
				mpc.createUID(c.UID);
				setCard(mpc);
				mpc.createUID(c.PairCard.UID);
				pdsm.setCard(mpc);
			}
			mpc.returnToParent();

		    Client.Game.FieldManager.AddCardToField(mpc);
		    Client.Dragging = false;
			
			Destroy(c.PairCard.gameObject);
			Destroy(c.gameObject);
		}
	}
}
