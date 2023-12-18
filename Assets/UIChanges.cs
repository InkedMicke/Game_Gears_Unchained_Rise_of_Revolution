using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChanges : MonoBehaviour
{
    [Header("Audio")]
    [Header("Graphics")]
    [SerializeField] private OptionsScreen optScreen;
    private Vector2Int _temporalRes;
    private int _temporalIntRes;
    private bool _temporalScreen;
    private int _temporalIntScreen;
    //[Header("Language")]
    //[Header("Controls")]

    public void SaveChanges()
    {
        // Actualizar variables temporales
        _temporalRes = optScreen.GetActiveRes();
        _temporalIntRes = optScreen.resDropdown.value;
        _temporalScreen = optScreen.fullScreen;
        _temporalIntScreen = optScreen.screenDropdown.value;

        // Aplicar cambios
        optScreen.ChangeResolution(_temporalRes, _temporalScreen);

        
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
