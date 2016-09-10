using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using ExitGames.Client.Photon.LoadBalancing;
using JetBrains.Annotations;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public enum MenuItem {
	
	Awaken, Discard, CardSelect, CardUnlock, Confirm
}

public class ClientGame : MonoBehaviour
{

    public GUIStyle Style;

    public SceneManager Game;
	private Deck _playerDeck;
    private ArrayList _selectedCards;

    public bool Dragging, Awakening, PlayersTurn, ConfirmCheck;
	public Card CardDragged;
	public CardMonster CardAwakening;

	public GameObject PlayerHandObj, EnemyHandObj, PlayerMonsters, EnemyMonsters, MonsterCardPrefab, AuxCardPrefab, MultiCardPrefab, CardMenu, CardMenuItem;

    [UsedImplicitly]
    private void Start () {

		Application.runInBackground = true;
		CardPool.associateCards ();

	    Game = new SceneManager {
	        AppId = "d5b703ed-840c-4cc1-8771-ffa8f9a6b49c",
	        FieldManager = FindObjectOfType<FieldManager>()
	    };
        Game.OnStateChangeAction += OnStateChanged;
		Game.ConnectToRegionMaster ("us");

        _selectedCards = new ArrayList ();
	}

	[UsedImplicitly]
	private void Update () {

		Game.Service ();
		if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Q))
			Application.Quit();

		if (Awakening && Input.GetKeyDown (KeyCode.Escape))
			closeAwakenCardMenu ();

	}
	
	public void OnGUI() {

		float scale = Screen.width / 1840.0f;
		Style.fontSize = (int)(30.0f * scale);
	    if (Game.ClientPlayer != null)
	    {
	        GUI.Box(new Rect(118.0f * scale, Screen.height / 2.0f + 70.0f * scale, 155.4f * scale, 60.0f * scale),
	            string.Format("{0} LP", Game.ClientPlayer.CustomProperties["l"]), Style);
	    }
	    if (Game.EnemyPlayer != null)
	    {
	        GUI.Box(new Rect(118.0f * scale, Screen.height / 2.0f - 130.0f * scale, 155.4f * scale, 60.0f * scale),
	            string.Format("{0} LP", Game.EnemyPlayer.CustomProperties["l"]), Style);
	    }
	    GUI.Box(new Rect(118.0f * scale, Screen.height / 2.0f - 30.0f * scale, 155.4f * scale, 60.0f * scale), Game.CurrentStage.ToString (), Style);
		if ((Game.ClientPlayer != null && Game.ClientPlayer.isReady ()) || Game.CurrentStage == GameStage.WAITING)
			GUI.Box (new Rect (Screen.width - (271.0f * scale), (Screen.height / 2.0f) - (30.0f * scale), 217.0f * scale, 60.0f * scale), "...", Style);
		else {
			if (!ConfirmCheck) {
				if (GUI.Button (new Rect (Screen.width - (271.0f * scale), (Screen.height / 2.0f) - (30.0f * scale), 217.0f * scale, 60.0f * scale), "End Turn", Style)) {
					if (Game.CurrentStage == GameStage.PREP) {
						ConfirmCheck = true;
						this.setRemainingMonstersToDefend();
					} else {
						Game.ClientPlayer.changeStatus (true);
						Game.advanceStage ();
					}
				}
			} else {
				if (GUI.Button (new Rect (Screen.width - (271.0f * scale), (Screen.height / 2.0f) - (30.0f * scale), 217.0f * scale, 60.0f * scale), "Confirm", Style)) {
					ConfirmCheck = false;
					this.setRemainingMonstersToDefend();
					Game.ClientPlayer.changeStatus (true);
					Game.advanceStage ();
				}
			}
		}
	}
	
	private void OnStateChanged(ClientState state)
	{
		if (state == ClientState.ConnectedToMaster)
		{
			Debug.Log("Connected to master.");
			Game.OpJoinRandomRoom (null, 0);
		}
	}

	public void OnApplicationQuit()
	{
		if (Game != null && Game.loadBalancingPeer != null)
		{
			Game.Disconnect();
			Game.loadBalancingPeer.StopThread();
		}
		Game = null;
	}
	
	public bool isACardSelected() {
		
		return _selectedCards.Count == 1;
	}

	public bool isCardSelected(Card c) {

		return _selectedCards.Contains (c);
	}

	public void deselectCard(Card c) {

		_selectedCards.Remove (c);
	}

	public void selectCard(Card c, bool selectOne = false) {

		if (selectOne)
			_selectedCards.Clear ();
		_selectedCards.Add (c);
	}

	public void selectEnemyCard(CardMonster ec) {

		Card sCard = _selectedCards [0] as Card;
		if (sCard.canTarget (ec)) {
			sCard.assignTarget(ec);
		}

		deselectCard (sCard);
	}

	public void setRemainingMonstersToDefend() {

		foreach (CardMonster c in PlayerMonsters.GetComponentsInChildren<CardMonster>()) {

			if (c != null && c.State == Card.States.INPLAY && c.Target == null && !c.isDefending())
				c.setDefending(true);
		}
	}

	public int getSacrPower() {

		return Game.totalCardPower (_selectedCards);
	}

	/// <summary>
	/// Finds a CurrentCard based on a unique CurrentCard ID.
	/// </summary>
	/// <returns>The CurrentCard matching the UID, or null if no matches were found.</returns>
	/// <param name="uid">The unique CurrentCard ID to look for.</param>
	/// <typeparam name="T">The type of CurrentCard to iterate through (CardMonster, ECardMonster, etc).</typeparam>
	public T getCard<T>(int uid) where T : Card {

		foreach (T c in GameObject.FindObjectsOfType<T>()) {
			if (c.UID == uid) {
				return c;
			}
		}
		return null;
	}	

	/// <summary>
	/// Finds all cards that match the specified type (CardMonster, ECardMonster, etc).
	/// </summary>
	/// <returns>A list containing all the cards found.</returns>
	/// <typeparam name="T">The type of CurrentCard to iterate through (CardMonster, ECardMonster, etc).</typeparam>
	public List<T> getCardsByObjType<T>() where T : Card {

		List<T> cards = new List<T>();
		foreach (T c in GameObject.FindObjectsOfType<T>()) {
			cards.Add(c);
		}
		return cards;
	}	

	/// <summary>
	/// Finds all cards that match the specified CurrentCard type (Monster, Auxiliary, etc).
	/// </summary>
	/// <returns>A list containing all the cards found.</returns>
	/// <param name="cType">The CurrentCard type to look for.</param>
	public List<Card> getCardsByCardType(CardInfo.CardType cType) {
		
		List<Card> cards = new List<Card>();
		switch (cType) {

		case (CardInfo.CardType.Monster):
			cards.AddRange( getCardsByObjType<CardMonster>().Cast<Card>().ToList() );
			break;
		}
		return cards;
	}	

	/// <summary>
	/// Finds all the enemy cards.
	/// </summary>
	/// <returns>A list containing all the found enemy cards.</returns>
	public List<Card> getEnemyCards() {
		
		List<Card> cards = new List<Card>();
		var allMonsterCards = getCardsByObjType<CardMonster>().ToList();
	    foreach (var monsterCard in allMonsterCards)
	        if (monsterCard.IsEnemyCard) cards.Add(monsterCard);
		return cards;
	}

	/// <summary>
	/// Plays an enemy CurrentCard of the specified unique ID in the specified slot.
	/// </summary>
	/// <param name="uid">The unique ID of the CurrentCard to be played.</param>
	/// <param name="slotID">The slot to play the CurrentCard in.</param>
	public void playEnemyCard(int uid, int slotID) {

		foreach (Slot slot in FindObjectsOfType<Slot>()) {
			if (slot.SlotID == slotID + 12) {

				Card c = getCard<Card>(uid);

			    switch (c.CardInfo.GetCardType())
			    {
                    case (CardInfo.CardType.Monster):
                        CardMonster cm = c as CardMonster;
                        if (cm.CardInfo.AssoCardInfo.ContainsKey (CardRelation.PairR)) {

                            Transform pairDropSlot = EnemyMonsters.transform.FindChild(string.Format("enemyMonster{0}", slotID + 1));
                            //		foreach (SlotMonster sm in this.transform.parent.GetComponentsInChildren<SlotMonster>()) {
                            //			if (sm.gameObject.name.Equals(string.Format("playerMonster{0}", SlotID + dir)))
                            //				pairDropSlot = sm;
                            //		}
                            if (pairDropSlot == null)
                                break;
                            Slot pdsm = pairDropSlot.GetComponent<Slot> ();
                            if (pdsm == null)
                                break;
    //						SlotMonster pairDropSlot = null;
    //						foreach (SlotMonster sm in slot.transform.parent.GetComponentsInChildren<SlotMonster>()) {
    //							if (sm.gameObject.name.Equals(string.Format("enemyMonster{0}", slotID + 1)))
    //								pairDropSlot = sm;
    //						}
    //						if (pairDropSlot == null)
    //							break;
                            CardMonster ecmPair = pdsm.GetComponentInChildren<CardMonster>();
                            GameObject o = GameObject.Instantiate(MultiCardPrefab);

                            CardMultiPart mpc = o.GetComponent<CardMultiPart>();
                            mpc.IsEnemyCard = true;
                            mpc.changeCard((c.CardInfo as MonsterInfo) + (ecmPair.CardInfo as MonsterInfo));
                            mpc.createUID(uid);
                            mpc.GetComponent<SpriteRenderer> ().sortingOrder += pdsm.GetComponent<SpriteRenderer> ().sortingOrder;
                            mpc.State = Card.States.INPLAY;
                            mpc.changeReturnParent(pdsm.transform);
                            mpc.returnToParent();

                            slot.CurrentCard = pdsm.CurrentCard = mpc;

                            FindObjectOfType<FieldManager>().AddCardToField(mpc);

                            Destroy(c.gameObject);
                            Destroy(ecmPair.gameObject);

                            return;
                        }
                        break;
                    case CardInfo.CardType.Auxiliary:
                        if (slot.MonsterSlot != null && (slot.MonsterSlot.CurrentCard as CardMonster) != null)
                            ((CardAuxiliary) c).OnPlay((CardMonster) slot.MonsterSlot.CurrentCard);
			            break;
			    }

			    FindObjectOfType<FieldManager>().AddCardToField(c);
			    c.GetComponent<SpriteRenderer> ().sortingOrder += slot.GetComponent<SpriteRenderer> ().sortingOrder;
			    c.State = Card.States.INPLAY;
			    slot.CurrentCard = c;
			    c.changeReturnParent(slot.transform);
			    c.returnToParent();
			    return;
			}
		}
	}

	/// <summary>
	/// Provides a unique CurrentCard ID for updates the current room UID to ensure no duplicates.
	/// </summary>
	/// <returns>The unique CurrentCard ID.</returns>
	public int getUID() {

		int currentUID = (int)Game.CurrentRoom.CustomProperties ["u"];
		Game.CurrentRoom.SetCustomProperties (new Hashtable() {{"u", ++currentUID}});
		return currentUID;
	}

	/// <summary>
	/// Sends a specified CurrentCard to the graveyard.
	/// </summary>
	/// <param name="uid">The unique ID of the CurrentCard to remove.</param>
	public void removeCard(int uid) {
		Card c = getCard<Card>(uid);
		if (c != null)
			c.sendCardToGraveyard ();
	}

	/// <summary>
	/// Checks whether a CurrentCard is in the player's hand based on CurrentCard info.
	/// </summary>
	/// <returns><c>true</c>, if the CurrentCard info was found in the hand, <c>false</c> otherwise.</returns>
	/// <param name="ci">The CurrentCard info to search for.</param>
	public bool isCardInHand(CardInfo ci, out Card cardFound) {
		cardFound = null;
		foreach (Card c in PlayerHandObj.GetComponentsInChildren<Card>()) {
			if (c.CardInfo == ci) {
				cardFound = c;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Checks whether a specific CurrentCard is in the player's hand based on its UID.
	/// </summary>
	/// <returns><c>true</c>, if the CurrentCard info was found in the hand, <c>false</c> otherwise.</returns>
	/// <param name="uid">The unique CurrentCard ID to search for.</param>
	public bool isCardInHand(int uid, out Card cardFound) {
		cardFound = null;
		foreach (Card c in PlayerHandObj.GetComponentsInChildren<Card>()) {
			if (c.UID == uid) {
				cardFound = c;
				return true;
			}
		}
		return false;
	}

	public bool awakenCard() {

		if (Game.canAwaken (CardAwakening, _selectedCards)) {
			//server event code here DO NOT FORGET!!!
			foreach (Card c in _selectedCards) {

				Game.SendCardToGraveyard(c);
			}
			CardAwakening.awakenCard();
			closeAwakenCardMenu();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Deals a CurrentCard to the players hand and instantiates the appropriate prefab.
	/// </summary>
	/// <returns>The CurrentCard dealt to player.</returns>
	/// <param name="cInfo">The info of the CurrentCard to be dealt.</param>
	public Card dealCardToPlayer(CardInfo cInfo) {

		Card cardDealt = null;
		switch (cInfo.GetCardType()) {
		
		case (CardInfo.CardType.Monster):
			GameObject m = Instantiate(MonsterCardPrefab);
			m.transform.SetParent(PlayerHandObj.transform);
			cardDealt = m.GetComponent<CardMonster>();
			(cardDealt as CardMonster).changeCard(cInfo as MonsterInfo);
			break;
		case (CardInfo.CardType.Auxiliary):
			GameObject a = Instantiate(AuxCardPrefab);
			a.transform.SetParent(PlayerHandObj.transform);
			cardDealt = a.GetComponent<CardAuxiliary>();
			(cardDealt as CardAuxiliary).changeCard(cInfo as AuxiliaryInfo);
			break;
		}

		if (cardDealt != null) {
			cardDealt.changeReturnParent (PlayerHandObj.transform);
			cardDealt.State = Card.States.INHAND;
			cardDealt.createUID (getUID());
		}

		return cardDealt;
	}	

	/// <summary>
	/// Deals a CurrentCard to the enemy with the specified UID.
	/// </summary>
	/// <param name="uid">The unique CurrentCard ID to assign to the new CurrentCard.</param>
	/// <param name="cInfo">The CurrentCard info of the new CurrentCard.</param>
	public void dealCardToEnemy(int uid, CardInfo cInfo) {

		Card cardDealt = null;
		switch (cInfo.GetCardType())
		{
            case CardInfo.CardType.Monster:
                GameObject m = Instantiate(MonsterCardPrefab);
                m.transform.SetParent(EnemyHandObj.transform);
                cardDealt = m.GetComponent<CardMonster>();
                (cardDealt as CardMonster).changeCard(cInfo as MonsterInfo);
                (cardDealt as CardMonster).IsEnemyCard = true;
                (cardDealt as CardMonster).setCardFOW(true);
                break;
            case CardInfo.CardType.Auxiliary:
                GameObject a = Instantiate(AuxCardPrefab);
                a.transform.SetParent(EnemyHandObj.transform);
                cardDealt = a.GetComponent<CardAuxiliary>();
                (cardDealt as CardAuxiliary).changeCard(cInfo as AuxiliaryInfo);
                (cardDealt as CardAuxiliary).IsEnemyCard = true;
                (cardDealt as CardAuxiliary).setCardFOW(true);
                break;
		}
		
		if (cardDealt != null) {
			cardDealt.changeReturnParent (EnemyHandObj.transform);
			cardDealt.State = Card.States.DISABLED;
			cardDealt.createUID (uid);
		}
	}

	public void openAwakenCardMenu(CardMonster cardToAwaken) {
	
		CardAwakening = cardToAwaken;
		bool leviathan = (CardAwakening.CardInfo as MonsterInfo).GetLevel() > 7;
		Awakening = true;
		foreach (CardStatComponent csc in CardMenu.GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(CardAwakening.CardInfo);
		CardMenu.SetActive (true);
		GameObject.FindGameObjectWithTag ("blockRays").GetComponent<BoxCollider2D>().enabled = true;
		foreach (CardMonster c in GameObject.FindObjectsOfType<CardMonster>()) {

			if (c.canSacrifice(leviathan)) {

				GameObject cmi = GameObject.Instantiate(CardMenuItem);
				cmi.transform.SetParent(CardMenu.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform);
				cmi.GetComponent<CardMenuSelect>().setCard(c);
			}
		}
	}

	public void closeAwakenCardMenu() {
		
		CardAwakening = null;
		Awakening = false;
		CardMenu.SetActive (false);
		_selectedCards.Clear ();
		GameObject.FindGameObjectWithTag ("blockRays").GetComponent<BoxCollider2D>().enabled = false;
	}



	public int getDamageToLife() {

		int total = 0;

		foreach (CardMonster c in PlayerMonsters.GetComponentsInChildren<CardMonster>()) {
			if (c.Target != null) {
				Game.SendAttackEv(c.UID, c.Target.UID, (c.Target as CardMonster).isDefending());
				ActionQueue.calcAttack(c, c.Target, (c.Target as CardMonster).isDefending());
				c.clearTarget();
			} else {
				c.setDefending(true);
				total += (c.CardInfo as MonsterInfo).Attack;
			}
		}
//
//		foreach (CardMonster c in playerMonsters.GetComponentsInChildren<CardMonster>()) {
//			if (c.isDefending())
//				total += (c.CardI as MonsterInfo).Attack;
//		}
	    Debug.LogError(EnemyMonsters.GetComponentsInChildren<Card>().Length);
		if (EnemyMonsters.GetComponentsInChildren<Card> ().Length < 1) return total;
		return -1;
	}
}
