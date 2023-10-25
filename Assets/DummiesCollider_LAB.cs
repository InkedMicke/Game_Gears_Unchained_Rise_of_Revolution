using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummiesCollider_LAB : MonoBehaviour
{
    List<GameObject> colisionList;

    private void OnTriggerEnter(Collider other)
    {
        if(!colisionList.Contains(other.gameObject))
        {
            colisionList.Add(other.gameObject);
        }
    }

    public void ClearList()
    {
        colisionList.Clear();
    }

    public void MakeChild()
    {
        foreach(GameObject g in colisionList)
        {
            g.transform.SetParent(transform);
        }
    }

    public void UndoChild()
    {

    }
}
