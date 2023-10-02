using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightKiller : MonoBehaviour
{
    CCTVController controller;

    public GameObject luzObj;
    public GameObject CCTV;
    Light luz;

    [System.NonSerialized] public bool isFocusingPlayer;

    private void Awake()
    {
        luz = luzObj.GetComponent<Light>();
        controller = CCTV.GetComponent<CCTVController>();
    }

    public void WhiteLight()
    {
        luz.color = Color.white;
        isFocusingPlayer = false;
    }

    public void RedLight()
    {
        if (!controller.rayGotObstruction)
        {
            luz.color = Color.red;
            isFocusingPlayer = true;
        }
    }

    public void ResetMemory()
    {
        WhiteLight();
    }
}
