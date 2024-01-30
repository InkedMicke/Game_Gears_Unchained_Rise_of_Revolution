using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCProtectionAndHealAbility : MonoBehaviour
{
    private MainCVFX _mainCVFX;
    private MainCLayers _mainCLayers;
    private MainCAnimatorController _mainCAnimatorController;
    private MainCMovement _mainCMovement;
    private PlayerInputActions _playerInputActions;
    [SerializeField] private MainCHealthManager _mainCHealth;

    [SerializeField] private SkinnedMeshRenderer _mattSkinned;

    private Material[] originalMaterials;
    [SerializeField] private Material forcefieldMaterials;

    private bool _isProtectionEnabled;
    private bool _isHealEnabled;

    [SerializeField] private float protectionDuration = 3f;

    private void Awake()
    {
        originalMaterials = _mattSkinned.materials;

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.PlayerPC.BastetAttack.performed += InputPC;
        _playerInputActions.PlayerGamepad.BastetAttack.performed += InputGamepad;
        _mainCLayers = GetComponent<MainCLayers>();
        _mainCAnimatorController = GetComponent<MainCAnimatorController>();
        _mainCMovement = GetComponent<MainCMovement>();
        _mainCVFX = GetComponent<MainCVFX>();
    }

    private void InputPC(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Protection)
        {
            StartAnimProtection();
        }        
        
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Heal)
        {
            Heal();
        }
        var x = _mattSkinned.materials;
    }

    private void InputGamepad(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Protection)
        {
            StartAnimProtection();
        }

        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Heal)
        {
            Heal();
        }
    }

    private void StartAnimProtection()
    {
        if (!_isProtectionEnabled)
        {
            _mainCLayers.EnableHackLayer();
            _mainCAnimatorController.TriggerShield();
            _mainCMovement.DisableMovement();
            _mainCVFX.ActivateShieldGlow();
        }
    }
    private void StartAnimHeal()
    {
        if (!_isHealEnabled)
        {
            _isHealEnabled = true;
            _mainCLayers.EnableHackLayer();
            _mainCAnimatorController.TriggerHeal();
            _mainCMovement.DisableMovement();
            _mainCVFX.ActivateHealGlow();
        }
        _isHealEnabled = false;
    }

    private IEnumerator Protection()
    {
       


        _mainCHealth.SetCanReceiveDamage(false);
        _isProtectionEnabled = true;

        var newMat = new Material[_mattSkinned.materials.Length];


        for (int i = 0; i < newMat.Length; i++)
        {
            newMat[i] = forcefieldMaterials;
        }
        _mattSkinned.materials = newMat;

        yield return new WaitForSeconds(protectionDuration);
        _mainCHealth.SetCanReceiveDamage(true);
        _isProtectionEnabled = false;
        _mattSkinned.materials = originalMaterials;
   

    }

    private void Heal()
    {
        
        StartAnimHeal();
        _mainCHealth.GetHealth(_mainCHealth.maxHealth - _mainCHealth.currentHealth);
        
    }
    public void EndAnimProtection()
    {
        _mainCLayers.DisableHackLayer();
        _mainCMovement.EnableMovement();
    }
    public void StartProtectionAbility()
    {
        StartCoroutine(Protection());

    }
}
