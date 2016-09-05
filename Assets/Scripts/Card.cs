using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//theredhex FFBDBD
//fontfordesc antipasto : avantgarde : sentrygothic

public abstract class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	public enum States {

		INHAND, INPLAY, INFO, EXPANDINHAND, DISABLED
	};

	protected Transform parentToReturnTo;
	protected States stateToReturnTo;
	
	public GameObject fullInfoCanvas, statOverlay, lcMenu, deathHandler, selectedIndicator;
	public ClientGame client;
	
	public int UID { get; set; }
	public CardInfo CardI { get; set; }
	public States State;// { get; set; }
	
	public virtual void Start () {

		client = GameObject.FindObjectOfType<ClientGame> ();

		fullInfoCanvas.SetActive (false);

		if (lcMenu != null)
			lcMenu.SetActive (false);

		float width = this.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
		width = 2.22f / width;		
		float height = this.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		height = 3.01f / height;
		this.transform.localScale = new Vector2 (width, height);
	}

	public virtual void Update () {

		this.GetComponentInChildren<Canvas> ().sortingOrder = this.GetComponent<SpriteRenderer> ().sortingOrder + 1;


		if (Input.GetKeyDown (KeyCode.Escape)) {	//Handles closing menus

			if (this.State == States.INFO) {	//Close full info menu
				this.transform.localScale = Vector3.one;
				fullInfoCanvas.SetActive (false);
				statOverlay.SetActive (true);
				this.GetComponent<SpriteRenderer> ().sortingOrder -= 100;
				this.returnToParent();


				this.State = (stateToReturnTo == States.EXPANDINHAND ? States.INHAND : stateToReturnTo);

				GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = false;
			} else if (this.State == States.INPLAY && client.isCardSelected(this)) {	//deselects this card
				client.deselectCard(this);
			}
		}

		if (client.isCardSelected(this)) {		//activates the selected indicator
			selectedIndicator.SetActive(true);
		} else
			selectedIndicator.SetActive(false);
	}

	public abstract void changeCard (CardInfo c);
	public abstract bool canTarget (Card target);
	public abstract void assignTarget (Card target);
	
	public void clearTarget() {
		
		this.assignTarget (null);
	}

	public void changeReturnParent(Transform newParent) {

		parentToReturnTo = newParent;
	}

	/// <summary>
	/// Assigns a unique ID to the card.
	/// </summary>
	/// <param name="uid">The unique ID to assign.</param>
	public void createUID(int uid) {
		UID = uid;
		Debug.Log (string.Format("Card assigned UID: {0}", UID));
	}

	public virtual void sendCardToGraveyard () {

		if (State == States.INHAND && State != States.EXPANDINHAND) {
			Vector3 pos = this.transform.localPosition;
			pos.y += 2.28f;
			this.transform.localPosition = pos;
			State = States.EXPANDINHAND;
		}
		if (lcMenu.activeSelf)
			lcMenu.SetActive (false);
		deathHandler.SetActive (true);
	}

	public void returnToParent() {
		this.transform.SetParent (parentToReturnTo);
		this.transform.localPosition = Vector3.zero;
	}


	public virtual bool dragPass() {

		if (!client.game.canTakeAction (Actions.PLAY) && this.GetComponent<BoxCollider2D>().enabled)
			return false;

		if (deathHandler.activeSelf)
			return false;

		if (State != States.INHAND && State != States.EXPANDINHAND) {
			
			if (State == States.INPLAY && this.GetComponent<BoxCollider2D>().enabled)
				return false;
		}

		return true;
	}

	public void prepareDrag() {

		GameObject.FindObjectOfType<ClientGame> ().dragging = true;
		client.cardDragged = this;
		
		if (lcMenu.activeSelf)
			lcMenu.SetActive (false);
		parentToReturnTo = this.transform.parent;
		this.transform.SetParent (null);
		this.GetComponent<SpriteRenderer> ().sortingOrder += 100;
		this.GetComponent<BoxCollider2D> ().enabled = false;
	}

	public void endDrag() {
		
		GameObject.FindObjectOfType<ClientGame> ().dragging = false;
		client.cardDragged = null;

		this.returnToParent ();
		if (State == States.EXPANDINHAND)
			State = States.INHAND;
		
		if (this.GetComponent<SpriteRenderer> ().sortingOrder > 99)
			this.GetComponent<SpriteRenderer> ().sortingOrder -= 100;

		this.GetComponent<BoxCollider2D> ().enabled = true;
	}

	public virtual void OnBeginDrag(PointerEventData eventData) {

		if (eventData.button != PointerEventData.InputButton.Left || !dragPass())
			return;
		prepareDrag ();
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

	public virtual void OnPointerClick(PointerEventData eventData) {

		if (client.dragging || deathHandler.activeSelf)
			return;

		switch (eventData.button) {

		case (PointerEventData.InputButton.Right):

			if (client.isCardSelected(this))
			    client.deselectCard(this);

			lcMenu.SetActive(false);

			this.transform.localScale = new Vector3(3.0f, 3.0f, 1.0f);
			this.transform.SetParent(null);
			this.GetComponent<SpriteRenderer> ().sortingOrder += 100;
			statOverlay.SetActive (false);

			fullInfoCanvas.SetActive (true);
			this.transform.localPosition = new Vector3(-3.33f, 0.0f, 0.0f);
			stateToReturnTo = State;
			State = States.INFO;

			GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = true;

			break;
		case (PointerEventData.InputButton.Left):

			if (!client.game.canTakeAction(Actions.MENU))
				break;

			if (State == States.INPLAY && client.game.canTakeAction(Actions.ATTACK)) {

				if (client.isCardSelected(this))
					client.deselectCard(this);
				else
					client.selectCard(this, true);
			}
			lcMenu.SetActive(true);
			break;
  		}
	}

	public virtual void OnPointerEnter(PointerEventData eventData) {
		
		if (client.dragging)
			return;

		if (State == States.INHAND) {
			Vector3 pos = this.transform.localPosition;
			pos.y += 2.28f;
//			parentToReturnTo = this.transform.parent;
//			this.transform.SetParent(null);
			this.transform.localPosition = pos;
			State = States.EXPANDINHAND;
		}
	}
	
	public virtual void OnPointerExit(PointerEventData eventData) {

		if (client.dragging)
			return;

		if (State == States.EXPANDINHAND) {

			Vector3 pos = this.transform.localPosition;
			if (this.transform.localPosition.y > 2.0f)
				pos.y -= 2.28f;
//			this.transform.SetParent (parentToReturnTo);
			this.transform.localPosition = pos;
			State = States.INHAND;
		}
		if (lcMenu.activeSelf && !client.awakening)
			lcMenu.SetActive (false);
	}
}