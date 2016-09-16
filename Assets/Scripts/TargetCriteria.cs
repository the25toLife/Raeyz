using System.Collections.Generic;

public class TargetCriteria
{
    public int LevelMin { get; set; }
    public int LevelMax { get; set; }
    public List<CardInfo.CardAffinity> Affinities { get; set; }
    public List<CardInfo.CardAffinity> AffinitiesBlacklist { get; set; }
    public List<CardInfo.CardType> CardTypes { get; set; }
    public List<CardInfo.CardType> CardTypesBlacklist { get; set; }
    public List<int> CardIds { get; set; }
    public List<int> CardIdsBlacklist { get; set; }
    public List<Card.States> States { get; set; }
    public bool AllyOnly { get; set; }
    public bool EnemyOnly { get; set; }

    public TargetCriteria()
    {
        LevelMin = LevelMax = -1;
        Affinities = new List<CardInfo.CardAffinity>();
        AffinitiesBlacklist = new List<CardInfo.CardAffinity>();
        CardTypes = new List<CardInfo.CardType>();
        CardTypesBlacklist = new List<CardInfo.CardType>();
        CardIds = new List<int>();
        CardIdsBlacklist = new List<int>();
        States = new List<Card.States> {Card.States.INPLAY};
        AllyOnly = EnemyOnly = false;
    }

    public bool Matches(Card card)
    {
        if (card == null) return false;

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
        if (AffinitiesBlacklist.Count > 0 && (AffinitiesBlacklist.Contains(CardInfo.CardAffinity.All)
            || AffinitiesBlacklist.Contains(card.CardInfo.GetAffinity()))) return false;

        // Check card type requirements if they exist
        if (CardTypes.Count > 0 && !CardTypes.Contains(card.CardInfo.GetCardType())) return false;
        if (CardTypesBlacklist.Count > 0 && CardTypesBlacklist.Contains(card.CardInfo.GetCardType())) return false;

        // Check card ID requirements if they exist
        if (CardIds.Count > 0 && !CardIds.Contains(card.CardInfo.GetId())) return false;
        if (CardIdsBlacklist.Count > 0 && CardIdsBlacklist.Contains(card.CardInfo.GetId())) return false;

        // Check card state requirements if they exist
        if (States.Count > 0 && !States.Contains(card.State)) return false;

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