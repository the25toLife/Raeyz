using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class StatusInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	ClientGame client;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (client == null)
			client = GameObject.FindObjectOfType<ClientGame> ();
	}	

	public void OnPointerEnter(PointerEventData eventData) {
		
		if (client.dragging)
			return;
		
		
		if (this.transform.localPosition.x < 20.0f) {
			Vector3 pos = this.transform.localPosition;
			pos.x += 25.0f;
			//			parentToReturnTo = this.transform.parent;
			//			this.transform.SetParent(null);
			this.transform.localPosition = pos;
		}
	}
	
	public void OnPointerExit(PointerEventData eventData) {
		
		if (client.dragging)
			return;
			
			Vector3 pos = this.transform.localPosition;
			if (this.transform.localPosition.x > 20.0f)
				pos.x -= 25.0f;
			//			this.transform.SetParent (parentToReturnTo);
		this.transform.localPosition = pos;
	}
}
