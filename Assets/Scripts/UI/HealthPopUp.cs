using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthPopUp : MonoBehaviour {

    public Animator animator;
    public Text healthText;

    void OnEnable()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
        healthText = animator.GetComponent<Text>();
    }


    public void setText(string text)
    {
        healthText.text = text;
    }
    
}
