using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RaeyzPlayer : Player
{
	private int _lifePoints = 200;
	private bool Ready, Turn;
	public int Life { get { return _lifePoints; } }

	protected internal RaeyzPlayer(string name, int actorID, bool isLocal, Hashtable actorProperties) : base(name, actorID, isLocal, actorProperties)
	{
	}
	
	public override string ToString()
	{
		return base.ToString() + ((this.IsInactive) ? " (inactive)" : "");
	}

	public void damagePlayer(int dmg) {

		_lifePoints -= dmg;
		this.SetCustomProperties( new Hashtable() {{"l", _lifePoints}});
	}

	public void changeStatus(bool b) {
		Ready = b;
		this.SetCustomProperties( new Hashtable() {{"r", Ready}});
	}

	public void changeTurnIndication(bool b) {
		Turn = b;
	}

	public bool isTurn() {
		return Turn;
	}

	public bool isReady() {
		return Ready;
	}
}