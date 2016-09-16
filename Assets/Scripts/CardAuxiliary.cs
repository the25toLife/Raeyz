using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardAuxiliary : Card
{

    private CardMonster _cardMonster;

    public override void Update()
    {
        base.Update();
        if (State == States.INPLAY && _cardMonster == null) Destroy(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (_cardMonster == null) return;
        foreach (StatusEffect statusEffect in _cardMonster.StatusEffects.ToArray())
        {
            if (statusEffect.CardAppliedBy != null && statusEffect.CardAppliedBy.UID == UID) statusEffect.Remove(true);
        }
    }

    public override void changeCard (CardInfo ci)
	{

	    CardInfo = ci as AuxiliaryInfo;
	    if (CardInfo != null)
	    {
	        var s = Resources.Load("Cards/" + CardInfo.GetId(), typeof(Sprite)) as Sprite;
	        if (s != null)
	        {
	            Image image = transform.Find("CardImage").GetComponent<Image>();
	            image.sprite = s;
	        }
	        foreach (CardStatComponent csc in GetComponentsInChildren<CardStatComponent>(true))
	            csc.changeStat(CardInfo);
	    }
	    else
	    {
	        Debug.LogError("Cannot change auxiliary card (UID:" + UID + ") to contain " + ci.GetType());
	    }
	}

	public override bool canTarget (Card target)
	{
	    return (CardInfo.TargetCriteria.Matches(target) && !target.StatusEffects.OfType<DissipateEffect>().Any());
	}

	public override void assignTarget (Card target) {

	}

    public void OnPlay(CardMonster target)
    {
        OnPlay();
        _cardMonster = target;
        foreach (StatusEffect statusEffect in ((AuxiliaryInfo) CardInfo).StatusEffects)
            statusEffect.Clone().AddToCard(_cardMonster, this);
    }
}