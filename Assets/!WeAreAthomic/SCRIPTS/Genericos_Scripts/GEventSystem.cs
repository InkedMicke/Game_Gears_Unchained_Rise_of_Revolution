using UnityEngine;
using UnityEngine.EventSystems;


namespace Generics
{
    public class GEventSystem : MonoBehaviour
    {
        public void EventSystemSelectedGameObject(GameObject go)
        {
            EventSystem.current.SetSelectedGameObject(go);
        }
    }
}
