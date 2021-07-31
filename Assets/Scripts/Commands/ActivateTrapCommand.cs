using UnityEngine;
using System.Collections;



public class ActivateTrapCommand : Command {
    public ActivateTrapCommand()
    {
    }

    public override void StartCommandExecution()
    {
        GameObject go = GameObject.Instantiate(GlobalSettings.Instance.TrapSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        go.SetActive(true);
        ShowTrapSelector s = go.GetComponentInChildren<ShowTrapSelector>();
        s.ShowTrap();   
    }




}
