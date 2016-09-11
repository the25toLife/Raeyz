using System;
using UnityEngine;
using UnityEngine.UI;

public enum Trigger
{
    OnPlay, OnTurn, OnKill, OnDeath
}

public abstract class StatusEffect
{
    public CardInfo.CardAffinity Affinity { get; set; }
    public Trigger Trigger { get; set; }

    protected StatusEffect()
    {
        Trigger = Trigger.OnPlay;
    }

    public abstract void Apply(Card c);
    public abstract void Remove(Card c, bool complete);
}

public class StatEffect : StatusEffect
{
    public int AttackMod { get; set; }
    public int DefenseMod { get; set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }

    public override void Apply(Card c)
    {
        if (Affinity != CardInfo.CardAffinity.All && c.CardInfo.GetAffinity() != Affinity) return;

        if (c.StatusEffects.Contains(this))
            Remove(c, false);
        else
            c.StatusEffects.Add(this);

        Attack = AttackMod;
        Defense = DefenseMod;

        if (Trigger == Trigger.OnKill)
        {
            CardMonster cardMonster = c as CardMonster;
            if (cardMonster == null) return;
            Attack = AttackMod * cardMonster.Kills;
            Defense = DefenseMod * cardMonster.Kills;
        }

        switch (c.CardInfo.GetCardType())
        {
            case CardInfo.CardType.Monster:
                CardMonster cardMonster = (CardMonster) c;
                MonsterInfo monsterInfo = (MonsterInfo) cardMonster.CardInfo;
                cardMonster.changeCard(monsterInfo + this);
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

    public override void Remove(Card c, bool complete)
    {
        if (Affinity != CardInfo.CardAffinity.All && c.CardInfo.GetAffinity() != Affinity) return;
        if (!c.StatusEffects.Contains(this)) return;
        switch (c.CardInfo.GetCardType())
        {
            case CardInfo.CardType.Monster:
                CardMonster cardMonster = (CardMonster) c;
                MonsterInfo monsterInfo = (MonsterInfo) cardMonster.CardInfo;
                cardMonster.changeCard(monsterInfo - this);
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
        if (complete) c.StatusEffects.Remove(this);
    }
}

public class ConfusionEffect : StatusEffect
{
    public float ChanceMod { get; set; }
    public float Chance { get; private set; }

    public override void Apply(Card c)
    {
        if (Affinity != CardInfo.CardAffinity.All && c.CardInfo.GetAffinity() != Affinity) return;

        if (!c.StatusEffects.Contains(this)) c.StatusEffects.Add(this);

        Chance = ChanceMod;

        if (Trigger == Trigger.OnKill)
        {
            CardMonster cardMonster = c as CardMonster;
            if (cardMonster == null) return;
            Chance = ChanceMod * cardMonster.Kills;
        }
    }

    public override void Remove(Card c, bool complete)
    {
        if (complete) c.StatusEffects.Remove(this);
    }
}

public class HealEffect : StatusEffect
{
    public int HealMod { get; set; }
    public int Heal { get; private set; }

    public override void Apply(Card c)
    {
        if (Affinity != CardInfo.CardAffinity.All && c.CardInfo.GetAffinity() != Affinity) return;

        if (!c.StatusEffects.Contains(this)) c.StatusEffects.Add(this);

        Heal = HealMod;

        if (Trigger == Trigger.OnKill)
        {
            CardMonster cardMonster = c as CardMonster;
            if (cardMonster == null) return;
            Heal = HealMod * cardMonster.Kills;
        }

        RaeyzPlayer player = c.IsEnemyCard ? c.Client.Game.EnemyPlayer : c.Client.Game.ClientPlayer;
        player.damagePlayer(-Heal);
    }

    public override void Remove(Card c, bool complete)
    {
        if (complete) c.StatusEffects.Remove(this);
    }
}