using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SethEyeAttack : MonoBehaviour
{
    [SerializeField] private GameObject eyePrefab;
    private GameObject _currentEye;

    [SerializeField] private Transform eyeWaypointsContainer;
    [SerializeField] private Transform eyeOriginalPos;

    private int _pathIndex;

    [SerializeField] private float eyeSize = 10f;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private Ease ease;

    public void StarEyeAttacking()
    {
        _currentEye = Instantiate(eyePrefab);
        _currentEye.transform.position = eyeOriginalPos.position;
        _currentEye.transform.localScale = Vector3.one * eyeSize;
        StartCoroutine(EyeAttack());
    }

    private IEnumerator EyeAttack()
    {
        while(_pathIndex <= GetPath().Count)
        {
            if(Vector3.Distance(_currentEye.transform.position, GetPointInPath(_pathIndex).position) < 0.1f)
            {
                _pathIndex++;
            }

            _currentEye.transform.position = Vector3.MoveTowards(_currentEye.transform.position, GetPointInPath(_pathIndex).position, speed * Time.deltaTime);
            var difference = GetPointInPath(_pathIndex).position - _currentEye.transform.position;
            var targetRotation = Quaternion.LookRotation(difference);
            _currentEye.transform.rotation = Quaternion.Slerp(_currentEye.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

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

    private Transform GetPointInPath(int childCount)
    {
        return GetPath()[childCount];
    }
}

