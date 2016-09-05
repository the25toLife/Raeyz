using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardStatComponent : MonoBehaviour {

	public enum StatType {

		LEVEL, ATTACK, DEFENSE, NAME, TYPE, AFFINITY, AFFICO, DESC, IMAGE
	}

	public StatType statType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeStat (CardInfo c) {

		switch (statType) {

		case (StatType.LEVEL):

			if (c is MonsterInfo) {

				MonsterInfo m = (MonsterInfo)c;
				this.GetComponent<Image> ().sprite = Resources.Load (string.Format ("Cards/Stats/num_{0}", m.Level), typeof(Sprite)) as Sprite;
			}
			break;
		case (StatType.ATTACK):
			
			if (c is MonsterInfo) {
				
				MonsterInfo m = (MonsterInfo)c;
				this.GetComponent<Image> ().sprite = Resources.Load (string.Format ("Cards/Stats/num_{0}", m.Attack), typeof(Sprite)) as Sprite;
			}
			break;
		case (StatType.DEFENSE):
		
			if (c is MonsterInfo) {
				
				MonsterInfo m = (MonsterInfo)c;
				this.GetComponent<Image> ().sprite = Resources.Load (string.Format ("Cards/Stats/num_{0}", m.Defense), typeof(Sprite)) as Sprite;
			}
			break;
		case (StatType.NAME):
		
			this.GetComponent<Text>().text = c.Name;
			break;
		case (StatType.TYPE):
			
			this.GetComponent<Text>().text = c.Type.ToString();
			break;
		case (StatType.AFFINITY):

			if (c is MonsterInfo) {
				
				MonsterInfo m = (MonsterInfo)c;
				this.GetComponent<Text>().text = m.Affinity.ToString().ToLower();
			}
			break;
		case (StatType.AFFICO):
			
			if (c is MonsterInfo) {
				
				MonsterInfo m = (MonsterInfo)c;
				this.GetComponent<Image> ().sprite = Resources.Load (string.Format ("Cards/Stats/{0}", m.Affinity.ToString().ToLower()), typeof(Sprite)) as Sprite;
			}
			break;
		case (StatType.DESC):

			this.GetComponent<Text>().text = c.Desc;
			break;
		case (StatType.IMAGE):

			this.GetComponent<Image> ().sprite = Resources.Load (string.Format ("Cards/{0}", c.ID), typeof(Sprite)) as Sprite;
			break;
		}
	}
}
