using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OneMonsterManager : MonoBehaviour 
{
    public CardAsset cardAsset;
    public OneCardManager PreviewManager;
    [Header("Image References")]
    public Image MonsterGraphicImage;


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

    private FieldPosition MonsterState;
    public FieldPosition monsterState
    {
        get
        {
            return MonsterState;
        }

        set
        {
            MonsterState = value;
            cardAsset.state = value;
        }
    }

    public void ReadMonsterFromAsset()
    {
        // Change the card graphic sprite
        MonsterGraphicImage.sprite = cardAsset.CardImage;

        monsterState = cardAsset.state;
        if (PreviewManager != null)
        {
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }
    }	

    public void TakeDamage(int amount, int healthAfter)
    {

    }
}
