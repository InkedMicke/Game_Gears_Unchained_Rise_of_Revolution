using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SethEyeAttack : MonoBehaviour
{
    [SerializeField] private GameObject eyePrefab;
    private GameObject _currentEye;

    [SerializeField] private Transform eyeWaypointsContainer;
    [SerializeField] private Transform eyeOriginalPos;
    public List<Transform> targets;

    [SerializeField] private float speed;
    [SerializeField] private Ease ease;

    private void Awake()
    {
        var childrens = eyeWaypointsContainer.GetComponentsInChildren<Transform>();
        foreach (var x in childrens)
        {
            targets.Add(x);
        }
        targets.RemoveAt(0);
    }

    public void StarEyeAttacking()
    {
        _currentEye = Instantiate(eyePrefab);
        _currentEye.transform.position = eyeOriginalPos.position;
        _currentEye.transform.localScale = Vector3.one * 9;
        //_currentEye.transform.DOMove(targets[0].position, speed).SetEase(ease).OnComplete(EyeAttack);
    }

    private void EyeAttack()
    {

    }
}

