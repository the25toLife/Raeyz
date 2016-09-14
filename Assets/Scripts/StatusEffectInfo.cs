using JetBrains.Annotations;
using UnityEngine;

public class StatusEffectInfo : MonoBehaviour
{
    public GameObject StatusEffectInfoElementPrefab;

    public void UpdateList(Card card)
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        if (card == null) return;

        foreach (StatusEffect statusEffect in card.StatusEffects)
        {
            GameObject statusEffectInfoElement = Instantiate(StatusEffectInfoElementPrefab);
            statusEffectInfoElement.GetComponent<StatusEffectInfoElement>().StatusEffect = statusEffect;
            statusEffectInfoElement.transform.SetParent(transform);
        }
    }
}
