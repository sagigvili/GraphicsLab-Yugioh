using UnityEngine;
using System.Collections;

// place first and last elements in children array manually
// others will be placed automatically with equal distances between first and last elements
public class TableOrganizer : MonoBehaviour {

    public Transform[] Children;

    public AreaPosition side;

	// Use this for initialization
	void Awake () 
    {
        if (side == AreaPosition.Low)
        {
            Children[1].transform.position = Children[0].transform.position + new Vector3((float)574.8, 0, 0);
            Children[2].transform.position = Children[1].transform.position + new Vector3(222, 0, 0);
            Children[3].transform.position = Children[0].transform.position - new Vector3(0, 2000, 0);
            Children[3].transform.position = Children[3].transform.position - new Vector3((float)1421.84, 0, 0);
            Children[4].transform.position = Children[3].transform.position + new Vector3(2000, 0, 0);
            Children[5].transform.position = Children[4].transform.position + new Vector3(2000, 0, 0);
        } else {
            Children[1].transform.position = Children[0].transform.position + new Vector3((float)574.8, 0, 0);
            Children[2].transform.position = Children[1].transform.position + new Vector3(222, 0, 0);
            Children[3].transform.position = Children[0].transform.position + new Vector3(0, 2000, 0);
            Children[3].transform.position = Children[3].transform.position - new Vector3((float)1421.84, 0, 0);
            Children[4].transform.position = Children[3].transform.position + new Vector3(2000, 0, 0);
            Children[5].transform.position = Children[4].transform.position + new Vector3(2000, 0, 0);
        }

    }
	
	
}
