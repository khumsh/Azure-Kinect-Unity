using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseSimilarity : MonoBehaviour
{
    public GameObject azureTracker;



    // pose1: azure tracker 포즈 x, y 좌표
    // pose2: azure tracker copy 포즈 x, y 좌표
    // ex) [x1, y1, x2, y2, ..., x17, y17]
    private float cosineSimilarity(float[] pose1, float[] pose2)
    {
        float temp1 = 0;
        float temp2 = 0;
        float pose1Dotpose2 = 0;

        for (int i = 0; i < pose1.Length; ++i)
        {
            temp1 += pose1[i] * pose1[i];
            temp2 += pose2[i] * pose2[i];

            pose1Dotpose2 = pose1[i] * pose2[i];
        }

        temp1 = Mathf.Sqrt(temp1);
        temp2 = Mathf.Sqrt(temp2);

        return pose1Dotpose2 / (temp1 * temp2);

    }

    private float cosineDistanceMatching(float[] pose1, float[] pose2)
    {
        float cosineSim = cosineSimilarity(pose1, pose2);

        return Mathf.Sqrt(2 * (1 - cosineSim));
    }
}
