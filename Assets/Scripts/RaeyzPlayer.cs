using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RaeyzPlayer : Player
{
	private int lifePoints = 200;
	private bool ready, turn;
	public int Life { get { return lifePoints; } }

	protected internal RaeyzPlayer(string name, int actorID, bool isLocal, Hashtable actorProperties) : base(name, actorID, isLocal, actorProperties)
	{
	}
	
	public override string ToString()
	{
		return base.ToString() + ((this.IsInactive) ? " (inactive)" : "");
	}

	public void damagePlayer(int dmg) {

		lifePoints -= dmg;
		this.SetCustomProperties( new Hashtable() {{"l", lifePoints}});
	}

	public void changeStatus(bool b) {
		ready = b;
		this.SetCustomProperties( new Hashtable() {{"r", ready}});
	}

	public void changeTurnIndication(bool b) {
		turn = b;
	}

	public bool isTurn() {
		return turn;
	}

	public bool isReady() {
		return ready;
	}
}