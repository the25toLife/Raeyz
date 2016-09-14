using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectInfoElement : MonoBehaviour
{
    public StatusEffect StatusEffect { get; set; }
    private Image _cardImage, _icon;
    private Text _desc;
    private LayoutElement _layoutElementComponent;

    [UsedImplicitly]
    private void Start ()
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
        if (StatusEffect == null) return;

        Sprite s;
        s = Resources.Load("Cards/" + StatusEffect.CardAppliedBy.CardInfo.GetId(), typeof(Sprite)) as Sprite;
        if (s != null)
            _cardImage.sprite = s;

        string type = "";
        if (StatusEffect is StatEffect) type = "Boost";
        else if (StatusEffect is ConfusionEffect) type = "Confusion";
        else if (StatusEffect is HealthEffect) type = "Heal";
        s = Resources.Load("SpecialIcons/" + type, typeof(Sprite)) as Sprite;
        if (s != null)
            _icon.sprite = s;

        _desc.text = StatusEffect.ToString();
    }
}
