using DaggerfallWorkshop.Game.Entity;
using TMPro;
using UnityEngine;

public class NPCHealthIndicator : MonoBehaviour
{
    private const float IndicatorMargin = 0.1f;

    private DaggerfallEntityBehaviour _entityBehaviour;
    private TextMeshPro _textMeshPro;

    private void Start()
    {
        _textMeshPro = gameObject.GetComponent<TextMeshPro>();
        _entityBehaviour = GetComponentInParent<DaggerfallEntityBehaviour>();
    }

    private void Update()
    {
        UpdateHealthText();
        UpdateTransform();
    }

    private void UpdateHealthText()
    {
        var healthPercent = _entityBehaviour.Entity.CurrentHealthPercent;
        _textMeshPro.SetText(
            healthPercent == 0 || healthPercent == 1
                ? string.Empty
                : string.Format("HP: {0:0}%", healthPercent * 100)
        );
    }

    private void UpdateTransform()
    {
        var camera = Camera.main;
        var billboard = transform.parent.Find("MobileUnitBillboard") ?? transform.parent.Find("MobilePersonBillboard");
        var billboardHeight = billboard != null ? billboard.transform.localScale.y : 1.0f;
        transform.position =
            (transform.parent.position - ((transform.parent.position - camera.transform.position).normalized * 0.1f)) +
            new Vector3(0, billboardHeight / 2 + IndicatorMargin, 0);
        transform.rotation = camera.transform.rotation;
    }
}
