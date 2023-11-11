using System;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Genericos
{
    public class GLookAtWithRotateZ : MonoBehaviour
    {
        [SerializeField] private float rotateAmount;
        private void Update()
        {
            var cameraPos = Camera.main.transform.position;
            transform.LookAt(cameraPos);
            transform.Rotate(transform.position.x, cameraPos.y, rotateAmount);
            
        }
    }
}
