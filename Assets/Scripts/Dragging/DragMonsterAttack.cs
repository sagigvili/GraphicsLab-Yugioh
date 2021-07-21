using UnityEngine;
using System.Collections;

public class DragMonsterAttack : DraggingActions {

    // reference to the sprite with a round "Target" graphic
    private SpriteRenderer sr;
    // LineRenderer that is attached to a child game object to draw the arrow
    private LineRenderer lr;
    // reference to WhereIsTheCardOrMonster to track this object`s state in the game
    private WhereIsTheCardOrMonster whereIsThisMonster;
    // the pointy end of the arrow, should be called "Triangle" in the Hierarchy
    private Transform triangle;
    // SpriteRenderer of triangle. We need this to disable the pointy end if the target is too close.
    private SpriteRenderer triangleSR;
    // when we stop dragging, the gameObject that we were targeting will be stored in this variable.
    private GameObject Target;
    // Reference to monster manager, attached to the parent game object
    private OneMonsterManager manager;

    void Awake()
    {
        // establish all the connections
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponentInChildren<LineRenderer>();
        lr.sortingLayerName = "AboveEverything";
        triangle = transform.Find("Triangle");
        triangleSR = triangle.GetComponent<SpriteRenderer>();

        manager = GetComponentInParent<OneMonsterManager>();
        whereIsThisMonster = GetComponentInParent<WhereIsTheCardOrMonster>();
    }

    public override bool CanDrag
    {
        get
        {
            // we can drag this card if 
            // a) we can control this our player (this is checked in base.canDrag)
            // b) monster "CanAttackNow" - this info comes from logic part of our code into each monster`s manager script
            return base.CanDrag && manager.CanAttackNow && manager.cardAsset.MonsterState == FieldPosition.Attack;
        }
    }

    public override void OnStartDrag()
    {
        if (TurnManager.Instance.whoseTurn.table.InAttackPhase)
        {
            whereIsThisMonster.VisualState = VisualStates.Dragging;
            // enable target graphic
            sr.enabled = true;
            // enable line renderer to start drawing the line.
            lr.enabled = true;
        }
    }

    public override void OnDraggingInUpdate()
    {
        Vector3 notNormalized = transform.position - transform.parent.position;
        Vector3 direction = notNormalized.normalized;
        float distanceToTarget = (direction*2.3f).magnitude;
        if (notNormalized.magnitude > distanceToTarget)
        {
            // draw a line between the monster and the target
            lr.SetPositions(new Vector3[]{ transform.parent.position, transform.position - direction*2.3f });
            lr.enabled = true;

            // position the end of the arrow between near the target.
            triangleSR.enabled = true;
            triangleSR.transform.position = transform.position - 1.5f*direction;

            // proper rotarion of arrow end
            float rot_z = Mathf.Atan2(notNormalized.y, notNormalized.x) * Mathf.Rad2Deg;
            triangleSR.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
        else
        {
            // if the target is not far enough from monster, do not show the arrow
            lr.enabled = false;
            triangleSR.enabled = false;
        }
            
    }

    public override void OnEndDrag()
    {
        Target = null;
        RaycastHit[] hits;

        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30000f);

        foreach (RaycastHit h in hits)
        {

            if ((h.transform.tag == "Player2" && this.tag == "LowMonster") ||
                (h.transform.tag == "Player1" && this.tag == "TopMonster"))
            {
                // go face

                Target = h.transform.gameObject;
            }
            else if ((h.transform.tag == "TopMonster" && this.tag == "LowMonster") ||
                    (h.transform.tag == "LowMonster" && this.tag == "TopMonster"))
            {
                // hit a monster, save parent transform
                Target = h.transform.parent.gameObject;
            }
               
        }

        bool targetValid = false;

        if (Target != null && TurnManager.Instance.whoseTurn.table.InAttackPhase)
        {
            if (IsThereAnyTrapOnOpponentsField())
            {
                new ActivateTrapCommand().AddToQueue();
            }
            int targetID = Target.GetComponent<IDHolder>().UniqueID;
            if (targetID == GlobalSettings.Instance.LowPlayer.PlayerID || targetID == GlobalSettings.Instance.TopPlayer.PlayerID)
            {
                // attack character
                if (TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual.getMonstersOnTableCount() == 0)
                {
                    MonsterLogic.MonstersCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].GoFace();
                    targetValid = true;
                }
            }
            else if (MonsterLogic.MonstersCreatedThisGame[targetID] != null)
            {
                // if targeted monster is still alive, attack monster

                targetValid = true;
                MonsterLogic.MonstersCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].AttackMonsterWithID(targetID);

            }
                
        }

        if (!targetValid)
        {
            // not a valid target, return
            whereIsThisMonster.VisualState = VisualStates.LowTable;
            whereIsThisMonster.SetTableSortingOrder();
        }

        // return target and arrow to original position
        transform.localPosition = Vector3.zero;
        sr.enabled = false;
        lr.enabled = false;
        triangleSR.enabled = false;

    }

    // NOT USED IN THIS SCRIPT
    protected override bool DragSuccessful()
    {
        return true;
    }

    public bool IsThereAnyTrapOnOpponentsField()
    {
        foreach (SpellTrapLogic st in TurnManager.Instance.whoseTurn.otherPlayer.table.SpellsTrapsOnTable)
        {
            Debug.Log("Card in oppent field " + st.ca.name);
            if (st.Type == SpellOrTrap.Trap)
                return true;
        }
        return false;
    }

}

