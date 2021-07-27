using UnityEngine;
using System.Collections;

public enum AreaPosition{Top, Low}

public class PlayerArea : MonoBehaviour 
{
    public AreaPosition owner;
    public bool ControlsON = true;
    public PlayerDeckVisual PDeck;
    public HandVisual handVisual;
    public PlayerPortraitVisual Portrait;
    public PlayerPowerButton HeroPower;
    //public EndTurnButton EndTurnButton;
    public TableVisual tableVisual;
    public PlayerGraveyardVisual graveyardVisual;
    public Transform PortraitPosition;
    public Transform DamageSpot;

    public bool AllowedToControlThisPlayer
    {
        get;
        set;
    }      


}
