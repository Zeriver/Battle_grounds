using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DamagePopUp : MonoBehaviour {

    public Animator animator;
    private Text damageText;

    void OnEnable()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
        damageText = animator.GetComponent<Text>();
    }


    public void setText(string text)
    {
        damageText.text = text;
    }
    
}
