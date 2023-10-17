using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BastetController : MonoBehaviour
{
    CharacterController _cc;

    private GameObject playerObj;

    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    public void InvokeMoveToPlayer(float value)
    {
        StartCoroutine(MoveToPlayer(value));
    }

    private IEnumerator MoveToPlayer(float value)
    {
        var canEnableLayer = true;

        while (canEnableLayer)
        {
            var direction = playerObj.transform.position - transform.position;
            _cc.Move(direction.normalized * moveSpeed);

            if (Vector3.Distance(playerObj.transform.position, transform.position) < 0.1f)
            {
                canEnableLayer = false;
                Destroy(this.gameObject);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
}
