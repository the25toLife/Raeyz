using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardUnique : Card
{
    public override void changeCard(CardInfo ci)
    {

        CardInfo = ci as SpecialInfo;
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
            Debug.LogError("Cannot change unique card (UID:" + UID + ") to contain " + ci.GetType());
        }
    }

/*    public override void endDrag()
    {
        if (Math.Abs(transform.position.y) < 0.715f) OnPlay(null);
        base.endDrag();
    }*/

    public override bool canTarget(Card target)
    {
        return (CardInfo.TargetCriteria.Matches(target) && !target.StatusEffects.OfType<DissipateEffect>().Any());
    }

    public override void assignTarget(Card target)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlay(Card target)
    {
  /*      foreach (StatusEffect statusEffect in ((SpecialInfo) CardInfo).StatusEffects)
        {
            if (target == null && statusEffect.TargetCriteria.AffinitiesBlacklist.Contains(CardInfo.CardAffinity.All))
                statusEffect.Clone().AddToCard(target, this);
        }*/
        OnPlay();
        foreach (StatusEffect statusEffect in ((SpecialInfo) CardInfo).StatusEffects)
            statusEffect.Clone().AddToCard(target, this);
        Client.Game.SendPlayCardWithTargetEv(UID, target.UID);
        sendCardToGraveyard();
    }
}