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
        targetImage.sprite = targetManager.CardImageFront.sprite;
        targetName.text = targetManager.cardAsset.Name;
        effect = targetManager.cardAsset.Effect;
    }

    public SpellTrapEffects getEffect()
    {
        return effect;
    }

}
