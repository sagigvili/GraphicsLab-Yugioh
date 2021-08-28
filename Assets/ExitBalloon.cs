using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBalloon : MonoBehaviour
{
    public void ExitNow()
    {
        GetComponent<StatesChanger>().panel.gameObject.SetActive(false);
    }
}
