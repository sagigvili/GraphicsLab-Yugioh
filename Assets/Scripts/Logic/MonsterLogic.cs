using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public enum FieldPosition
{
    Attack,
    Defence
}

public class MonsterLogic : ICharacter
{
    // PUBLIC FIELDS
    public Player owner;
    public CardAsset ca;
    public MonsterEffect effect;
    public int UniqueMonsterID;
    public int ID
    {
        get{ return UniqueMonsterID; }
    }
    public FieldPosition MonsterPosition;

    //// the basic health that we have in CardAsset
    //private int baseHealth;
    //// health with all the current buffs taken into account
    //public int MaxHealth
    //{
    //    get{ return baseHealth;}
    //}
        
    //private int health;

    //public int Health
    //{
    //    get{ return health; }

    //    set
    //    {
    //        if (value > MaxHealth)
    //            health = baseHealth;
    //        else if (value <= 0)
    //            Die();
    //        else
    //            health = value;
    //    }
    //}

    public bool CanAttack
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            return (ownersTurn && (AttacksLeftThisTurn > 0) && (MonsterPosition == FieldPosition.Attack));
        }
    }

    private int baseAttack;
    public int Attack
    {
        get{ return baseAttack; }

    }

    private int baseDefence;
    public int Defence
    {
        get { return baseDefence; }

    }

    private int attacksForOneTurn = 1;
    public int AttacksLeftThisTurn
    {
        get;
        set;
    }
    public int Health { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    // CONSTRUCTOR
    public MonsterLogic(Player owner, CardAsset ca)
    {
        this.ca = ca;
        MonstersCreatedThisGame.Add(UniqueMonsterID, this);
    }

    public void OnTurnStart()
    {
        AttacksLeftThisTurn = attacksForOneTurn;
    }

    public void Die()
    {
        // TODO send to graveyard
        owner.table.MonstersOnTable.Remove(this);

        new MonsterDieCommand(UniqueMonsterID, owner).AddToQueue();
    }

    public void GoFace()
    {
        AttacksLeftThisTurn--;
        int targetHealthAfter = owner.otherPlayer.Health - Attack;
        new MonsterAttackCommand(owner.otherPlayer.PlayerID, UniqueMonsterID, 0, Attack, targetHealthAfter).AddToQueue();
        owner.otherPlayer.Health -= Attack;
    }

    public void AttackMonster (MonsterLogic target)
    {
        AttacksLeftThisTurn--;
        // calculate the values so that the monster does not fire the DIE command before the Attack command is sent
        if (target.MonsterPosition == FieldPosition.Defence)
        {
            if (Attack > target.Defence)
            {
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, 0, 0, owner.otherPlayer.Health).AddToQueue();
                target.Die();
            } else if ( Attack == target.Defence)
            {
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, 0, 0, owner.otherPlayer.Health).AddToQueue();
            } else
            {
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, 0, 0, owner.otherPlayer.Health).AddToQueue();
                owner.Health -= target.Defence - Attack;
            }

        } else
        {
            if (Attack > target.Attack)
            {
                int Damage = Attack - target.Attack;
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, 0, Damage, owner.otherPlayer.Health - Damage).AddToQueue();
                target.Die();
                owner.otherPlayer.Health -= Damage;
            }
            else if (Attack == target.Attack)
            {
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, 0, 0, owner.otherPlayer.Health).AddToQueue();
                Die();
                target.Die();
            }
            else
            {
                int Damage = target.Attack - Attack;
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, Damage, 0, owner.otherPlayer.Health).AddToQueue();
                Die();
                owner.Health -= Damage;
            }
        }

    }

    public void AttackMonsterWithID(int uniqueMonsterID)
    {
        MonsterLogic target = MonsterLogic.MonstersCreatedThisGame[uniqueMonsterID];
        AttackMonster(target);
    }

    // STATIC For managing IDs
    public static Dictionary<int, MonsterLogic> MonstersCreatedThisGame = new Dictionary<int, MonsterLogic>();

}
