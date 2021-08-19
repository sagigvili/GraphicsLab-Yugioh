using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public enum FieldPosition
{
    Attack,
    Defence,
    Set
}

public class MonsterLogic : ICharacter
{
    // PUBLIC FIELDS
    public Player owner;
    public CardAsset ca;
    public int UniqueMonsterID;
    public int ID
    {
        get{ return UniqueMonsterID; }
    }

    public bool CanAttack
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            return (ownersTurn && (AttacksLeftThisTurn > 0) && (ca.MonsterState == FieldPosition.Attack));
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
        baseAttack = ca.Attack;
        baseDefence = ca.Defence;
        attacksForOneTurn = ca.AttacksForOneTurn;
        this.owner = owner;
        UniqueMonsterID = IDFactory.GetUniqueID();
        MonstersCreatedThisGame.Add(UniqueMonsterID, this);
    }

    public void OnTurnStart()
    {
        AttacksLeftThisTurn = attacksForOneTurn;
    }

    public void Die()
    {
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

    public void AttackMonster(MonsterLogic target)
    {
        AttacksLeftThisTurn--;
        //calculate the values so that the monster does not fire the DIE command before the Attack command is sent
        if (target.ca.MonsterState == FieldPosition.Set)
        {
            GameObject target_visual = IDHolder.GetGameObjectWithID(target.UniqueMonsterID);
            target_visual.transform.DOScaleZ(1, 1f);
            target.ca.MonsterState = FieldPosition.Defence;
            Transform monsterInfo = target_visual.transform.GetChild(5);
            monsterInfo.gameObject.SetActive(true);
            if (TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual.owner == AreaPosition.Top)
                monsterInfo.localPosition = new Vector3(monsterInfo.localPosition.x, -1355.72f, monsterInfo.localPosition.z);
        }
        if (target.ca.MonsterState == FieldPosition.Defence)
        {
            if (Attack > target.Defence)
            {
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, 0, 0, owner.otherPlayer.Health).AddToQueue();
                target.Die();
            }
            else if (Attack == target.Defence)
            {
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, 0, 0, owner.otherPlayer.Health).AddToQueue();
            }
            else
            {
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, target.Defence - Attack, 0, owner.otherPlayer.Health).AddToQueue();
                owner.Health -= target.Defence - Attack;
                if (owner.Health < 0)
                {
                    owner.Health = 0;
                }
                owner.PArea.Portrait.HealthText.text = owner.Health.ToString();
            }


        }
        else
        {
            if (Attack > target.Attack)
            {
                int Damage = Attack - target.Attack;
                new MonsterAttackCommand(target.UniqueMonsterID, UniqueMonsterID, 0, Damage, owner.otherPlayer.Health - Damage).AddToQueue();
                target.Die();
                owner.otherPlayer.Health -= Damage;
                if (owner.otherPlayer.Health < 0)
                {
                    owner.otherPlayer.Health = 0;
                }
                owner.otherPlayer.PArea.Portrait.HealthText.text = owner.otherPlayer.Health.ToString();
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
                if (owner.Health < 0)
                {
                    owner.Health = 0;
                }
                owner.PArea.Portrait.HealthText.text = owner.Health.ToString();
            }
        }

    }

    public void AttackMonsterWithID(int uniqueMonsterID)
    {
       MonsterLogic target = MonstersCreatedThisGame[uniqueMonsterID];
       AttackMonster(target);
    }

    public void ChangeState(FieldPosition fp)
    {
        ca.MonsterState = fp;
        owner.PArea.tableVisual.ChangeMonsterPosition(UniqueMonsterID, fp);
    }

    
    


    // STATIC For managing IDs
    public static Dictionary<int, MonsterLogic> MonstersCreatedThisGame = new Dictionary<int, MonsterLogic>();

}
