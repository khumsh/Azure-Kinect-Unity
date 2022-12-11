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

    public Vector3[] azurePoseXYZ;
    public Vector3[] modelPoseXYZ;

    [SerializeField] float similarBase; // 부위별 동작 유사 기준치 (이거보다 낮으면 유사하지 않음 판정)
    [SerializeField] Material normalColor;
    [SerializeField] Material errorColor;


    private float cosineSimilarityValue = 0;
    private float cosineDistanceValue = 0;
    public List<float> bodyPartsSimilarity;
    private int checkParts = 26; // 자식 26개까지 확인 (총 32개인데 귀 코 같은 부위 빼면 25까지)
    
    private int[] partIndex = {2, 1, 4, 5, 6, 7, 11, 12, 13, 14, 18, 19, 20, 22, 23, 24};
    
    void Start()
    {
        azureTrackerPoseXYZ = new float[checkParts * 3];
        modelAvatarPoseXYZ = new float[checkParts * 3];
    }

    void Update()
    {
        // azureTrackerPoseXYZ = GetPoseXY(azureTrackerPose);    
        // modelAvatarPoseXYZ = GetPoseXY(modelAvatarPose);

        // cosineSimilarityValue = cosineSimilarity(azureTrackerPoseXYZ, modelAvatarPoseXYZ);
        // cosineDistanceValue = cosineDistanceMatching(azureTrackerPoseXYZ, modelAvatarPoseXYZ);


        azurePoseXYZ = GetPoseXYZ(azureTrackerPose);
        modelPoseXYZ = GetPoseXYZ(modelAvatarPose);

        cosineSimilarityValue = cosineSimilarity2(azurePoseXYZ, modelPoseXYZ);
        

        Debug.Log("cosineSimilarity = " + cosineSimilarityValue);




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

    private Vector3[] GetPoseXYZ(GameObject tracker)
    {
        /*
        partIndex = [2, 1, 4, 5, 6, 7, 11, 12, 13, 14, 18, 19, 20, 22, 23, 24]

        List<Vector3> poseXYZ = new Vector3[16];
        List<float> poseXYZSimilarity = new float[16];

        ~~


        색 바꾸기
        poseXYZSimilarity[i] < similarBase
        => trackerPose.GetChild(partIndex[i]).GetChild(0).GetComponent<MeshRenderer>().material = errorColor;

                     관절 17개              a -> b  =>  (b - a)
        0          head  -> Chest          : 26 -> 2
        1          Chest -> Naval          : 2  -> 1

        2          Chest -> Left Clavicle  : 2  -> 4
        3  Left Clavicle -> Left Shoulder  : 4  -> 5
        4  Left Shoulder -> Left Elbow     : 5  -> 6
        5  Left Elbow    -> Left Wrist     : 6  -> 7

        6          Chest -> Right Clavicle : 2  -> 11
        7 Right Clavicle -> Right Shoulder : 11 -> 12
        8 Right Shoulder -> Right Elbow    : 12 -> 13
        9 Right Elbow    -> Right Wrist    : 13 -> 14
        
        10         Naval -> Left Hip       : 1  -> 18
        11     Left Hip  -> Left Knee      : 18 -> 19
        12     Left Knee -> Left Ankle     : 19 -> 20

        13         Naval -> Right Hip      : 1  -> 22
        14    Right Hip  -> Right Knee     : 22 -> 23
        15    Right Knee -> Right Ankle    : 23 -> 24

        */

        Transform trackerPose = tracker.transform.GetChild(0);

        Vector3[] poseXYZ = new Vector3[] {
            (trackerPose.GetChild(2).transform.position - trackerPose.GetChild(26).transform.position).normalized,
            (trackerPose.GetChild(1).transform.position - trackerPose.GetChild(2).transform.position).normalized,
            (trackerPose.GetChild(4).transform.position - trackerPose.GetChild(2).transform.position).normalized,
            (trackerPose.GetChild(5).transform.position - trackerPose.GetChild(4).transform.position).normalized,
            (trackerPose.GetChild(6).transform.position - trackerPose.GetChild(5).transform.position).normalized,
            (trackerPose.GetChild(7).transform.position - trackerPose.GetChild(6).transform.position).normalized,
            (trackerPose.GetChild(11).transform.position - trackerPose.GetChild(2).transform.position).normalized,
            (trackerPose.GetChild(12).transform.position - trackerPose.GetChild(11).transform.position).normalized,
            (trackerPose.GetChild(13).transform.position - trackerPose.GetChild(12).transform.position).normalized,
            (trackerPose.GetChild(14).transform.position - trackerPose.GetChild(13).transform.position).normalized,
            (trackerPose.GetChild(18).transform.position - trackerPose.GetChild(1).transform.position).normalized,
            (trackerPose.GetChild(19).transform.position - trackerPose.GetChild(18).transform.position).normalized,
            (trackerPose.GetChild(20).transform.position - trackerPose.GetChild(19).transform.position).normalized,
            (trackerPose.GetChild(22).transform.position - trackerPose.GetChild(1).transform.position).normalized,
            (trackerPose.GetChild(23).transform.position - trackerPose.GetChild(22).transform.position).normalized,
            (trackerPose.GetChild(24).transform.position - trackerPose.GetChild(23).transform.position).normalized,
        };


        return poseXYZ;
    }

    private float cosineSimilarity2(Vector3[] pose1, Vector3[] pose2)
    {
        float result = 0;

        float pose1Square = 0;
        float pose2Square = 0;
        float pose1Dotpose2 = 0;

        float similarity = 0;

        bodyPartsSimilarity.Clear();

        for(int i = 0; i < pose1.Length; ++i)
        {
            pose1Square = pose1[i].x * pose1[i].x + pose1[i].y * pose1[i].y + pose1[i].z * pose1[i].z;
            pose2Square = pose2[i].x * pose2[i].x + pose2[i].y * pose2[i].y + pose2[i].z * pose2[i].z;
            pose1Dotpose2 = pose1[i].x * pose2[i].x + pose1[i].y * pose2[i].y + pose1[i].z * pose2[i].z;

            similarity = pose1Dotpose2 / Mathf.Sqrt(pose1Square) * Mathf.Sqrt(pose2Square);
            bodyPartsSimilarity.Add(similarity);

            result += similarity;
        }

        return result;
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
                //trackerPose.GetChild(i).GetChild(0).GetComponent<MeshRenderer>().material = errorColor;

                trackerPose.GetChild(partIndex[i]).GetChild(0).GetComponent<MeshRenderer>().material = errorColor;
            }
            else
            {
                //trackerPose.GetChild(i).GetChild(0).GetComponent<MeshRenderer>().material = normalColor;

                trackerPose.GetChild(partIndex[i]).GetChild(0).GetComponent<MeshRenderer>().material = normalColor;
            }


        }



    }

   

}
