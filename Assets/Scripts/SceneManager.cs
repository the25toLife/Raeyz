using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using ExitGames.Client.Photon.LoadBalancing;
using System.Collections.Generic;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Represents different stages of a game.  Does not dictate who's turn it is.
/// </summary>
public enum GameStage {

	WAITING, SETUP, PREP, BATTLE
}

/// <summary>
/// Represents possible actions of any type of CurrentCard.
/// </summary>
public enum Actions {
	
	PLAY, DISCARD, ATTACK, AWAKEN, MENU, DEFEND
}

public class SceneManager : LoadBalancingClient
{
    private Deck _playerDeck, _playerGrave, _enemyDeck;
	private ClientGame _clientGame;
	private ArrayList _playerActions;
	private bool _running, _firstMove, _isTurn, _oppReady;
	private GameStage _stage, _nextStage;
	public GameStage CurrentStage { get { return _stage; } }
	public GameObject PlayerShuffleCard, EnemyShuffleCard, PlayerHandObj, HandCardPrefab;

	public RaeyzPlayer ClientPlayer, EnemyPlayer;
	public float TurnNumber;

    public const byte EvCreateDeck = 1;
	public const byte EvDealCard = 2;
	public const byte EvStageChanged = 3;
	public const byte EvGraveCard = 4;
	public const byte EvPlayCard = 5;
    public const byte EvPlayCardWithTarget = 6;
    public const byte EvDefenseToggle = 7;
	public const byte EvAttack = 8;

    public event EventHandler TurnStart, EnemyTurnStart;

	protected internal override Player CreatePlayer(string actorName, int actorNumber, bool isLocal, Hashtable actorProperties)
	{
		return new RaeyzPlayer(actorName, actorNumber, isLocal, new Hashtable {{"l", 200}, {"r", false}});
	}

	public override void OnOperationResponse(OperationResponse operationResponse)
	{
		base.OnOperationResponse(operationResponse);
		
		switch (operationResponse.OperationCode)
		{
//		case (byte)OperationCode.WebRpc:
//			Debug.Log("WebRpc-Response: " + operationResponse.ToStringFull());
//			if (operationResponse.ReturnCode == 0)
//			{
//				this.OnWebRpcResponse(new WebRpcResponse(operationResponse));
//			}
//			break;
		case (byte)OperationCode.JoinGame:
			Debug.Log ("Joining match.");
			setupPlayer();
			if (this.CurrentRoom.PlayerCount > 1)
				startGame();
			break;
		case (byte)OperationCode.CreateGame:

			if (this.Server == ServerConnection.GameServer)
			{
				if (operationResponse.ReturnCode == 0)
					setupPlayer();
			}
			break;
		case (byte)OperationCode.JoinRandomGame:
			if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
			{
				Debug.Log ("No match found.");
				this.CreateTurnbasedRoom();
			}
			break;
		}
	}

	/// <summary>
	/// Creates a new game room allowing two players, persisting for five seconds, and sets up custom room properties.
	/// </summary>
	public void CreateTurnbasedRoom()
	{
		Debug.Log("Creating new room.");
		
		RoomOptions demoRoomOptions = new RoomOptions()
		{
			MaxPlayers = (byte)2,
			CustomRoomProperties = new Hashtable() {{"t#", 1.0f}, {"tp", -1}, {"u", 1}},
			EmptyRoomTtl = 5000,
			PlayerTtl = int.MaxValue
		};
		this.OpCreateRoom(null, demoRoomOptions, TypedLobby.Default);
	}

	/// <summary>
	/// Informs other players that a deck was created for this client.
	/// </summary>
	/// <param name="d">int[] of CurrentCard IDs in the deck.</param>
	public void SendCreateDeckEv(int[] d)
	{
		Hashtable content = new Hashtable();
		content[(byte)1] = d;
		this.loadBalancingPeer.OpRaiseEvent(EvCreateDeck, content, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache });
	}

	/// <summary>
	/// Informs other players that a CurrentCard has been dealt to this client.
	/// </summary>
	/// <param name="c">The CurrentCard dealt.</param>
	public void SendDealCardEv(Card c)
	{
		Hashtable content = new Hashtable();
		content[(byte)1] = c.UID;
		content[(byte)2] = c.CardInfo.GetId();
		this.loadBalancingPeer.OpRaiseEvent(EvDealCard, content, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache });
	}

	/// <summary>
	/// Informs other players that this client is prepared to change, or has changed, stages.
	/// </summary>
	/// <param name="newStage">The stage this client is moving to.</param>
	public void SendStageChangedEv(GameStage newStage) {
		
		Hashtable content = new Hashtable();
		content[(byte)1] = (int)newStage;
		this.loadBalancingPeer.OpRaiseEvent(EvStageChanged, content, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache });
	}

	/// <summary>
	/// Informs other players that one of this clients cards has been sent to graveyard.  Do not call for other player's cards.
	/// </summary>
	/// <param name="id">The unique CurrentCard ID that has been sent to the graveyard.</param>
	public void SendGraveCardEv(int uid) {
		
		Hashtable content = new Hashtable();
		content[(byte)1] = uid;
		this.loadBalancingPeer.OpRaiseEvent(EvGraveCard, content, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache });
	}

	/// <summary>
	/// Informs other players that this client has played a CurrentCard.
	/// </summary>
	/// <param name="cid">The unique CurrentCard ID of the CurrentCard played.</param>
	/// <param name="slotid">The slot that the CurrentCard has been played in.</param>
	public void SendPlayCardEv(int uid, int slotid) {
		
		Hashtable content = new Hashtable();
		content[(byte)1] = uid;
		content[(byte)2] = slotid;
		this.loadBalancingPeer.OpRaiseEvent(EvPlayCard, content, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache });
	}

    public void SendPlayCardWithTargetEv(int uid, int[] targetUids)
    {

        Hashtable content = new Hashtable();
        content[(byte) 1] = uid;
        content[(byte) 2] = targetUids;
        this.loadBalancingPeer.OpRaiseEvent(EvPlayCardWithTarget, content, true,
            new RaiseEventOptions() {CachingOption = EventCaching.AddToRoomCache});
    }

    /// <summary>
	/// Informs the other players this client has toggled the defense state of a CurrentCard.
	/// </summary>
	/// <param name="cid">The unique CurrentCard ID of the CurrentCard.</param>
	/// <param name="state">The new defense state of the CurrentCard.</param>
	public void SendDefenseToggleEv(int uid, bool state) {
		
		Hashtable content = new Hashtable();
		content[(byte)1] = uid;
		content[(byte)2] = state;
		this.loadBalancingPeer.OpRaiseEvent(EvDefenseToggle, content, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache });
	}

	/// <summary>
	/// Informs the other players that this client has issued an attack.
	/// </summary>
	/// <param name="auid">Attacking CurrentCard's unique ID.</param>
	/// <param name="tuid">Target CurrentCard's unique ID.</param>
	/// <param name="defend">If set to <c>true</c>, the target's defense stat will be used instead of the attack stat.</param>
	public void SendAttackEv(int auid, int tuid, bool defend) {
		
		Hashtable content = new Hashtable();
		content[(byte)1] = auid;
		content[(byte)2] = tuid;
		content[(byte)3] = defend;
		this.loadBalancingPeer.OpRaiseEvent(EvAttack, content, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache });
	}
	
	public override void OnEvent(EventData photonEvent)
	{
		base.OnEvent(photonEvent);
		
		object content;
		switch (photonEvent.Code)
		{
		case (byte)EvDealCard:
			content = photonEvent.Parameters[ParameterCode.CustomEventContent];
			Hashtable cardInfo = content as Hashtable;
			if (cardInfo != null)
			{
				int uid = (int)cardInfo[(byte)1];
				CardInfo c = CardPool.Cards[(int)cardInfo[(byte)2]-1];

				_clientGame.dealCardToEnemy(uid, c);
				Debug.Log(string.Format("The enemy was dealt a CurrentCard with UID: {0} and name: {1}.", uid, c.GetName()));
			}
			break;
		case (byte)EvCreateDeck:
			content = photonEvent.Parameters[ParameterCode.CustomEventContent];
			Hashtable deckSent = content as Hashtable;
			if (deckSent != null)
			{
				_enemyDeck = new Deck((int[])deckSent[(byte)1]);

				Debug.Log("The other player has created a deck.");
			}
			break;
		case (byte)EvStageChanged:
			content = photonEvent.Parameters[ParameterCode.CustomEventContent];
			Hashtable stageInfo = content as Hashtable;
			if (stageInfo != null)
			{
				GameStage stageTemp = (GameStage)Enum.ToObject(typeof(GameStage), (int)stageInfo[(byte)1]);

				if (stageTemp != GameStage.SETUP) {		//If the next stage is SETUP, start the game.  The stage changes in startGame().

					if (ClientPlayer.isReady()) {	//Both players are ready at this point so change stages.  This should always be true or we have problems.
						
						if(_stage == GameStage.SETUP && !_isTurn)		//Set initial monster to defend if the client isn't going first.
							_clientGame.setRemainingMonstersToDefend();

						_stage = _nextStage;
						if (_stage == GameStage.BATTLE)
							startBattleStage();
						if (_stage == GameStage.PREP) 
							startPrepStage();
						else
							_oppReady = false;
						if (_stage == GameStage.WAITING)		//If the next stage is WAITING, keep the client in the ready state.
							advanceStage();
						else 
							ClientPlayer.changeStatus(false);
					} else
						_oppReady = true;
				} else
					startGame ();

				Debug.Log(string.Format("Other player ready. Game entering {0} stage.", _stage));
			}
			break;
		case (byte)EvGraveCard:

			//Probably gonna see a lot of these...
			//Implement unique CurrentCard ID
			//
			//
			content = photonEvent.Parameters[ParameterCode.CustomEventContent];
			Hashtable graveInfo = content as Hashtable;
			if (graveInfo != null)
			{
				int uid = (int)graveInfo[(byte)1];
				_clientGame.removeCard(uid);
				
				Debug.Log(string.Format("Removing CurrentCard with ID {0}", uid));
			}
			break;
			//
			//
			//end

		case (byte)EvPlayCard:

			//omg why am i doing this before i fixed the cards
			//
			//
			content = photonEvent.Parameters[ParameterCode.CustomEventContent];
			Hashtable playInfo = content as Hashtable;
			if (playInfo != null)
			{
				int uid = (int)playInfo[(byte)1];
				int slotid = (int)playInfo[(byte)2];
				_clientGame.playEnemyCard(uid, slotid);
				
				Debug.Log(string.Format("Playing card with ID {0} in slot {1}", uid, slotid));
			}
			break;
			//
			//end

            case EvPlayCardWithTarget:
		        content = photonEvent.Parameters[ParameterCode.CustomEventContent];
		        Hashtable playWithTargetInfo = content as Hashtable;
		        if (playWithTargetInfo != null)
		        {
		            int uid = (int) playWithTargetInfo[(byte) 1];
		            int[] targetUids = (int[]) playWithTargetInfo[(byte) 2];
		            _clientGame.PlayEnemyCardWithTargets(uid, targetUids);

		            Debug.Log(string.Format("Playing card with ID {0} targeting card with ID {1}", uid, targetUids));
		        }
		        break;

		case (byte)EvDefenseToggle:

			//thought this one would be fine but no.
			//ill go fix the cards after the event section.
			//
			//
			content = photonEvent.Parameters[ParameterCode.CustomEventContent];
			Hashtable defInfo = content as Hashtable;
			if (defInfo != null)
			{
				int uid = (int)defInfo[(byte)1];
				bool state = (bool)defInfo[(byte)2];
				CardMonster c = _clientGame.getCard<CardMonster>(uid);
				c.setDefending(state);
				
				Debug.Log(string.Format("CardAppliedTo with UID: {0} is {1}", uid, (state ? "now defending." : "no longer defending.")));
			}
			break;
		case (byte)EvAttack:
			content = photonEvent.Parameters[ParameterCode.CustomEventContent];
			Hashtable attackInfo = content as Hashtable;
			if (attackInfo != null)
			{
				int auid = (int)attackInfo[(byte)1];
				int tuid = (int)attackInfo[(byte)2];
				bool defend = (bool)attackInfo[(byte)3];

				ActionQueue.calcAttack(_clientGame.getCard<Card>(auid), _clientGame.getCard<Card>(tuid), defend);
				
				Debug.Log(string.Format("{0} attacked {1} who {2} defending.", auid, tuid, defend ? "is" : "is not"));
			}
			break;

//		case (byte)EvExecuteAction:
//
//			//gg
//			//gfg
//			//
//			//
//			content = photonEvent.Parameters[ParameterCode.CustomEventContent];
//			Hashtable actionInfo = content as Hashtable;
//			if (actionInfo != null)
//			{
//				Actions a = (Actions)Enum.ToObject(typeof(Actions), (int)actionInfo[(byte)1]);
//				int actionCardID = (int)actionInfo[(byte)2];
//				int targetCardID = (int)actionInfo[(byte)3];
//
//				GameAction action = new GameAction(a, clientGame.getCard(CardPool.Cards[actionCardID-1], CardInfo.CardType.MONSTER, true), clientGame.getCard(CardPool.Cards[targetCardID-1]));
//				action.executeAction();
//				
//				Debug.Log(string.Format("CardAppliedTo with ID {0} is performing action: {1} on CurrentCard with ID {2}", actionCardID, a, targetCardID));
//			}
//			break;
			//
			//end
			
		case EventCode.PropertiesChanged:
			if (ClientPlayer != null && EnemyPlayer != null) {
				TurnNumber = (float)this.CurrentRoom.CustomProperties["t#"];
				_isTurn = ((int)this.CurrentRoom.CustomProperties["tp"] == this.LocalPlayer.ID);
				Debug.Log(string.Format("Other player updated room properties. Turn #: {0}, Client's Turn: {1}", TurnNumber, _isTurn));
			}
			break;
		}
	}

	/// <summary>
	/// Checks the current running state of the game.
	/// </summary>
	/// <returns><c>true</c> if the game is running, <c>false</c> otherwise.</returns>
	public bool isGameRunning() {
		return _running;
	}

	/// <summary>
	/// Advances the game to the next turn.  Increases turn number and adjusts who's turn it currently is.
	/// </summary>
	public void nextTurn() {
		TurnNumber += 0.5f;
	    if (_isTurn)
	        this.OpSetCustomPropertiesOfRoom(new Hashtable() {{"t#", TurnNumber}, {"tp", EnemyPlayer.ID}});
	    else
	        this.OpSetCustomPropertiesOfRoom(new Hashtable() {{"t#", TurnNumber}, {"tp", ClientPlayer.ID}});
	    _isTurn = !_isTurn;
	}

	/// <summary>
	/// Starts the game.  Assigns player values and advances the stage to SETUP.
	/// </summary>
	private void startGame() {

		if (isGameRunning ())
			return;
		_running = true;

		foreach (KeyValuePair<int, Player> kvp in this.CurrentRoom.Players) {
			if (kvp.Value.ID == this.LocalPlayer.ID)
				ClientPlayer = kvp.Value as RaeyzPlayer;
			else
				EnemyPlayer = kvp.Value as RaeyzPlayer;
		}

		_stage = GameStage.SETUP;
		SendStageChangedEv (_stage);
	}

	/// <summary>
	/// Initiates basic player elements (the client instance, hand, and deck).
	/// </summary>
	private void setupPlayer() {
		
		_clientGame = GameObject.FindObjectOfType<ClientGame> ();
		_playerDeck = createDeck ();
		_playerActions = new ArrayList ();

/*		CardInfo c = CardPool.Cards [UnityEngine.Random.Range (0, 397)];	//assuming only monsters are assigned to first 398 IDs.
		while ((c as MonsterInfo).GetLevel() > 4)	//Guarentees a basic CurrentCard in the initial hand.
			c = CardPool.Cards [UnityEngine.Random.Range (0, 397)];
		dealCardToPlayer (c);
		for (int i = 0; i < 4; i++)
			dealCardToPlayer ();*/
		dealCardToPlayer (CardPool.Cards [286]);
		dealCardToPlayer (CardPool.Cards [545]);
		dealCardToPlayer (CardPool.Cards [425]);
		dealCardToPlayer (CardPool.Cards [594]);
		dealCardToPlayer (CardPool.Cards [4]);

		_stage = GameStage.PREP;
	}

	/// <summary>
	/// Creates a deck.
	/// </summary>
	/// <returns>The deck created.</returns>
	private Deck createDeck() {

		ArrayList cardsInPool = new ArrayList ();
		foreach (CardInfo c in CardPool.Cards) {
			// run if checks here to exclude card IDs
            cardsInPool.Add(c);
		}
		
		Deck d = new Deck (cardsInPool, 200);
		SendCreateDeckEv (d.sendDeckAsEv ());
		return d;
	}

	/// <summary>
	/// Deals a CurrentCard to the player, either the next CurrentCard in the deck or one specified in the arguments.
	/// </summary>
	/// <param name="c">The specific CurrentCard to deal to the player (optional).</param>
	private void dealCardToPlayer(CardInfo c = null) {

		if (PlayerHandObj.transform.childCount > 4) return;

		CardInfo cardInfo = _playerDeck.drawCard();
		if (c != null)
			cardInfo = c;

		Card cardDrawn = _clientGame.dealCardToPlayer (cardInfo);
	    SendDealCardEv(cardDrawn);
	}

	/// <summary>
	/// Deals a full hand to the player, not exceeding five cards.
	/// </summary>
	private void dealFullHandToPlayer() {

		while (PlayerHandObj.transform.childCount < 5)
			dealCardToPlayer();
	}

	/// <summary>
	/// Plays a CurrentCard on the field.
	/// </summary>
	/// <param name="c">The CurrentCard to play.</param>
	/// <param name="slot">The slot ID to play the CurrentCard in.</param>
	public void playCard(Card c, int slot) {

		if (!_firstMove)
			_firstMove = true;
	    SendPlayCardEv (c.UID, slot);
	}


	/// <summary>
	/// Sends the specified CurrentCard to the graveyard.
	/// </summary>
	/// <param name="c">The CurrentCard to send to the graveyard.</param>
	public void SendCardToGraveyard(Card c) {

		c.sendCardToGraveyard ();
		SendGraveCardEv (c.UID);
	}

	//hahahahahahaha
	//
	//goodbye.
	//
	//
	public bool ScheduleAction(GameAction a) {

		if (!canTakeAction (a.A))
			return false;

		_playerActions.Add (a);
		foreach (GameAction ga in _playerActions)
			Debug.Log (string.Format ("{0} scheduled targeting: {1}", ga.A, ga.getTarget().CardInfo.GetName()));
		return true;
	}

	public bool UnscheduleAction(GameAction a) {
		
//		if (!canTakeAction (a.A))
//			return false;
		
		_playerActions.Remove (a);
		return true;
	}

	public void takeFirstAction() {
		_firstMove = true;
	}

	public bool canTakeAction(Actions a) {

		bool flag = false;
		switch (a) {

		case (Actions.PLAY):
			flag = (_stage == GameStage.SETUP && !_firstMove) || (_stage == GameStage.PREP);
			break;
		case (Actions.ATTACK):
			flag = (_stage == GameStage.PREP);
			break;
		case (Actions.MENU):
			flag = (_stage != GameStage.WAITING && _stage != GameStage.BATTLE);
			break;
		case (Actions.AWAKEN):
			flag = (_stage == GameStage.PREP);
			break;
		case (Actions.DISCARD):
			flag = (_stage == GameStage.SETUP) || (_stage == GameStage.PREP);
			break;
		default:
			flag = false;
			break;
		}
		return flag;
	}
	//
	//
	//your end.

	/// <summary>
	/// Advances, or prepares to advance, game stage.
	/// </summary>
	/// <param name="wait">If set to <c>true</c>, waits for other player to send a stage change event regardless of any checks.</param>
	public void advanceStage() {

		Debug.Log (_oppReady);
		if (_oppReady) {		//If the other player is ready, change stage.

			if(_stage == GameStage.SETUP && !_isTurn)		//Set initial monster to defend if the client isn't going first.
				_clientGame.setRemainingMonstersToDefend();

			if (_stage == GameStage.SETUP)
				_stage = (_isTurn) ? GameStage.PREP : GameStage.WAITING;
			else if (_stage == GameStage.PREP)
				_stage = GameStage.BATTLE;
			else if (_stage == GameStage.WAITING)
				_stage = GameStage.BATTLE;
			else if (_stage == GameStage.BATTLE)
				_stage = (_isTurn) ? GameStage.PREP : GameStage.WAITING;

			SendStageChangedEv (_stage);		//Inform the other player that this client is now ready.
			if (_stage == GameStage.BATTLE)
				startBattleStage();
			if (_stage == GameStage.PREP)
				startPrepStage();
			else
				_oppReady = false;
			if (_stage == GameStage.WAITING)		//If the next stage is WAITING, keep the client in the ready state.
				advanceStage();
			else 
				ClientPlayer.changeStatus(false);

		} else {	//If the other player isn't ready yet or wait is true, prepare to change stage.

			if (_stage == GameStage.SETUP) {		//Determine which player is first if it is the first turn.
				if (UnityEngine.Random.Range (1, 2) > 1) {
					this.OpSetCustomPropertiesOfRoom(new Hashtable() {{"tp", ClientPlayer.ID}});
					_isTurn = true;
				} else
					this.OpSetCustomPropertiesOfRoom(new Hashtable() {{"tp", EnemyPlayer.ID}});
			}

			if (_stage == GameStage.SETUP)
				_nextStage = (_isTurn) ? GameStage.PREP : GameStage.WAITING;
			else if (_stage == GameStage.PREP)
				_nextStage = GameStage.BATTLE;
			else if (_stage == GameStage.WAITING)
		    {
		        _nextStage = GameStage.BATTLE;
		        EventHandler handler = EnemyTurnStart;
		        // ReSharper disable once UseNullPropagation
		        if (handler != null) handler.Invoke(this, EventArgs.Empty);
		    }
		    else if (_stage == GameStage.BATTLE)
		    {
		        nextTurn(); //The first player to ready up during the BATTLE stage advances the room's turn.
		        _nextStage = _isTurn ? GameStage.PREP : GameStage.WAITING;
		    }
		    SendStageChangedEv (_nextStage);
		}
	}

	/// <summary>
	/// Starts the PREP stage for the client.
	/// </summary>
	private void startPrepStage() {

		dealFullHandToPlayer ();
	    EventHandler handler = TurnStart;
	    // ReSharper disable once UseNullPropagation
	    if (handler != null) handler.Invoke(this, EventArgs.Empty);
	}


	//gonna need tweaks
	//
	//
	private void startBattleStage() {

		if (!_isTurn)
			return;
		int playerDmg = _clientGame.getDamageToLife ();
		//Debug.LogError (playerDmg);
		if (playerDmg > 0) {
			EnemyPlayer.damagePlayer(playerDmg);
//			SendDamagePlayerEv (playerDmg);
		}
	}
	//
	//end.	

	/// <summary>
	/// Gets the total level of an array of cards.	Used in awakening cards.
	/// </summary>
	/// <returns>The total of all the cards' levels.</returns>
	/// <param name="cards">An ArrayList of cards to calculate.</param>
	public int totalCardPower(ArrayList cards) {

		int total = 0;
		foreach (Card c in cards) {

			if (c.CardInfo is MonsterInfo)
				total += (c.CardInfo as MonsterInfo).GetLevel();
		}
		return total;
	}

	/// <summary>
	/// Checks whether or not a CurrentCard can be awakened by sacrificing the given cards.
	/// </summary>
	/// <returns><c>true</c> if the CurrentCard can be awakened, <c>false</c> otherwise.</returns>
	/// <param name="c">The CurrentCard to check.</param>
	/// <param name="sacr">An ArrayList of cards to sacrifice.</param>
	public bool canAwaken(Card c, ArrayList sacr) {

		if (!(c.CardInfo is MonsterInfo))
			return false;
		if (totalCardPower (sacr) >= (c.CardInfo as MonsterInfo).GetLevel())
			return true;
		return false;
	}
}



public class GameAction {

	private Actions action;
	private Card actionCard, targetCard;
	/// <summary>
	/// Returns the action to perform.
	/// </summary>
	/// <value>A.</value>
	public Actions A { get { return action; } }

	public GameAction(Actions a, Card c) {

		action = a;
		actionCard = c;
		targetCard = null;
	}	
	public GameAction(Actions a, Card c1, Card c2) {
		
		action = a;
		actionCard = c1;
		targetCard = c2;
	}
	public void executeAction() {

		switch (action) {

		case (Actions.ATTACK):

			int attackerStat = (actionCard.CardInfo as MonsterInfo).Attack;
			int targetStat = ((targetCard as CardMonster).isDefending()) ? (targetCard.CardInfo as MonsterInfo).Defense : (targetCard.CardInfo as MonsterInfo).Attack;

			if (attackerStat > targetStat)
				targetCard.sendCardToGraveyard();
			else if (attackerStat < targetStat)
				actionCard.sendCardToGraveyard();
			else {
				targetCard.sendCardToGraveyard();
				actionCard.sendCardToGraveyard();
			}
			break;
		case (Actions.DEFEND):


			break;
		}
	}

	public Card getTarget() {
		return targetCard;
	}
	
	public Card getActionCard() {
		return actionCard;
	}
}

public class Deck {
	
	private ArrayList cards;
	private int maxSize;
	
	public Deck(ArrayList pool, int size) {

		maxSize = size;
		cards = new ArrayList (maxSize);
		for (int i = 0; i < maxSize; i++)
		{

		    int index = UnityEngine.Random.Range(0, pool.Count);
		    while (pool[index] == null) //Guarentees a basic CurrentCard in the initial hand.
		        index = UnityEngine.Random.Range(0, pool.Count);
		    CardInfo cardInfo = (CardInfo) pool[index];
		    pool.RemoveAt (index);
			cards.Add(cardInfo);
		}
	}	

	public Deck(int[] deckInfo) {

		maxSize = deckInfo.Length;
		cards = new ArrayList (maxSize);
		foreach (int i in deckInfo) {

			CardInfo c = (CardInfo)CardPool.Cards[i-1];
			cards.Add(c);
		}
	}

	public CardInfo drawCard() {

		CardInfo tempCard = (CardInfo)cards [0];
		cards.RemoveAt (0);
		return tempCard;
	}

	public void printDeck() {

		foreach (CardInfo i in cards) {
			Debug.Log(i.GetName());
		}
	}

	public int[] sendDeckAsEv() {

		int[] deckToSend = new int[cards.Count];
		for (int i = 0; i < cards.Count; i++) {
			CardInfo c = (CardInfo)cards [i];
			deckToSend [i] = c.GetId();
		}
		return deckToSend;
	}
}