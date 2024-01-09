using UnityEngine;
using UnityEngine.EventSystems;

public class GEventSystem : MonoBehaviour
{
    public void EventSystemSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(go);
    }
}
