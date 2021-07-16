using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TableVisual : MonoBehaviour 
{
    // PUBLIC FIELDS

    // an enum that mark to whish caracter this table belongs. The alues are - Top or Low
    public AreaPosition owner;
    public int setsPerTurn = 1;

    // a referense to a game object that marks positions where we should put new Creatures
    public TableOrganizer MonstersSlots;

    public TableOrganizer SpellsTrapsSlots;

    // are we hovering over this table`s collider with a mouse
    public bool passedThroughMonstersCollider = false;
    public bool passedThroughTrapsSpellsCollider = false;

    // PRIVATE FIELDS

    // list of all the monster cards on the table as GameObjects
    private List<GameObject> MonstersOnTable = new List<GameObject>();

    private List<GameObject> SpellsTrapsOnTable = new List<GameObject>();

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
        return MonstersOnTable.Count;
    }

    public GameObject getMonsterOnTable(int index)
    {
        return MonstersOnTable[index];
    }

    public int getSpellsTrapsOnTableCount()
    {
        return SpellsTrapsOnTable.Count;
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
            {
                passedThroughMonstersCollider = true;
            }
            else if (h.collider == SpellsTrapsCol)
                passedThroughTrapsSpellsCollider = true;
        }
    }
   
    // method to create a new monster and add it to the table
    public void AddMonsterAtIndex(CardAsset ca, int UniqueID ,int index)
    {
        // create a new Monster from prefab
        GameObject monster = GameObject.Instantiate(GlobalSettings.Instance.MonsterFieldPrefab, MonstersSlots.Children[index].transform.position, Quaternion.identity) as GameObject;

        // apply the look from CardAsset
        OneMonsterManager manager = monster.GetComponent<OneMonsterManager>();
        manager.cardAsset = ca;
        manager.ReadMonsterFromAsset();
        var tempColor = manager.CardImageFront.color;
        tempColor.a = 255;
        manager.CardImageFront.color = tempColor;
        if (manager.cardAsset.MonsterState == FieldPosition.Set)
        {
            monster.transform.Rotate(0, 0, 90);
            monster.transform.localScale = new Vector3(monster.transform.localScale.x, monster.transform.localScale.y, -1);
        } else
        {
            Transform monsterInfo = monster.transform.GetChild(5);
            monsterInfo.gameObject.SetActive(true);
            if (owner == AreaPosition.Top)
                monsterInfo.localPosition = new Vector3(monsterInfo.localPosition.x, -1355.72f, monsterInfo.localPosition.z);
        }
        // add tag according to owner
        foreach (Transform t in monster.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString() + "Monster";

        // parent a new monster gameObject to table slots
        GameObject.Destroy(MonstersSlots.Children[index].GetChild(0).gameObject);
        monster.transform.SetParent(MonstersSlots.Children[index].transform);

        // add a new monster to the list
        MonstersOnTable.Insert(index, monster);

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
        manager.cardAsset = ca;
        manager.ReadCardFromAsset();
        var tempColor = manager.CardImageFront.color;
        tempColor.a = 255;
        manager.CardImageFront.color = tempColor;
        if (manager.cardAsset.SpellTrapState == SpellTrapPosition.Set)
        {
            spelltrap.transform.localScale = new Vector3(spelltrap.transform.localScale.x, spelltrap.transform.localScale.y, -1);
        }
        if (ca.SpellTrapState == SpellTrapPosition.FaceUp)
        {
            SpellTrapEffect.ActivateEffect(ca);
            Destroy(spelltrap.gameObject);
            GameObject newSpellTrapField = GameObject.Instantiate(GlobalSettings.Instance.SpellTrapFieldPrefab, SpellsTrapsSlots.Children[index].transform.position, Quaternion.identity) as GameObject;
            newSpellTrapField.transform.SetParent(SpellsTrapsSlots.Children[index].transform);
        } else {
            // add tag according to owner
            foreach (Transform t in spelltrap.GetComponentsInChildren<Transform>())
                t.tag = owner.ToString() + "Monster";

            // add a new spell or trap to the list
            SpellsTrapsOnTable.Insert(index, spelltrap);

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
        }

        // end command execution
        Command.CommandExecutionComplete();
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
        // TODO: This has to last for some time
        // Adding delay here did not work because it shows one monster die, then another monster die. 
        // 
        //Sequence s = DOTween.Sequence();
        //s.AppendInterval(1f);
        //s.OnComplete(() =>
        //   {
                
        //    });
        GameObject monsterToRemove = IDHolder.GetGameObjectWithID(IDToRemove);
        MonstersOnTable.Remove(monsterToRemove);
        Destroy(monsterToRemove);

        ShiftSlotsGameObjectAccordingToNumberOfMonsters();
        PlaceMonstersOnNewSlots();
        Command.CommandExecutionComplete();
    }

    public void RemoveSpellTrapWithID(int IDToRemove)
    {
        // TODO: This has to last for some time
        // Adding delay here did not work because it shows one monster die, then another monster die. 
        // 
        //Sequence s = DOTween.Sequence();
        //s.AppendInterval(1f);
        //s.OnComplete(() =>
        //   {

        //    });
        GameObject spellTrapToRemove = IDHolder.GetGameObjectWithID(IDToRemove);
        SpellsTrapsOnTable.Remove(spellTrapToRemove);
        Destroy(spellTrapToRemove);

        //ShiftSlotsGameObjectAccordingToNumberOfMonsters();
        //PlaceMonstersOnNewSlots();
        Command.CommandExecutionComplete();
    }

    /// <summary>
    /// Shifts the slots game object according to number of monsters.
    /// </summary>
    void ShiftSlotsGameObjectAccordingToNumberOfMonsters()
    {
        float posX;
        if (MonstersOnTable.Count > 0)
            posX = (MonstersSlots.Children[0].transform.localPosition.x - MonstersSlots.Children[MonstersOnTable.Count - 1].transform.localPosition.x) / 2f;
        else
            posX = 0f;

        MonstersSlots.gameObject.transform.DOLocalMoveX(posX, 0.3f);  
    }

    /// <summary>
    /// After a new monster is added or an old monster dies, this method
    /// shifts all the monsters and places the monsters on new slots.
    /// </summary>
    void PlaceMonstersOnNewSlots()
    {
        foreach (GameObject g in MonstersOnTable)
        {
            g.transform.DOLocalMoveX(MonstersSlots.Children[MonstersOnTable.IndexOf(g)].transform.localPosition.x, 0.3f);
            // apply correct sorting order and HandSlot value for later 
            // TODO: figure out if I need to do something here:
            // g.GetComponent<WhereIsTheCardOrCreature>().SetTableSortingOrder() = MonstersOnTable.IndexOf(g);
        }
    }

}
