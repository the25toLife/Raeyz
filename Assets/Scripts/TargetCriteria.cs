using System.Collections.Generic;

public class TargetCriteria
{
    public int LevelMin { get; set; }
    public int LevelMax { get; set; }
    public List<CardInfo.CardAffinity> Affinities { get; set; }
    public List<CardInfo.CardType> CardTypes { get; set; }
    public List<int> CardIds { get; set; }
    public bool AllyOnly { get; set; }
    public bool EnemyOnly { get; set; }

    public TargetCriteria()
    {
        LevelMin = LevelMax = -1;
        Affinities = new List<CardInfo.CardAffinity>();
        CardTypes = new List<CardInfo.CardType>();
        CardIds = new List<int>();
        AllyOnly = EnemyOnly = false;
    }

    public bool Matches(Card card)
    {
        // Check level requirements if they exist
        if (LevelMin > -1 || LevelMax > -1)
        {
            MonsterInfo monsterInfo = card.CardInfo as MonsterInfo;
            if (monsterInfo == null) return false;
            if (LevelMin > -1 && monsterInfo.GetLevel() < LevelMin) return false;
            if (LevelMax > -1 && monsterInfo.GetLevel() > LevelMax) return false;
        }

        // Check affinity requirements if they exist
        if (Affinities.Count > 0 && !Affinities.Contains(CardInfo.CardAffinity.All)
            && !Affinities.Contains(card.CardInfo.GetAffinity())) return false;

        // Check card type requirements if they exist
        if (CardTypes.Count > 0 && !CardTypes.Contains(card.CardInfo.GetCardType())) return false;

        // Check player affiliation requirements if they exist
        // If AllyOnly and EnemyOnly equal either player is assumed to be a valid target
        if (AllyOnly != EnemyOnly)
        {
            if (AllyOnly && card.IsEnemyCard) return false;
            if (EnemyOnly && !card.IsEnemyCard) return false;
        }

        return true;
    }
}