using UnityEngine;

public class CardDeathHandler : MonoBehaviour {

	// Update is called once per frame
	void Update () {
	
		if (!this.GetComponent<Animation> ().isPlaying)
			GameObject.Destroy(this.GetComponentInParent<Card> ().gameObject);
	}
}
