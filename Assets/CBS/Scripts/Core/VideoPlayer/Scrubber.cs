using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrubber : MonoBehaviour
{
    //[SerializeField] VideoController VideoController;
    [SerializeField] MediaPlayerController VideoController;
    bool isScrubbing;
    public void StartScrub()
    {
        isScrubbing = true;
        VideoController.StartScrub();
    }

    public void StopScrub()
    {
        if (isScrubbing)
        {
        VideoController.StopScrub();
            isScrubbing=false;
        }
    }
}
