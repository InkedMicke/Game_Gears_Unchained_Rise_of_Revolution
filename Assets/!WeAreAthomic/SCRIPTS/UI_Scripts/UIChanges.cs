using System.Collections.Generic;
using UnityEngine;

public class UIChanges : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private List<GameObject> panels;
    //[Header("Audio")]
    [Header("Graphics")]
    [SerializeField] private OptionsScreen optScreen;
    private Vector2Int _temporalRes;
    private int _temporalIntRes;
    private bool _temporalScreenMode;
    private int _temporalIntScreen;
    private int _temporalIntQuality;
    //[Header("Language")]
    [Header("Controls")]
    [SerializeField] private OptionsControls optControls;
    private TypeOfInput _temporalTypeInput = TypeOfInput.pc;
    private int _temporalIntTypeInput;
    private int _temporalSensivityX;
    private int _temporalSensivityY;
    private int _temporalSliderSensivityX;
    private int _temporalSliderSensivityY;



    private int _currentPanel;

    public void AutoSaveChanges()
    {
        foreach (var obj in panels)
        {
            _currentPanel++;
            if (obj.activeSelf)
            {
                switch (_currentPanel)
                {
                    case 1:
                        SaveChangesAudio();
                        break;
                    case 2:
                        SaveChangesGraphics();
                        break;
                    case 3:
                        SaveChangesLanguage();
                        break;
                    case 4:
                        SaveChangesControls();
                        break;
                }
                _currentPanel = 0;
                break;
            }
        }
    }

    public void SaveChangesAudio()
    {

    }

    public void SaveChangesGraphics()
    {
        // Actualizar variables temporales
        _temporalRes = optScreen.GetActiveRes();
        _temporalIntRes = optScreen.resDropdown.value;
        _temporalScreenMode = optScreen.fullScreen;
        _temporalIntScreen = optScreen.screenDropdown.value;
        _temporalIntQuality = optScreen.qualityDropdown.value;

        // Aplicar cambios
        optScreen.ChangeResolution(_temporalRes, _temporalScreenMode);
        optScreen.SetQualityLevelDropdown(_temporalIntQuality);
    }

    public void SaveChangesLanguage()
    {

    }

    public void SaveChangesControls()
    {
        // Actualizar variables temporales
        _temporalTypeInput = GameManagerSingleton.Instance.typeOfInput;
        _temporalIntTypeInput = optControls.inputDropdown.value;
        _temporalSensivityX = GameManagerSingleton.Instance.sensivityX;
        _temporalSensivityY = GameManagerSingleton.Instance.sensivityY;
        _temporalSliderSensivityX = (int)optControls.sensivityXSlider.value;
        _temporalSliderSensivityY = (int)optControls.sensivityYSlider.value;


        // Aplicar cambios
    }


    public void CancelChanges()
    {
        // Aplicar cambios directamente
        GameManagerSingleton.Instance.typeOfInput = _temporalTypeInput;
        GameManagerSingleton.Instance.sensivityX = _temporalSensivityX;
        GameManagerSingleton.Instance.sensivityY = _temporalSensivityY;

        // Aplicar cambios a la UI
        optScreen.resDropdown.SetValueWithoutNotify(_temporalIntRes);
        optScreen.screenDropdown.SetValueWithoutNotify(_temporalIntScreen);
        optScreen.qualityDropdown.SetValueWithoutNotify(_temporalIntQuality);
        optControls.inputDropdown.SetValueWithoutNotify(_temporalIntTypeInput);
        optControls.sensivityXSlider.value = _temporalSliderSensivityX;
        optControls.sensivityYSlider.value = _temporalSliderSensivityY;

    }
}
