using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// This class will show damage dealt to creatures or payers
/// </summary>

public class DamageEffect : MonoBehaviour {

    // CanvasGropup should be attached to the Canvas of this damage effect
    // It is used to fade away the alpha value of this effect
    public CanvasGroup cg;

    // The text component to show the amount of damage taken by target like: "-2"
    public Text AmountText;


    // A Coroutine to control the fading of this damage effect
    private IEnumerator ShowDamageEffect()
    {
        // make this effect non-transparent
        cg.alpha = 1f;
        // wait for 1 second before fading
        yield return new WaitForSeconds(1f);
        // gradually fade the effect by changing its alpha value
        while (cg.alpha > 0)
        {
            cg.alpha -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        // after the effect is shown it gets destroyed.
        Destroy(this.gameObject);
    }
    /// <summary>
    /// Creates the damage effect.
    /// This is a static method, so it should be called like this: DamageEffect.CreateDamageEffect(transform.position, 5);
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="isDamage">If true it is a damage effect, else it is an healing effect</param>
   
    public static void CreateDamageEffect(Vector3 position, int amount, bool isDamage=true)
    {
        // Instantiate a DamageEffect from prefab
        GameObject newDamageEffect = GameObject.Instantiate(GlobalSettings.Instance.DamageEffectPrefab, position, Quaternion.identity) as GameObject;
        // Get DamageEffect component in this new game object
        DamageEffect de = newDamageEffect.GetComponent<DamageEffect>();
        // Change the amount text to reflect the amount of damage dealt or HP added
        if (isDamage)
            de.AmountText.text = "-"+amount.ToString();
        else
            de.AmountText.text = "+" + amount.ToString();
        // start a coroutine to fade away and delete this effect after a certain time
        de.StartCoroutine(de.ShowDamageEffect());
    }
}
