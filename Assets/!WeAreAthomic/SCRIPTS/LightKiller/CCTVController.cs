using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVController : MonoBehaviour
{
    LightKiller lKiller;

    public List<Quaternion> rotationList = new List<Quaternion>();

    bool isCameraOff;
    [System.NonSerialized] public bool rayGotObstruction;

    private int currentRotation;

    public GameObject lightKiller;

    Transform lightKillerTr;
    Transform playerTr;

    public LayerMask groundLayer;
    public LayerMask obstructionLayer;

    [SerializeField] private float lerpSpeed = 0.005f;
    [SerializeField] private float timeToOnCamera = 3f;
    [SerializeField] private float redLightSpeedWhenChasing = 0.01f;

    private void Awake()
    {
        lKiller = lightKiller.GetComponent<LightKiller>();
    }

    private void Start()
    {
        currentRotation = Random.Range(0, rotationList.Count);

        lightKillerTr = lightKiller.transform;

        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (transform.rotation == rotationList[currentRotation] && !isCameraOff && !lKiller.isFocusingPlayer)
        {
            currentRotation = Random.Range(0, rotationList.Count);
        }
        else
        {
            if (!lKiller.isFocusingPlayer)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotationList[currentRotation], lerpSpeed);
                rayGotObstruction = false;
            }
        }

        if(isCameraOff)
        {
            Quaternion offRotation = Quaternion.Euler(80f, 0f, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, offRotation, lerpSpeed);
        }

        if(!isCameraOff)
        {
            LookToPlayer();
            SetLightPosition();
        }
    }

    public void SetLightPosition()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 50f, groundLayer))
        {
            if (!lKiller.isFocusingPlayer)
            {
                lightKillerTr.position = hitInfo.point;
            }
            else
            {
                lightKillerTr.position = Vector3.Lerp(lightKillerTr.position, hitInfo.point, redLightSpeedWhenChasing);
            }
        }
    }

    public void TurnOffCamera()
    {
        isCameraOff = true;
        lightKiller.SetActive(false);
        Invoke(nameof(TurnOnCamera), timeToOnCamera);
    }

    public void TurnOnCamera()
    {
        lightKiller.SetActive(true);
        isCameraOff = false;
    }

    void LookToPlayer()
    {
        if (lKiller.isFocusingPlayer)
        {
            transform.LookAt(playerTr.position);

            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 50f, obstructionLayer))
            {
                if (lKiller.isFocusingPlayer)
                {
                    lKiller.WhiteLight();
                    rayGotObstruction = true;
                }
            }
        }
    }
}
