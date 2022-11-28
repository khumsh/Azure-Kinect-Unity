using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAvatarAnimation : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
    public void PlayAnimation(string animationName)
    {
        anim.SetTrigger(animationName);
    }
}
