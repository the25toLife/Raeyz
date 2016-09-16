public class StatusEffect422 : HealthEffect
{
    protected override Card CardAppliedTo
    {
        set
        {
            base.CardAppliedTo = value;
            if (CardAppliedTo.CardInfo is MonsterInfo)
            {
                MonsterInfo monsterInfo = CardAppliedTo.CardInfo as MonsterInfo;
                HealthMod = -monsterInfo.Attack;
            }
        }
    }

    public StatusEffect422()
    {
        Lifetime = 0;
        AffectPlayer = true;
    }

    public override StatusEffect Clone()
    {
        return base.Clone(new StatusEffect422
        {
            HealthMod = HealthMod,
            Health = Health,
            AffectPlayer = AffectPlayer,
            AffectEnemy = AffectEnemy
        });
    }
}

public class StatusEffect423 : HealthEffect
{
   protected override Card CardAppliedTo
   {
       set
       {
           base.CardAppliedTo = value;
           if (CardAppliedTo.CardInfo is MonsterInfo)
           {
               MonsterInfo monsterInfo = CardAppliedTo.CardInfo as MonsterInfo;
               HealthMod = monsterInfo.Attack + monsterInfo.Defense;
           }
       }
   }

   public StatusEffect423()
   {
       Lifetime = 0;
       AffectPlayer = true;
   }

   public override StatusEffect Clone()
   {
       return base.Clone(new StatusEffect423
       {
           HealthMod = HealthMod,
           Health = Health,
           AffectPlayer = AffectPlayer,
           AffectEnemy = AffectEnemy
       });
   }
}

public class StatusEffect447 : HealthEffect
{
    protected override Card CardAppliedTo
    {
        set
        {
            base.CardAppliedTo = value;
            if (CardAppliedTo.CardInfo is MonsterInfo)
            {
                MonsterInfo monsterInfo = CardAppliedTo.CardInfo as MonsterInfo;
                HealthMod = -monsterInfo.Attack;
            }
        }
    }

    public StatusEffect447()
    {
        Lifetime = 0;
        AffectEnemy = true;
    }

    public override StatusEffect Clone()
    {
        return base.Clone(new StatusEffect447
        {
            HealthMod = HealthMod,
            Health = Health,
            AffectPlayer = AffectPlayer,
            AffectEnemy = AffectEnemy
        });
    }
}