using UnityEngine;
using System.Collections;
using System;

public class SelectStateToTableCommand : Command {

    GameObject dmot;

    public SelectStateToTableCommand(GameObject _dmot)
    {
        dmot = _dmot;
    }

    public override void StartCommandExecution()
    {
        dmot.transform.Find("StatesBalloon").transform.Find("Panel").gameObject.SetActive(true);
    }
}
