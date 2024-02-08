using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCProtectionAndHealAbility : MonoBehaviour
{
    private MainCSounds _mainCSounds;
    private MainCVFX _mainCVFX;
    private MainCLayers _mainCLayers;
    private MainCAnimatorController _mainCAnimatorController;
    private MainCMovement _mainCMovement;
    private PlayerInputActions _playerInputActions;
    private MainCPlayerInterface _mainCPlayerInterface;
    private MainCPistol _mainCPistol;
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
        _mainCSounds = GetComponent<MainCSounds>();
        _mainCPlayerInterface = GetComponent<MainCPlayerInterface>();
        _mainCPistol = GetComponent<MainCPistol>();
    }

    private void InputPC(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Protection)
        {
            StartProtection();
        }        
        
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Heal)
        {
            StartHeal();
        }
    }

    private void InputGamepad(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Protection)
        {
            StartProtection();
        }

        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Heal)
        {
            StartHeal();
        }
    }

    private void StartProtection()
    {
        if (!_isProtectionEnabled)
        {
            _mainCSounds.PlayBastetCall();
            _isProtectionEnabled = true;
            _mainCLayers.EnableHackLayer();
            _mainCAnimatorController.TriggerShield();
            _mainCMovement.DisableMovement();
        }
    }

    private IEnumerator Protection()
    {
        _mainCPlayerInterface.TakeEnergy(50);
        _mainCHealth.SetCanReceiveDamage(false);
        yield return new WaitForSeconds(protectionDuration);
        _mainCHealth.SetCanReceiveDamage(true);
        _isProtectionEnabled = false;
        _mattSkinned.materials = originalMaterials;
    }

    public void ProtectionEffects()
    {
        var newMat = new Material[_mattSkinned.materials.Length];

        for (int i = 0; i < newMat.Length; i++)
        {
            newMat[i] = forcefieldMaterials;
        }

        _mattSkinned.materials = newMat;
        _mainCVFX.ActivateShieldGlow();

        StartCoroutine(Protection());
    }

    public void EndAnimProtection()
    {
        _mainCLayers.DisableHackLayer();
        _mainCMovement.EnableMovement();
        _mainCPistol.StartRecoveringEnergy(.1f);
    }

    private void StartHeal()
    {
        if (!_isHealEnabled)
        {
            _mainCSounds.PlayBastetCall();
            _isHealEnabled = true;
            _mainCLayers.EnableHackLayer();
            _mainCAnimatorController.TriggerHeal();
            _mainCMovement.DisableMovement();
        }        
    }

    public void HealEffects()
    {
        _mainCVFX.ActivateHealGlow();
        _mainCPlayerInterface.TakeEnergy(50);
        _mainCHealth.GetHealth(_mainCHealth.maxHealth - _mainCHealth.currentHealth);
    }

    public void EndAnimHeal()
    {
        _mainCLayers.DisableHackLayer();
        _mainCMovement.EnableMovement();
        _isHealEnabled = false;
        _mainCPistol.StartRecoveringEnergy(.1f);
    }
}
