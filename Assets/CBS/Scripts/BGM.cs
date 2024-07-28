using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public AudioClip Clip;
    // Start is called before the first frame update
    void Start()
    {
        if(Clip != null)
        SoundsManager.instance.PlayBGM(Clip);
    }

    
}
