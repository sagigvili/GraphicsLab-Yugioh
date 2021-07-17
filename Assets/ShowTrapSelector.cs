using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTrapSelector : MonoBehaviour
{
    GameObject newTarpSelector;


    public void ShowTrap()
    {
        newTarpSelector = GameObject.Instantiate(GlobalSettings.Instance.TrapSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        newTarpSelector.SetActive(true);
        StartCoroutine(WaitForFinished());
    }



    IEnumerator WaitForFinished()
    {
        while (!newTarpSelector.GetComponent<TrapSelector>().getFinished())
            yield return null;
        newTarpSelector.SetActive(false);
        Destroy(newTarpSelector.gameObject);
        Command.CommandExecutionComplete();
    }
}
