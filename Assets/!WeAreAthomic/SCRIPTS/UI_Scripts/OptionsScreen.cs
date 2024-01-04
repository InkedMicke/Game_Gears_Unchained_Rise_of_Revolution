using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OptionsScreen : MonoBehaviour
{
    public TMP_Dropdown resDropdown;
    public TMP_Dropdown screenDropdown;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown antiAliDropdown;

    [SerializeField] private UniversalRenderPipelineAsset _urpQualityLow;
    [SerializeField] private UniversalRenderPipelineAsset _urpQualityMedium;
    [SerializeField] private UniversalRenderPipelineAsset _urpQualityHigh;
    [SerializeField] private UniversalRenderPipelineAsset _urpQualityUltra;

    private Vector2Int screenSize;

    public bool fullScreen = true;
    private bool resFounded;

    private int resIndex;

    public List<Vector2Int> resolutions = new List<Vector2Int>();
    public List<string> resString = new List<string>();
    public List<string> qualityListES;
    public List<string> qualityListEN;

    private void Awake()
    {
        resolutions.Add(new Vector2Int(640, 480));
        resolutions.Add(new Vector2Int(720, 480));
        resolutions.Add(new Vector2Int(1280, 720));
        resolutions.Add(new Vector2Int(1280, 960));
        resolutions.Add(new Vector2Int(1366, 768));
        resolutions.Add(new Vector2Int(1600, 900));
        resolutions.Add(new Vector2Int(1920, 1080));

        qualityListES.Add("BAJA");
        qualityListES.Add("MEDIA");
        qualityListES.Add("ALTA");
        qualityListES.Add("ULTRA");

        qualityListEN.Add("LOW");
        qualityListEN.Add("MID");
        qualityListEN.Add("HIGH");
        qualityListEN.Add("ULTRA");

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(qualityListES);

        foreach (var res in resolutions)
        {
            resIndex++;
            if (res.x == Screen.width && res.y == Screen.height)
            {
                resFounded = true;
                break;
            }
        }

        if (!resFounded)
        {
            resolutions.Add(new Vector2Int(Screen.width, Screen.height));
        }

        foreach (var res in resolutions)
        {
            var currentString = res.x.ToString() + "x" + res.y.ToString();
            resString.Add(currentString);
        }

        if (!resFounded)
        {
            resString.Add(resString[resString.Count - 1] + " (Recomendado)");
            resString.RemoveAt(resString.Count - 2);
        }

        resDropdown.ClearOptions();
        resDropdown.AddOptions(resString);

        resDropdown.SetValueWithoutNotify(GetValueFromVector2Int(resolutions[resolutions.Count - 1]));
    }

    public void SetFullscreenValue()
    {
        if(screenDropdown.value == 0)
        {
            fullScreen = true;
        }
        else
        {
            fullScreen = false;
        }
    }

    public void NextResolution()
    {
        if (resIndex <= resolutions.Count)
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

    public int GetValueFromVector2Int(Vector2Int number)
    {
        foreach(var res in resolutions)
        {
            if(number == res)
            {
                return resolutions.IndexOf(number);
            }
        }
        return 0;
    }

    public void SetActiveFullscreen()
    {
        if(screenDropdown.value == 0)
        {
            fullScreen = true;
        }
        else
        {
            fullScreen = false;
        }
    }

    public Vector2Int GetActiveRes()
    {
        return resolutions[resDropdown.value];
    }

    public void UpdateText()
    {
        //m_resText.text = resolutions[resIndex - 1].x.ToString() + " x " + resolutions[resIndex - 1].y.ToString();
    }

    public void ChangeResolution(Vector2Int screenSize, bool fullscreen)
    {
        this.screenSize = screenSize;
        Screen.SetResolution(screenSize.x, screenSize.y, fullscreen);
    }
    public void SetQualityLevelDropdown(int index)
    {
        switch (index)
        {
            case 0:
                _urpQualityLow.shadowDistance = 20;
                break;

            case 1:
                _urpQualityMedium.shadowDistance = 50;
                break;
            case 2:
                _urpQualityHigh.shadowDistance = 75;
                break;
            case 3:
                _urpQualityUltra.shadowDistance = 100;
                break;
        }

        QualitySettings.SetQualityLevel(index, false);
    }

  
}
