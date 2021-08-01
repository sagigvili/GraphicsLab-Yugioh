using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PlayerPortraitVisual : MonoBehaviour {

    // TODO : get ID from players when game starts
    public CharacterAsset charAsset;
    [Header("Text Component References")]
    //public Text NameText;
    public TextMeshPro HealthText;
    public GameObject Model;
    public Transform HealPoint;

    private void Awake()
    {
        ApplyLookFromAsset();
    }

    public void ApplyLookFromAsset()
    {
        HealthText.text = charAsset.MaxHealth.ToString();
        Model.SetActive(true);

    }

    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            DamageEffect.CreateDamageEffect(transform.position, amount);
            HealthText.text = healthAfter.ToString();
        }
    }

    public void Explode()
    {
        //Instantiate(GlobalSettings.Instance.ExplosionPrefab, new Vector3(0,0,600), Quaternion.identity);
        Model.GetComponent<Animator>().SetTrigger("Lose");
        Sequence s = DOTween.Sequence();
        s.PrependInterval(2f);
        s.OnComplete(() => GlobalSettings.Instance.GameOverCanvas.SetActive(true));
    }



}
