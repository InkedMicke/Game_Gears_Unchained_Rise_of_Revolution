using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AjustesGraficos : MonoBehaviour
{
    bool fullScreen;
    Vector2Int screenSize;

    public void SetFullscreen(bool on) 
    { 
        fullScreen = on;
        Screen.SetResolution(screenSize.x, screenSize.y, fullScreen);
    }
    public void ChangeResolution(Vector2Int screenSize)
    {
        this.screenSize = screenSize;
        Screen.SetResolution(screenSize.x, screenSize.y, fullScreen);
    }
}
