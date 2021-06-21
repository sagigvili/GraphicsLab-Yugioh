using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TableVisual : MonoBehaviour 
{
    // PUBLIC FIELDS

    // an enum that mark to whish caracter this table belongs. The alues are - Top or Low
    public AreaPosition owner;

    // a referense to a game object that marks positions where we should put new Creatures
    public TableOrganizer slots;

    // PRIVATE FIELDS

    // list of all the creature cards on the table as GameObjects
    private List<GameObject> MonstersOnTable = new List<GameObject>();

    private List<GameObject> SpellsTrapsOnTable = new List<GameObject>();

    // are we hovering over this table`s collider with a mouse
    private bool cursorOverThisTable = false;

    // A 3D collider attached to this game object
    private BoxCollider col;

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

    // returns true only if we are hovering over this table`s collider
    public bool CursorOverThisTable
    {
        get{ return cursorOverThisTable; }
    }

    // METHODS

    // MONOBEHAVIOUR SCRIPTS (mouse over collider detection)
    void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    // CURSOR/MOUSE DETECTION
    void Update()
    {
        // we need to Raycast because OnMouseEnter, etc reacts to colliders on cards and cards "cover" the table
        // create an array of RaycastHits
        RaycastHit[] hits;
        // raycst to mousePosition and store all the hits in the array
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30f);

        bool passedThroughTableCollider = false;
        foreach (RaycastHit h in hits)
        {
            // check if the collider that we hit is the collider on this GameObject
            if (h.collider == col)
                passedThroughTableCollider = true;
        }
        cursorOverThisTable = passedThroughTableCollider;
    }
   
    // method to create a new creature and add it to the table
    public void AddMonsterAtIndex(CardAsset ca, int UniqueID ,int index)
    {
        // create a new Monster from prefab
        GameObject monster = GameObject.Instantiate(GlobalSettings.Instance.MonsterFieldPrefab, slots.Children[index].transform.position, Quaternion.identity) as GameObject;

        // apply the look from CardAsset
        OneMonsterManager manager = monster.GetComponent<OneMonsterManager>();
        manager.cardAsset = ca;
        manager.ReadMonsterFromAsset();

        // add tag according to owner
        foreach (Transform t in monster.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString()+"Monster";
        
        // parent a new creature gameObject to table slots
        monster.transform.SetParent(slots.transform);

        // add a new creature to the list
        MonstersOnTable.Insert(index, monster);

        // let this creature know about its position
        WhereIsTheCardOrMonster w = monster.GetComponent<WhereIsTheCardOrMonster>();
        w.Slot = index;
        w.VisualState = VisualStates.LowTable;

        // add our unique ID to this creature
        IDHolder id = monster.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;

        // after a new creature is added update placing of all the other creatures
        ShiftSlotsGameObjectAccordingToNumberOfMonsters();
        PlaceMonstersOnNewSlots();

        // end command execution
        Command.CommandExecutionComplete();
    }


    // returns an index for a new creature based on mousePosition
    // included for placing a new creature to any positon on the table
    public int TablePosForNewMonster(float MouseX)
    {
        // if there are no creatures or if we are pointing to the right of all creatures with a mouse.
        // right - because the table slots are flipped and 0 is on the right side.
        if (MonstersOnTable.Count == 0 || MouseX > slots.Children[0].transform.position.x)
            return 0;
        else if (MouseX < slots.Children[MonstersOnTable.Count - 1].transform.position.x) // cursor on the left relative to all creatures on the table
            return MonstersOnTable.Count;
        for (int i = 0; i < MonstersOnTable.Count; i++)
        {
            if (MouseX < slots.Children[i].transform.position.x && MouseX > slots.Children[i + 1].transform.position.x)
                return i + 1;
        }
        Debug.Log("Suspicious behavior. Reached end of TablePosForNewMonster method. Returning 0");
        return 0;
    }

    // Destroy a creature
    public void RemoveMonsterWithID(int IDToRemove)
    {
        // TODO: This has to last for some time
        // Adding delay here did not work because it shows one monster die, then another creature die. 
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

    /// <summary>
    /// Shifts the slots game object according to number of creatures.
    /// </summary>
    void ShiftSlotsGameObjectAccordingToNumberOfMonsters()
    {
        float posX;
        if (MonstersOnTable.Count > 0)
            posX = (slots.Children[0].transform.localPosition.x - slots.Children[MonstersOnTable.Count - 1].transform.localPosition.x) / 2f;
        else
            posX = 0f;

        slots.gameObject.transform.DOLocalMoveX(posX, 0.3f);  
    }

    /// <summary>
    /// After a new creature is added or an old creature dies, this method
    /// shifts all the creatures and places the creatures on new slots.
    /// </summary>
    void PlaceMonstersOnNewSlots()
    {
        foreach (GameObject g in MonstersOnTable)
        {
            g.transform.DOLocalMoveX(slots.Children[MonstersOnTable.IndexOf(g)].transform.localPosition.x, 0.3f);
            // apply correct sorting order and HandSlot value for later 
            // TODO: figure out if I need to do something here:
            // g.GetComponent<WhereIsTheCardOrCreature>().SetTableSortingOrder() = MonstersOnTable.IndexOf(g);
        }
    }

}
