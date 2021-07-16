using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OneMonsterManager : OneCardManager 
{
    [Header("Image References")]
    public TMPro.TextMeshProUGUI ATK;
    public TMPro.TextMeshProUGUI DEF;


    void Awake()
    {
        if (cardAsset != null)
            ReadMonsterFromAsset();
    }

    private bool canAttackNow = false;
    public bool CanAttackNow
    {
        get
        {
            return canAttackNow;
        }

        set
        {
            canAttackNow = value;
        }
    }

    public void ReadMonsterFromAsset()
    {
        // Change the card graphic sprite
        CardImageFront.sprite = cardAsset.CardImage;
        ATK.text = cardAsset.Attack.ToString();
        DEF.text = cardAsset.Defence.ToString();
        if (PreviewManager != null)
        {
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }
    }
}
