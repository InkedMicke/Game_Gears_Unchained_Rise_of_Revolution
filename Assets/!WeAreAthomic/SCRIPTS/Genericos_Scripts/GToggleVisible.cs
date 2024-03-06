using UnityEngine;

namespace Generics
{
    public class GToggleVisible : MonoBehaviour
    {

        public void EnableVisible(float value)
        {
            Invoke(nameof(Visible), value);
        }
        
        public void DisableVisible(float value)
        {
            Invoke(nameof(Hide), value);
        }

        private void Visible()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}
