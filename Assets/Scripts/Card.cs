using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

//theredhex FFBDBD
//fontfordesc antipasto : avantgarde : sentrygothic

public abstract class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	public enum States {

		INHAND, INPLAY, INFO, EXPANDINHAND, DISABLED
	}

    // Differentiates player and enemy cards
    public bool IsEnemyCard { get; set; }

    // Determines whether or not the player can see the CurrentCard's information or only the CurrentCard's back
    public bool FowActive { get; private set; }

    protected Transform ParentToReturnTo;
	protected States StateToReturnTo;
    protected Vector3 Scale { get; set; }

	public GameObject FullInfoCanvas, StatOverlay, LcMenu, DeathHandler, SelectedIndicator;
	public ClientGame Client;

    // ReSharper disable once InconsistentNaming
	public int UID { get; set; }
	public CardInfo CardInfo { get; set; }
	public States State { get; set; }
    public ArrayList StatusEffects { get; set; }

    public virtual void Start () {

		Client = FindObjectOfType<ClientGame> ();

        StatusEffects = new ArrayList();

        FullInfoCanvas.SetActive (false);

		if (LcMenu != null)
			LcMenu.SetActive (false);

		float width = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
		float height = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		transform.localScale = Scale = new Vector3 (2.22f / width, 3.01f / height, 1);
	}

	public virtual void Update () {

	    Canvas canvas = GetComponentInChildren<Canvas>();
	    if (canvas != null) canvas.sortingOrder = GetComponent<SpriteRenderer> ().sortingOrder + 1;

	    if (IsEnemyCard && (Client.Game.CurrentStage == GameStage.SETUP ||
	                        (State != States.INPLAY && State != States.INFO)))
	        setCardFOW(true);
	    else
	        setCardFOW(false);

	    if (Input.GetKeyDown (KeyCode.Escape)) {	//Handles closing menus

			if (State == States.INFO) {	//Close full info menu
			    transform.localScale = Scale;
				FullInfoCanvas.SetActive (false);
				StatOverlay.SetActive (true);
				GetComponent<SpriteRenderer> ().sortingOrder -= 100;
				returnToParent();

				State = StateToReturnTo == States.EXPANDINHAND ? States.INHAND : StateToReturnTo;

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
        var fieldManager = FindObjectOfType<FieldManager>();
        if (fieldManager != null) fieldManager.RemoveCardFromField(this);
    }

    public virtual void OnPlay()
    {
        State = States.INPLAY;
    }

    public virtual void sendCardToGraveyard ()
	{
        if (FowActive) Destroy(gameObject);
	    if (State == States.INHAND && State != States.EXPANDINHAND) {
			Vector3 pos = this.transform.localPosition;
			pos.y += 2.28f;
			this.transform.localPosition = pos;
			State = States.EXPANDINHAND;
		}
		if (LcMenu.activeSelf)
			LcMenu.SetActive (false);

	    transform.SetParent(null);
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
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<SpriteRenderer>().sprite =
                Resources.Load(IsEnemyCard ? "Cards/cardBackRed" : "Cards/cardBackBlue", typeof(Sprite)) as Sprite;
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            Sprite s = Resources.Load("Cards/" + CardInfo.GetId(), typeof(Sprite)) as Sprite;
            if (s != null)
                GetComponent<SpriteRenderer>().sprite = s;
            else
                GetComponent<SpriteRenderer>().sprite = Resources.Load("Cards/_empty", typeof(Sprite)) as Sprite;
        }
    }


    public virtual bool dragPass()
	{

	    if (IsEnemyCard) return false;

		if (!Client.Game.canTakeAction (Actions.PLAY) && GetComponent<BoxCollider2D>().enabled)
			return false;

		if (DeathHandler.activeSelf)
			return false;

		if (State != States.INHAND && State != States.EXPANDINHAND) {
			
			if (State == States.INPLAY && GetComponent<BoxCollider2D>().enabled)
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
		this.GetComponent<SpriteRenderer> ().sortingOrder += 100;
		this.GetComponent<BoxCollider2D> ().enabled = false;
	}

	public void endDrag() {
		
		Client.Dragging = false;
		Client.CardDragged = null;

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

	public virtual void OnPointerClick(PointerEventData eventData) {

		if (Client.Dragging || DeathHandler.activeSelf || FowActive)
			return;

		switch (eventData.button)
		{

		case PointerEventData.InputButton.Right:

		        if (Client.isCardSelected(this))
		            Client.deselectCard(this);

			LcMenu.SetActive(false);

			this.transform.localScale = new Vector3(3.0f * Scale.x, 3.0f * Scale.y, 1.0f);
			this.transform.SetParent(null);
			this.GetComponent<SpriteRenderer> ().sortingOrder += 100;
			StatOverlay.SetActive (false);

			FullInfoCanvas.SetActive (true);
			this.transform.localPosition = new Vector3(-3.33f, 0.0f, 0.0f);
			StateToReturnTo = State;
			State = States.INFO;

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

		if (Client.Dragging)
			return;

		if (State == States.EXPANDINHAND) {

			Vector3 pos = this.transform.localPosition;
			if (this.transform.localPosition.y > 2.0f)
				pos.y -= 2.28f;
//			this.transform.SetParent (parentToReturnTo);
			this.transform.localPosition = pos;
			State = States.INHAND;
		}
		if (LcMenu.activeSelf && !Client.Awakening)
			LcMenu.SetActive (false);
	}
}