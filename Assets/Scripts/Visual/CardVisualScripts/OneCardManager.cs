using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// holds the refs to all the Text, Images on the card
public class OneCardManager : MonoBehaviour {

    public CardAsset cardAsset;
    public OneCardManager PreviewManager;
    [Header("Image References")]
    public Image CardImageFront;
    public Image CardImageBack;

    void Awake()
    {
        if (cardAsset != null)
            ReadCardFromAsset();
        if (transform.Find("StatesBalloon"))
        {
            SelectStateToTable StatesBalloon = transform.Find("StatesBalloon").transform.Find("Panel").GetComponent<SelectStateToTable>();
            StatesBalloon.cardInHand = this;
        }
    }

    private void OnMouseDown()
    {
        if (PreviewManager == null)
            HoverPreview.StopAllPreviews();
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

    private bool ToLoop = false;
    public bool toLoop
    {
        get
        {
            return ToLoop;
        }

        set
        {
            ToLoop = value;
        }
    }

    public void ReadCardFromAsset()
    {

        CardImageFront.sprite = cardAsset.CardImage;

        if (PreviewManager != null)
        {
            // this is a card and not a preview
            // Preview GameObject will have OneCardManager as well, but PreviewManager should be null there
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }
    }
}
