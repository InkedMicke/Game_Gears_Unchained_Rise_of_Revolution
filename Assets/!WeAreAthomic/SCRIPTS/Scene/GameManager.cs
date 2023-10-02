using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> gameObjectsList;
    public List<GameObject> closestGameObjectsList;
    public List<Transform> railList;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        gameObjectsList = new List<GameObject>();

        gameObjectsList.Clear();
        closestGameObjectsList.Clear();
        railList.Clear();

        // Mantener este GameObject en todas las escenas
        DontDestroyOnLoad(gameObject);

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

    public void SortClosestsGameObject()
    {
        closestGameObjectsList.Sort((obj1, obj2) =>
        {
            Transform trs1 = obj1.transform;
            Transform trs2 = obj2.transform;
            float distance1 = Vector3.Distance(transform.position, trs1.position);
            float distance2 = Vector3.Distance(transform.position, trs2.position);
            return distance1.CompareTo(distance2);
        });
    }

    public void AddRaiTr(Transform railTr)
    {
        railList.Add(railTr);
    }

    public void RemoveRaiTr(Transform railTr)
    {
        railList.Remove(railTr);
    }

    public void ClearRaiTr()
    {
        railList.Clear();
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

    public List<Transform> GetRailList()
    {
        return railList;
    }



}
