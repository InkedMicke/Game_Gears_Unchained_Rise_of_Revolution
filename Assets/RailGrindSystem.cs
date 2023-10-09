using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RailGrindSystem : MonoBehaviour
{
    private CharacterController _cc;
    private Animator _anim;
    private MainCLayers _mainCLayers;
    private MainCMovement _mainCMove;
    private MainCAttack _mainCAttack;
    private PlayerInputActions _playerInputActions;

    [SerializeField] private Transform groundCheck;

    [SerializeField] private LayerMask railLayer;

    [System.NonSerialized] public bool isSliding;
    bool canSlide;
    bool canBoost;

    float playbackMultiplier = 0f;

    public int childActual = 0;

    [SerializeField] private float railSpeed = 10f;
    [SerializeField] private float railSpeedBoost = 30f;

    Vector3 currentDestination;
    Vector3 directionMove;

    public List<Transform> directionsList = new List<Transform>();

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _mainCLayers = GetComponent<MainCLayers>();
        _mainCMove = GetComponent<MainCMovement>();
        _mainCAttack = GetComponent<MainCAttack>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Running.started += IncreaseBoost;
        _playerInputActions.Player.Running.canceled += DrecreaseBoost;
    }

    private void Update()
    {
        if (IsOnRail())
        {
            _anim.SetFloat(string.Format("moveSpeed"), value: 0);
            StartSliding();
        }
        else
        {
            if (isSliding)
            {
                //anim.SetLayerWeight(anim.GetLayerIndex("StartSliding"), 0f);
                isSliding = false;
                canSlide = false;
                directionsList.Clear();
                childActual = 0;
            }
        }
        BoostManager();
        Slide();
        //anim.SetLayerWeight(anim.GetLayerIndex("EndSliding"), playbackMultiplier);
        Test();
    }

    public void StartSliding()
    {
        if (!isSliding)
        {
            isSliding = true;
            GetAllTransforms();
            //anim.SetLayerWeight(anim.GetLayerIndex("StartSliding"), 1);
        }
    }

    void GetAllTransforms()
    {

        Transform allTransformsObj;

        var r = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(r, out RaycastHit hitInfo, 5f, railLayer))
        {
            var padre1 = hitInfo.collider.gameObject.transform.parent;
            var padre2 = padre1.transform.parent;


            allTransformsObj = padre2.transform.GetChild(padre2.transform.childCount - 1);


            Transform[] allChildren = allTransformsObj.GetComponentsInChildren<Transform>();

            foreach (Transform child in allChildren)
            {
                if (child.CompareTag(string.Format("RailTransform")))
                {
                    directionsList.Add(child);
                }
            }

            SortList();

            Vector3 playerToObjDirection = directionsList[childActual].position - transform.position;

            float dotProduct = Vector3.Dot(playerToObjDirection.normalized, transform.forward.normalized);

            if(dotProduct > .01f)
            {
                currentDestination = directionsList[childActual].position;
                canSlide = true;
            }
            else
            {
                directionsList.RemoveAt(0);
                currentDestination = directionsList[childActual].position;
                canSlide = true;
            }

            /*
                        _cc.enabled = false;

                        transform.position = new Vector3(transform.position.x, transform.position.y, currentDestination.z);

                        _cc.enabled = true;*/
        }
    }

    void Test()
    {
        var testObj = GameObject.FindGameObjectWithTag("Test");

        Vector3 playerToObjDirection = testObj.transform.position - transform.position;

        float dotProduct = Vector3.Dot(playerToObjDirection.normalized, transform.forward.normalized);

    }

    void Slide()
    {
        if (directionsList.Count > 0 && canSlide)
        {
            directionMove = (currentDestination - transform.position).normalized;
            _cc.Move(directionMove * railSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, currentDestination) < 0.05f)
            {
                childActual++;
                if (childActual < directionsList.Count)
                {
                    currentDestination = directionsList[childActual].position;
                }
                else
                {
                    // If we've reached the end of the list, do something (e.g., exit sliding)
                    // For now, we'll simply stop sliding
                    canSlide = false;
                }
            }
            Debug.Log(directionsList[childActual].gameObject);
        }
    }

    void IncreaseBoost(InputAction.CallbackContext context)
    {
        canBoost = true;

    }

    void DrecreaseBoost(InputAction.CallbackContext context)
    {
        canBoost = false;
        playbackMultiplier = 0;
        railSpeed = 10f;
    }

    void BoostManager()
    {
        if (playbackMultiplier < 1 && isSliding && canBoost)
        {
            playbackMultiplier += Time.deltaTime * 0.5f;
        }

        if (railSpeed < railSpeedBoost && canBoost && isSliding)
        {
            railSpeed += Time.deltaTime * 4;
        }
    }
    void SortList()
    {
        directionsList.Sort((a, b) =>
        {
            float distanceToA = Vector3.Distance(a.position, transform.position);
            float distanceToB = Vector3.Distance(b.position, transform.position);
            return distanceToA.CompareTo(distanceToB);
        });
    }



    public bool IsOnRail()
    {
        return Physics.CheckSphere(groundCheck.position, .1f, railLayer);
    }


}
