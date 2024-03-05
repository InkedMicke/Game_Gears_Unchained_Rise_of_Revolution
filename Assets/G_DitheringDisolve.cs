using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_DitheringDisolve : MonoBehaviour
{

    [SerializeField] private Material _mat;

    [SerializeField] float ditherTime = 5f;

    MeshRenderer[] renderers;

    // Start is called before the first frame update
    public void StartDisolveDithering()
    { 
        renderers = GetComponentsInChildren<MeshRenderer>();
        Material newMat = new(_mat);

        foreach (var renderer in renderers)
        {
            renderer.sharedMaterial = newMat;
        }
        newMat.DOFloat(0f, "_DitherThreshold", ditherTime).SetEase(Ease.InQuint).OnComplete(() => { Destroy(gameObject); });
    }

   

}
