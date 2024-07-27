using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnim : MonoBehaviour
{
    public GameObject Fire;
    // Start is called before the first frame update
    void Start()
    {
        Fire.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayFireSound()
    {

        SoundsManager.instance.PlayClick(12);
    }
    public void OpenFire()
    {
        Fire.SetActive(true);
    }

    public void CloseFire()
    {
        Fire.SetActive(false);
    }
}
