using Interfaces;
using Seth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SethEyeHurtBox : MonoBehaviour, IDamageable
{
    SethEye m_sethEye;

    [SerializeField] GameObject vfxFire;

    bool m_canReceiveDamage = true;

    int m_index;

    private void Awake()
    {
        m_sethEye = GetComponent<SethEye>();
    }

    public bool CanReceiveDamage()
    {
        return m_canReceiveDamage;
    }

    public void GetDamage(float value)
    {
        switch (m_index)
        {
            case 0:
                vfxFire.SetActive(false);
                break;
            case 1:
                m_sethEye.SetIsEyeAttacking(false);
                m_canReceiveDamage = false;
                m_sethEye.GoToStartPos();
                break;
        }
        m_index++;
    }
}
