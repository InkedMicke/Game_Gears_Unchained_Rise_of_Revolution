using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_resText;

    private Vector2Int screenSize;

    public bool fullScreen;
    private bool resFounded;

    private int resIndex;

    public List<Vector2Int> resolutions;

    private void Start()
    {
        resolutions.Add(new Vector2Int(640, 480));
        resolutions.Add(new Vector2Int(720, 480));
        resolutions.Add(new Vector2Int(1280, 720));
        resolutions.Add(new Vector2Int(1280, 960));
        resolutions.Add(new Vector2Int(1366, 768));
        resolutions.Add(new Vector2Int(1600, 900));
        resolutions.Add(new Vector2Int(1920, 1080));

        foreach(var res in resolutions)
        {
            resIndex++;
            if (res.x == Screen.width && res.y == Screen.height)
            {
                resFounded = true;
                break;
            }
        }

        if(!resFounded)
        {
            resolutions.Add(new Vector2Int(Screen.width, Screen.height));
        }

        m_resText.text = resolutions[resIndex].x.ToString() + " x " +  resolutions[resIndex].y.ToString();
    }

    public void SetFullscreen(bool on)
    {
        fullScreen = on;
        Screen.SetResolution(screenSize.x, screenSize.y, fullScreen);
    }

    public void NextResolution()
    {
        if(resIndex < resolutions.Count)
        {
            resIndex++;
            UpdateText();
        }
    }
    
    public void PreviousResolution()
    {
        if (resIndex > 0)
        {
            resIndex--;
            UpdateText();
        }
    }

    public void UpdateText()
    {
        m_resText.text = resolutions[resIndex].x.ToString() + " x " + resolutions[resIndex].y.ToString();
    }

    public void ChangeResolution(Vector2Int screenSize)
    {
        this.screenSize = screenSize;
        Screen.SetResolution(screenSize.x, screenSize.y, fullScreen);
    }
}
