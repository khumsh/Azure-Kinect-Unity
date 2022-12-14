using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;


// https://docs.unity3d.com/2020.1/Documentation/ScriptReference/Animations.GameObjectRecorder.html
public class RecordTransformHierarchy : MonoBehaviour
{
    public AnimationClip recordClip;

    public AnimationClip squatClip;

    private GameObjectRecorder m_Recorder;

    private bool recording;
    private Animator anim;

    void Start()
    {
        recording = false;

        anim = GetComponent<Animator>();
        // // Create recorder and record the script GameObject.
        // m_Recorder = new GameObjectRecorder(gameObject);
        
        // // Bind all the Transforms on the GameObject and all its children.
        // m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
    }

    void LateUpdate()
    {
        if (recordClip == null)
            return;
        
        if (!recording)
            return;

        // Take a snapshot and record all the bindings values for this frame.
        m_Recorder.TakeSnapshot(Time.deltaTime);
    }

    public void OnDisable()
    {
        if (recordClip == null)
            return;

        if (!recording)
            return;

        if (m_Recorder.isRecording)
        {
            // Save the recorded session to the recordClip.
            m_Recorder.SaveToClip(recordClip);

            Debug.Log("Save recordClip");
        }

    }

    public void RecordStartButton()
    {
        recording = true;

        // Create recorder and record the script GameObject.
        m_Recorder = new GameObjectRecorder(gameObject);

        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);

        Debug.Log("Recording Start");

    }

    public void PlayAnimation(string animationName)
    {
        anim.SetTrigger(animationName);
    }

}