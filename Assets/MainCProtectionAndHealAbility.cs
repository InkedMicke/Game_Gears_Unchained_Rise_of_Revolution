using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCProtectionAndHealAbility : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    [SerializeField] private MainCHealthManager _mainCHealth;

    [SerializeField] private SkinnedMeshRenderer _mattSkinned;

    private Material[] originalMaterials;
    [SerializeField] private Material[] protectionMaterials;

    private bool _isProtectionEnabled;

    [SerializeField] private float protectionDuration = 3f;

    private void Awake()
    {
        originalMaterials = _mattSkinned.materials;

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.PlayerPC.BastetAttack.performed += InputPC;
        _playerInputActions.PlayerGamepad.BastetAttack.performed += InputGamepad;
    }

    private void InputPC(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Protection)
        {
            StartProtection();
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
            StartProtection();
        }

        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad && GameManagerSingleton.Instance.currentAbility == CurrentAbility.Heal)
        {
            Heal();
        }
    }

    private void StartProtection()
    {
        if (!_isProtectionEnabled)
        {
            StartCoroutine(Protection());
        }
    }

    private IEnumerator Protection()
    {
        _mainCHealth.SetCanReceiveDamage(false);
        _isProtectionEnabled = true;
        _mattSkinned.materials = protectionMaterials;
        yield return new WaitForSeconds(protectionDuration);
        _mainCHealth.SetCanReceiveDamage(true);
        _isProtectionEnabled = false;
        _mattSkinned.materials = originalMaterials;
    }

    private void Heal()
    {
        _mainCHealth.GetHealth(_mainCHealth.maxHealth - _mainCHealth.currentHealth);
    }
}
