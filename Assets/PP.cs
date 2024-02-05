using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PP : MonoBehaviour
{
    public List<GameObject> lst_CurrentUIGameObjects = new();

    public void AddObjToCurrentUIGameObjectList(GameObject obj)
    {
        lst_CurrentUIGameObjects.Add(obj);
    }    
    
    public void RemoveObjToCurrentUIGameObjectList(GameObject obj)
    {
        lst_CurrentUIGameObjects.Remove(obj);
    }

    public void SetActiveToCurrentUIGameObjectList(bool isActive)
    {
        foreach(var x in lst_CurrentUIGameObjects)
        {
            x.SetActive(isActive);
        }
    }
}
