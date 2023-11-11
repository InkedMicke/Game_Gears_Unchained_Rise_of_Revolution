using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hedenrag.ExVar;

public class GameManagerSingleton : SingletonScriptableObject<GameManagerSingleton>, ICallOnAll
{
    public enum Settings
    {
        SensivityX,
        SensivityY
    }

    public Settings settings;

    public List<GameObject> gameObjectsList;
    public List<GameObject> closestGameObjectsList;

    public bool isGamePaused;

    public int sensivityX;
    public int sensivityY;

    public float currentHealth = 100f;

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
        if(currentSetting == Settings.SensivityX)
        {
            sensivityX = value;
        }
        else if(currentSetting == Settings.SensivityY)
        {
            sensivityY = value;
        }
    }


}
