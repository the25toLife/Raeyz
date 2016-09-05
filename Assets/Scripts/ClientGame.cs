using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public enum MenuItem {
	
	AWAKEN, DISCARD, CARDSELECT, CARDUNLOCK, CONFIRM
}

public class ClientGame : MonoBehaviour {

	public SceneManager game;
	public GUIStyle style, lifeStyle;
	private Deck playerDeck;

	public bool dragging, awakening, playersTurn, confirmCheck;
	public Card cardDragged;
	public CardMonster cardAwakening;
	public ArrayList selectedCards;

	public GameObject playerHandObj, enemyHandObj, playerMonsters, enemyMonsters, monsterCardPrefab, auxCardPrefab, eMonsterCardPrefab, eMultiCardPrefab, cardMenu, cardMenuItem;

	void Start () {

		Application.runInBackground = true;
		CardPool.associateCards ();

		game = new SceneManager ();
		game.AppId = "d5b703ed-840c-4cc1-8771-ffa8f9a6b49c";
		game.OnStateChangeAction += this.OnStateChanged;
		game.ConnectToRegionMaster ("us");

		selectedCards = new ArrayList ();
	}

	void Update () {

		game.Service ();

		if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Q))
			Application.Quit();

		if (awakening && Input.GetKeyDown (KeyCode.Escape))
			closeAwakenCardMenu ();

	}
	
	public void OnGUI() {

		float scale = Screen.width / 1840.0f;
		lifeStyle.fontSize = (int)(40.0f * scale);
		style.fontSize = (int)(30.0f * scale);
		if (game.clientPlayer != null)
			GUI.Box(new Rect(118.0f * scale, (Screen.height / 2.0f) + (70.0f * scale), 155.4f * scale, 60.0f * scale), string.Format("{0} LP", game.clientPlayer.CustomProperties["l"].ToString()), style);
			//GUI.Box(new Rect(Screen.width - (246.0f * scale), Screen.height - (80.0f * scale), 216.0f * scale, 60.0f * scale), string.Format("me: {0} turn: {1}", game.clientPlayer.ID, game.CurrentRoom.CustomProperties["tp"]), lifeStyle);
			//GUI.Box(new Rect(Screen.width - 246.0f, Screen.height - 80.0f, 215f, 60.0f), game.clientPlayer.Life.ToString(), lifeStyle);
		if (game.enemyPlayer != null)
			GUI.Box(new Rect(118.0f * scale, (Screen.height / 2.0f) - (130.0f * scale), 155.4f * scale, 60.0f * scale), string.Format ("{0} LP",game.enemyPlayer.CustomProperties["l"].ToString()), style);
			//GUI.Box(new Rect(28.5f * scale, 20.0f * scale, 216.0f * scale, 60.0f * scale), game.CurrentRoom.CustomProperties["t#"].ToString(), lifeStyle);
			//GUI.Box(new Rect(31.0f, 20.0f, 215.0f, 60.0f), (game.enemyPlayer).CustomProperties["l"].ToString(), lifeStyle);
		GUI.Box(new Rect(118.0f * scale, (Screen.height / 2.0f) - (30.0f * scale), 155.4f * scale, 60.0f * scale), game.CurrentStage.ToString (), style);
		if ((game.clientPlayer != null && game.clientPlayer.isReady ()) || game.CurrentStage == GameStage.WAITING)
			GUI.Box (new Rect (Screen.width - (271.0f * scale), (Screen.height / 2.0f) - (30.0f * scale), 217.0f * scale, 60.0f * scale), "...", style);
		else {
			if (!confirmCheck) {
				if (GUI.Button (new Rect (Screen.width - (271.0f * scale), (Screen.height / 2.0f) - (30.0f * scale), 217.0f * scale, 60.0f * scale), "End Turn", style)) {
					if (game.CurrentStage == GameStage.PREP) {
						confirmCheck = true;
						this.setRemainingMonstersToDefend();
					} else {
						game.clientPlayer.changeStatus (true);
						game.advanceStage ();
					}
				}
			} else {
				if (GUI.Button (new Rect (Screen.width - (271.0f * scale), (Screen.height / 2.0f) - (30.0f * scale), 217.0f * scale, 60.0f * scale), "Confirm", style)) {
					confirmCheck = false;
					this.setRemainingMonstersToDefend();
					game.clientPlayer.changeStatus (true);
					game.advanceStage ();
				}
			}
		}
	}
	
	private void OnStateChanged(ClientState state)
	{
		if (state == ClientState.ConnectedToMaster)
		{
			Debug.Log("Connected to master.");
			game.OpJoinRandomRoom (null, 0);
		}
	}

	public void OnApplicationQuit()
	{
		if (game != null && game.loadBalancingPeer != null)
		{
			game.Disconnect();
			game.loadBalancingPeer.StopThread();
		}
		game = null;
	}
	
	public bool isACardSelected() {
		
		return selectedCards.Count == 1;
	}

	public bool isCardSelected(Card c) {

		return selectedCards.Contains (c);
	}

	public void deselectCard(Card c) {

		selectedCards.Remove (c);
	}

	public void selectCard(Card c, bool selectOne = false) {

		if (selectOne)
			selectedCards.Clear ();
		selectedCards.Add (c);
	}

	public void selectEnemyCard(ECardMonster ec) {

		Card sCard = selectedCards [0] as Card;
		if (sCard.canTarget (ec)) {
			sCard.assignTarget(ec);
		}

		deselectCard (sCard);
	}

	public void setRemainingMonstersToDefend() {

		foreach (CardMonster c in playerMonsters.GetComponentsInChildren<CardMonster>()) {

			if (c != null && c.State == Card.States.INPLAY && c.Target == null && !c.isDefending())
				c.setDefending(true);
		}
	}

	public int getSacrPower() {

		return game.totalCardPower (selectedCards);
	}

	/// <summary>
	/// Finds a card based on a unique card ID.
	/// </summary>
	/// <returns>The card matching the UID, or null if no matches were found.</returns>
	/// <param name="uid">The unique card ID to look for.</param>
	/// <typeparam name="T">The type of card to iterate through (CardMonster, ECardMonster, etc).</typeparam>
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
	/// <typeparam name="T">The type of card to iterate through (CardMonster, ECardMonster, etc).</typeparam>
	public List<T> getCardsByObjType<T>() where T : Card {

		List<T> cards = new List<T>();
		foreach (T c in GameObject.FindObjectsOfType<T>()) {
			cards.Add(c);
		}
		return cards;
	}	

	/// <summary>
	/// Finds all cards that match the specified card type (Monster, Auxiliary, etc).
	/// </summary>
	/// <returns>A list containing all the cards found.</returns>
	/// <param name="cType">The card type to look for.</param>
	public List<Card> getCardsByCardType(CardInfo.CardType cType) {
		
		List<Card> cards = new List<Card>();
		switch (cType) {

		case (CardInfo.CardType.Monster):
			cards.AddRange( getCardsByObjType<CardMonster>().Cast<Card>().ToList() );
			cards.AddRange(getCardsByObjType<ECardMonster>().Cast<Card>().ToList() );
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
		cards.AddRange(getCardsByObjType<ECardMonster>().Cast<Card>().ToList() );
		return cards;
	}

	/// <summary>
	/// Plays an enemy card of the specified unique ID in the specified slot.
	/// </summary>
	/// <param name="uid">The unique ID of the card to be played.</param>
	/// <param name="slotID">The slot to play the card in.</param>
	public void playEnemyCard(int uid, int slotID) {

		foreach (Slot slot in GameObject.FindObjectsOfType<Slot>()) {
			if (slot.SlotID == slotID + 12) {

				Card c = getCard<Card>(uid);
				switch (c.CardI.Type) {

				case (CardInfo.CardType.Monster):
					ECardMonster ecm = c as ECardMonster;
					if (ecm.CardI.AssoCardInfo.ContainsKey (CardRelation.RPAIR)) {

						Transform pairDropSlot = this.transform.parent.FindChild(string.Format("enemyMonster{0}", slotID + 1));
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
						ECardMonster ecmPair = pdsm.GetComponentInChildren<ECardMonster>();
						GameObject o = GameObject.Instantiate(eMultiCardPrefab);
						
						CardMultiPart mpc = o.GetComponent<CardMultiPart>();
						mpc.changeCard((c.CardI as MonsterInfo) + (ecmPair.CardI as MonsterInfo));
						mpc.createUID(uid);
						mpc.GetComponent<SpriteRenderer> ().sortingOrder = pdsm.GetComponent<SpriteRenderer> ().sortingOrder + 1;
						mpc.State = Card.States.INPLAY;
						mpc.changeReturnParent(pdsm.transform);
						mpc.returnToParent();

						GameObject.Destroy(c.gameObject);
						GameObject.Destroy(ecmPair.gameObject);
					}
					break;
				}

				if (c != null) {							
					c.GetComponent<SpriteRenderer> ().sortingOrder = slot.GetComponent<SpriteRenderer> ().sortingOrder + 1;
					c.State = Card.States.INPLAY;
					c.changeReturnParent(slot.transform);
					c.returnToParent();
				}
			}
		}
	}

	/// <summary>
	/// Provides a unique card ID for updates the current room UID to ensure no duplicates.
	/// </summary>
	/// <returns>The unique card ID.</returns>
	public int getUID() {

		int currentUID = (int)game.CurrentRoom.CustomProperties ["u"];
		game.CurrentRoom.SetCustomProperties (new Hashtable() {{"u", ++currentUID}});
		return currentUID;
	}

	/// <summary>
	/// Sends a specified card to the graveyard.
	/// </summary>
	/// <param name="uid">The unique ID of the card to remove.</param>
	public void removeCard(int uid) {
		Card c = getCard<Card>(uid);
		if (c != null)
			c.sendCardToGraveyard ();
	}

	/// <summary>
	/// Checks whether a card is in the player's hand based on card info.
	/// </summary>
	/// <returns><c>true</c>, if the card info was found in the hand, <c>false</c> otherwise.</returns>
	/// <param name="ci">The card info to search for.</param>
	public bool isCardInHand(CardInfo ci, out Card cardFound) {
		cardFound = null;
		foreach (Card c in playerHandObj.GetComponentsInChildren<Card>()) {
			if (c.CardI == ci) {
				cardFound = c;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Checks whether a specific card is in the player's hand based on its UID.
	/// </summary>
	/// <returns><c>true</c>, if the card info was found in the hand, <c>false</c> otherwise.</returns>
	/// <param name="uid">The unique card ID to search for.</param>
	public bool isCardInHand(int uid, out Card cardFound) {
		cardFound = null;
		foreach (Card c in playerHandObj.GetComponentsInChildren<Card>()) {
			if (c.UID == uid) {
				cardFound = c;
				return true;
			}
		}
		return false;
	}

	public bool awakenCard() {

		if (game.canAwaken (cardAwakening, selectedCards)) {
			//server event code here DO NOT FORGET!!!
			foreach (Card c in selectedCards) {

				game.sendCardToGraveyard(c);
			}
			cardAwakening.awakenCard();
			closeAwakenCardMenu();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Deals a card to the players hand and instantiates the appropriate prefab.
	/// </summary>
	/// <returns>The card dealt to player.</returns>
	/// <param name="cInfo">The info of the card to be dealt.</param>
	public Card dealCardToPlayer(CardInfo cInfo) {

		Card cardDealt = null;
		switch (cInfo.Type) {
		
		case (CardInfo.CardType.Monster):
			GameObject m = Instantiate(monsterCardPrefab);
			m.transform.SetParent(playerHandObj.transform);
			cardDealt = m.GetComponent<CardMonster>();
			(cardDealt as CardMonster).changeCard(cInfo as MonsterInfo);
			break;
		case (CardInfo.CardType.AUXILIARY):
			GameObject a = Instantiate(auxCardPrefab);
			a.transform.SetParent(playerHandObj.transform);
			cardDealt = a.GetComponent<CardSpecial>();
			(cardDealt as CardSpecial).changeCard(cInfo as SpecialInfo);
			break;
		}

		if (cardDealt != null) {
			cardDealt.changeReturnParent (playerHandObj.transform);
			cardDealt.State = Card.States.INHAND;
			cardDealt.createUID (getUID());
		}

		return cardDealt;
	}	

	/// <summary>
	/// Deals a card to the enemy with the specified UID.
	/// </summary>
	/// <param name="uid">The unique card ID to assign to the new card.</param>
	/// <param name="cInfo">The card info of the new card.</param>
	public void dealCardToEnemy(int uid, CardInfo cInfo) {

		Card cardDealt = null;
		switch (cInfo.Type) {
			
		case (CardInfo.CardType.Monster):
			GameObject c = Instantiate(eMonsterCardPrefab);
			c.transform.SetParent(enemyHandObj.transform);
			cardDealt = c.GetComponent<ECardMonster>();
			(cardDealt as ECardMonster).changeCard(cInfo as MonsterInfo);
			(cardDealt as ECardMonster).setCardFOW(true);
			break;
		case (CardInfo.CardType.AUXILIARY):
			break;
		}
		
		if (cardDealt != null) {
			cardDealt.changeReturnParent (playerHandObj.transform);
			cardDealt.State = Card.States.DISABLED;
			cardDealt.createUID (uid);
		}
	}

	public void openAwakenCardMenu(CardMonster cardToAwaken) {
	
		cardAwakening = cardToAwaken;
		bool leviathan = (cardAwakening.CardI as MonsterInfo).Level > 7;
		awakening = true;
		foreach (CardStatComponent csc in cardMenu.GetComponentsInChildren<CardStatComponent>(true))
			csc.changeStat(cardAwakening.CardI);
		cardMenu.SetActive (true);
		GameObject.FindGameObjectWithTag ("blockRays").GetComponent<BoxCollider2D>().enabled = true;
		foreach (CardMonster c in GameObject.FindObjectsOfType<CardMonster>()) {

			if (c.canSacrifice(leviathan)) {

				GameObject cmi = GameObject.Instantiate(cardMenuItem);
				cmi.transform.SetParent(cardMenu.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform);
				cmi.GetComponent<CardMenuSelect>().setCard(c);
			}
		}
	}

	public void closeAwakenCardMenu() {
		
		cardAwakening = null;
		awakening = false;
		cardMenu.SetActive (false);
		selectedCards.Clear ();
		GameObject.FindGameObjectWithTag ("blockRays").GetComponent<BoxCollider2D>().enabled = false;
	}



	public int getDamageToLife() {

		int total = 0;

		foreach (CardMonster c in playerMonsters.GetComponentsInChildren<CardMonster>()) {
			if (c.Target != null) {
				game.SendAttackEv(c.UID, c.Target.UID, (c.Target as ECardMonster).isDefending());
				ActionQueue.calcAttack(c, c.Target, (c.Target as ECardMonster).isDefending());
				c.clearTarget();
			} else {
				c.setDefending(true);
				total += (c.CardI as MonsterInfo).Attack;
			}
		}
//
//		foreach (CardMonster c in playerMonsters.GetComponentsInChildren<CardMonster>()) {
//			if (c.isDefending())
//				total += (c.CardI as MonsterInfo).Attack;
//		}
		if (enemyMonsters.GetComponentsInChildren<Card> ().Length < 1) return total;
		return -1;
	}
}
