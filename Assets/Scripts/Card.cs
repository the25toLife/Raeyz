using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//theredhex FFBDBD
//fontfordesc antipasto : avantgarde : sentrygothic

public abstract class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler,
    IPointerExitHandler, IPointerClickHandler, IDropHandler {

	public enum States {

		INHAND, INPLAY, INFO, EXPANDINHAND, DISABLED
	}

    // Differentiates player and enemy cards
    public bool IsEnemyCard { get; set; }

    // Determines whether or not the player can see the CurrentCard's information or only the CurrentCard's back
    public bool FowActive { get; private set; }

    protected Transform ParentToReturnTo;
	protected States StateToReturnTo;

	public GameObject FullInfoCanvas, StatOverlay, LcMenu, DeathHandler, SelectedIndicator;
	public ClientGame Client;

    // ReSharper disable once InconsistentNaming
	public int UID { get; set; }
	public CardInfo CardInfo { get; set; }
	public States State { get; private set; }
    public List<StatusEffect> StatusEffects { get; set; }

    //public delegate void StateChangedEventHandler(StateChangedEventArgs e);

    public event EventHandler StateChangedEvent;

    public virtual void Start () {

		Client = FindObjectOfType<ClientGame> ();

        StatusEffects = new List<StatusEffect>();

        FullInfoCanvas.SetActive (false);

		if (LcMenu != null)
			LcMenu.SetActive (false);
    }

	public virtual void Update () {

	    if (IsEnemyCard && (Client.Game.CurrentStage == GameStage.SETUP ||
	                        (State != States.INPLAY && State != States.INFO)))
	        setCardFOW(true);
	    else
	        setCardFOW(false);

	    if (Input.GetKeyDown (KeyCode.Escape)) {	//Handles closing menus

			if (State == States.INFO) {	//Close full info menu
			    transform.localScale = Vector3.one;
				FullInfoCanvas.SetActive (false);
				StatOverlay.SetActive (true);
				GetComponent<Canvas> ().sortingOrder -= 100;
				returnToParent();

				ChangeState(StateToReturnTo == States.EXPANDINHAND ? States.INHAND : StateToReturnTo);

				GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = false;
			} else if (State == States.INPLAY && Client.isCardSelected(this)) {	//deselects this CurrentCard
				Client.deselectCard(this);
			}
		}

	    SelectedIndicator.SetActive(Client.isCardSelected(this));
	}

	public abstract void changeCard (CardInfo c);
	public abstract bool canTarget (Card target);
	public abstract void assignTarget (Card target);
	
	public void clearTarget() {
		
		assignTarget (null);
	}

	public void changeReturnParent(Transform newParent) {

		ParentToReturnTo = newParent;
	}

	/// <summary>
	/// Assigns a unique ID to the CurrentCard.
	/// </summary>
	/// <param name="uid">The unique ID to assign.</param>
	public void createUID(int uid) {
		UID = uid;
		Debug.Log (string.Format("Card assigned UID: {0}", UID));
	}

    public virtual void OnDestroy()
    {
        FieldManager.RemoveCardFromField(this);
    }

    public virtual void OnPlay()
    {
        ChangeState(States.INPLAY);
    }

    public virtual void sendCardToGraveyard ()
	{
        if (FowActive) Destroy(gameObject);
	    if (State == States.INHAND && State != States.EXPANDINHAND) {
			Vector3 pos = this.transform.localPosition;
			pos.y += 2.28f;
			this.transform.localPosition = pos;
			ChangeState(States.EXPANDINHAND);
		}
		if (LcMenu.activeSelf)
			LcMenu.SetActive (false);

	    if (IsEnemyCard && State != States.INHAND) transform.SetParent(null);
	    DeathHandler.SetActive (true);
	}

	public void returnToParent() {
	    this.transform.SetParent (ParentToReturnTo);
		this.transform.localPosition = Vector3.zero;
	}

    public void setCardFOW(bool fow)
    {
        if (CardInfo == null || FowActive == fow) return;
        FowActive = fow;
        if (FowActive)
        {
            transform.Find("Overlay").gameObject.SetActive(false);
            Image image = transform.Find("CardImage").GetComponent<Image>();
            image.sprite =
                Resources.Load(IsEnemyCard ? "Cards/cardBackRed" : "Cards/cardBackBlue", typeof(Sprite)) as Sprite;
        }
        else
        {
            transform.Find("Overlay").gameObject.SetActive(true);
            Sprite s = Resources.Load("Cards/" + CardInfo.GetId(), typeof(Sprite)) as Sprite;
            if (s != null)
            {
                Image image = transform.Find("CardImage").GetComponent<Image>();
                image.sprite =
                    Resources.Load("Cards/" + CardInfo.GetId(), typeof(Sprite)) as Sprite;
            }
        }
    }

    public void ReturnToHand()
    {
        if (State == States.INHAND || State == States.EXPANDINHAND) return;

        ChangeState(States.INHAND);
        ParentToReturnTo = GameObject.Find(IsEnemyCard ? "enemyHand" : "playerHand").transform;
        if (IsEnemyCard) setCardFOW(true);
        returnToParent();
    }

    public virtual bool dragPass()
	{

	    if (IsEnemyCard) return false;

		if (!Client.Game.canTakeAction (Actions.PLAY) && GetComponent<GraphicRaycaster>().enabled)
			return false;

		if (DeathHandler.activeSelf)
			return false;

		if (State != States.INHAND && State != States.EXPANDINHAND) {
			
			if (State == States.INPLAY && GetComponent<GraphicRaycaster>().enabled)
				return false;
		}

		return true;
	}

	public void prepareDrag() {

		Client.Dragging = true;
		
		if (LcMenu.activeSelf)
			LcMenu.SetActive (false);
		ParentToReturnTo = this.transform.parent;
		this.transform.SetParent (null);
		this.GetComponent<Canvas> ().sortingOrder += 100;
		this.GetComponent<GraphicRaycaster> ().enabled = false;
	}

	public virtual void endDrag() {
		
		Client.Dragging = false;
		Client.CardDragged = null;

	    this.returnToParent ();
		if (State == States.EXPANDINHAND)
			ChangeState(States.INHAND);
		
		if (this.GetComponent<Canvas> ().sortingOrder > 99)
			this.GetComponent<Canvas> ().sortingOrder -= 100;

		this.GetComponent<GraphicRaycaster> ().enabled = true;
	}

	public virtual void OnBeginDrag(PointerEventData eventData) {

		if (eventData.button != PointerEventData.InputButton.Left || !dragPass())
			return;
		prepareDrag ();
	    Client.CardDragged = this;
	}

	public virtual void OnDrag(PointerEventData eventData) {
		
		if (eventData.button != PointerEventData.InputButton.Left || !dragPass())
			return;
		this.transform.position = Camera.main.ScreenToWorldPoint (eventData.position) + new Vector3(0.0f, 0.0f, 10.0f);
	}

	public virtual void OnEndDrag(PointerEventData eventData) {

		if (eventData.button != PointerEventData.InputButton.Left || !dragPass())
			return;
		endDrag ();
	}

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;

        Card card = eventData.pointerDrag.GetComponent<Card>();
        if (!card.dragPass()) return;
        if (card is CardUnique)
        {
            CardUnique cardUnique = (CardUnique) card;
            if (cardUnique.canTarget(this)) cardUnique.OnPlay(this);
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData) {

		if (Client.Dragging || DeathHandler.activeSelf || FowActive)
			return;

		switch (eventData.button)
		{

		case PointerEventData.InputButton.Right:

		        if (Client.isCardSelected(this))
		            Client.deselectCard(this);

			LcMenu.SetActive(false);

		    transform.localScale = new Vector3(3.0f, 3.0f, 1.0f);
			this.transform.SetParent(null);
			this.GetComponent<Canvas> ().sortingOrder += 100;
			StatOverlay.SetActive (false);

			FullInfoCanvas.SetActive (true);
			this.transform.localPosition = new Vector3(-3.33f, 0.0f, 0.0f);
			StateToReturnTo = State;
			ChangeState(States.INFO);

			GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = true;

			break;
		case PointerEventData.InputButton.Left:

		        if (IsEnemyCard) break;

			if (!Client.Game.canTakeAction(Actions.MENU))
				break;

			if (State == States.INPLAY && CardInfo.GetCardType() != CardInfo.CardType.Auxiliary && Client.Game.canTakeAction(Actions.ATTACK)) {

				if (Client.isCardSelected(this))
					Client.deselectCard(this);
				else
					Client.selectCard(this, true);
			}
			LcMenu.SetActive(true);
			break;
  		}
	}

	public virtual void OnPointerEnter(PointerEventData eventData) {
		
		if (Client.Dragging)
			return;

	    if (State == States.INHAND)
	    {
	        Vector3 pos = this.transform.localPosition;
	        pos.y += 2.28f;
//			parentToReturnTo = this.transform.parent;
//			this.transform.SetParent(null);
	        this.transform.localPosition = pos;
	        ChangeState(States.EXPANDINHAND);
	    }
	    else
	    {
	        GameObject.Find("StatusEffectInfo").GetComponent<StatusEffectInfo>().UpdateList(this);
	    }
	}
	
	public virtual void OnPointerExit(PointerEventData eventData) {

		if (Client.Dragging)
			return;

		if (State == States.EXPANDINHAND) {

			Vector3 pos = this.transform.localPosition;
			if (this.transform.localPosition.y > 2.0f)
				pos.y -= 2.28f;
//			this.transform.SetParent (parentToReturnTo);
			this.transform.localPosition = pos;
			ChangeState(States.INHAND);
		}
		if (LcMenu.activeSelf && !Client.Awakening)
			LcMenu.SetActive (false);
	    GameObject.Find("StatusEffectInfo").GetComponent<StatusEffectInfo>().UpdateList(null);
	}

    public void ChangeState(States state)
    {
        State = state;
        if (State == States.INHAND) GetComponent<Canvas>().sortingOrder = 10;
        EventHandler handler = StateChangedEvent;
        if (handler != null) handler.Invoke(this, new StateChangedEventArgs(State));
    }
}

public class StateChangedEventArgs : EventArgs
{
    public Card.States State { get; private set; }

    public StateChangedEventArgs(Card.States state)
    {
        State = state;
    }
}