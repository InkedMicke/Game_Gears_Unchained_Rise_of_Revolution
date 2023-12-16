using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_UIMaterial_Changer : MonoBehaviour
{

    [SerializeField] private Material baseMat;
    [SerializeField] private Material enterMat;
    [SerializeField] private Material clickMat;

    [SerializeField] private List<GameObject> objectsToChangeMat;


    public void OnPointerEnter()
    {
        foreach (var objtochange in objectsToChangeMat)
        {
            objtochange.GetComponent<MeshRenderer>().material = enterMat;
        }
    }

    public void OnPointerExit()
    {
        foreach (var objtochange in objectsToChangeMat)
        {
            objtochange.GetComponent<MeshRenderer>().material = baseMat;
        }
    }

    public void OnClick()
    {
        foreach (var objtochange in objectsToChangeMat)
        {
            objtochange.GetComponent<MeshRenderer>().material = clickMat;
        }
    }

}
