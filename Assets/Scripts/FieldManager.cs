using System;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
public class FieldManager : MonoBehaviour {

    // Temporary public fields for debugging
    public int[] CardTypeCount = new int[Enum.GetNames(typeof(CardInfo.CardType)).Length];
    public int[] CardAffinityType = new int[Enum.GetNames(typeof(CardInfo.CardAffinity)).Length];
    private ArrayList _cardsOnField;

    [UsedImplicitly]
    private void Start()
    {
        _cardsOnField = new ArrayList();
    }

    [UsedImplicitly]
    private void Update()
    {
        Array types = Enum.GetValues(typeof(CardInfo.CardType));
        Array affinities = Enum.GetValues(typeof(CardInfo.CardAffinity));
        for (int i = 0; i < types.Length; i++)
            CardTypeCount[i] = GetOnFieldCardCount((CardInfo.CardType) types.GetValue(i), null);
        for (int i = 0; i < affinities.Length; i++)
            CardAffinityType[i] = GetOnFieldCardCount(null, (CardInfo.CardAffinity) affinities.GetValue(i));
    }

    public void AddCardToField(Card c)
    {
        _cardsOnField.Add(c);
    }

    public void RemoveCardFromField(Card c)
    {
        _cardsOnField.Remove(c);
    }

    public int GetOnFieldCardCount(CardInfo.CardType? cardTypePar, CardInfo.CardAffinity? cardAffinityPar)
    {
        var count = 0;
        foreach (Card c in _cardsOnField)
        {
            if ((!cardTypePar.HasValue || c.CardInfo.GetCardType().Equals(cardTypePar))
                && (!cardAffinityPar.HasValue || c.CardInfo.GetAffinity().Equals(cardAffinityPar)))
                count++;
        }
        return count;
    }
}
