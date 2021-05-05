using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// holds the refs to all the Text, Images on the card
public class OneCardManager : MonoBehaviour {

    public CardAsset cardAsset;
    public OneCardManager PreviewManager;
    [Header("Text Component References")]
    public Text NameText;
    public Text TypeText;
    public Text DescriptionText;
    public Text Attack;
    public Text Defence;
    public Text Attribute;
    public Text Rank;
/*    [Header("GameObject Refrences")]
    public GameObject AttackIcon;*/
    [Header("Image References")]
    public Image CardImageFront;
    public Image CardImageBack;

    void Awake()
    {
        if (cardAsset != null)
            ReadCardFromAsset();
    }

    private bool canBePlayedNow = false;
    public bool CanBePlayedNow
    {
        get
        {
            return canBePlayedNow;
        }

        set
        {
            canBePlayedNow = value;
        }
    }

    public void ReadCardFromAsset()
    {
        // 2) add card name
/*        NameText.text = cardAsset.Name;
        // 4) add description

        DescriptionText.text = cardAsset.Description;*/


        if (cardAsset.Attack != -1)//
        {
            Attack.text = cardAsset.Attack.ToString();
            Defence.text = cardAsset.Defence.ToString();
            Rank.text = cardAsset.Rank.ToString();
/*            Attribute.text = cardAsset.Attribute;
            TypeText.text = cardAsset.Type;*/
        }

        if (PreviewManager != null)
        {
            // this is a card and not a preview
            // Preview GameObject will have OneCardManager as well, but PreviewManager should be null there
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }
    }
}
