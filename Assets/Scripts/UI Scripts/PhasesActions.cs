using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhasesActions : MonoBehaviour
{
    // flag == 1 : we're in Phase 1
    // flag == 2 : we're in Attack Phase
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
        TurnManager.Instance.whoseTurn.table.InAttackPhase = true;
    }

    public void SetFlag(int val)
    {
        flag = val;
            
    }

}
