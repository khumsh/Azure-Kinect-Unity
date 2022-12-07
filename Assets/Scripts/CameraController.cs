using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraArm;
    [SerializeField] private GameObject LeftCamera;
    [SerializeField] private GameObject RightCamera;
    [SerializeField] private float sensitivity;


    private Vector3 firstPoint;
    private Vector3 secondPoint;
    private float xAngle;

    private bool bMultipleCamera = false;

    void Update()
    {
        //LookAround();

        if (Input.GetMouseButtonDown(0))
        {
            firstPoint = Input.mousePosition;
            
        }

        if (Input.GetMouseButton(0))
        {
            secondPoint = Input.mousePosition;
            xAngle = secondPoint.x - firstPoint.x;
            
            transform.rotation = Quaternion.Euler(0.0f, xAngle * sensitivity * 0.01f, 0.0f);
        }
    }

    public void MultipleCameraOnOffButton()
    {
        bMultipleCamera = !bMultipleCamera; // on -> off, off -> on

        if (bMultipleCamera)
        {
            LeftCamera.SetActive(true);
            RightCamera.SetActive(true);
        }
        else
        {
            LeftCamera.SetActive(false);
            RightCamera.SetActive(false);
        }
    }
}
