using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

public static class FieldManager {

//    public int[] CardTypeCount = new int[Enum.GetNames(typeof(CardInfo.CardType)).Length];
//    public int[] CardAffinityType = new int[Enum.GetNames(typeof(CardInfo.CardAffinity)).Length];

    private static readonly List<Card> CardsOnField = new List<Card>();
    public static event EventHandler OnFieldChanged;

    public static void AddCardToField(Card c)
    {
        CardsOnField.Add(c);
        EventHandler handler = OnFieldChanged;
        if (handler != null) handler.Invoke(null, EventArgs.Empty);
    }

    public static void RemoveCardFromField(Card c)
    {
        CardsOnField.Remove(c);
        EventHandler handler = OnFieldChanged;
        if (handler != null) handler.Invoke(null, EventArgs.Empty);
    }

    public static int GetOnFieldCardCount(TargetCriteria targetCriteria)
    {
        var count = 0;
        foreach (Card card in CardsOnField)
        {
            if (targetCriteria.Matches(card))
                count++;
        }
        return count;
    }

    public static List<Card> SearchOnFieldCards(TargetCriteria targetCriteria)
    {
        List<Card> cardsOnField = new List<Card>();
        foreach (Card card in CardsOnField)
            if (targetCriteria.Matches(card)) cardsOnField.Add(card);
        return cardsOnField;
    }

    public static List<Card> SearchAllCards(TargetCriteria targetCriteria)
    {
        List<Card> cardsOnField = new List<Card>();
        foreach (Card card in Object.FindObjectsOfType<Card>())
            if (targetCriteria.Matches(card)) cardsOnField.Add(card);
        return cardsOnField;
    }

    public static int GetOnFieldCardCount(CardInfo.CardType? cardTypePar, CardInfo.CardAffinity? cardAffinityPar)
    {
        var count = 0;
        foreach (Card c in CardsOnField)
        {
            if ((!cardTypePar.HasValue || c.CardInfo.GetCardType().Equals(cardTypePar))
                && (!cardAffinityPar.HasValue || c.CardInfo.GetAffinity().Equals(cardAffinityPar)))
                count++;
        }
        return count;
    }

    public static ArrayList SearchOnFieldCards(CardInfo.CardType? cardTypePar, CardInfo.CardAffinity? cardAffinityPar)
    {
        ArrayList cardsOnField = new ArrayList();
        foreach (Card c in CardsOnField)
        {
            if ((!cardTypePar.HasValue || c.CardInfo.GetCardType().Equals(cardTypePar))
                && (!cardAffinityPar.HasValue || c.CardInfo.GetAffinity().Equals(cardAffinityPar)))
                cardsOnField.Add(c);
        }
        return cardsOnField;
    }
}
