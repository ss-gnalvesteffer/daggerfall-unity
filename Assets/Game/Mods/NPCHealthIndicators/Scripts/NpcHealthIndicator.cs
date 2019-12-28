using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using TMPro;
using UnityEngine;

public class NpcHealthIndicator : MonoBehaviour
{
    private const float IndicatorMargin = 0.1f;

    private DaggerfallEntityBehaviour _entityBehaviour;
    private EnemyMotor _enemyMotor;
    private TextMeshPro _textMeshPro;
    private GameObject _healthBar;
    private SpriteRenderer _healthBarBackgroundFillSpriteRenderer;
    private SpriteRenderer _healthBarForegroundFillSpriteRenderer;
    private float _fullHealthBarFillWidth;
    private bool _shouldShowHealthBar;
    private Color _friendlyColor;
    private Color _enemyColor;
    private Color _pacifiedColor;
    private int _indicatorVisibilityDistance;
    private bool _shouldOnlyShowIndicatorIfNpcIsHurt;
    private int _textDisplayType;

    private void Start()
    {
        _textMeshPro = gameObject.GetComponentInChildren<TextMeshPro>();
        _entityBehaviour = GetComponentInParent<DaggerfallEntityBehaviour>();
        _enemyMotor = _entityBehaviour.GetComponent<EnemyMotor>();
        _healthBar = transform.Find("HealthBar").gameObject;
        _healthBarBackgroundFillSpriteRenderer = _healthBar.transform.Find("BackgroundFill").GetComponent<SpriteRenderer>();
        _healthBarForegroundFillSpriteRenderer = _healthBar.transform.Find("ForegroundFill").GetComponent<SpriteRenderer>();
        _fullHealthBarFillWidth = _healthBarForegroundFillSpriteRenderer.size.x;
        ApplySettings();
    }

    private void Update()
    {
        UpdateHealthBar();
        UpdateDisplayText();
        UpdateTransform();
    }

    private void ApplySettings()
    {
        var settings = NpcHealthIndicatorMod.Mod.GetSettings();
        _shouldShowHealthBar = settings.GetBool("Settings", "ShowHealthBar");
        _healthBar.SetActive(_shouldShowHealthBar);
        _friendlyColor = settings.GetColor("Settings", "FriendlyNpcIndicatorColor");
        _enemyColor = settings.GetColor("Settings", "EnemyNpcIndicatorColor");
        _pacifiedColor = settings.GetColor("Settings", "PacifiedNpcIndicatorColor");
        _indicatorVisibilityDistance = settings.GetInt("Settings", "IndicatorVisibilityDistance");
        _shouldOnlyShowIndicatorIfNpcIsHurt = settings.GetBool("Settings", "ShowWhenNpcIsHurt");
        _textDisplayType = settings.GetInt("Settings", "TextDisplayType");
    }

    private void UpdateHealthBar()
    {
        if (!_shouldShowHealthBar)
        {
            return;
        }
        _healthBar.SetActive(ShouldShowHealthIndicator());
        var color = GetIndicatorColor();
        _healthBarBackgroundFillSpriteRenderer.color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, color.a);
        _healthBarForegroundFillSpriteRenderer.color = color;
        _healthBarForegroundFillSpriteRenderer.size = new Vector2(
            _fullHealthBarFillWidth * _entityBehaviour.Entity.CurrentHealthPercent,
            _healthBarForegroundFillSpriteRenderer.size.y
        );
    }

    private void UpdateDisplayText()
    {
        if (_textDisplayType == 0 || !ShouldShowHealthIndicator())
        {
            _textMeshPro.SetText(string.Empty);
            return;
        }

        if (_shouldShowHealthBar)
        {
            _textMeshPro.color = Color.white;
        }
        else
        {
            _textMeshPro.color = GetIndicatorColor();
        }

        switch (_textDisplayType)
        {
            case 1: // health percentage
            {
                _textMeshPro.SetText(
                    string.Format(
                        "HP: {0:0}%",
                        _entityBehaviour.Entity.CurrentHealthPercent * 100
                    )
                );
                return;
            }
            case 2: // hitpoints / total hitpoints
            {
                _textMeshPro.SetText(
                    string.Format(
                        "HP: {0}/{1}",
                        _entityBehaviour.Entity.CurrentHealth,
                        _entityBehaviour.Entity.MaxHealth
                    )
                );
                return;
            }
            case 3: // hitpoints
            {
                _textMeshPro.SetText(
                    string.Format(
                        "HP: {0}",
                        _entityBehaviour.Entity.CurrentHealth
                    )
                );
                return;
            }
            case 4: // name
            {
                _textMeshPro.SetText(_entityBehaviour.Entity.Name);
                return;
            }
        }
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

    private bool ShouldShowHealthIndicator()
    {
        var healthPercent = _entityBehaviour.Entity.CurrentHealthPercent;
        return
            (!_shouldOnlyShowIndicatorIfNpcIsHurt || _shouldOnlyShowIndicatorIfNpcIsHurt && healthPercent > 0 && healthPercent < 1) &&
            (transform.position - Camera.main.transform.position).magnitude <= _indicatorVisibilityDistance;
    }

    private Color GetIndicatorColor()
    {
        if (_entityBehaviour.Entity.Team == MobileTeams.PlayerAlly)
        {
            return _friendlyColor;
        }
        if (_enemyMotor && !_enemyMotor.IsHostile)
        {
            return _pacifiedColor;
        }
        return _enemyColor;
    }
}
