using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilar : MonoBehaviour
{
    [SerializeField] Material cableDisableMat;

    [SerializeField] GameObject disableParent;
    [SerializeField] GameObject cable;
    [SerializeField] GameObject smoke;

    public void DisablePilar()
    {
        disableParent.SetActive(false);
        cable.GetComponent<MeshRenderer>().material = cableDisableMat;
        smoke.SetActive(true);
    }

}
