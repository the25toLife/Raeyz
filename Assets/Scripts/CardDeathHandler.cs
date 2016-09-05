using UnityEngine;
using System.Collections;

public class CardDeathHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		if (!this.GetComponent<Animation> ().isPlaying)
			GameObject.Destroy(this.GetComponentInParent<Card> ().gameObject);
	}
}
