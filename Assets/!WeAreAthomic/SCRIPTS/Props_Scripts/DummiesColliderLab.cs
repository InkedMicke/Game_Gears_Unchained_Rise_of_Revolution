using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class DummiesColliderLab : MonoBehaviour
    {
        
        
        [SerializeField] private GameObject movableFloor;

        [SerializeField] private UnityEvent ListClear;
        
        [SerializeField] private List<GameObject> colisionList;

        private void OnTriggerEnter(Collider other)
        {
            if(!colisionList.Contains(other.gameObject))
            {
                colisionList.Add(other.gameObject);
            }

            MakeChild();
        }
        public void AllDefeat()
        {
            if(colisionList.Count == 0)
            {
                ListClear.Invoke();
            }
        }
        public void ClearList()
        {
            colisionList.Clear();
        }

        public void MakeChild()
        {
            foreach(var g in colisionList)
            {
                var parent = g.transform.parent;
                //parent.DetachChildren();
                parent.SetParent(movableFloor.transform);
            }
        }

        public void UndoChild(GameObject obj)
        {
            foreach(var g in colisionList)
            {
                var parent = g.transform.parent;
                //parent.DetachChildren();
                parent.SetParent(obj.transform);
            }
        }
    }
}
