using System;

// Currently completed triggers include: OnPlay, OnTurn, OnKill, OnAllyKilled, OnEnemyKilled, OnFieldChanged
// Still needing to be implemented: OnDeath
public enum Trigger
{
    OnPlay, OnTurn, OnKill, OnDeath, OnAllyKilled, OnEnemyKilled, OnFieldChange
}

public abstract class StatusEffect
{
    public TargetCriteria TargetCriteria { get; set; }
    public Trigger Trigger { get; set; }
    public TargetCriteria AffectingCardCriteria { get; set; }
    public int Counter { get; set; }
    protected int Multiplier { get; set; }
    protected Card CardAppliedTo { get; set; }
    public Card CardAppliedBy { get; set; }

    protected StatusEffect()
    {
        TargetCriteria = new TargetCriteria();
        AffectingCardCriteria = new TargetCriteria();
        Trigger = Trigger.OnPlay;
    }

    public void AddToCard(Card cardAppliedTo, Card cardAppliedBy)
    {
        // Return if the status effect already exists on or cannot be applied to the target
        if (cardAppliedTo.StatusEffects.Contains(this) || !TargetCriteria.Matches(cardAppliedTo)) return;

        // Set and add status effect to the target and create necessary event handlers
        CardAppliedBy = cardAppliedBy;
        CardAppliedTo = cardAppliedTo;
        CardAppliedTo.StatusEffects.Add(this);
        switch (Trigger)
        {
            case Trigger.OnPlay:
                Apply();
                break;
            case Trigger.OnTurn:
                if (cardAppliedTo.IsEnemyCard)
                    CardAppliedTo.Client.Game.EnemyTurnStart += ApplyOnTrigger;
                else
                    CardAppliedTo.Client.Game.TurnStart += ApplyOnTrigger;
                break;
            case Trigger.OnAllyKilled:
                Counter = ActionQueue.AlliesKilled;
                if (CardAppliedTo.IsEnemyCard)
                    ActionQueue.EnemyKilled += ApplyOnTrigger;
                else
                    ActionQueue.AllyKilled += ApplyOnTrigger;
                break;
            case Trigger.OnEnemyKilled:
                Counter = ActionQueue.EnemiesKilled;
                if (CardAppliedTo.IsEnemyCard)
                    ActionQueue.AllyKilled += ApplyOnTrigger;
                else
                    ActionQueue.EnemyKilled += ApplyOnTrigger;
                break;
            case Trigger.OnFieldChange:
                Apply();
                FieldManager.OnFieldChanged += ApplyOnTrigger;
                break;
        }
    }

    public virtual void Apply()
    {
        // Return if the effect has not yet been applied to a card
        if (CardAppliedTo == null) return;
        // If the status effect already exists on the target, clear it's effects so it can be reapplied.
        if (CardAppliedTo.StatusEffects.Contains(this))
            Remove(false);

        // Update the multiplier if necessary
        switch (Trigger)
        {
            case Trigger.OnKill:
                CardMonster cardMonster = CardAppliedTo as CardMonster;
                if (cardMonster == null) return;
                Multiplier = cardMonster.Kills;
                break;
            case Trigger.OnAllyKilled:
                Multiplier = ActionQueue.AlliesKilled - Counter;
                break;
            case Trigger.OnEnemyKilled:
                Multiplier = ActionQueue.EnemiesKilled - Counter;
                break;
            case Trigger.OnFieldChange:
                Multiplier = FieldManager.GetOnFieldCardCount(AffectingCardCriteria);
                break;
            default:
                Multiplier = 1;
                break;
        }
    }

    public virtual void Remove(bool complete)
    {
        // Return if the status effect does not exist on the target
        if (CardAppliedTo == null || !CardAppliedTo.StatusEffects.Contains(this)) return;

        // If a complete removal is needed, remove the status effect from the target and remove any event handlers
        if (complete)
        {
            CardAppliedTo.StatusEffects.Remove(this);
            switch (Trigger)
            {
                case Trigger.OnTurn:
                    if (CardAppliedTo.IsEnemyCard)
                        CardAppliedTo.Client.Game.EnemyTurnStart -= ApplyOnTrigger;
                    else
                        CardAppliedTo.Client.Game.TurnStart -= ApplyOnTrigger;
                    break;
                case Trigger.OnAllyKilled:
                    Counter = ActionQueue.AlliesKilled;
                    if (CardAppliedTo.IsEnemyCard)
                        ActionQueue.EnemyKilled -= ApplyOnTrigger;
                    else
                        ActionQueue.AllyKilled -= ApplyOnTrigger;
                    break;
                case Trigger.OnEnemyKilled:
                    Counter = ActionQueue.EnemiesKilled;
                    if (CardAppliedTo.IsEnemyCard)
                        ActionQueue.AllyKilled -= ApplyOnTrigger;
                    else
                        ActionQueue.EnemyKilled -= ApplyOnTrigger;
                    break;
                case Trigger.OnFieldChange:
                    FieldManager.OnFieldChanged -= ApplyOnTrigger;
                    break;
            }
        }
    }

    private void ApplyOnTrigger(object sender, EventArgs e)
    {
        Apply();
    }

    public abstract StatusEffect Clone();

    protected StatusEffect Clone(StatusEffect statusEffect)
    {
        statusEffect.TargetCriteria = TargetCriteria;
        statusEffect.Trigger = Trigger;
        statusEffect.AffectingCardCriteria = AffectingCardCriteria;
        statusEffect.Counter = Counter;
        statusEffect.Multiplier = Multiplier;
        return statusEffect;
    }
}

public class StatEffect : StatusEffect
{
    public int AttackMod { get; set; }
    public int DefenseMod { get; set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }

    public override void Apply()
    {
        base.Apply();

        Attack = AttackMod * Multiplier;
        Defense = DefenseMod * Multiplier;

        switch (CardAppliedTo.CardInfo.GetCardType())
        {
            case CardInfo.CardType.Monster:
                CardMonster cardMonster = (CardMonster) CardAppliedTo;
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

    public override void Remove(bool complete)
    {
        base.Remove(complete);

        switch (CardAppliedTo.CardInfo.GetCardType())
        {
            case CardInfo.CardType.Monster:
                CardMonster cardMonster = (CardMonster) CardAppliedTo;
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
    }

    public override StatusEffect Clone()
    {
        return base.Clone(new StatEffect
        {
            AttackMod = AttackMod,
            DefenseMod = DefenseMod,
            Attack = Attack,
            Defense = Defense
        });
    }
}

public class ConfusionEffect : StatusEffect
{
    public float ChanceMod { get; set; }
    public float Chance { get; private set; }

    public override void Apply()
    {
        base.Apply();

        Chance = ChanceMod * Multiplier;
    }

    public override StatusEffect Clone()
    {
        return base.Clone(new ConfusionEffect
        {
            ChanceMod = ChanceMod,
            Chance = Chance
        });
    }

}

public class HealEffect : StatusEffect
{
    public int HealMod { get; set; }
    public int Heal { get; private set; }

    public override void Apply()
    {
        base.Apply();

        Heal = HealMod * Multiplier;

        RaeyzPlayer player = CardAppliedTo.IsEnemyCard ? CardAppliedTo.Client.Game.EnemyPlayer : CardAppliedTo.Client.Game.ClientPlayer;
        player.damagePlayer(-Heal);
    }

    public override StatusEffect Clone()
    {
        return base.Clone(new HealEffect
        {
            HealMod = HealMod,
            Heal = Heal
        });
    }
}