using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatesChanger : MonoBehaviour
{
    public SelectState panel;
    public Button DefenceButton;
    public Button AttackSummonButton;
    public Text AttackSummonText;
    public Button ExitButton;
    public BoxCollider MonsterCol;
    public float mTime = 1f;

    private Transform Parent;

    private bool canChangeState = false;
    public bool CanChangeState
    {
        get { return canChangeState; }
        set { canChangeState = value; }
    }

    void Awake()
    {
        //ExitButton.onClick.AddListener(ExitChanger);
    }

    private void Update()
    {
        // we need to Raycast because OnMouseEnter, etc reacts to colliders on cards and cards "cover" the table
        // create an array of RaycastHits
        RaycastHit[] hits;
        // raycst to mousePosition and store all the hits in the array
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30000f);
        foreach (RaycastHit h in hits)
        {
            // check if the collider that we hit is the collider on this GameObject
            if (h.collider == MonsterCol)
                if (Input.GetMouseButtonDown(1) && canChangeState)
                {
                    ShowSelector();
                    return;
                }

        }
    }

    private void ShowSelector()
    {
        if (panel.canChangeState)
        {

            if (((SelectStateOnTable)panel).monsterInTable.monsterState == FieldPosition.Attack)
            {
                AttackSummonButton.gameObject.SetActive(false);
                DefenceButton.gameObject.SetActive(true);
            }
            else if (((SelectStateOnTable)panel).monsterInTable.monsterState == FieldPosition.Set)
            {
                DefenceButton.gameObject.SetActive(false);
                AttackSummonButton.gameObject.SetActive(true);
                AttackSummonText.text = "Flip Summon";
            }
            else
            {
                DefenceButton.gameObject.SetActive(false);
                AttackSummonButton.gameObject.SetActive(true);
                AttackSummonText.text = "Attack Position";
            }
            panel.gameObject.SetActive(true);
        }
    }

    public void ChangeMonsterStateOnTable(int state)
    {
        panel.gameObject.SetActive(false);
        if (state == 1)
        {
            // In case we're flip summon a monster
            if (((SelectStateOnTable)this.panel).monsterInTable.monsterState == FieldPosition.Set)
            {
                Parent = this.transform.parent.transform;
                Parent.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, 1);
                StartCoroutine(ToAttackPosition());
            }
            else // In case we're changing from Defence to Attack
            {
                Parent = this.transform.parent.transform;
                StartCoroutine(ToAttackPosition());
            }

        }
        else if (state == 2) // In case we're changing from Attack to Defence
        {
            Parent = this.transform.parent.transform;
            StartCoroutine(ToDefencePosition());
        }
        canChangeState = false;
    }

    /// <summary>
    /// flip to the front
    /// </summary>
    IEnumerator ToAttackPosition()
    {
        // displace the card so that we can select it in the scene easier.
        Parent.DORotate(new Vector3(0, 0, 0), mTime);
        for (float i = mTime; i >= 0; i -= Time.deltaTime)
            yield return 0;
    }

    IEnumerator ToDefencePosition()
    {
        Parent.DORotate(new Vector3(0, 0, 90), mTime);
        for (float i = mTime; i >= 0; i -= Time.deltaTime)
            yield return 0;
    }

    public void ExitChanger()
    {
        panel.gameObject.SetActive(false);
    }
}
