using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

    public Animator animator;

    void Start()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length - 0.1f);
    }

    public void setText(string damageNum)
    {
        animator.GetComponent<Text>().text = damageNum;
    }

}
