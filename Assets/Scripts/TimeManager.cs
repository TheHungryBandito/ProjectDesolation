using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private float resetTimer;
    private bool isTimeModified = false;

    private void Start()
    {
        OverwatchAction.OnAnyOverwatchTriggered += OverwatchAction_OnAnyOverwatchTriggered;
    }

    private void Update()
    {
        if(!isTimeModified)
        {
            return;
        }

        resetTimer -= Time.deltaTime;
        if(resetTimer <= 0)
        {
            Time.timeScale = 1f;
            isTimeModified = false;
        }
    }

    private void ModifyTime(float timeScale, float resetTimer)
    {
        Time.timeScale = timeScale;
        float timerAmount = resetTimer * Time.timeScale;
        this.resetTimer = timerAmount;
        isTimeModified = true;
    }

    private void OverwatchAction_OnAnyOverwatchTriggered(object sender, OverwatchAction.OnOverwatchTriggeredArgs e)
    {
        ModifyTime(0.1f, 2f);
    }
}
