using UnityEngine;
using System.Collections;

public class BiteOwner : MonsterEffect
{  
    public BiteOwner(Player owner, MonsterLogic monster, int specialAmount): base(owner, monster, specialAmount)
    {}

    public override void RegisterEffect()
    {
        owner.EndTurnEvent += CauseEffect;
        Debug.Log("Registered bite effect!!!!");
    }

    public override void CauseEffect()
    {
        Debug.Log("InCauseEffect: owner: "+ owner + " specialAmount: "+ specialAmount);
        new DealDamageCommand(owner.PlayerID, specialAmount, owner.Health - specialAmount).AddToQueue();
        owner.Health -= specialAmount;
    }


}
