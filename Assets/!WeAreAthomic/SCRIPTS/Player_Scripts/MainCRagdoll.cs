using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCRagdoll : MonoBehaviour
{

    private Rigidbody[] _rbs;

    [SerializeField] private GameObject matrig;
    [SerializeField] private GameObject matrigPrefab;
    [SerializeField] private GameObject godmodeContainer;
    [SerializeField] private GameObject hips;
    private GameObject wrenchClone;
    private GameObject leftFootClone;
    private GameObject rightFootClone;

    public List<Rigidbody> prueba;
    public List<GameObject> bones;
    public List<Vector3> bonesPos;
    public List<Quaternion> bonesRot;

    public bool ragDoll;

    private void Awake()
    {
        bones.Add(hips);
        bones = AllChilds(hips);

        foreach(var b in bones)
        {
            bonesPos.Add(b.transform.position);
        }        
        
        foreach(var b in bones)
        {
            bonesRot.Add(b.transform.rotation);
        }
    }

    private void Start()
    {
        _rbs = transform.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in _rbs)
        {
            prueba.Add(rb);
        }

        prueba.RemoveAt(prueba.Count - 1);

        SetEnabled(false);
    }

    private void Update()
    {
        if(ragDoll)
        {
            SetEnabled(true);
        }
    }

    private List<GameObject> AllChilds(GameObject root)
    {
        List<GameObject> result = new List<GameObject>();
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(result, VARIABLE.gameObject);
            }
        }
        return result;
    }

    private void Searcher(List<GameObject> list, GameObject root)
    {
        list.Add(root);
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(list, VARIABLE.gameObject);
            }
        }
    }

    public void ResetBody()
    {
        for(var i = 0; i < bones.Count ; i++)
        {
            bones[i].transform.position = bonesPos[i];
            bones[i].transform.rotation = bonesRot[i];
        }
    }

    public void SetEnabled(bool enabled)
    {
        var isKinematic = !enabled;
        foreach(var rb in prueba)
        {
            rb.isKinematic = isKinematic;
        }
    }
}
