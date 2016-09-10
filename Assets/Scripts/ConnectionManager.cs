using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class ConnectionManager : MonoBehaviour {

	public SceneManager Game;
	public GUIStyle MyStyle;
	public GameObject HandCardPrefab;

	void Start () {

		Application.runInBackground = true;
		Game = new SceneManager ();
		Game.AppId = "d5b703ed-840c-4cc1-8771-ffa8f9a6b49c";
		
		Game.OnStateChangeAction += this.OnStateChanged;
		
		Game.ConnectToRegionMaster ("us");

	}

	private void OnStateChanged(ClientState state)
	{
		if (state == ClientState.ConnectedToMaster)
		{
			Debug.Log("connected to master");
			Game.OpJoinRandomRoom (null, 0);
		}
	}
	
	void Update() {
		
		Game.Service ();

		if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Q))
		{
			Application.Quit();
		} 
	}

	public void OnApplicationQuit()
	{
		if (Game != null && Game.loadBalancingPeer != null)
		{
			Game.Disconnect();
			Game.loadBalancingPeer.StopThread();
		}
		Game = null;
	}

	public void OnGUI() {

//		if (game.State == ClientState.Joined)
//			GUILayout.Label(string.Format("In Room: {0} as {1}", game.CurrentRoom.Name, game.LocalPlayer.ID), myStyle);
	}
}
