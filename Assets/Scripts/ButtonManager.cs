using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    private GameObject _main;
    private GameObject _modelAvatar;

    private main _mainScript;
    private ModelAvatarAnimation _modelAvatarScript;

    void Start()
    {
        _main = GameObject.FindGameObjectWithTag("Main");
        _mainScript = _main.GetComponent<main>();

        _modelAvatar = GameObject.FindGameObjectWithTag("tracker");
        _modelAvatarScript = _modelAvatar.GetComponent<ModelAvatarAnimation>();
    }

    public void CameraStartButton()
    {
        _mainScript.m_tracker.GetComponent<Animator>().enabled = false;

        _mainScript.camera_start = true;

        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        _mainScript.m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
    }

    public void PlayAnimation(string animationName)
    {
        Animator anim = _modelAvatar.GetComponent<Animator>();
        anim.SetTrigger(animationName);
    }
}
