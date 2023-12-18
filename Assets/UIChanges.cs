using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChanges : MonoBehaviour
{
    [Header("Audio")]
    [Header("Graphics")]
    [SerializeField] private OptionsScreen optScreen;
    private Vector2Int temporalRes;
    //[Header("Language")]
    //[Header("Controls")]

    private void Start()
    {

    }

    public void SetTemporalChanges()
    {
        temporalRes = optScreen.GetActiveRes();
    }

    public void ApplyChanges()
    {
        optScreen.ChangeResolution(optScreen.GetActiveRes());
        temporalRes = optScreen.GetActiveRes();
    }

    public void UndoChanges()
    {
        optScreen.ChangeResolution(temporalRes);
        optScreen.resDropdown.SetValueWithoutNotify(optScreen.GetValueFromVector2Int(temporalRes));
    }
}
