using Cinemachine;
using System;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Genericos
{
    public class GLookAtWithRotateZ : MonoBehaviour
    {
        [SerializeField] private float rotateAmount;

        private CinemachineVirtualCamera m_virtualCamera;

        private void Awake()
        {
            m_virtualCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            if (m_virtualCamera != null)
            {
                var cameraPos = m_virtualCamera.transform.position;
                transform.LookAt(new Vector3(cameraPos.x, transform.position.y, cameraPos.z), Vector3.up);
                transform.Rotate(0f, 0f, rotateAmount);
            }
        }
    }
}
