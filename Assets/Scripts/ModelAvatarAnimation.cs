using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAvatarAnimation : MonoBehaviour
{
    private Animator anim;
    private bool loop = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("Loop", loop);
    }

    
    public void PlayAnimation(string animationName)
    {
        anim.SetTrigger(animationName);
    }

    public void LoopOnOff()
    {
        loop = !loop;

        anim.SetBool("Loop", loop);
    }
}
