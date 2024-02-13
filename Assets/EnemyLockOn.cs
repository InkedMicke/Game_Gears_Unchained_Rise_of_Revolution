using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyLockOn : MonoBehaviour
{
    private PlayerInputActions m_inputActions;

    [SerializeField] private RectTransform m_lockOnCanvas;

    [SerializeField] private LayerMask m_enemyLayer;

    [SerializeField] private Transform m_playerTr;
    [SerializeField] private Transform m_enemyTarget_Locator;
    private Transform m_currentTarget;
    private Transform m_cam;

    private Vector3 m_tarLoc;

    private bool m_enemyLocked;
    private bool m_zeroVert_Look;

    [Tooltip("Angle_Degree")]
    [SerializeField] private float m_maxNoticeAngle = 60;
    [SerializeField] private float m_noticeZone;
    [SerializeField] private float m_lookAtSmoothing = 2;
    [SerializeField] private float m_crossHair_Scale = 0.1f;
    private float m_currentYOffset;

    private void Awake()
    {
        m_inputActions = new PlayerInputActions();
        m_inputActions.Enable();
        //m_inputActions.PlayerPC.FocusEnemy.performed += ToggleFocus;
    }

    private void Update()
    {
        
    }

    private void ToggleFocus(InputAction.CallbackContext x)
    {
        if(m_currentTarget)
        {
            ResetTarget();
            return;
        }

        if (m_currentTarget = ScanNearBy()) FoundTarget(); else ResetTarget();

        if(m_enemyLocked)
        {
            if (!TargetOnRange()) ResetTarget();
            LookAtTarget();
        }

    }

    private void FoundTarget()
    {
        m_enemyLocked = true;
    }

    private void ResetTarget()
    {
        m_currentTarget = null;
        m_enemyLocked = false;
    }

    private Transform ScanNearBy()
    {
        var nearByTargets = Physics.OverlapSphere(m_playerTr.position, m_noticeZone, m_enemyLayer);
        var closestAngle = m_maxNoticeAngle;
        Transform closestTarget = null;
        if (nearByTargets.Length <= 0) return null;

        for(int i = 0; i < nearByTargets.Length; i++)
        {
            var dir = nearByTargets[i].transform.position - m_cam.position;
            dir.y = 0;
            var _angle = Vector3.Angle(m_cam.forward, dir);

            if (_angle < closestAngle)
            {
                closestTarget = nearByTargets[i].transform;
                closestAngle = _angle;
            }
        }

        if (closestTarget) return null;
        var h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        var h2 = closestTarget.localScale.y;
        var h = h1 * h2;
        var half_h = (h / 2) / 2;
        m_currentYOffset = h - half_h;
        if (m_zeroVert_Look && m_currentYOffset > 1.6f && m_currentYOffset < 1.6f * 3) m_currentYOffset = 1.6f;
        m_tarLoc = closestTarget.position + new Vector3(0, m_currentYOffset, 0);
        if (Blocked()) return null;
        return closestTarget;
    }

    private void LookAtTarget()
    {
        if(m_currentTarget == null)
        {
            ResetTarget();
            return;
        }

        m_lockOnCanvas.position = m_tarLoc;
        m_lockOnCanvas.localScale = Vector3.one * ((m_cam.position - m_tarLoc).magnitude * m_crossHair_Scale);

        m_enemyTarget_Locator.position = m_tarLoc;
        var dir = m_currentTarget.position - m_playerTr.position;
        dir.y = 0;
        var rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * m_lookAtSmoothing);
    }

    private bool Blocked()
    {
        RaycastHit hit;
        if(Physics.Linecast(m_playerTr.position + Vector3.up * .5f, m_tarLoc, out hit))
        {
            if(hit.transform.CompareTag("Enemy")) return true;
        }

        return false;
    }



    private bool TargetOnRange()
    {
        var dis = (m_playerTr.position - m_tarLoc).magnitude;
        if (dis / 2 > m_noticeZone) return false; else return true;
    }
}
