using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhasesActions : MonoBehaviour
{
    private int flag = 1;

    public void ClickByFlag()
    {
        switch (flag)
        {
            case 1:
                EndTurn();
                flag = 2;
                GlobalSettings.Instance.EndTurnButton.GetComponentInChildren<Text>().text = "Attack Phase";
                break;
            case 2:
                OnAttackPhase();
                GlobalSettings.Instance.EndTurnButton.GetComponentInChildren<Text>().text = "End Turn";
                flag = 1;
                break;
        }

    }
    public void EndTurn()
    {
        TurnManager.Instance.EndTurn();
    }

    public void OnAttackPhase()
    {
        TurnManager.Instance.whoseTurn.AddAttackToAllMonstersOnTable();
        TurnManager.Instance.whoseTurn.HighlightPlayableCards();
    }

}
