using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OneMonsterManager : OneCardManager 
{
    [Header("Image References")]
    public TMPro.TextMeshProUGUI ATK;
    public TMPro.TextMeshProUGUI DEF;
    public int transXOffset;
    public int transYOffset;
    public int transZOffset;

    private GameObject Model;
    public GameObject model
    {
        get
        {
            return Model;
        }
        set
        {
            Model = value;
        }
    }

    private GameObject LoadEffect;
    public GameObject loadEffect
    {
        get
        {
            return LoadEffect;
        }
        set
        {
            LoadEffect = value;
        }
    }

    private GameObject ShotEffect;
    public GameObject shotEffect
    {
        get
        {
            return ShotEffect;
        }
        set
        {
            ShotEffect = value;
        }
    }

    private GameObject AttackEffect;
    public GameObject attackEffect
    {
        get
        {
            return AttackEffect;
        }
        set
        {
            AttackEffect = value;
        }
    }

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
        Model = cardAsset.Model;
        LoadEffect = cardAsset.LoadEffect;
        AttackEffect = cardAsset.AttackEffect;
        transXOffset = cardAsset.xTransOffset;
        transYOffset = cardAsset.yTransOffset;
        transZOffset = cardAsset.zTransOffset;
        shotEffect = cardAsset.ShotAttack;
        if (PreviewManager != null)
        {
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }
    }
}
