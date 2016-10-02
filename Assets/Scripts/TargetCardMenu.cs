using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TargetCardMenu : MonoBehaviour
{
    public GameObject TargetCardMenuItem;

    public Card Card { get; set; }
    public List<Card> SelectedCards { get; private set; }

    private GameObject _menuObject, _cardCanvas;
    private bool _allowMultipleSelections;

    [UsedImplicitly]
    private void Awake()
    {
        _menuObject = transform.Find("Canvas").gameObject;
        _cardCanvas = _menuObject.transform.Find("Card").gameObject;
    }

    [UsedImplicitly]
    private void Start()
    {
        SelectedCards = new List<Card>();
    }

    [UsedImplicitly]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _menuObject.activeSelf)
            CloseMenu();
    }

    public void OpenMenu(Card card, bool allowMultipleSelections)
    {
        if (card == null) return;

        Card = card;
        _allowMultipleSelections = allowMultipleSelections;

        bool isMonster = Card is CardMonster;

        _cardCanvas.transform.Find("unlockTrigger").gameObject.SetActive(isMonster);
        _cardCanvas.transform.Find("statOverlay").gameObject.SetActive(isMonster);
        _cardCanvas.transform.Find("ActivateTrigger").gameObject.SetActive(!isMonster);

        foreach (var statComponent in _menuObject.GetComponentsInChildren<CardStatComponent>())
            statComponent.changeStat(card.CardInfo);

        GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = true;
        _menuObject.SetActive(true);

        TargetCriteria validTargetCriteria = new TargetCriteria
            {
                CardTypes = {CardInfo.CardType.Monster},
                AllyOnly = true,
                States = new List<Card.States>()
            };
        if (isMonster)
        {
            MonsterInfo monsterInfo = card.CardInfo as MonsterInfo;
            if (monsterInfo != null)
            {
                if (monsterInfo.GetLevel() > 7) validTargetCriteria.LevelMin = 5;
            }
        }
        else
        {
            validTargetCriteria = card.CardInfo.TargetCriteria;
        }
        foreach (var targetCard in FieldManager.SearchAllCards(validTargetCriteria))
        {
            if (targetCard == Card) continue;
            GameObject targetCardMenuItem = Instantiate(TargetCardMenuItem);
            targetCardMenuItem.transform.SetParent(_menuObject.transform);
            CardMenuSelect cardMenuSelect = targetCardMenuItem.GetComponent<CardMenuSelect>();
            if (cardMenuSelect != null)
            {
                cardMenuSelect.AllowMultipleSelections = allowMultipleSelections;
                cardMenuSelect.setCard(targetCard);
            }
            else
            {
                throw new Exception("No CardMenuSelect component found.");
            }
        }
    }

    private void CloseMenu()
    {
        _menuObject.SetActive(false);
        DeselectAllCards();
        GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = false;
    }

    public void SelectCard(Card card)
    {
        if (SelectedCards.Contains(card)) return;
        if (!_allowMultipleSelections) DeselectAllCards();
        SelectedCards.Add(card);
        Image cardImage = card.gameObject.transform.Find("CardImage").GetComponent<Image>();
        if (cardImage != null)
            cardImage.color = new Color(0.7f, 0.16f, 0.16f);
    }

    public void DeselectCard(Card card)
    {
        if (!SelectedCards.Contains(card)) return;
        SelectedCards.Remove(card);
        Image cardImage = card.gameObject.transform.Find("CardImage").GetComponent<Image>();
        if (cardImage != null)
            cardImage.color = Color.white;
    }

    private void DeselectAllCards()
    {
        foreach (var selectedCard in SelectedCards.ToArray())
            DeselectCard(selectedCard);
    }

    public int GetTotalLevels()
    {
        int totalLevels = 0;
        foreach (var card in SelectedCards)
        {
            MonsterInfo monsterInfo = card.CardInfo as MonsterInfo;
            if (monsterInfo == null) continue;
            totalLevels += monsterInfo.GetLevel();
        }
        return totalLevels;
    }

    public void TakeAction()
    {
        if (SelectedCards.Count < 1) return;
        bool isMonster = Card is CardMonster;
        if (isMonster)
        {
            MonsterInfo monsterInfo = Card.CardInfo as MonsterInfo;
            if (monsterInfo == null) return;
            if (GetTotalLevels() >= monsterInfo.GetLevel())
            {
                foreach (var selectedCard in SelectedCards.ToArray())
                    selectedCard.sendCardToGraveyard();
                ((CardMonster) Card).SetAwake(true);
            }
            else return;
        }
        else
        {
            CardUnique cardUnique = Card as CardUnique;
            if (cardUnique == null) return;
            cardUnique.OnPlay(SelectedCards);
        }
        CloseMenu();
    }
}