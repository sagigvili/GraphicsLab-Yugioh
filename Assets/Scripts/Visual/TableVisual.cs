using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class TableVisual : MonoBehaviour 
{
    // PUBLIC FIELDS

    // an enum that mark to whish caracter this table belongs. The alues are - Top or Low
    public AreaPosition owner;
    public int setsPerTurn = 1;

    // a referense to a game object that marks positions where we should put new Creatures
    public TableOrganizer MonstersSlots;

    public TableOrganizer SpellsTrapsSlots;

    public PlayerGraveyardVisual graveyard;

    // are we hovering over this table`s collider with a mouse
    public bool passedThroughMonstersCollider = false;
    public bool passedThroughTrapsSpellsCollider = false;

    // PRIVATE FIELDS

    // list of all the monster cards on the table as GameObjects
    private Dictionary<int, GameObject> MonstersOnTable = new Dictionary<int, GameObject>();

    private Dictionary<int, GameObject> SpellsTrapsOnTable = new Dictionary<int, GameObject>();

    // A 3D collider attached to this game object
    private BoxCollider MonstersCol;

    private BoxCollider SpellsTrapsCol;


    // PROPERTIES

    // returns true if we are hovering over any player`s table collider
    public static bool CursorOverSomeTable
    {
        get
        {
            TableVisual[] bothTables = GameObject.FindObjectsOfType<TableVisual>();
            return (bothTables[0].CursorOverThisTable || bothTables[1].CursorOverThisTable);
        }
    }

    public int getMonstersOnTableCount()
    {
        for(int i=0; i < 3; i++)
        {
            if (MonstersSlots.GetAChildInTable(i).gameObject.GetComponent<OneMonsterManager>().isFieldOnly)
                return i;
        }
        return -1;
    }

    public bool doesMonsterIndexExist(int index)
    {
        return MonstersOnTable.ContainsKey(index);
    }

    public GameObject getMonsterOnTable(int index)
    {
        return MonstersOnTable[index];
    }

    public int getSpellsTrapsOnTableCount()
    {
        for (int i = 0; i < 3; i++)
        {
            if (SpellsTrapsSlots.GetAChildInTable(i).GetComponent<OneCardManager>().isFieldOnly)
            {
                return i;
            }
                
        }
        return -1;
    }

    public int numOfSpellsTrapsOnField()
    {
        return SpellsTrapsOnTable.Count;
    }

    public int getSpellsTrapsOnTableCountAI()
    {
        for (int i = 0; i < 3; i++)
        {
           
            if (SpellsTrapsSlots.GetAChildInTable(i).GetComponent<OneCardManager>().isFieldOnly)
            {
                SpellsTrapsSlots.GetAChildInTable(i).GetComponent<OneCardManager>().isFieldOnly = false;
                return i;
            }

        }
        return -1;
    }

    public bool doesSpellTrapIndexExist(int index)
    {
        return SpellsTrapsOnTable.ContainsKey(index);
    }

    public GameObject getSpellTrapOnTable(int index)
    {
       return SpellsTrapsOnTable[index];
    }

    // returns true only if we are hovering over this table`s collider
    public bool CursorOverThisTable
    {
        get{
            if (passedThroughMonstersCollider)
                return passedThroughMonstersCollider;
            if (passedThroughTrapsSpellsCollider)
                return passedThroughTrapsSpellsCollider;
            return false;
            }
    }

    // METHODS

    // MONOBEHAVIOUR SCRIPTS (mouse over collider detection)
    void Awake()
    {
        MonstersCol = MonstersSlots.GetComponent<BoxCollider>();
        SpellsTrapsCol = SpellsTrapsSlots.GetComponent<BoxCollider>();
    }

    // CURSOR/MOUSE DETECTION
    void Update()
    {
        // we need to Raycast because OnMouseEnter, etc reacts to colliders on cards and cards "cover" the table
        // create an array of RaycastHits
        RaycastHit[] hits;
        // raycst to mousePosition and store all the hits in the array
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30000f);

        passedThroughMonstersCollider = false;
        passedThroughTrapsSpellsCollider = false;
        foreach (RaycastHit h in hits)
        {
            
            // check if the collider that we hit is the collider on this GameObject
            if (h.collider == MonstersCol)
                passedThroughMonstersCollider = true;
            else if (h.collider == SpellsTrapsCol)
                passedThroughTrapsSpellsCollider = true;
        }
    }
   
    // method to create a new monster and add it to the table
    public void AddMonsterAtIndex(CardAsset ca, int UniqueID ,int index)
    {
        // create a new Monster from prefab
        GameObject monster = GameObject.Instantiate(GlobalSettings.Instance.MonsterFieldPrefab, MonstersSlots.Children[index].transform.position, Quaternion.identity) as GameObject;
        //SetLayerRecursively(monster, "OpponentMonster");
        // apply the look from CardAsset
        OneMonsterManager manager = monster.GetComponent<OneMonsterManager>();
        manager.isFieldOnly = false;
        manager.cardAsset = ca;
        manager.ReadMonsterFromAsset();
        var tempColor = manager.CardImageFront.color;
        tempColor.a = 255;
        manager.CardImageFront.color = tempColor;
        if (manager.cardAsset.MonsterState == FieldPosition.Set)
        {
            Transform t = monster.transform.GetChild(3).GetChild(0);
            t.Rotate(0, 0, 90);
            monster.transform.localScale = new Vector3(monster.transform.localScale.x, monster.transform.localScale.y, -1);
        } else
        {
            Transform monsterInfo = monster.transform.GetChild(5);
            monsterInfo.gameObject.SetActive(true);
            if (owner == AreaPosition.Top)
                monsterInfo.localPosition = new Vector3(monsterInfo.localPosition.x, -1355.72f, monsterInfo.localPosition.z);

            // create model above the card
            GameObject model = GameObject.Instantiate(manager.model, manager.transform.position, Quaternion.identity) as GameObject;
            if (owner == AreaPosition.Top)
            {
                SetLayerRecursively(model, "OpponentMonster");
                Vector3 modelPos = new Vector3(model.transform.localPosition.x + manager.transXOffset, model.transform.localPosition.y + manager.transYOffset, model.transform.localPosition.z + manager.transZOffset);
                model.transform.localPosition = modelPos;
                model.transform.rotation = new Quaternion(model.transform.rotation.x, 180.0f, model.transform.rotation.z, model.transform.rotation.w);
            }
            else
            {
                Vector3 modelPos = new Vector3(model.transform.localPosition.x + manager.transXOffset, model.transform.localPosition.y + manager.transYOffset, model.transform.localPosition.z + manager.transZOffset);
                model.transform.localPosition = modelPos;
            }

            model.transform.SetParent(monster.transform);
        }
        // add tag according to owner
        foreach (Transform t in monster.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString() + "Monster";

        // parent a new monster gameObject to table slots
        GameObject.Destroy(MonstersSlots.Children[index].GetChild(0).gameObject);
        monster.transform.SetParent(MonstersSlots.Children[index].transform);

        // add a new monster to the dict
        MonstersOnTable.Add(index, monster);

        // let this monster know about its position
        WhereIsTheCardOrMonster w = monster.GetComponent<WhereIsTheCardOrMonster>();
        w.Slot = index;
        if (manager.cardAsset.MonsterState == FieldPosition.Set && owner == AreaPosition.Top)
            w.VisualState = VisualStates.TopTable;
        else
            w.VisualState = VisualStates.LowTable;

        // add our unique ID to this monster
        IDHolder id = monster.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;

        // end command execution
        Command.CommandExecutionComplete();
    }

    // method to create a new spell or trap card and add it to the table
    public void AddSpellTrapAtIndex(CardAsset ca, int UniqueID, int index)
    {
        // create a new Monster from prefab
        GameObject spelltrap = GameObject.Instantiate(GlobalSettings.Instance.SpellTrapFieldPrefab, SpellsTrapsSlots.Children[index].transform.position, Quaternion.identity) as GameObject;
        // parent a new spell or trap gameObject to table slots
        GameObject.Destroy(SpellsTrapsSlots.Children[index].GetChild(0).gameObject);
        spelltrap.transform.SetParent(SpellsTrapsSlots.Children[index].transform);

        // apply the look from CardAsset
        OneCardManager manager = spelltrap.GetComponent<OneCardManager>();
        manager.isFieldOnly = false;
        manager.cardAsset = ca;
        manager.ReadCardFromAsset();
        var tempColor = manager.CardImageBack.color;
        tempColor.a = 255;
        manager.CardImageBack.color = tempColor;
        if (manager.cardAsset.SpellTrapState != SpellTrapPosition.Set)
        {
            spelltrap.transform.localScale = new Vector3(spelltrap.transform.localScale.x, spelltrap.transform.localScale.y, 1);
            var tempColor1 = manager.CardImageFront.color;
            tempColor1.a = 255;
            manager.CardImageFront.color = tempColor1;
        }

        // add tag according to owner
        foreach (Transform t in spelltrap.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString() + "Monster";

        // add a new spell or trap to the list
        SpellsTrapsOnTable.Add(index, spelltrap);

        // let this spell or trap know about its position
        WhereIsTheCardOrMonster w = spelltrap.GetComponent<WhereIsTheCardOrMonster>();
        w.Slot = index;
        if (manager.cardAsset.SpellTrapState == SpellTrapPosition.Set && owner == AreaPosition.Top)
            w.VisualState = VisualStates.TopTable;
        else
            w.VisualState = VisualStates.LowTable;

        // add our unique ID to this spell or trap
        IDHolder id = spelltrap.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;

        if (ca.SpellTrapState == SpellTrapPosition.FaceUp)
        {
            //StartCoroutine(ToFront(spelltrap));
            if (ca.Effect != SpellTrapEffects.Draw && ca.Effect != SpellTrapEffects.Revive)
                new DelayCommand(2.0f).AddToQueue();
            if (owner == AreaPosition.Low)
                SpellTrapEffect.ActivateEffect(SpellTrapLogic.SpellTrapsCreatedThisGame[UniqueID]);
            else
            {
                SpellTrapEffect.ActivateEffectAI(ca);
                SpellTrapLogic.SpellTrapsCreatedThisGame[UniqueID].Die();
            }
                
        }

            // end command execution
            Command.CommandExecutionComplete();
    }


    public void SetLayerRecursively(GameObject go, string s)
    {
        go.layer = LayerMask.NameToLayer(s);

        foreach (Transform child in go.transform)
        {
            SetLayerRecursively(child.gameObject, s);
        }
    }


    // returns an index for a new monster based on mousePosition
    // included for placing a new monster to any positon on the table
    public int TablePosForNewCard(float MouseX)
    {
        return MonstersOnTable.Count;
    }

    // Destroy a monster
    public void RemoveMonsterWithID(int IDToRemove)
    {
        GameObject monsterToRemove = IDHolder.GetGameObjectWithID(IDToRemove);
        //if (monsterToRemove.transform.childCount > 6)  // In case card was destroied while Set, we don't have a model so we don't need to call Die trigger
        //    monsterToRemove.transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
        if (monsterToRemove.transform.childCount > 6)
            Destroy(monsterToRemove.transform.GetChild(6).gameObject);
        GameObject newMonsterField = GameObject.Instantiate(GlobalSettings.Instance.MonsterFieldPrefab, monsterToRemove.transform.position, Quaternion.identity) as GameObject;
        newMonsterField.transform.SetParent(monsterToRemove.transform.parent);
        foreach (int key in MonstersOnTable.Keys)
        {
            if (MonstersOnTable[key] == monsterToRemove)
            {
                MonstersOnTable.Remove(key);
                break;
            }  
        }
        GameObject temp = graveyard.AddCardToGraveyard(monsterToRemove);
        Destroy(monsterToRemove);
        Sequence s = DOTween.Sequence();
        s.AppendInterval(1f);
        s.Append(temp.transform.DOMove(graveyard.transform.position, 1).SetEase(Ease.InOutSine));
        s.AppendInterval(1f);
        s.OnComplete(() =>
           {
               Command.CommandExecutionComplete();
           });

        
    }

    public void RemoveSpellTrapWithID(int IDToRemove)
    {
        GameObject spellTrapToRemove = IDHolder.GetGameObjectWithID(IDToRemove);
        GameObject newSpellTrapField = Instantiate(GlobalSettings.Instance.SpellTrapFieldPrefab, spellTrapToRemove.transform.position, Quaternion.identity);
        newSpellTrapField.transform.SetParent(spellTrapToRemove.transform.parent);
        foreach (int key in SpellsTrapsOnTable.Keys)
        {
            if (SpellsTrapsOnTable[key] == spellTrapToRemove)
            {
                SpellsTrapsOnTable.Remove(key);
                break;
            }
        }
        Destroy(spellTrapToRemove);

        Command.CommandExecutionComplete();
    }

    public void flipSpellTrapCard(int spellTrapID)
    {
        GameObject card = IDHolder.GetGameObjectWithID(spellTrapID);
        var tempColor1 = card.GetComponent<OneCardManager>().CardImageFront.color;
        tempColor1.a = 255;
        card.GetComponent<OneCardManager>().CardImageFront.color = tempColor1;
        StartCoroutine(ToFront(card));
    }

    public void ChangeMonsterPosition(int id, FieldPosition fp)
    {
        Transform t = IDHolder.GetGameObjectWithID(id).transform;
        if (fp == FieldPosition.Defence)
        {
            try
            {
                t.GetChild(6).GetComponent<Animator>().SetTrigger("Defence_State");
            } catch (Exception e)
            {
                Debug.Log(e);
            }
            
            ToDefencePosition(t.GetComponent<OneMonsterManager>().CardImageFront.transform.parent);
        } 
        else
        {
            try
            {
                t.GetChild(6).GetComponent<Animator>().SetTrigger("Attack_State");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            ToAttackPosition(t.GetComponent<OneMonsterManager>().CardImageFront.transform.parent);
            StartCoroutine(ToFront(t.gameObject));
        }
            
    }

    IEnumerator ToFront(GameObject go)
    {
        go.GetComponent<OneCardManager>().CardImageFront.gameObject.SetActive(true);
        go.GetComponent<OneCardManager>().CardImageBack.gameObject.SetActive(false);
        for (float i = 10f; i >= 0; i -= Time.deltaTime)
            yield return 0;
    }


    /// <summary>
    /// flip to the front
    /// </summary>
    public void ToAttackPosition(Transform t)
    {
        // displace the card so that we can select it in the scene easier.
        t.DORotate(new Vector3(0, 0, 0), 1);
    }

    public void ToDefencePosition(Transform t)
    {
        t.DORotate(new Vector3(0, 0, 90), 1);
    }

}
