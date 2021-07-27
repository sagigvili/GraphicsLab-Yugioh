using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectState : MonoBehaviour
{

    private bool CanChangeState = false;
    public bool canChangeState
    {
        get
        {
            return CanChangeState;
        }
        set
        {
            CanChangeState = value;
        }
    }

}
