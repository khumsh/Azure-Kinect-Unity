using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseSimilarity : MonoBehaviour
{
    public GameObject azureTrackerPose;     // 실시간 트래킹 포즈
    public GameObject modelAvatarPose;      // 모범 포즈

    public Vector3[] azurePoseXYZ; // 실시간 트래킹 포즈 키값들
    public Vector3[] modelPoseXYZ; // 모범 포즈 키값들

    [SerializeField] float similarBase; // 부위별 동작 유사 기준치 (이거보다 낮으면 유사하지 않음 판정)
    [SerializeField] float cosineDistanceBase; // 코사인 거리 기준치
    [SerializeField] Material normalColor;
    [SerializeField] Material errorColor;


    private float cosineSimilarityValue = 0;
    private float cosineDistanceValue = 0;
    public List<float> bodyPartsSimilarity;

    // bodyPart Index (자식이 몇 번째 오브젝트인지)
    private int[] partIndex = {2, 1, 4, 5, 6, 7, 11, 12, 13, 14, 18, 19, 20, 22, 23, 24};
    

    void Update()
    {
        azurePoseXYZ = GetPoseXYZ(azureTrackerPose);
        modelPoseXYZ = GetPoseXYZ(modelAvatarPose);

        cosineSimilarityValue = cosineSimilarity(azurePoseXYZ, modelPoseXYZ);
        cosineDistanceValue = cosineDistanceMatching(azurePoseXYZ, modelPoseXYZ);

        Debug.Log("cosineSimilarity = " + cosineSimilarityValue);

        SkeltonColorChange(azureTrackerPose);
    }


    private Vector3[] GetPoseXYZ(GameObject tracker)
    {
        /*
                     관절 17개                a -> b  =>  (b - a)
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

    private float cosineSimilarity(Vector3[] pose1, Vector3[] pose2)
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


    // 코사인 거리 계산
    private float cosineDistanceMatching(Vector3[] pose1, Vector3[] pose2)
    {
        float cosineSim = cosineSimilarity(pose1, pose2);

        return Mathf.Sqrt(2 * (1 - cosineSim));
    }

    private float cosineDistanceMatching(float similarity)
    {
        return Mathf.Sqrt(2 * (1 - similarity));
    }

    private void SkeltonColorChange (GameObject tracker)
    {

        Transform trackerPose = tracker.transform.GetChild(0);

        for (int i = 0; i < bodyPartsSimilarity.Count; ++i)
        {
            if (bodyPartsSimilarity[i] < similarBase)
            {
                trackerPose.GetChild(partIndex[i]).GetChild(0).GetComponent<MeshRenderer>().material = errorColor;
            }
            else
            {
                trackerPose.GetChild(partIndex[i]).GetChild(0).GetComponent<MeshRenderer>().material = normalColor;
            }

            // if (cosineDistanceMatching(bodyPartsSimilarity[i]) < cosineDistanceBase)
            //     trackerPose.GetChild(partIndex[i]).GetChild(0).GetComponent<MeshRenderer>().material = errorColor;
            // else
            //     trackerPose.GetChild(partIndex[i]).GetChild(0).GetComponent<MeshRenderer>().material = normalColor;


        }



    }

   

}
