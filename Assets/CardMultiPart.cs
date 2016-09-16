using UnityEngine;
using UnityEngine.EventSystems;

public class CardMultiPart : CardMonster {

	// Use this for initialization
	public override void Start () {
		base.Start ();

		SetAwake (true);
		setFullInfoAnimation ();
	}

	private void setFullInfoAnimation() {
		Animation a = this.FullInfoCanvas.GetComponentsInChildren<Animation>(true)[0];
		switch (this.CardInfo.GetId()) {

		case 284:
		case 285:
			a.clip = a.GetClip("showCardInfoFullL");
			break;
		}
	}

	public override void changeCard (CardInfo mi) {

		CardInfo = mi as MonsterInfo;
		if (CardInfo != null) {
			Sprite s = Resources.Load ("Cards/MultiPart/"+CardInfo.GetId(), typeof(Sprite)) as Sprite;
		    if (s != null)
		    {
		        SpriteRenderer spriteRenderer = transform.Find("CardSprite").GetComponent<SpriteRenderer>();
		        spriteRenderer.sprite = s;

		        // Scale the sprite
		        float width = spriteRenderer.sprite.bounds.size.x;
		        float height = spriteRenderer.sprite.bounds.size.y;
		        spriteRenderer.transform.localScale = new Vector3(4.72f / width, 3.01f / height, 1);
		    }
		}
		
		foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(mi);
	}

	public override void OnPointerClick(PointerEventData eventData) {

		if (eventData.button == PointerEventData.InputButton.Right) {

			if (Client.isCardSelected(this))
				Client.deselectCard(this);
			
			LcMenu.SetActive(false);
			Shield.SetActive(false);
			
			this.transform.localScale = new Vector3(2.8f, 2.8f, 1.0f);
			this.transform.SetParent(null);
			this.GetComponent<SpriteRenderer> ().sortingOrder += 100;
			StatOverlay.SetActive (false);
			
			FullInfoCanvas.SetActive (true);
			this.transform.localPosition = new Vector3(-3.5f, 0.0f, 0.0f);
			StateToReturnTo = State;
			ChangeState(States.INFO);
			
			GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = true;

		} else
			base.OnPointerClick (eventData);
	}
}
