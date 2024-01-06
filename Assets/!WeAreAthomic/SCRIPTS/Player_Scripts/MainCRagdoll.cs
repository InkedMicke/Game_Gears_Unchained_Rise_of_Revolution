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
    private GameObject wrenchClone;
    private GameObject leftFootClone;
    private GameObject rightFootClone;

    public List<Rigidbody> prueba;

    public bool ragDoll;

    private void Start()
    {
        _rbs = transform.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in _rbs)
        {
            prueba.Add(rb);
        }

        SetEnabled(false);
    }

    private void Update()
    {
        if(ragDoll)
        {
            SetEnabled(true);
        }
    }

    public void ResetBody()
    {
        var clone = Instantiate(matrigPrefab, transform.position, Quaternion.identity);
        clone.name = matrigPrefab.name;
        clone.transform.SetParent(transform);
        var player = clone.transform.parent.gameObject;
        GetWrenchFromMatrig(clone);
        GetLeftFootFromMatrig(clone);
        GetRightFootFromMatrig(clone);
        player.GetComponent<MainCAttack>().weaponObj = wrenchClone;
        player.GetComponent<MainCHackingSystem>().wrenchObj = wrenchClone;
        var vfx = player.GetComponent<MainVFXCharacter>();
        vfx.leftFoot = leftFootClone.transform;
        vfx.rightFoot = rightFootClone.transform;
        player.GetComponent<G_MeshTrail>().skinnedMeshRenderers[0] = clone.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        
        godmodeContainer.SetActive(false);
        GameManagerSingleton.Instance.SetGodModeBool(false);
        GameManagerSingleton.Instance.FreezeTime(false);
        GameManagerSingleton.Instance.PauseGame(false);
        GameManagerSingleton.Instance.CursorMode(false);
        Destroy(matrig);
    }

    private void GetWrenchFromMatrig(GameObject clone)
    {
        var hips = clone.transform.GetChild(0);
        var spine0 = hips.transform.GetChild(2);
        var spine1 = spine0.transform.GetChild(0);
        var spine2 = spine1.transform.GetChild(0);
        var rightShoulder = spine2.transform.GetChild(2);
        var rightArm = rightShoulder.transform.GetChild(0);
        var rightForeArm = rightArm.transform.GetChild(0);
        var rightHand = rightForeArm.transform.GetChild(0);
        wrenchClone = rightHand.transform.GetChild(5).gameObject;
    }    
    
    private void GetLeftFootFromMatrig(GameObject clone)
    {
        var hips = clone.transform.GetChild(0);
        var leftLegUp = hips.transform.GetChild(0);
        var leftLeg = leftLegUp.transform.GetChild(0);
        leftFootClone = leftLeg.transform.GetChild(0).gameObject;
    }

    private void GetRightFootFromMatrig(GameObject clone)
    {
        var hips = clone.transform.GetChild(0);
        var rightLegUp = hips.transform.GetChild(0);
        var rightLeg = rightLegUp.transform.GetChild(0);
        rightFootClone = rightLeg.transform.GetChild(0).gameObject;
    }

    public void SetEnabled(bool enabled)
    {
        var isKinematic = !enabled;
        foreach(var rb in _rbs)
        {
            rb.isKinematic = isKinematic;
        }
    }
}
