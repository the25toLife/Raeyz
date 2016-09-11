using UnityEngine;

public class CardAuxiliary : Card
{

    private CardMonster _cardMonster;


/*
	public override void Start() {
		base.Start ();
	}
*/

    public override void Update()
    {
        base.Update();
        if (State == States.INPLAY && _cardMonster == null) Destroy(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (_cardMonster == null) return;
        foreach (StatusEffect statusEffect in ((AuxiliaryInfo) CardInfo).StatusEffects)
        {
            statusEffect.Remove(_cardMonster, true);
        }
    }

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
		return target.CardInfo.GetAffinity() == CardInfo.CardAffinity.All
		       || target.CardInfo.GetAffinity() == CardInfo.GetAffinity();
	}

	public override void assignTarget (Card target) {

	}

    public void OnPlay(CardMonster target)
    {
        OnPlay();
        _cardMonster = target;
        foreach (StatusEffect statusEffect in ((AuxiliaryInfo) CardInfo).StatusEffects)
        {
            //if (statusEffect.Trigger == Trigger.OnPlay)
                statusEffect.Apply(_cardMonster);
        }
    }
}