using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class ConnectionManager : MonoBehaviour {

	public SceneManager game;
	public GUIStyle myStyle;
	public GameObject handCardPrefab;

	void Start () {

		Application.runInBackground = true;
		game = new SceneManager ();
		game.AppId = "d5b703ed-840c-4cc1-8771-ffa8f9a6b49c";
		
		game.OnStateChangeAction += this.OnStateChanged;
		
		game.ConnectToRegionMaster ("us");

	}

	private void OnStateChanged(ClientState state)
	{
		if (state == ClientState.ConnectedToMaster)
		{
			Debug.Log("connected to master");
			game.OpJoinRandomRoom (null, 0);
		}
	}
	
	void Update() {
		
		game.Service ();

		if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Q))
		{
			Application.Quit();
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

	public void OnGUI() {

//		if (game.State == ClientState.Joined)
//			GUILayout.Label(string.Format("In Room: {0} as {1}", game.CurrentRoom.Name, game.LocalPlayer.ID), myStyle);
	}
}
