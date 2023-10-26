using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props
{
    public class DummiesColliderLab : MonoBehaviour
    {
        [SerializeField] private GameObject movableFloor;
        
        [SerializeField] private List<GameObject> colisionList;

        private void OnTriggerEnter(Collider other)
        {
            if(!colisionList.Contains(other.gameObject))
            {
                colisionList.Add(other.gameObject);
            }

            MakeChild();
        }

        public void ClearList()
        {
            colisionList.Clear();
        }

        public void MakeChild()
        {
            foreach(var g in colisionList)
            {
                Debug.Log("hola5");
                var parent = g.transform.parent;
                //parent.DetachChildren();
                parent.SetParent(movableFloor.transform);
            }
        }

        public void UndoChild()
        {
            foreach(var g in colisionList)
            {
                var parent = g.transform.parent;
                //parent.DetachChildren();
                parent.SetParent(movableFloor.transform);
            }
        }
    }
}
