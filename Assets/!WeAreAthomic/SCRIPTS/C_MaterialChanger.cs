using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_MaterialChanger : MonoBehaviour
{
   
    [SerializeField] private Material neutralMat;
    [SerializeField] private Material catchMat;

    [SerializeField] private List<GameObject> objectsToChangeMat;
  


    public void OnPlayerCatched()
    {
        foreach (var objtochange in objectsToChangeMat) 
        {
            objtochange.GetComponent<MeshRenderer>().material = catchMat;
        }
    }
    public void OnEnemiHit()
    {
        foreach (var objtochange in objectsToChangeMat)
        {
            objtochange.GetComponent<SkinnedMeshRenderer>().material = catchMat;
        }
    }

    public void OnPlayerNeutral()
    {
        foreach (var objtochange in objectsToChangeMat)
        {
            objtochange.GetComponent<MeshRenderer>().material = neutralMat;
        }
    }
    public void OnEnemiNeutral()
    {
        foreach (var objtochange in objectsToChangeMat)
        {
            objtochange.GetComponent<SkinnedMeshRenderer>().material = neutralMat;
        }
    }


}
