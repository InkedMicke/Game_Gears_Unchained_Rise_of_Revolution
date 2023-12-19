using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChanges : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private List<GameObject> panels;
    [Header("Audio")]
    [Header("Graphics")]
    [SerializeField] private OptionsScreen optScreen;
    private Vector2Int _temporalRes;
    private int _temporalIntRes;
    private bool _temporalScreen;
    private int _temporalIntScreen;
    //[Header("Language")]
    [Header("Controls")]
    [SerializeField] private OptionsControls optControls;

    public void AutoSaveChanges()
    {
        foreach(var obj in panels)
        {
            if(obj.activeSelf)
            {
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
        _temporalScreen = optScreen.fullScreen;
        _temporalIntScreen = optScreen.screenDropdown.value;

        // Aplicar cambios
        optScreen.ChangeResolution(_temporalRes, _temporalScreen);


    }

    public void SaveChangesLanguage()
    {

    }

    public void SaveChangesControls()
    {

    }


    public void CancelChanges()
    {
        // Aplicar cambios directamente
        optScreen.ChangeResolution(_temporalRes, _temporalScreen);

        // Aplicar cambios a la UI
        optScreen.resDropdown.SetValueWithoutNotify(_temporalIntRes);
        optScreen.screenDropdown.SetValueWithoutNotify(_temporalIntScreen);
    }
}
