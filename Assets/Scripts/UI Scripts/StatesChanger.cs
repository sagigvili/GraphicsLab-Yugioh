using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatesChanger : MonoBehaviour
{
    public SelectState panel;
    public Button DefenceButton;
    public Button AttackSummonButton;
    public Text AttackSummonText;
    public Button ExitButton;
    public BoxCollider Col;
    public float mTime = 1f;

    private Transform Parent;

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
            if (h.collider == Col)
            {
                if (Input.GetMouseButtonDown(1) && panel.canChangeState && GlobalSettings.Instance.CanControlThisPlayer(TurnManager.Instance.whoseTurn) && !TurnManager.Instance.whoseTurn.table.InAttackPhase)
                {
                    ShowSelector();
                    return;
                }
            }

        }
    }

    private void ShowSelector()
    {
        if (this.transform.parent.name.StartsWith("MonsterField")) {
            int targetID = ((SelectStateOnTable)panel).cardInTable.gameObject.GetComponent<IDHolder>().UniqueID;
            MonsterLogic monster = MonsterLogic.MonstersCreatedThisGame[targetID];
            if (monster.ca.MonsterState == FieldPosition.Attack)
            {
                AttackSummonButton.gameObject.SetActive(false);
                DefenceButton.gameObject.SetActive(true);
            }
            else if (monster.ca.MonsterState == FieldPosition.Set)
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
        else if (this.transform.parent.name.StartsWith("SpellTrapField")) {
            OneCardManager card = ((SelectStateOnTable)panel).cardInTable;
            SpellTrapEffects effect = card.cardAsset.Effect;
            if (card.cardAsset.SpellTrapState == SpellTrapPosition.Set)
            {
                if (effect == SpellTrapEffects.Heal || effect == SpellTrapEffects.Draw || effect == SpellTrapEffects.DirectAttack)
                {
                    DefenceButton.gameObject.SetActive(false);
                    AttackSummonButton.gameObject.SetActive(true);
                    AttackSummonText.text = "Activate";
                    panel.gameObject.SetActive(true);
                }
                else if (TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual.getMonstersOnTableCount() > 0)
                {
                    if (effect == SpellTrapEffects.DestoryMonster)
                    {
                        DefenceButton.gameObject.SetActive(false);
                        AttackSummonButton.gameObject.SetActive(true);
                        AttackSummonText.text = "Activate";
                        panel.gameObject.SetActive(true);
                    }
                    if ((effect == SpellTrapEffects.ChangeToAttack) && (TurnManager.Instance.whoseTurn.otherPlayer.table.AnyDefenceMonsters()))
                    {
                        DefenceButton.gameObject.SetActive(false);
                        AttackSummonButton.gameObject.SetActive(true);
                        AttackSummonText.text = "Activate";
                        panel.gameObject.SetActive(true);
                    }
                    if ((effect == SpellTrapEffects.ChangeToDefence) && (TurnManager.Instance.whoseTurn.otherPlayer.table.AnyAttackMonsters()))
                    {
                        DefenceButton.gameObject.SetActive(false);
                        AttackSummonButton.gameObject.SetActive(true);
                        AttackSummonText.text = "Activate";
                        panel.gameObject.SetActive(true);
                    }

                } else if (effect == SpellTrapEffects.DestorySpellTrap && TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual.getSpellsTrapsOnTableCount() > 0)
                {
                    DefenceButton.gameObject.SetActive(false);
                    AttackSummonButton.gameObject.SetActive(true);
                    AttackSummonText.text = "Activate";
                    panel.gameObject.SetActive(true);
                } else if (effect == SpellTrapEffects.Revive && TurnManager.Instance.whoseTurn.graveyard.cards.Count > 0)
                {
                    DefenceButton.gameObject.SetActive(false);
                    AttackSummonButton.gameObject.SetActive(true);
                    AttackSummonText.text = "Activate";
                    panel.gameObject.SetActive(true);
                }

            }
        }
    }

    public void ChangeMonsterStateOnTable(int state)
    {
        panel.gameObject.SetActive(false);
        if (state == 1)
        {
            int targetID = ((SelectStateOnTable)panel).cardInTable.gameObject.GetComponent<IDHolder>().UniqueID;
            MonsterLogic monster = MonsterLogic.MonstersCreatedThisGame[targetID];
            // In case we flip summon a monster
            if (monster.ca.MonsterState == FieldPosition.Set)
            {
                Parent = this.transform.parent.GetChild(3).GetChild(0).transform;
                Transform monsterInfo = this.transform.parent.GetChild(5).transform;
                monsterInfo.gameObject.SetActive(true);
                if (TurnManager.Instance.whoseTurn.PArea.tableVisual.owner == AreaPosition.Top)
                    monsterInfo.localPosition = new Vector3(monsterInfo.localPosition.x, -1355.72f, monsterInfo.localPosition.z);
                this.transform.parent.transform.localScale = new Vector3(this.transform.parent.transform.localScale.x, this.transform.parent.transform.localScale.y, 1);
                StartCoroutine(ToAttackPosition());

                // create model above the card
                OneMonsterManager manager = this.transform.parent.GetComponent<OneMonsterManager>();
                GameObject model = GameObject.Instantiate(manager.model, manager.transform.position, Quaternion.identity) as GameObject;
                if (TurnManager.Instance.whoseTurn.PArea.tableVisual.owner == AreaPosition.Top)
                {
                    model.transform.rotation = new Quaternion(model.transform.rotation.x, 180.0f, model.transform.rotation.z, model.transform.rotation.w);
                }
                else
                {
                    Vector3 modelPos = new Vector3(model.transform.localPosition.x + manager.transXOffset, model.transform.localPosition.y + manager.transYOffset, model.transform.localPosition.z + manager.transZOffset);
                    model.transform.localPosition = modelPos;
                }

                model.transform.SetParent(this.transform.parent);
            }
            else // In case we're changing from Defence to Attack
            {
                Parent = this.transform.parent.GetChild(3).GetChild(0);
                StartCoroutine(ToAttackPosition());
                transform.parent.GetChild(6).GetComponent<Animator>().SetTrigger("Attack_State");
            }

        }
        else if (state == 2) // In case we're changing from Attack to Defence
        {
            Parent = this.transform.parent.GetChild(3).GetChild(0).transform;
            StartCoroutine(ToDefencePosition());
            transform.parent.GetChild(6).GetComponent<Animator>().SetTrigger("Defence_State");
        }
        panel.canChangeState = false;
    }

    public void ChangeSpellTrapStateOnTable(int state)
    {
        panel.gameObject.SetActive(false);
        // In case we flip activate a spell or a trap
        Parent = this.transform.parent.transform;
        Parent.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, 1);
        StartCoroutine(ToAttackPosition());
        panel.canChangeState = false;
        SpellTrapEffect.ActivateEffect(SpellTrapLogic.SpellTrapsCreatedThisGame[((SelectStateOnTable)this.panel).cardInTable.gameObject.GetComponent<IDHolder>().UniqueID]);
        
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
