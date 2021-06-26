using UnityEngine;
using System.Collections;
using System;

public class ShowMessageCommand : Command {

    bool turn;
    float duration;

    public ShowMessageCommand(bool turn, float duration)
    {
        this.turn = turn;
        this.duration = duration;
    }

    public override void StartCommandExecution()
    {
        MessageManager.Instance.ShowMessage(turn, duration, this);
    }
}
