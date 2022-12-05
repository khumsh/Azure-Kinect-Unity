using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseSimilarity : MonoBehaviour
{
    public GameObject azureTrackerPose;     // 실시간 트래킹 포즈
    public GameObject modelAvatarPose;      // 모범 포즈

    public float[] azureTrackerPoseXYZ = new float[96];
    public float[] modelAvatarPoseXYZ = new float[96];

    private float cosineSimilarityValue = 0;
    private float cosineDistanceValue = 0;
    private List<string> errorParts;
    

    private string[] bodyParts = 
    new string[]
    { 
        "pelvis",
        "spineNaval",
        "spineChest",
        "neck",
        "leftClavicle",
        "leftShoulder",
        "lefttElbow",
        "leftWrist",
        "leftHand",
        "leftFingerTip",
        "leftThumb",
        "rightClavicle",
        "rightShoulder",
        "rightElbow",
        "rightWrist",
        "rightHand",
        "rightFingerTip",
        "rightThumb",
        "leftHip",
        "leftKnee",
        "leftAnkle",
        "leftFoot",
        "rightHip",
        "rightKnee",
        "rightAnkle",
        "rightFoot",
        "head",
        "nose",
        "eyeLeft",
        "earLeft",
        "eyeRight",
        "earRight"
    };
    


    void Update()
    {
        azureTrackerPoseXYZ = GetPoseXY(azureTrackerPose);    
        modelAvatarPoseXYZ = GetPoseXY(modelAvatarPose);

        cosineSimilarityValue = cosineSimilarity(azureTrackerPoseXYZ, modelAvatarPoseXYZ);
        cosineDistanceValue = cosineDistanceMatching(azureTrackerPoseXYZ, modelAvatarPoseXYZ);

        Debug.Log("cosineSimilarity = " + cosineSimilarityValue + ", cosineDistance = " + cosineDistanceValue);
    }

    private float[] GetPoseXY(GameObject tracker)
    {
        // 일단 32개로 해보고 (x좌표 32개, y좌표 32개)
        // [x1, y1, z1, x2, y2, z2, ... , x32, y32, z32]
        // 중요한 부분만 추려서 계산 해보던지 해야할듯


        float[] pose = new float[96];
        
        Transform trackerPose = tracker.transform.GetChild(0);

        int q = 0;
        int p = 0;
        while(p < 32)
        {
            pose[q] = tracker.transform.GetChild(0).GetChild(p).transform.localPosition.x;
            pose[q+1] = tracker.transform.GetChild(0).GetChild(p).transform.localPosition.y;
            pose[q+2] = tracker.transform.GetChild(0).GetChild(p).transform.localPosition.z;

            q += 3;
            p += 1;
        }



        return pose;
    }
    
    // 코사인 유사도 계산
    // pose1: azure tracker 포즈 x, y, z 좌표
    // pose2: azure tracker copy 포즈 x, y, z좌표
    // ex) [x1, y1, z1, x2, y2, z2, ..., x32, y32, z32]
    private float cosineSimilarity(float[] pose1, float[] pose2)
    {
        float pose1_temp = 0;
        float pose2_temp = 0;
        float pose1Dotpose2 = 0;

        float temp1 = 0;
        float temp2 = 0;
        float temp3 = 0;

        

        for (int i = 0; i < pose1.Length; ++i)
        {
            temp1 = pose1[i] * pose1[i];
            temp2 = pose2[i] * pose2[i];
            temp3 = pose1[i] * pose2[i];

            pose1_temp += temp1;
            pose2_temp += temp2;

            pose1Dotpose2 += temp3;


        }



        

        pose1_temp = Mathf.Sqrt(pose1_temp);
        pose2_temp = Mathf.Sqrt(pose2_temp);

        return pose1Dotpose2 / (pose1_temp * pose2_temp);

    }

    // 코사인 거리 계산
    private float cosineDistanceMatching(float[] pose1, float[] pose2)
    {
        float cosineSim = cosineSimilarity(pose1, pose2);

        return Mathf.Sqrt(2 * (1 - cosineSim));
    }
}
