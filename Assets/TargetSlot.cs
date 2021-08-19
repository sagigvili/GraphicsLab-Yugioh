using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSlot : MonoBehaviour
{
    public Transform target;
    public Image targetImage;
    public Text targetName;
    private SpellTrapEffects effect;

    public void getTargetDetails(GameObject g)
    {
        target = g.transform;
        OneCardManager targetManager = g.transform.GetComponentInChildren<OneCardManager>();
        if (targetManager.CardImageFront.gameObject.activeSelf == true)
        {
            targetImage.sprite = targetManager.CardImageFront.sprite;
            targetName.text = targetManager.cardAsset.Name;
        } else
        {
            targetImage.sprite = targetManager.CardImageBack.sprite;
            targetName.text = "";
        }    
        effect = targetManager.cardAsset.Effect;
    }

    public SpellTrapEffects getEffect()
    {
        return effect;
    }

}
