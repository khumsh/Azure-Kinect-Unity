using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseSimilarity : MonoBehaviour
{
    SpriteRenderer sr;
    public GameObject azureTrackerPose;     // 실시간 트래킹 포즈
    public GameObject modelAvatarPose;      // 모범 포즈

    public float[] azureTrackerPoseXYZ; //96
    public float[] modelAvatarPoseXYZ; // right foot 까지 25 , xyz 3개씩 => 25 * 3 = 75

    [SerializeField] float similarBase; // 부위별 동작 유사 기준치 (이거보다 낮으면 유사하지 않음 판정)
    [SerializeField] Material normalColor;
    [SerializeField] Material errorColor;


    private float cosineSimilarityValue = 0;
    private float cosineDistanceValue = 0;
    public List<float> bodyPartsSimilarity;
    private int checkParts = 26; // 자식 26개까지 확인 (총 32개인데 귀 코 같은 부위 빼면 25까지)
    
    void Start()
    {
        azureTrackerPoseXYZ = new float[checkParts * 3];
        modelAvatarPoseXYZ = new float[checkParts * 3];
    }

    void Update()
    {
        azureTrackerPoseXYZ = GetPoseXY(azureTrackerPose);    
        modelAvatarPoseXYZ = GetPoseXY(modelAvatarPose);

        cosineSimilarityValue = cosineSimilarity(azureTrackerPoseXYZ, modelAvatarPoseXYZ);
        cosineDistanceValue = cosineDistanceMatching(azureTrackerPoseXYZ, modelAvatarPoseXYZ);

        Debug.Log("cosineSimilarity = " + cosineSimilarityValue + ", cosineDistance = " + cosineDistanceValue);

        SkeltonColorChange(azureTrackerPose);
    }

    private float[] GetPoseXY(GameObject tracker)
    {
        // 일단 32개로 해보고 (x좌표 32개, y좌표 32개)
        // [x1, y1, z1, x2, y2, z2, ... , x32, y32, z32]
        // 중요한 부분만 추려서 계산 해보던지 해야할듯


        float[] pose = new float[checkParts * 3];
        float SqrtSum = 0;
        
        Transform trackerPose = tracker.transform.GetChild(0);

        int q = 0;
        int p = 0;
        while(p < checkParts)
        {
            pose[q] = tracker.transform.GetChild(0).GetChild(p).transform.localPosition.x;
            pose[q+1] = tracker.transform.GetChild(0).GetChild(p).transform.localPosition.y;
            pose[q+2] = tracker.transform.GetChild(0).GetChild(p).transform.localPosition.z;

            SqrtSum = Mathf.Sqrt(pose[q]*pose[q] + pose[q+1]*pose[q+1] + pose[q+2]*pose[q+2]);

            // 정규화
            pose[q] /= SqrtSum;
            pose[q+1] /= SqrtSum;
            pose[q+2] /= SqrtSum;

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
        float temp1 = 0;
        float temp2 = 0;
        float pose1Dotpose2 = 0;

        float _cosineSim = 0;
        float temp1Sum = 0;
        float temp2Sum = 0;
        float dotSum = 0;

        bodyPartsSimilarity.Clear();

        for (int i = 0; i < pose1.Length; ++i)
        {
            temp1 += pose1[i] * pose1[i];
            temp2 += pose2[i] * pose2[i];

            pose1Dotpose2 += pose1[i] * pose2[i];

            if (i != 0 && (i + 1) % 3 == 0)
            {
                _cosineSim = (pose1Dotpose2 - dotSum) / (Mathf.Sqrt(temp1 - temp1Sum) * Mathf.Sqrt(temp2 - temp2Sum));
                temp1Sum = temp1;
                temp2Sum = temp2;
                dotSum = pose1Dotpose2;

                bodyPartsSimilarity.Add(_cosineSim);
            }
        }

        temp1 = Mathf.Sqrt(temp1);
        temp2 = Mathf.Sqrt(temp2);
        

        return pose1Dotpose2 / (temp1 * temp2);
    }

    // 코사인 거리 계산
    private float cosineDistanceMatching(float[] pose1, float[] pose2)
    {
        float cosineSim = cosineSimilarity(pose1, pose2);

        return Mathf.Sqrt(2 * (1 - cosineSim));
    }

    private void SkeltonColorChange (GameObject tracker)
    {
        //cosineDistanceMatching(pose1, pose2);

        //sr.color = Color.red;

        Transform trackerPose = tracker.transform.GetChild(0);

        for (int i = 0; i < bodyPartsSimilarity.Count; ++i)
        {
            if (bodyPartsSimilarity[i] < similarBase)
            {
                trackerPose.GetChild(i).GetChild(0).GetComponent<MeshRenderer>().material = errorColor;
            }
            else
            {
                trackerPose.GetChild(i).GetChild(0).GetComponent<MeshRenderer>().material = normalColor;
            }
        }
    }

   

}
