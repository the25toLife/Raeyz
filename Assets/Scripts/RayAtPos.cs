using UnityEngine;
using System.Collections;

public class RayAtPos : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnGUI() {

		RaycastHit2D hit = Physics2D.Raycast(new Vector2(this.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x,this.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
		if (hit) {
			//    Debug.DrawLine (ray.origin, hit.point);
			GameObject objectHit = hit.collider.gameObject;
			GUILayout.Label (objectHit.name);
		}
	}
}
