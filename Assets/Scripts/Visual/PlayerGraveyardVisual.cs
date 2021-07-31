 using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;


// this class should be attached to the deck
// generates new cards and places them into the hand
public class PlayerGraveyardVisual : MonoBehaviour {

    private List<GameObject> CardsInGraveyard = new List<GameObject>();
    public AreaPosition owner;
    public GameObject EmptySlot;
    public TMPro.TextMeshPro CardsCounter;
    public GameObject CardsCanvas;
    public GameObject CardsContainer;
    public GameObject CardInScrollArea;

    void Start()
    {
        CardsInGraveyardCounter = GlobalSettings.Instance.Players[owner].graveyard.cards.Count;
        CardsCounter.text = CardsInGraveyardCounter.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && owner == AreaPosition.Top)
        {

            if (CardsCanvas.activeSelf)
                CardsCanvas.SetActive(false);
            else
                CardsCanvas.SetActive(true);
        }
    }

    private int cardsInGraveyardCounter = 0;
    public int CardsInGraveyardCounter
    {
        get{ return cardsInGraveyardCounter; }

        set
        {
            cardsInGraveyardCounter = value;
            CardsCounter.text = CardsInGraveyardCounter.ToString();
                

        }
    }

    public GameObject AddCardToGraveyard(GameObject card)
    {
        CardsInGraveyard.Add(card);
        GlobalSettings.Instance.Players[owner].graveyard.cards.Add(card.GetComponent<OneCardManager>().cardAsset);
        CardsInGraveyardCounter++;

        GameObject newCardInGraveyard = GameObject.Instantiate(GlobalSettings.Instance.CardInGraveyard, card.transform.position, Quaternion.identity) as GameObject;
        newCardInGraveyard.GetComponent<OneCardManager>().CardImageFront.sprite = card.GetComponent<OneCardManager>().CardImageFront.sprite;
        newCardInGraveyard.GetComponent<OneCardManager>().cardAsset = card.GetComponent<OneCardManager>().cardAsset;
        newCardInGraveyard.transform.SetParent(EmptySlot.transform);
        AddCardToContainer(card.GetComponent<OneCardManager>().cardAsset, card.GetComponent<OneCardManager>().CardImageFront.sprite);
        return newCardInGraveyard;
    }

    public void AddCardToContainer(CardAsset ca, Sprite img)
    {
        Vector3 pos;
        if (CardsInGraveyard.Count % 2 == 1)
            pos = CardsContainer.transform.GetChild(0).transform.position;
        else
        {
            pos = CardsContainer.transform.GetChild(1).transform.position;
            RectTransform rt = CardsContainer.GetComponent<RectTransform>();
            CardsContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(rt.rect.width, rt.rect.height + 500);

            Vector3 vP = CardsContainer.transform.GetChild(0).transform.position;
            CardsContainer.transform.GetChild(0).transform.position = new Vector3(vP.x, vP.y - 250, vP.z);

            CardsContainer.transform.GetChild(1).transform.position = new Vector3(pos.x, pos.y - 250, pos.z);
        }
        GameObject newCardInScrollArea = GameObject.Instantiate(CardInScrollArea, pos, Quaternion.identity) as GameObject;
        newCardInScrollArea.transform.SetParent(CardsContainer.transform);
        newCardInScrollArea.GetComponent<OneCardManager>().CardImageFront.sprite = img;
        newCardInScrollArea.GetComponent<OneCardManager>().cardAsset = ca;
    }

    public GameObject FindCard(CardAsset ca)
    {
        foreach (GameObject go in CardsInGraveyard)
            if (go != null)
                if (go.GetComponent<OneCardManager>().cardAsset.Name == ca.Name)
                    return go;
        return null;
    }

    public void ReviveACard(CardAsset ca)
    {
        GameObject go = FindCard(ca);
        CardsInGraveyard.Remove(go);
        string cardAssetNameToRemove = ca.Name;
        for (int i = 2; i < CardsContainer.transform.childCount; i++)
        {
            if(CardsContainer.transform.GetChild(i).GetComponent<OneCardManager>().cardAsset.Name == cardAssetNameToRemove)
            {
                Destroy(CardsContainer.transform.GetChild(i).gameObject);
                break;
            }
        }

        for (int i = 1; i < EmptySlot.transform.childCount; i++)
        {
            if (EmptySlot.transform.GetChild(i).GetComponent<OneCardManager>().cardAsset.Name == cardAssetNameToRemove)
            {
                Destroy(EmptySlot.transform.GetChild(i).gameObject);
                break;
            }
        }
        CardsInGraveyardCounter--;
        CardsCanvas.SetActive(false);
    }
   
}
