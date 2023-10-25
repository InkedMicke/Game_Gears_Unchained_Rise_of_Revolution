using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LabThirdRoomController : MonoBehaviour
{
    DummiesCollider_LAB _dummiesCollider;

    [SerializeField] private GameObject wave1;
    [SerializeField] private GameObject wave2;
    [SerializeField] private GameObject movableFloor;
    [SerializeField] private GameObject dummiesControllerObj;
    [SerializeField] private float speedOfFloor = 0.1f;

    [SerializeField] private UnityEvent seActivaCuandoSube;
    [SerializeField] private UnityEvent seActivaCuandoBaja;

    private void Awake()
    {
        _dummiesCollider = dummiesControllerObj.GetComponent<DummiesCollider_LAB>();
    }

    public void InvokeMoveUp()
    {
        wave1.SetActive(true);
        _dummiesCollider.MakeChild();
        StartCoroutine(MoveUp());
    }
    
    public void InvokeMoveDown()
    {
        StartCoroutine(MoveDown());
    }
    
    private IEnumerator MoveUp()
    {
        var enable = true;

        while (enable)
        {
            var temp = movableFloor.transform.localPosition;
            temp.y += speedOfFloor;
            movableFloor.transform.localPosition = temp;
            
            Debug.Log(movableFloor.transform.localPosition.y);
            if (movableFloor.transform.localPosition.y >= -0.918f)
            {
                temp.y = -0.9186499f;
                movableFloor.transform.localPosition = temp;
                enable = false;
                seActivaCuandoSube.Invoke();
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator MoveDown()
    {
        var enable = true;

        while (enable)
        {
            var temp = movableFloor.transform.localPosition;
            temp.y -= speedOfFloor;
            movableFloor.transform.localPosition = temp;
            
            if (movableFloor.transform.localPosition.y <= -9)
            {
                temp.y = -9;
                movableFloor.transform.localPosition = temp;
                enable = false;
                seActivaCuandoBaja.Invoke();
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
}
