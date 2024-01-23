using UnityEngine;
using UnityEngine.EventSystems;


namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GEventSystem : MonoBehaviour
    {
        public void EventSystemSelectedGameObject(GameObject go)
        {
            EventSystem.current.SetSelectedGameObject(go);
        }
    }
}
