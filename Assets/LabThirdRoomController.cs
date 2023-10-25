using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LabThirdRoomController : MonoBehaviour
{
    public GameObject movableFloor;
    [SerializeField] private float speedOfFloor = 0.1f;

    [SerializeField] private UnityEvent seActivaCuandoSube;
    [SerializeField] private UnityEvent seActivaCuandoBaja;
    public void InvokeMoveUp()
    {
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
