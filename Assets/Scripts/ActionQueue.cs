using UnityEngine;
using System;

public class ActionQueue
{

    public static event EventHandler AllyKilled;
    public static event EventHandler EnemyKilled;

    public static int AlliesKilled { get; set; }
    public static int EnemiesKilled { get; set; }

	public static void calcAttack(Card mi1, Card mi2, bool defending) {

		int attackerStat = (mi1.CardInfo as MonsterInfo).Attack;
		int targetStat = defending ? (mi2.CardInfo as MonsterInfo).Defense : (mi2.CardInfo as MonsterInfo).Attack;

	    if (attackerStat > targetStat)
	    {
	        (mi1 as CardMonster).Kills++;
	        foreach (StatusEffect statusEffect in mi1.StatusEffects)
	        {
	            if (statusEffect.Trigger == Trigger.OnKill) statusEffect.Apply();
	        }
	        if (mi2.IsEnemyCard)
	            OnEnemyKilled();
	        else
	            OnAllyKilled();
	        mi2.sendCardToGraveyard();
	    }
		else if (attackerStat < targetStat)
	    {
	        (mi2 as CardMonster).Kills++;
	        foreach (StatusEffect statusEffect in mi1.StatusEffects)
	        {
	            if (statusEffect.Trigger == Trigger.OnKill) statusEffect.Apply();
	        }
	        if (mi1.IsEnemyCard)
	            OnEnemyKilled();
	        else
	            OnAllyKilled();
	        mi1.sendCardToGraveyard();
	    }
	    else
	    {
	        if (defending)
	        {
	            (mi2 as CardMonster).Kills++;
	            foreach (StatusEffect statusEffect in mi1.StatusEffects)
	            {
	                if (statusEffect.Trigger == Trigger.OnKill) statusEffect.Apply();
	            }
	        }
	        else
	        {
	            if (mi2.IsEnemyCard)
	                OnEnemyKilled();
	            else
	                OnAllyKilled();
	            mi2.sendCardToGraveyard();
	        }
	        if (mi1.IsEnemyCard)
	            OnEnemyKilled();
	        else
	            OnAllyKilled();
	        mi1.sendCardToGraveyard();
	    }
	}

    private static void OnAllyKilled()
    {
        AlliesKilled++;
        EventHandler handler = AllyKilled;
        // ReSharper disable once UseNullPropagation
        if (handler != null) handler.Invoke(null, EventArgs.Empty);
    }

    private static void OnEnemyKilled()
    {
        EnemiesKilled++;
        EventHandler handler = EnemyKilled;
        // ReSharper disable once UseNullPropagation
        if (handler != null) handler.Invoke(null, EventArgs.Empty);
    }
}

