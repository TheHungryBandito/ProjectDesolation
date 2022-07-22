using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        //Updates when any unit changes actionPoints
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }
    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void OnDestroy()
    {
        healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
    }

}
