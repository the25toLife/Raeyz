using UnityEngine;
using System;
using System.Collections.Generic;

public enum EffectTrigger {
	
	TURNSTART, TURNEND, DRAW, DISCARD, SACRIFICE, MONSTERKILLED, MONSTERPLAYED, AUX
}

public abstract class Effect {

	public EffectTrigger Trigger { get; set; }
	/// <summary>
	/// The max number of turns the effect can last.
	/// </summary>
	public int TimeLimit = 1;
	/// <summary>
	/// The current number of turns the effect has been in play.
	/// </summary>
	public int LifeTime = 0;

	public Effect (EffectTrigger t) {
		Trigger = t;
	}

	public abstract bool canActivate (Card target);
	public abstract bool activate (Card target);

	public bool incrementLifeTime() {

		LifeTime++;
		if (LifeTime >= TimeLimit)
			return true;
		return false;
	}

	public void resetLifeTime() {

		LifeTime = 0;
	}

	public Effect clone() {

		return (Effect)this.MemberwiseClone ();
	}
}

public abstract class SStatEffect : Effect {

	public int AttackEff { get; set; }
	public int DefenseEff { get; set; }

	public SStatEffect (EffectTrigger t, int atkPar, int defPar) : base(t) {

		AttackEff = atkPar;
		DefenseEff = defPar;
	}

	public override bool canActivate(Card target) {
		if (target.CardInfo.GetCardType() != CardInfo.CardType.Monster)
			return false;
		return true;
	}
}

public class Effect402 : SStatEffect {

	public Effect402 () : base(EffectTrigger.AUX, 1, 0) {

	}

	public override bool activate (Card target) {
		if (!canActivate (target))
			return false;
		if ((target.CardInfo as MonsterInfo).GetAffinity() == CardInfo.CardAffinity.Light)
			AttackEff = 2;
		else
			AttackEff = 1;
		return true;
	}
}