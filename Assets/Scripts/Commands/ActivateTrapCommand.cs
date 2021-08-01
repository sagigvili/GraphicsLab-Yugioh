using UnityEngine;
using System.Collections;



public class ActivateTrapCommand : Command {
    private int trapOwner;
    private SpellTrapLogic triggredTrap;
    public ActivateTrapCommand(int cardOwnerID, SpellTrapLogic _triggeredTrap)
    {
        trapOwner = cardOwnerID;
        triggredTrap = _triggeredTrap;
    }

    public override void StartCommandExecution()
    {
        if (trapOwner == 1)
        {
            GameObject go = GameObject.Instantiate(GlobalSettings.Instance.TrapSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            go.SetActive(true);
            ShowTrapSelector s = go.GetComponentInChildren<ShowTrapSelector>();
            s.ShowTrap();
        } else
        {
            //GameObject spellTrapObject = IDHolder.GetGameObjectWithID(triggredTrap.ID);
            //spellTrapObject.transform.position = new Vector3(spellTrapObject.transform.position.x, spellTrapObject.transform.position.y, -2);
            TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual.flipSpellTrapCard(triggredTrap.ID);
            new DelayCommand(2.0f).AddToQueue();
            triggredTrap.Die();
            Command.CommandExecutionComplete();
        }

    }




}
