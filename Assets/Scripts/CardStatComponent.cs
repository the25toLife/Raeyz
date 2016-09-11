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
				this.GetComponent<Image> ().sprite = Resources.Load (string.Format ("Cards/Stats/num_{0}", m.GetLevel()), typeof(Sprite)) as Sprite;
			}
			break;
		case (StatType.ATTACK):
			
			if (c is MonsterInfo) {
				
				MonsterInfo m = (MonsterInfo)c;
			    MonsterInfo baseStatInfo = ((MonsterInfo) CardPool.Cards[m.GetId()-1]);
			    Image image = GetComponent<Image>();
			    image.sprite = Resources.Load (string.Format ("Cards/Stats/num_{0}", m.Attack), typeof(Sprite)) as Sprite;
			    if (m.Attack < baseStatInfo.Attack)
			        image.color = new Color(0.843f, 0.522f, 0.522f);
			    else if (m.Attack > baseStatInfo.Attack)
			        image.color = new Color(0.580f, 0.804f, 0.902f);
			    else
			        image.color = Color.white;
			}
			break;
		case (StatType.DEFENSE):
		
			if (c is MonsterInfo) {
				
				MonsterInfo m = (MonsterInfo)c;
			    MonsterInfo baseStatInfo = ((MonsterInfo) CardPool.Cards[m.GetId()-1]);
			    Image image = GetComponent<Image>();
			    image.sprite = Resources.Load (string.Format ("Cards/Stats/num_{0}", m.Defense), typeof(Sprite)) as Sprite;
			    if (m.Defense < baseStatInfo.Defense)
			        image.color = new Color(0.843f, 0.522f, 0.522f);
			    else if (m.Defense > baseStatInfo.Defense)
			        image.color = new Color(0.580f, 0.804f, 0.902f);
			    else
			        image.color = Color.white;
			}
			break;
		case (StatType.NAME):
		
			this.GetComponent<Text>().text = c.GetName();
			break;
		case (StatType.TYPE):
			
			this.GetComponent<Text>().text = c.GetCardType().ToString();
			break;
		case (StatType.AFFINITY):

			if (c is MonsterInfo) {
				
				MonsterInfo m = (MonsterInfo)c;
				this.GetComponent<Text>().text = m.GetAffinity().ToString().ToLower();
			}
			break;
		case (StatType.AFFICO):
			
			if (c is MonsterInfo) {
				
				MonsterInfo m = (MonsterInfo)c;
				this.GetComponent<Image> ().sprite = Resources.Load (string.Format ("Cards/Stats/{0}", m.GetAffinity().ToString().ToLower()), typeof(Sprite)) as Sprite;
			}
			break;
		case (StatType.DESC):

			this.GetComponent<Text>().text = c.GetDesc();
			break;
		case (StatType.IMAGE):

			this.GetComponent<Image> ().sprite = Resources.Load (string.Format ("Cards/{0}", c.GetId()), typeof(Sprite)) as Sprite;
			break;
		}
	}
}
