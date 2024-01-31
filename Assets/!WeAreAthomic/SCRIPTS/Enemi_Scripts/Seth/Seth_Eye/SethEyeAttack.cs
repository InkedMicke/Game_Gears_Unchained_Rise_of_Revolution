using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SethEyeAttack : MonoBehaviour
{
    [SerializeField] private List<Transform> targets = new();
    [SerializeField] private List<Transform> targetsCopy;
    [SerializeField] private Transform eyeTr;

    private void Awake()
    {
        targetsCopy = targets;
    }
    public void StartEyeAttack()
    {
        var random = Random.Range(0, targetsCopy.Count);
        targetsCopy.RemoveAt(random - 1);
        eyeTr.transform.DOMove(eyePos, );
    }
}
