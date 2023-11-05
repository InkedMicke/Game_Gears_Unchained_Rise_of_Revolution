using System.Collections;
using System.Collections.Generic;
using _WeAreAthomic.SCRIPTS.Scene;
using UnityEngine;

public class SphereDetector : MonoBehaviour
{

    private GameObject _player;
    public string hurtBoxString = "HurtBox";
    [SerializeField] private GameObject arrow;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnTriggerEnter(Collider col)
    {
        var transform1 = col.transform;
        var position = transform1.position;
        var locUp = new Vector3(position.x, position.y + 2f, position.z);
        Instantiate(arrow, locUp, Quaternion.identity);
        if (col.gameObject.name == string.Format(hurtBoxString))
        {
            GameManager.Instance.AddGameObject(col.gameObject);
        }

    }


}
