using UnityEngine;
using System;

public class CardSpecial : Card {

	public CardInfo.CardType type;

//	public override void Start() {
//		base.Start ();
//	}

	public override void changeCard (CardInfo ci) {

		CardI = ci as SpecialInfo;
		if (CardI != null) {
			var s = Resources.Load ("Cards/"+CardI.ID, typeof(Sprite)) as Sprite;
			if (s != null)
				this.GetComponent<SpriteRenderer> ().sprite = s;
		}
		
		foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(CardI);
	}

	public override bool canTarget (Card target) {
		return false;
	}

	public override void assignTarget (Card target) {

	}
}