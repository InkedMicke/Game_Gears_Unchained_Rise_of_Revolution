using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RailGrindSystem : MonoBehaviour
{
    CharacterController _cc;
    Animator _anim;
    PlayerInputActions _playerInputActions;
    private MainCLayers _mainCLayers;

    [SerializeField] private Transform groundCheck;

    [SerializeField] private LayerMask railLayer;

    Vector3 _currentDestination;
    Vector3 _directionMove;

    [System.NonSerialized] public bool IsSliding;
    bool _canSlide;
    bool _canBoost;

    int childActual = 0;

    [SerializeField] private float railSpeed = 10f;
    [SerializeField] private float railSpeedBoost = 30f;
    private float _playbackMultiplier;

    public List<Transform> directionsList = new List<Transform>();

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _mainCLayers = GetComponent<MainCLayers>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Running.started += IncreaseBoost;
        _playerInputActions.Player.Running.canceled += DrecreaseBoost;
    }

    private void Update()
    {
        if (IsOnRail())
        {
            StartSliding();
        }
        else
        {
            if (IsSliding)
            {
                _anim.SetLayerWeight(_anim.GetLayerIndex("StartSliding"), 0f);
                IsSliding = false;
                _canSlide = false;
                directionsList.Clear();
                childActual = 0;
            }
        }
        BoostManager();
        Slide();
        _anim.SetLayerWeight(_anim.GetLayerIndex("EndSliding"), _playbackMultiplier);
    }

    public void StartSliding()
    {
        if (!IsSliding)
        {
            IsSliding = true;
            GetAllTransforms();
            _anim.SetLayerWeight(_anim.GetLayerIndex("StartSliding"), 1);
        }
    }

    void GetAllTransforms()
    {
        var ray = new Ray(groundCheck.position, -Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f, railLayer))
        {
            var padre1 = hit.collider.gameObject.transform.parent;
            var padre2 = padre1.parent;

            var railContainer = padre2.GetChild(padre2.childCount - 1);

            Debug.Log(railContainer);

            Transform[] allChildren = railContainer.GetComponentsInChildren<Transform>();

            foreach (Transform child in allChildren)
            {
                if (child.CompareTag("RailTransform"))
                {
                    directionsList.Add(child);
                }
            }


            SortList();

            foreach (Transform t in directionsList.ToList())
            {
                var playerToObjDirection = t.position - transform.position;
                var dotProduct = Vector3.Dot(playerToObjDirection.normalized, transform.forward.normalized);

                if (dotProduct < 0)
                {
                    directionsList.Remove(t);
                }
            }

            _currentDestination = directionsList[childActual].position;

            Debug.Log(directionsList[childActual]);
            _canSlide = true;

            _anim.SetBool(string.Format("isSliding"), true);
            _mainCLayers.EnableSlideLayer();


/*            _cc.enabled = false;

            transform.position = new Vector3(transform.position.x, transform.position.y, _currentDestination.z);

            _cc.enabled = true;*/
        }
    }

    void Slide()
    {
        if (directionsList.Count > 0 && _canSlide)
        {
            _directionMove = (_currentDestination - transform.position).normalized;
            _cc.Move(_directionMove * railSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, directionsList[childActual].position) < 0.3f)
            {
                childActual++;
                _currentDestination = directionsList[childActual].position;
            }
        }
    }

    void IncreaseBoost(InputAction.CallbackContext context)
    {
        _canBoost = true;

    }

    void DrecreaseBoost(InputAction.CallbackContext context)
    {
        _canBoost = false;
        _playbackMultiplier = 0;
        railSpeed = 10f;
    }

    void BoostManager()
    {
        if (_playbackMultiplier < 1 && IsSliding && _canBoost)
        {
            _playbackMultiplier += Time.deltaTime * 0.5f;
        }

        if (railSpeed < railSpeedBoost && _canBoost && IsSliding)
        {
            railSpeed += Time.deltaTime * 4;
            Debug.Log("hola");
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
        return Physics.CheckSphere(groundCheck.position, .2f, railLayer);
    }
}