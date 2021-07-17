using UnityEngine;
using System.Collections;
using DG.Tweening;


public class ShowTrapSelector : MonoBehaviour
{
    GameObject newTarpSelector;


    public void showTrap()
    {
        newTarpSelector = GameObject.Instantiate(GlobalSettings.Instance.TrapSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        newTarpSelector.SetActive(true);
        /*IEnumerator coroutine = WaitForFinished();*/

        StartCoroutine("WaitForFinished");
        Debug.Log("I'm after sheker");
        
    }



    IEnumerator WaitForFinished()
    {
        while (true)
        {
            Debug.Log(" HAKOL : " + newTarpSelector.GetComponent<TrapSelector>().getFinished());
            if (!newTarpSelector.GetComponent<TrapSelector>().getFinished())
            {
                newTarpSelector.SetActive(false);
                yield return 0;
            }
                
        }
    }
}



public class ActivateTrapCommand : Command {
    private GameObject go;

    public ActivateTrapCommand()
    {
        go = new GameObject();
    }

    public override void StartCommandExecution()
    {

        ShowTrapSelector s = go.GetComponent<ShowTrapSelector>();
        s.showTrap();
        CommandExecutionComplete();
    }




}
