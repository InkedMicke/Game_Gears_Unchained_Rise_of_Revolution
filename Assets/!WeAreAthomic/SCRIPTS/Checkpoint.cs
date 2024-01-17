using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public void SetCheckpoint(Transform tr) 
    {
        var pos = new Vector3(tr.position.x, tr.position.y, tr.position.z);
        GameManagerSingleton.Instance.currentCheckpoint = pos;
    }
}
