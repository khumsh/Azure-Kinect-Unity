using UnityEngine;

public class main : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    public GameObject m_tracker;
    private SkeletalTrackingProvider m_skeletalTrackingProvider;
    public BackgroundData m_lastFrameData = new BackgroundData();

    private bool camera_start;

    void Start()
    {
        camera_start = false;

        //tracker ids needed for when there are two trackers
        //const int TRACKER_ID = 0;
        //m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
    }

    void Update()
    {
        if (camera_start)
        {
            if (m_skeletalTrackingProvider.IsRunning)
            {
                if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData))
                {
                    if (m_lastFrameData.NumOfBodies != 0)
                    {
                        m_tracker.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData);
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (m_skeletalTrackingProvider != null)
        {
            m_skeletalTrackingProvider.Dispose();
        }
    }

    public void CameraStartButton()
    {
        camera_start = true;

        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
    }
}
