using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GCameraShake : MonoBehaviour
{
    public static GCameraShake Instance { get; private set; }

   [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float shakeTimer;
    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {

        if (shakeTimer > 0)
        {

            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
                cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0f;
            }

        }
    }

    // Update is called once per frame
    public void ShakeCamera (float amplitudeGain, float frequencyGain, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = amplitudeGain;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frequencyGain;
        shakeTimer = time;
    }

    public void SetCameraPriorityWithSeconds()
    {
        StartCoroutine(CameraPriority());
    }

    IEnumerator CameraPriority()
    {
        yield return new WaitForSeconds(2f);
        cinemachineVirtualCamera.Priority = 40;

    }
}
