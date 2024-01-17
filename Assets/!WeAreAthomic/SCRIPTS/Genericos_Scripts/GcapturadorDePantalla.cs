using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GcapturadorDePantalla 
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Pantalla/Captura")]
#endif
    public static void CapturarPantalla()
    {
        ScreenCapture.CaptureScreenshot("ScreenShoot.png", 1);
    }
}
