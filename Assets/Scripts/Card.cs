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

    protected Transform parentToReturnTo;
	protected States stateToReturnTo;

	public GameObject fullInfoCanvas, statOverlay, lcMenu, deathHandler, selectedIndicator;
	public ClientGame client;

	public int UID { get; set; }
	public CardInfo CardI { get; set; }
	public States State { get; set; }

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

	    Canvas canvas = GetComponentInChildren<Canvas>();
	    if (canvas != null) canvas.sortingOrder = GetComponent<SpriteRenderer> ().sortingOrder + 1;

	    if (IsEnemyCard && (client.game.CurrentStage == GameStage.SETUP ||
	                        (State != States.INPLAY && State != States.INFO)))
	        setCardFOW(true);
	    else
	        setCardFOW(false);

	    if (Input.GetKeyDown (KeyCode.Escape)) {	//Handles closing menus

			if (State == States.INFO) {	//Close full info menu
				transform.localScale = Vector3.one;
				fullInfoCanvas.SetActive (false);
				statOverlay.SetActive (true);
				GetComponent<SpriteRenderer> ().sortingOrder -= 100;
				returnToParent();

				State = stateToReturnTo == States.EXPANDINHAND ? States.INHAND : stateToReturnTo;

				GameObject.FindGameObjectWithTag("blockRays").GetComponent<BoxCollider2D>().enabled = false;
			} else if (State == States.INPLAY && client.isCardSelected(this)) {	//deselects this CurrentCard
				client.deselectCard(this);
			}
		}

	    selectedIndicator.SetActive(client.isCardSelected(this));
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
	/// Assigns a unique ID to the CurrentCard.
	/// </summary>
	/// <param name="uid">The unique ID to assign.</param>
	public void createUID(int uid) {
		UID = uid;
		Debug.Log (string.Format("Card assigned UID: {0}", UID));
	}

    [UsedImplicitly]
    private void OnDestroy()
    {
        FindObjectOfType<FieldManager>().RemoveCardFromField(this);
    }

    public virtual void sendCardToGraveyard ()
	{

	    if (State == States.INHAND && State != States.EXPANDINHAND) {
			Vector3 pos = this.transform.localPosition;
			pos.y += 2.28f;
			this.transform.localPosition = pos;
			State = States.EXPANDINHAND;
		}
		if (lcMenu.activeSelf)
			lcMenu.SetActive (false);

	    transform.SetParent(null);
	    deathHandler.SetActive (true);
	}

	public void returnToParent() {
		this.transform.SetParent (parentToReturnTo);
		this.transform.localPosition = Vector3.zero;
	}

    public void setCardFOW(bool fow)
    {
        if (CardI == null || FowActive == fow) return;
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
            Sprite s = Resources.Load("Cards/" + CardI.GetId(), typeof(Sprite)) as Sprite;
            if (s != null)
                GetComponent<SpriteRenderer>().sprite = s;
            else
                GetComponent<SpriteRenderer>().sprite = Resources.Load("Cards/_empty", typeof(Sprite)) as Sprite;
        }
    }


    public virtual bool dragPass()
	{

	    if (IsEnemyCard) return false;

		if (!client.game.canTakeAction (Actions.PLAY) && GetComponent<BoxCollider2D>().enabled)
			return false;

		if (deathHandler.activeSelf)
			return false;

		if (State != States.INHAND && State != States.EXPANDINHAND) {
			
			if (State == States.INPLAY && GetComponent<BoxCollider2D>().enabled)
				return false;
		}

		return true;
	}

	public void prepareDrag() {

		client.dragging = true;
		
		if (lcMenu.activeSelf)
			lcMenu.SetActive (false);
		parentToReturnTo = this.transform.parent;
		this.transform.SetParent (null);
		this.GetComponent<SpriteRenderer> ().sortingOrder += 100;
		this.GetComponent<BoxCollider2D> ().enabled = false;
	}

	public void endDrag() {
		
		client.dragging = false;
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
	    client.cardDragged = this;
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

		if (client.dragging || deathHandler.activeSelf || FowActive)
			return;

		switch (eventData.button) {

		case PointerEventData.InputButton.Right:

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
		case PointerEventData.InputButton.Left:

		        if (IsEnemyCard) break;

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