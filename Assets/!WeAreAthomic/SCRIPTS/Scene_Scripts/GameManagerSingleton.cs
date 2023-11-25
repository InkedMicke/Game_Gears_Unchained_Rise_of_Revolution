using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hedenrag.ExVar;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManagerSingleton : SingletonScriptableObject<GameManagerSingleton>, ICallOnAll
{
    public enum DifficultyLevel
    {
        historia,
        desafio,
        maestro
    }

    public enum Settings
    {
        SensivityX,
        SensivityY
    }

    public enum TypeOfEnemy
    {
        GreenSoldier,
        OrangeSoldier,
        RedSoldier,
        BlueSoldier
    }

    public Settings settings;
    public DifficultyLevel _currentDifficulty = DifficultyLevel.historia;

    public List<GameObject> gameObjectsList;
    public List<GameObject> closestGameObjectsList;

    public bool isGamePaused;
    public bool thereIsCanvasBelow;
    public bool HasUnlockedBastetAttack;
    public bool IsUnlimitedEnergy;
    public bool IsOnTutorialImage;
    private bool _toggledTotally;

    public int sensivityX;
    public int sensivityY;

    public float currentHealth = 100f;

    public float bastetEnergy = 100f;


    private void Awake()
    {

        gameObjectsList = new List<GameObject>();

        gameObjectsList.Clear();
        closestGameObjectsList.Clear();

        isGamePaused = false;

    }

    public void AddGameObject(GameObject gameObject)
    {
        gameObjectsList.Add(gameObject);
    }

    public void AddClosestGameObject(GameObject gameObject)
    {
        closestGameObjectsList.Add(gameObject);
    }

    public void RemoveGameObject(GameObject gameObject)
    {
        gameObjectsList.Remove(gameObject);
    }

    public void RemoveClosestsGameObject(GameObject gameObject)
    {
        closestGameObjectsList.Remove(gameObject);
    }

    public void ClearClosestsGameObject()
    {
        closestGameObjectsList.Clear();
    }

    public void ClearGameObject()
    {
        gameObjectsList.Clear();
    }

    public List<GameObject> GetGameObjectsList()
    {
        return gameObjectsList;
    }

    public GameObject GetClosestGameObjectsList()
    {
        return closestGameObjectsList[0];
    }

    public void PauseGame(bool condition)
    {
        isGamePaused = condition;
    }

    public void EnumToValue(Settings currentSetting, int value)
    {
        if (currentSetting == Settings.SensivityX)
        {
            sensivityX = value;
        }
        else if (currentSetting == Settings.SensivityY)
        {
            sensivityY = value;
        }
    }

    public void SetThereIsCanvasBelow(bool condition)
    {
        thereIsCanvasBelow = condition;
    }

    public void SetHasUnlockedBastetAttack(bool condition)
    {
        HasUnlockedBastetAttack = condition;
    }

    public void TakeEnergy(float value)
    {
        bastetEnergy -= value;
    }

    public void SetIsUnlimitedEnergy(bool condition)
    {
        IsUnlimitedEnergy = condition;
    }

    public void ToggleIsUnlimitedEnergy()
    {
        IsUnlimitedEnergy = !IsUnlimitedEnergy;

        if(IsUnlimitedEnergy)
        {
            bastetEnergy = 100f;
        }
    }

    public void SetIsOnTutorialImage(bool condition)
    {
        IsOnTutorialImage = condition;
    }

    public void CursorMode(bool condition)
    {
        if (condition)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    public void FreezeTime(bool condition)
    {
        Time.timeScale = condition ? 0 : 1;
    }

    public void GameState(bool condition)
    {
        isGamePaused = condition;
    }

    public void CloseTotallyWindow()
    {
        GameState(false);
        CursorMode(false);
        FreezeTime(false);
    }

    public void OpenTotallyWindow()
    {
        GameState(true);
        CursorMode(true);
        FreezeTime(true);
    }

    public void ToggleTotallyWindow()
    {
        _toggledTotally = !_toggledTotally;
        if (_toggledTotally)
        {
            GameState(!true);
            CursorMode(true);
            FreezeTime(true);
        }
        else
        {
            GameState(false);
            CursorMode(false);
            FreezeTime(false);
        }
    }

    public float GetDamage(PlayerDamageData damageData, Collider other)
    {
        switch (_currentDifficulty)
        {
            case DifficultyLevel.historia:
                if (other.gameObject.name == "DummieHurtBox")
                {
                    return damageData.easyDifficult.dummieDamage;
                }
                if (other.gameObject.name == "GreenHurtBox")
                {
                    return damageData.easyDifficult.greenDamage;
                }
                break;
            case DifficultyLevel.desafio:
                if (other.gameObject.name == "GreenHurtBox")
                {
                    return damageData.normalDifficult.greenDamage;
                }
                break;
            case DifficultyLevel.maestro:
                if (other.gameObject.name == "GreenHurtBox")
                {
                    return damageData.hardDifficult.greenDamage;
                }
                break;
        }

        return 0;
    }


}
