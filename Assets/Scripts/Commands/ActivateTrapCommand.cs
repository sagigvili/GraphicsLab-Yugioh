using UnityEngine;
using System.Collections;

public class ActivateTrapCommand : Command {


    public ActivateTrapCommand()
    {

    }

    public override void StartCommandExecution()
    {
        CommandExecutionComplete();
    }
}
