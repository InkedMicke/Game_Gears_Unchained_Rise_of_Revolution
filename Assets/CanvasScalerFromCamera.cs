using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScalerFromCamera : MonoBehaviour
{
    [SerializeField] private Camera cameraObj;

    private bool active;

    private void Update()
    {
        var canvasSize = transform.localScale;
        var canvas = GetComponent<Canvas>();
        transform.localScale = new Vector3(cameraObj.pixelRect.width / 2000, cameraObj.pixelRect.width / 2000, canvasSize.z);


    }
}
