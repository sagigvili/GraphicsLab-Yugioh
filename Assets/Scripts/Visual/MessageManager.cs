using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour 
{
    public GameObject MessagePanel;
    public Image YourTurn;
    public Image PcTurn;

    public static MessageManager Instance;

    void Awake()
    {
        Instance = this;
        MessagePanel.SetActive(false);
    }

    public void ShowMessage(bool turn, float Duration, Command com)
    {
        MessagePanel.SetActive(true);
        StartCoroutine(ShowMessageCoroutine(turn, Duration, com));
    }

    IEnumerator ShowMessageCoroutine(bool turn, float Duration, Command com)
    {
        // If turn is 1 then show "Your Turn" message, else "PC Turn"
        if (turn)
        {
            YourTurn.gameObject.SetActive(true);
            PcTurn.gameObject.SetActive(false);
        }
        else
        {
            YourTurn.gameObject.SetActive(false);
            PcTurn.gameObject.SetActive(true);
        }
        MessagePanel.SetActive(true);

        yield return new WaitForSeconds(Duration);

        MessagePanel.SetActive(false);
        Command.CommandExecutionComplete();
    }
}
