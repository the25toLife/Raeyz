using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CardMultiPart : CardMonster {

	// Use this for initialization
	public override void Start () {
		base.Start ();

		this.awakenCard ();
		this.setFullInfoAnimation ();
		float width = this.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
		this.transform.localScale = Vector3.one * (4.72f / width);
	}

	private void setFullInfoAnimation() {
		Animation a = this.fullInfoCanvas.GetComponentsInChildren<Animation>(true)[0];
		switch (this.CardI.GetId()) {

		case 284:
		case 285:
			a.clip = a.GetClip("showCardInfoFullL");
			break;
		}
	}

	public override void changeCard (CardInfo mi) {

		CardI = mi as MonsterInfo;
		if (CardI != null) {
			Sprite s = Resources.Load ("Cards/MultiPart/"+CardI.GetId(), typeof(Sprite)) as Sprite;
			if (s != null)
				this.GetComponent<SpriteRenderer> ().sprite = s;
		}
		
		foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(mi);
	}

	public override void OnPointerClick(PointerEventData eventData) {

		if (eventData.button == PointerEventData.InputButton.Right) {

			if (client.isCardSelected(this))
				client.deselectCard(this);
			
			lcMenu.SetActive(false);
			shield.SetActive(false);
			
			this.transform.localScale = new Vector3(2.8f, 2.8f, 1.0f);
			this.transform.SetParent(null);
			this.GetComponent<SpriteRenderer> ().sortingOrder += 100;
			statOverlay.SetActive (false);
			
			fullInfoCanvas.SetActive (true);
			this.transform.localPosition = new Vector3(-3.5f, 0.0f, 0.0f);
			stateToReturnTo = State;
			State = States.INFO;
			
			GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = true;

		} else
			base.OnPointerClick (eventData);
	}
}
