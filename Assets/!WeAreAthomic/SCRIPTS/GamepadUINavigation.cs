using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadUINavigation : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private UIChanges _uiChanges;

    private int _currentTab;

    private void Awake()
    {
        _uiChanges = GetComponent<UIChanges>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.UI.ReTab.performed += ReTab;
        _playerInputActions.UI.AvTab.performed += AvTab;

    }

    private void ReTab(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.IsSettingsMenuEnabled && !(_currentTab >= _uiChanges.panelsSelectors.Count - 1))
        {
            _uiChanges.panelsSelectors[_currentTab].GetComponent<C_UIMaterial_Changer>().OnPointerExit();
            _uiChanges.panels[_currentTab].SetActive(false);
            _currentTab++;
            _uiChanges.panelsSelectors[_currentTab].GetComponent<C_UIMaterial_Changer>().OnPointerEnter();
            _uiChanges.panels[_currentTab].SetActive(true);
        }
    }

    private void AvTab(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.IsSettingsMenuEnabled && !(_currentTab <= 0))
        {
            _uiChanges.panelsSelectors[_currentTab].GetComponent<C_UIMaterial_Changer>().OnPointerExit();
            _uiChanges.panels[_currentTab].SetActive(false);
            _currentTab--;
            _uiChanges.panelsSelectors[_currentTab].GetComponent<C_UIMaterial_Changer>().OnPointerEnter();
            _uiChanges.panels[_currentTab].SetActive(true);
        }
    }
}
