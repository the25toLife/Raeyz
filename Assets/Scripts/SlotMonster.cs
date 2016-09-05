using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SlotMonster : Slot {

//	public GameObject multiCardPrefab;
//
//	// Use this for initialization
//	void Start () {
//	
//	}
//
//	public void setCard(CardMonster c) {
//		card = c;
//		c.GetComponent<SpriteRenderer> ().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 1;
//		c.changeReturnParent(this.transform);
//		c.State = Card.States.INPLAY;
//		client.game.playCard (c, SlotID);
//	}
//	
//	public void setMultiCard(CardMonster c, int dir) {
//
//		Transform pairDropSlot = this.transform.parent.FindChild(string.Format("playerMonster{0}", SlotID + dir));
////		foreach (SlotMonster sm in this.transform.parent.GetComponentsInChildren<SlotMonster>()) {
////			if (sm.gameObject.name.Equals(string.Format("playerMonster{0}", SlotID + dir)))
////				pairDropSlot = sm;
////		}
//		if (pairDropSlot == null)
//			return;
//		SlotMonster pdsm = pairDropSlot.GetComponent<SlotMonster> ();
//		if (pairDropSlot != null && pdsm.canDrop ()) {
//
//			GameObject o = GameObject.Instantiate(multiCardPrefab);
//
//			CardMultiPart mpc = o.GetComponent<CardMultiPart>();
//			if (dir > 0) {
//				mpc.changeCard((c.CardI as MonsterInfo) + c.getPair());
//				mpc.createUID(c.PairCard.UID);
//				pdsm.setCard(mpc);
//				mpc.createUID(c.UID);
//				this.setCard(mpc);
//			} else {
//				mpc.changeCard(c.getPair() + (c.CardI as MonsterInfo));
//				mpc.createUID(c.UID);
//				this.setCard(mpc);
//				mpc.createUID(c.PairCard.UID);
//				pdsm.setCard(mpc);
//			}
//			mpc.returnToParent();
//			
//			GameObject.Destroy(c.PairCard.gameObject);
//			GameObject.Destroy(c.gameObject);
//		}
//	}
//
//	public override void OnDrop(PointerEventData eventData) {
//
//		if (!canDrop ())
//			return;
//		CardMonster c = eventData.pointerDrag.GetComponent<CardMonster> ();
//		if (c != null && c.dragPass ()) {
//			if (c.hasPair())
//				setMultiCard(c, (c.CardI.AssoCardInfo.ContainsKey (CardRelation.LPAIR) ? -1 : 1));
//			else
//				setCard (c);
//		}
//	}
}
