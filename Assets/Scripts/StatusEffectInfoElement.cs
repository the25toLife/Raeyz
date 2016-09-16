using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectInfoElement : MonoBehaviour
{
    private StatusEffect _statusEffect;
    private Image _cardImage, _icon;
    private Text _desc;
    private LayoutElement _layoutElementComponent;

    [UsedImplicitly]
    private void Awake ()
    {
        _cardImage = transform.Find("CardImage").GetComponent<Image>();
        _icon = transform.Find("Icon").GetComponent<Image>();
        _desc = transform.Find("Description").GetComponent<Text>();
        _layoutElementComponent = GetComponent<LayoutElement>();
    }

    [UsedImplicitly]
    private void Update ()
    {
        _layoutElementComponent.minHeight = (_desc.rectTransform.sizeDelta.y * 0.2f) + 65.2f;
        if (_statusEffect == null) return;

        _desc.text = _statusEffect.ToString();
    }

    public void SetStatusEffect(StatusEffect statusEffect)
    {
        _statusEffect = statusEffect;
        if (_statusEffect == null) return;

        Sprite s;
        s = Resources.Load("Cards/" + _statusEffect.CardAppliedById, typeof(Sprite)) as Sprite;
        if (s != null)
            _cardImage.sprite = s;

        string type = "";
        if (_statusEffect is StatEffect) type = "Boost";
        else if (_statusEffect is ConfusionEffect) type = "Confusion";
        else if (_statusEffect is HealthEffect) type = "Heal";
        else if (_statusEffect is DissipateEffect) type = "Immunity";
        s = Resources.Load("SpecialIcons/" + type, typeof(Sprite)) as Sprite;
        if (s != null)
            _icon.sprite = s;
    }
}
