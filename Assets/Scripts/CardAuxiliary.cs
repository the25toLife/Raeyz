using UnityEngine;
using System;

public class CardAuxiliary : Card
{

    private CardMonster _cardMonster;

/*
	public override void Start() {
		base.Start ();
	}
*/

	public override void changeCard (CardInfo ci)
	{

	    CardInfo = ci as AuxiliaryInfo;
	    if (CardInfo != null)
	    {
	        var s = Resources.Load("Cards/" + CardInfo.GetId(), typeof(Sprite)) as Sprite;
	        if (s != null)
	            GetComponent<SpriteRenderer>().sprite = s;
	    }
	    else
	    {
	        Debug.LogError("Cannot change auxiliary card (UID:" + UID + ") to contain " + ci.GetType());
	    }
		
		foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(CardInfo);
	}

	public override bool canTarget (Card target) {
		return false;
	}

	public override void assignTarget (Card target) {

	}

    public void OnPlay(CardMonster target)
    {
        OnPlay();
        _cardMonster = target;
    }
}