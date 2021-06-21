using UnityEngine;
using System.Collections;

public abstract class MonsterEffect 
{
    protected Player owner;
    protected MonsterLogic monster;
    protected int specialAmount;

    public MonsterEffect(Player owner, MonsterLogic monster, int specialAmount)
    {
        this.monster = monster;
        this.owner = owner;
        this.specialAmount = specialAmount;
    }

    public abstract void RegisterEffect();

    public abstract void CauseEffect();

}
