using System;
using System.Linq;
using System.Text;
using UnityEngine;

// Currently completed triggers include: OnPlay, OnTurn, OnKill, OnAllyKilled, OnEnemyKilled, OnFieldChanged
// Still needing to be implemented: OnDeath
public enum Trigger
{
    OnPlay, OnTurn, OnKill, OnDeath, OnAllyKilled, OnEnemyKilled, OnFieldChange
}

public abstract class StatusEffect
{/*
    public const int UseAttackStat = 0x100;
    public const int UseDefenseStat = 0x101;*/

    /// <summary>Designates which cards the status effect can be applied to.</summary>
    public TargetCriteria TargetCriteria { get; set; }

    /// <summary>
    /// Designates when the status effect is applied and when it should be reapplied/updated.
    /// Defaults to <c>OnPlay</c>.
    /// </summary>
    public Trigger Trigger { get; set; }

    /// <summary>
    /// Designates which cards contribute to the status effect's <see cref="Multiplier"/>.
    /// Only applicable when the <see cref="Trigger"/> is set to <c>OnFieldChange</c>.
    /// </summary>
    /// <example>
    /// Used when a status effect increases the ATTACK stat by 2 per friendly ICE monster on the field.
    /// <code>
    /// StatEffect statEffect = new StatEffect
    /// {
    ///     Trigger = Trigger.OnFieldChange,
    ///     AffectingCardCriteria = new TargetCriteria
    ///     {
    ///         Affinities = {CardInfo.CardAffinity.Ice},
    ///         CardTypes = {CardInfo.CardType.Monster},
    ///         AllyOnly = true
    ///     },
    ///     AttackMod = 2
    /// }
    /// </code>
    /// </example>
    public TargetCriteria AffectingCardCriteria { get; set; }

    /// <summary>
    /// Used when an effect can be applied to one type of card, but only takes affect against another.
    /// </summary>
    /// <example>
    /// Used when a status effect increases the ATTACK stat by 3, but only against FOREST monsters.
    /// <code>
    /// StatEffect statEffect = new StatEffect
    /// {
    ///     AppliesAgainstCriteria = new TargetCriteria
    ///     {
    ///         Affinities = {CardInfo.CardAffinity.Forest},
    ///         CardTypes = {CardInfo.CardType.Monster}
    ///     },
    ///     AttackMod = 3
    /// }
    /// </code>
    /// </example>
    public TargetCriteria AppliesAgainstCriteria { get; set; }

    /// <summary>
    /// Gets the card that initially applied the status effect to its target.
    /// Used to keep track of the source of a status effect.
    /// </summary>
    public Card CardAppliedBy { get; protected set; }
    public int CardAppliedById { get; protected set; }

    /// <summary>
    /// The number of turns that the status effect will be in play.
    /// A value of 0 means the effect will immeadiately be removed and a value of -1 mean the effect will last
    /// until removed, either by another effect or the removal of the the source card.
    /// </summary>
    public int Lifetime { get; set; }

    protected virtual Card CardAppliedTo { get; set; }
    protected int Counter { get; set; }
    protected int Multiplier { get; set; }


    protected StatusEffect()
    {
        TargetCriteria = new TargetCriteria();
        AffectingCardCriteria = new TargetCriteria();
        Lifetime = -1;
        Trigger = Trigger.OnPlay;
    }

    public void AddToCard(Card cardAppliedTo, Card cardAppliedBy)
    {
        // Return if the status effect already exists on or cannot be applied to the target
        if (cardAppliedTo.StatusEffects.Contains(this) || !TargetCriteria.Matches(cardAppliedTo)) return;

        // Set and add status effect to the target and create necessary event handlers
        CardAppliedBy = cardAppliedBy;
        CardAppliedById = cardAppliedBy.CardInfo.GetId();
        CardAppliedTo = cardAppliedTo;

        CardAppliedBy.StateChangedEvent += OnCardStateChanged;
        CardAppliedTo.StateChangedEvent += OnCardStateChanged;

        CardAppliedTo.StatusEffects.Add(this);
        switch (Trigger)
        {
            case Trigger.OnPlay:
                Apply();
                break;
            case Trigger.OnKill:
                CardMonster cardMonster = CardAppliedTo as CardMonster;
                if (cardMonster == null) return;
                Counter = cardMonster.Kills;
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
        if (Lifetime < 1)
        {
            if (Lifetime == 0) Remove(true);
            return;
        }
        if (CardAppliedTo.IsEnemyCard)
            CardAppliedTo.Client.Game.EnemyTurnStart += OnTurn;
        else
            CardAppliedTo.Client.Game.TurnStart += OnTurn;
    }

    public virtual bool Apply()
    {
        // Return if the effect has not yet been applied to a card
        if (CardAppliedTo == null) return false;
        // Return if the card being affected is also under the effects of DISSIPATE
        if (CardAppliedTo.StatusEffects.OfType<DissipateEffect>().Any() && !(this is DissipateEffect)) return false;
        // If the status effect already exists on the target, clear it's effects so it can be reapplied.
        if (CardAppliedTo.StatusEffects.Contains(this))
            Remove(false);

        // Update the multiplier if necessary
        switch (Trigger)
        {
            case Trigger.OnKill:
                CardMonster cardMonster = CardAppliedTo as CardMonster;
                if (cardMonster == null) return false;
                Multiplier = cardMonster.Kills - Counter;
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

        // Check if the status effect should only take affect when targeting cards matching a specific criteria
        if (AppliesAgainstCriteria != null)
        {
            CardMonster cardMonster = CardAppliedTo as CardMonster;
            if (cardMonster == null) return false;

            // If current target does not match the criteria, then nullify the status effect
            Multiplier *= AppliesAgainstCriteria.Matches(cardMonster.Target) ? 1 : 0;
        }

        return true;
    }

    public virtual void Remove(bool complete)
    {
        // Return if the status effect does not exist on the target
        if (CardAppliedTo == null || !CardAppliedTo.StatusEffects.Contains(this)) return;

        // If a complete removal is needed, remove the status effect from the target and remove any event handlers
        if (complete)
        {
            CardAppliedBy.StateChangedEvent -= OnCardStateChanged;
            CardAppliedTo.StateChangedEvent -= OnCardStateChanged;
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

            if (Lifetime < 1) return;
            if (CardAppliedTo.IsEnemyCard)
                CardAppliedTo.Client.Game.EnemyTurnStart -= OnTurn;
            else
                CardAppliedTo.Client.Game.TurnStart -= OnTurn;
        }
    }

    private void ApplyOnTrigger(object sender, EventArgs e)
    {
        Apply();
    }

    private void OnCardStateChanged(object sender, EventArgs e)
    {
        StateChangedEventArgs args = e as StateChangedEventArgs;
        if (args == null) return;
        if (sender.Equals(CardAppliedBy))
        {
            if (args.State == Card.States.INHAND || args.State == Card.States.DISABLED)
                Remove(true);
        }
        else if (sender.Equals(CardAppliedTo))
        {
            if (args.State == Card.States.INHAND)
                Remove(true);
        }
    }

    private void OnTurn(object sender, EventArgs e)
    {
        if (Lifetime < 1)
            Remove(true);
        else
            Lifetime--;
    }

    public abstract StatusEffect Clone();

    protected StatusEffect Clone(StatusEffect statusEffect)
    {
        statusEffect.TargetCriteria = TargetCriteria;
        statusEffect.Trigger = Trigger;
        statusEffect.AffectingCardCriteria = AffectingCardCriteria;
        statusEffect.AppliesAgainstCriteria = AppliesAgainstCriteria;
        statusEffect.Counter = Counter;
        statusEffect.Multiplier = Multiplier;
        statusEffect.Lifetime = Lifetime;
        return statusEffect;
    }

    protected string ToString(string str)
    {
        if (Lifetime > 0)
        {
            StringBuilder stringBuilder = new StringBuilder(str);
            stringBuilder.Append("<size=75>");
            stringBuilder.AppendFormat("{0} turn{1} left", Lifetime, Lifetime > 1 ? "s" : "");
            stringBuilder.Append("</size>");
            return stringBuilder.ToString();
        }

        return str;
    }
}

public class StatEffect : StatusEffect
{
    public const int Zero = 0x200;
    public const int Double = 0x201;

    public int AttackMod { get; set; }
    public int DefenseMod { get; set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }

    public override bool Apply()
    {
        if (!base.Apply()) return false;

        switch (CardAppliedTo.CardInfo.GetCardType())
        {
            case CardInfo.CardType.Monster:
                CardMonster cardMonster = (CardMonster) CardAppliedTo;
                MonsterInfo monsterInfo = (MonsterInfo) cardMonster.CardInfo;

                if (AttackMod == Double) AttackMod = monsterInfo.Attack;
                else if (AttackMod == Zero) AttackMod = -monsterInfo.Attack;

                if (DefenseMod == Double) DefenseMod = monsterInfo.Defense;
                else if (DefenseMod == Zero) DefenseMod = -monsterInfo.Defense;

                Attack = AttackMod * Multiplier;
                Defense = DefenseMod * Multiplier;
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

        return true;
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

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (AttackMod != 0)
        {
            stringBuilder.AppendFormat("{0}{1} ATK", Attack > 0 ? "+ " : "", Attack);
            stringBuilder.AppendLine();
        }
        if (DefenseMod != 0)
        {
            stringBuilder.AppendFormat("{0}{1} DEF", Defense > 0 ? "+ " : "", Defense);
            stringBuilder.AppendLine();
        }
        if (Trigger != Trigger.OnPlay && Trigger != Trigger.OnDeath)
        {
            stringBuilder.Append("<size=65>");
            if (AttackMod != 0) stringBuilder.AppendFormat("{0}{1} ATK", Attack > 0 ? "+" : "", Attack);
            if (AttackMod != 0 && DefenseMod != 0) stringBuilder.Append("  ");
            if (DefenseMod != 0) stringBuilder.AppendFormat("{0}{1} DEF", Defense > 0 ? "+" : "", Defense);
            stringBuilder.AppendLine();
            switch (Trigger)
            {
                case Trigger.OnPlay:
                    stringBuilder.Append("on play");
                    break;
                case Trigger.OnTurn:
                    stringBuilder.Append("per turn");
                    break;
                case Trigger.OnKill:
                    stringBuilder.Append("per kill");
                    break;
                case Trigger.OnDeath:
                    stringBuilder.Append("on death");
                    break;
                case Trigger.OnAllyKilled:
                    stringBuilder.Append("/ ally killed");
                    break;
                case Trigger.OnEnemyKilled:
                    stringBuilder.Append("/ enemy killed");
                    break;
                case Trigger.OnFieldChange:
                    stringBuilder.Append("on condition");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            stringBuilder.Append("</size>");
        }
        return base.ToString(stringBuilder.ToString());
    }
}

public class ConfusionEffect : StatusEffect
{
    public float ChanceMod { get; set; }
    public float Chance { get; private set; }

    public override bool Apply()
    {
        if (!base.Apply()) return false;

        Chance = ChanceMod * Multiplier;
        if (Chance > 1.0f) Chance = 1.0f;

        return true;
    }

    public override StatusEffect Clone()
    {
        return base.Clone(new ConfusionEffect
        {
            ChanceMod = ChanceMod,
            Chance = Chance
        });
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("{0}{1}% CON", Chance < 0 ? "- " : Chance > 0 ? "+ " : "", Chance * 100);
        stringBuilder.AppendLine();
        return base.ToString(stringBuilder.ToString());
    }
}

public class HealthEffect : StatusEffect
{
    public int HealthMod { get; set; }
    public int Health { get; protected set; }
    public bool AffectPlayer { get; set; }
    public bool AffectEnemy { get; set; }

    public override bool Apply()
    {
        if (!base.Apply()) return false;


        Health = HealthMod;

        RaeyzPlayer player;
        if (AffectPlayer)
        {
            player = CardAppliedTo.Client.Game.ClientPlayer;
            player.damagePlayer(-Health);
        }
        if (AffectEnemy)
        {
            player = CardAppliedTo.Client.Game.EnemyPlayer;
            player.damagePlayer(-Health);
        }

        return true;
    }

/*    public override void Remove(bool complete)
    {
        base.Remove(complete);
        RaeyzPlayer player;
        if (AffectPlayer)
        {
            player = CardAppliedTo.Client.Game.ClientPlayer;
            player.damagePlayer(Health);
        }
        if (AffectEnemy)
        {
            player = CardAppliedTo.Client.Game.EnemyPlayer;
            player.damagePlayer(Health);
        }
    }*/

    public override StatusEffect Clone()
    {
        return base.Clone(new HealthEffect
        {
            HealthMod = HealthMod,
            Health = Health,
            AffectPlayer = AffectPlayer,
            AffectEnemy = AffectEnemy
        });
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("{0}{1} LP", HealthMod < 0 ? "- " : HealthMod > 0 ? "+ " : "", HealthMod);
        stringBuilder.AppendLine();
        stringBuilder.Append("<size=75>");
        stringBuilder.AppendFormat("to {0}", AffectPlayer ? AffectEnemy ? "both players" : "the player" : "the enemy");
        stringBuilder.AppendLine();
        switch (Trigger)
        {
            case Trigger.OnPlay:
                stringBuilder.Append("on play");
                break;
            case Trigger.OnTurn:
                stringBuilder.Append("per turn");
                break;
            case Trigger.OnKill:
                stringBuilder.Append("per kill");
                break;
            case Trigger.OnDeath:
                stringBuilder.Append("on death");
                break;
            case Trigger.OnAllyKilled:
                stringBuilder.Append("/ ally killed");
                break;
            case Trigger.OnEnemyKilled:
                stringBuilder.Append("/ enemy killed");
                break;
            case Trigger.OnFieldChange:
                stringBuilder.Append("on condition");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        stringBuilder.Append("</size>");
        return base.ToString(stringBuilder.ToString());
    }
}

public class DissipateEffect : StatusEffect
{
    public override bool Apply()
    {
        if (!base.Apply()) return false;

        foreach (var statusEffect in CardAppliedTo.StatusEffects.ToArray())
        {
            if (statusEffect != this)
                statusEffect.Remove(statusEffect.CardAppliedBy == null);
        }

        return true;
    }

    public override void Remove(bool complete)
    {
        base.Remove(complete);

        if (complete)
        {
            foreach (var statusEffect in CardAppliedTo.StatusEffects.ToArray())
            {
                statusEffect.Remove(true);
                statusEffect.AddToCard(CardAppliedTo, statusEffect.CardAppliedBy);
            }
        }
    }

    public override StatusEffect Clone()
    {
        return base.Clone(new DissipateEffect());
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("IMMUNE");
        stringBuilder.Append("<size=75>");
        stringBuilder.AppendFormat("to status effects");
        stringBuilder.AppendLine();
        stringBuilder.Append("</size>");
        return base.ToString(stringBuilder.ToString());
    }
}

public class ReturnCardEffect : StatusEffect
{
    public bool UnawakenMonster { get; set; }

    public ReturnCardEffect()
    {
        Lifetime = 0;
        UnawakenMonster = false;
    }

    public override bool Apply()
    {
        if (!base.Apply()) return false;

        CardAppliedTo.ReturnToHand();
        if (CardAppliedTo is CardMonster && UnawakenMonster)
        {
            CardMonster cardMonster = (CardMonster) CardAppliedTo;
            cardMonster.SetAwake(false);
        }

        return true;
    }

    public override StatusEffect Clone()
    {
        return base.Clone(new ReturnCardEffect
        {
            UnawakenMonster = UnawakenMonster
        });
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("RETURN");
        stringBuilder.Append("<size=75>");
        stringBuilder.AppendFormat("to hand");
        stringBuilder.AppendLine();
        stringBuilder.Append("</size>");
        return base.ToString(stringBuilder.ToString());
    }
}

/*public class TimeEffect : StatusEffect
{
    public int LifetimeMod { get; set; }
    public int LifetimeChange { get; private set; }

    public TimeEffect()
    {
        Lifetime = 0;
    }

    public override bool Apply()
    {
        if (!base.Apply()) return false;

        foreach (var statusEffect in CardAppliedTo.StatusEffects.ToArray())
        {
            if (statusEffect != this)
                statusEffect.Remove(statusEffect.CardAppliedBy == null);
        }

        return true;
    }

    public override StatusEffect Clone()
    {
        return base.Clone(new TimeEffect());
    }
}*/