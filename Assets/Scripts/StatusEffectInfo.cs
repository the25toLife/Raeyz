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
            // If card contains DISSIPATE effect then hide all other effects
            if (statusEffect is DissipateEffect)
                foreach (Transform child in transform) Destroy(child.gameObject);

            GameObject statusEffectInfoElement = Instantiate(StatusEffectInfoElementPrefab);
            statusEffectInfoElement.GetComponent<StatusEffectInfoElement>().SetStatusEffect(statusEffect);
            statusEffectInfoElement.transform.SetParent(transform);

            if (statusEffect is DissipateEffect) break;
        }
    }
}
