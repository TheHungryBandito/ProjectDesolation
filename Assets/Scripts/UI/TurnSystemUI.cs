using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystemUI : MonoBehaviour
{

    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;


    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateTurnNumberText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnNumberText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnNumberText()
    {
        turnNumberText.text = $"TURN: {TurnSystem.Instance.GetTurnNumber()}";
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }
    private void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
