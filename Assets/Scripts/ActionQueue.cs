using UnityEngine;
using System;

public class ActionQueue
{

	public ActionQueue ()
	{

	}

	public static void calcAttack(Card mi1, Card mi2, bool defending) {

		int attackerStat = (mi1.CardInfo as MonsterInfo).Attack;
		int targetStat = defending ? (mi2.CardInfo as MonsterInfo).Defense : (mi2.CardInfo as MonsterInfo).Attack;

	    if (attackerStat > targetStat)
	    {
	        (mi1 as CardMonster).Kills++;
	        foreach (StatusEffect statusEffect in mi1.StatusEffects)
	        {
	            if (statusEffect.Trigger == Trigger.OnKill) statusEffect.Apply(mi1);
	        }
	        mi2.sendCardToGraveyard();
	    }
		else if (attackerStat < targetStat)
	    {
	        (mi2 as CardMonster).Kills++;
	        foreach (StatusEffect statusEffect in mi1.StatusEffects)
	        {
	            if (statusEffect.Trigger == Trigger.OnKill) statusEffect.Apply(mi1);
	        }
	        mi1.sendCardToGraveyard();
	    }
	    else
	    {
	        if (defending)
	        {
	            (mi2 as CardMonster).Kills++;
	            foreach (StatusEffect statusEffect in mi1.StatusEffects)
	            {
	                if (statusEffect.Trigger == Trigger.OnKill) statusEffect.Apply(mi1);
	            }
	        }
	        else
	            mi2.sendCardToGraveyard();
	        mi1.sendCardToGraveyard();
	    }
	}
}

