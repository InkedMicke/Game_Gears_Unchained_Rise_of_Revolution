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
            transform.LookAt(new Vector3(cameraPos.x, transform.position.y, cameraPos.z), Vector3.up);
            transform.Rotate(0f, 0f, rotateAmount);

        }
    }
}
