using System.Collections.Generic;
using UnityEngine;

public class G_InstanciateObj : MonoBehaviour
{
    [SerializeField] private List<GameObject>  latas;

    [SerializeField] private Transform objPosition;
    public void InstanciateObj()
    {
        var random = Random.Range(0, latas.Count);

        Instantiate(latas[random], objPosition.position, Quaternion.identity);
    }
}
