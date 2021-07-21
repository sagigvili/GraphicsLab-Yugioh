using UnityEngine;
using System.Collections;



public class PlayerPowerCommand : Command {
    private PlayerPowerButton ppb;
    public PlayerPowerCommand(PlayerPowerButton _ppb)
    {
        ppb = _ppb;
    }

    public override void StartCommandExecution()
    {
        GlobalSettings.Instance.Players[ppb.owner].UsePlayerPower();
        ppb.WasUsed = !ppb.WasUsed;
        CommandExecutionComplete();
    }




}
