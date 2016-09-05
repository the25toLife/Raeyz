using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {
	
	public int SlotID;
	public CardInfo.CardType slotType;
	public Card card { get; set; }
	public ClientGame client;
	public GameObject multiCardPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () {

		if (client != null && client.dragging && canDrop(client.cardDragged))
			this.GetComponent<SpriteRenderer>().color = new Color(0.396f, 0.718f, 1.0f);
		else
			this.GetComponent<SpriteRenderer>().color = Color.white;
	}
	
	public bool canDrop(Card c) {
		if (card != null || !client.game.canTakeAction (Actions.PLAY))
			return false;
		else {

			if (c == null || c.CardI.Type != slotType || !c.dragPass ())
				return false;

			switch (slotType) {
			case CardInfo.CardType.AUXILIARY:
				foreach (Slot s in GameObject.FindObjectsOfType<Slot>()) {
					if (s.SlotID == this.SlotID - 5 && s.card == null)
						return false;
				}
				break;
			}

			return true;
		}
	}

	public void OnDrop(PointerEventData eventData) {
		
		Card c = eventData.pointerDrag.GetComponent<Card> ();
		if (!canDrop (c))
			return;
		if (c is CardMonster && (c as CardMonster).hasPair())
			setMultiCard(c as CardMonster, (c.CardI.AssoCardInfo.ContainsKey (CardRelation.LPAIR) ? -1 : 1));
		else
			setCard (c);
	}	

	public void setCard(Card c) {
		card = c;
		c.GetComponent<SpriteRenderer> ().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 1;
		c.changeReturnParent(this.transform);
		c.State = Card.States.INPLAY;
		client.game.playCard (c, SlotID);
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
		if (pdsm != null && pdsm.canDrop (c)) {
			
			GameObject o = GameObject.Instantiate(multiCardPrefab);
			
			CardMultiPart mpc = o.GetComponent<CardMultiPart>();
			if (dir > 0) {
				mpc.changeCard((c.CardI as MonsterInfo) + c.getPair());
				mpc.createUID(c.PairCard.UID);
				pdsm.setCard(mpc);
				mpc.createUID(c.UID);
				this.setCard(mpc);
			} else {
				mpc.changeCard(c.getPair() + (c.CardI as MonsterInfo));
				mpc.createUID(c.UID);
				this.setCard(mpc);
				mpc.createUID(c.PairCard.UID);
				pdsm.setCard(mpc);
			}
			mpc.returnToParent();
			
			GameObject.Destroy(c.PairCard.gameObject);
			GameObject.Destroy(c.gameObject);
		}
	}
}
