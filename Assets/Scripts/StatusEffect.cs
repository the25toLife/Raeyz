using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class StatusEffect
{
    public CardInfo.CardAffinity Affinity { get; set; }
    public abstract void Apply(Card c);
    public abstract void Remove(Card c);
}

public class StatChangeEffect : StatusEffect
{
    public int Attack { get; set; }
    public int Defense { get; set; }

    public override void Apply(Card c)
    {
        if (Affinity != CardInfo.CardAffinity.All && c.CardInfo.GetAffinity() != Affinity) return;
        switch (c.CardInfo.GetCardType())
        {
            case CardInfo.CardType.Monster:
                CardMonster cardMonster = (CardMonster) c;
                MonsterInfo monsterInfo = (MonsterInfo) cardMonster.CardInfo;
                cardMonster.changeCard(monsterInfo + this);
                foreach (var csc in cardMonster.GetComponentsInChildren<CardStatComponent>())
                {
                    switch (csc.statType)
                    {
                        case CardStatComponent.StatType.ATTACK:
                            if (Attack < 0)
                                csc.GetComponent<Image>().color = new Color(0.843f, 0.102f, 0.082f);
                            else if (Attack > 0)
                                csc.GetComponent<Image>().color = new Color(0.063f, 0.647f, 0.900f);
                            break;
                        case CardStatComponent.StatType.DEFENSE:
                            if (Defense < 0)
                                csc.GetComponent<Image>().color = new Color(0.843f, 0.102f, 0.082f);
                            else if (Defense > 0)
                                csc.GetComponent<Image>().color = new Color(0.063f, 0.647f, 0.900f);
                            break;
                    }
                }
                break;
            case CardInfo.CardType.Auxiliary:
                break;
            case CardInfo.CardType.Ruse:
                break;
            case CardInfo.CardType.Unique:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void Remove(Card c)
    {
        if (Affinity != CardInfo.CardAffinity.All && c.CardInfo.GetAffinity() != Affinity) return;
        switch (c.CardInfo.GetCardType())
        {
            case CardInfo.CardType.Monster:
                CardMonster cardMonster = (CardMonster) c;
                MonsterInfo monsterInfo = (MonsterInfo) cardMonster.CardInfo;
                cardMonster.changeCard(monsterInfo - this);
                foreach (var csc in cardMonster.GetComponentsInChildren<CardStatComponent>())
                    csc.GetComponent<Image>().color = Color.white;
                break;
            case CardInfo.CardType.Auxiliary:
                break;
            case CardInfo.CardType.Ruse:
                break;
            case CardInfo.CardType.Unique:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}