using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SethEyeAttack : MonoBehaviour
{
    private SethEye _sethEye;

    private RaycastHit _currentHit;

    [SerializeField] private GameObject eyePrefab;
    private GameObject _currentEye;

    [SerializeField] private Transform eyeWaypointsContainer;
    [SerializeField] private Transform eyeOriginalPos;
    private List<Transform> _currentTarget;

    private Vector3 _currentBeamEndPos;

    public int _pathIndex;

    [SerializeField] private float eyeSize = 10f;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private Ease ease;

    private void Awake()
    {

    }

    public void StarEyeAttacking()
    {
        _currentEye = Instantiate(eyePrefab);
        _currentEye.transform.position = eyeOriginalPos.position;
        _currentEye.transform.localScale = Vector3.one * eyeSize;
        _sethEye = _currentEye.GetComponent<SethEye>();
        _sethEye.LaserBeam.gameObject.SetActive(false);
        StartCoroutine(EyeAttack());
    }

    private IEnumerator EyeAttack()
    {
        _currentTarget = GetPath();

        while (_pathIndex < GetPath().Count - 1)
        {
            if(Vector3.Distance(_currentEye.transform.position, GetPointInPath(_currentTarget, _pathIndex).position) < 0.1f)
            {
                _pathIndex++;
            }

            _currentEye.transform.position = Vector3.MoveTowards(_currentEye.transform.position, GetPointInPath(_currentTarget, _pathIndex).position, speed * Time.deltaTime);
            var difference = GetPointInPath(_currentTarget,_pathIndex).position - _currentEye.transform.position;
            var targetRotation = Quaternion.LookRotation(difference);
            _currentEye.transform.rotation = Quaternion.Slerp(_currentEye.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(EnableLaserAndRotate());
    }

    private IEnumerator EnableLaserAndRotate()
    {
        var targetRotation = Quaternion.LookRotation(-Vector3.up);
        while (_currentEye.transform.rotation != targetRotation)
        {
            _currentEye.transform.rotation = Quaternion.Slerp(_currentEye.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        _sethEye.LaserBeam.SetPosition(0, _sethEye.Muzzle.position);
        _sethEye.LaserBeam.SetPosition(1, _sethEye.Muzzle.position);
        _sethEye.LaserBeam.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);

        if (Physics.Raycast(_currentEye.transform.position, -Vector3.up, out var hit, 10f))
        {
            _currentHit = hit;
        }
        Debug.DrawRay(_currentEye.transform.position, -Vector3.up * 5f, Color.magenta, 10f);
        while (Vector3.Distance(_sethEye.LaserBeam.GetPosition(1), _currentHit.point) > .1f)
        {
            _currentBeamEndPos += -Vector3.up.normalized * Time.deltaTime * 2f;
            _sethEye.LaserBeam.SetPosition(1, _currentEye.transform.position + _currentBeamEndPos);
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(ReverseEyeAttack());
    }

    private IEnumerator ReverseEyeAttack()
    {
        _currentTarget = GetPath();

        while (_pathIndex > 0)
        {
            if (Vector3.Distance(_currentEye.transform.position, GetPointInPath(_currentTarget, _pathIndex).position) < 0.1f)
            {
                _pathIndex--;
            }
            if (Physics.Raycast(_currentEye.transform.position, -Vector3.up, out var hit2, 10f))
            {
                _sethEye.LaserBeam.SetPosition(0, _sethEye.Muzzle.position);
                _sethEye.LaserBeam.SetPosition(1, hit2.point);
            }
            _currentEye.transform.position = Vector3.MoveTowards(_currentEye.transform.position, GetPointInPath(_currentTarget, _pathIndex).position, speed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

    

    private List<Transform> GetPath()
    {
        var random = Random.Range(0, eyeWaypointsContainer.childCount - 1);
        var childs = eyeWaypointsContainer.GetChild(random).GetComponentsInChildren<Transform>();
        List<Transform> targets = new();
        foreach (var x in childs)
        {
            targets.Add(x);
        }

        targets.RemoveAt(0);

        return targets;
    }

    private Transform GetPointInPath(List<Transform> list ,int childCount)
    {
        return list[childCount];
    }
}

