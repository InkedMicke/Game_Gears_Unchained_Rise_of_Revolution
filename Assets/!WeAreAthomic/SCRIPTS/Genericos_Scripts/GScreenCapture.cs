using UnityEngine;

namespace Generics
{
    public class GScreenCapture
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Pantalla/Captura")]
#endif
        public static void CapturarPantalla()
        {
            ScreenCapture.CaptureScreenshot("ScreenShoot.png", 1);
        }
    }
}