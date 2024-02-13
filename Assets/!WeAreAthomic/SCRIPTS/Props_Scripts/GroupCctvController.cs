using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class GroupCctvController : MonoBehaviour
    {
        [SerializeField] private LightKillerHurtBox killerHurtBox;

        [SerializeField] private List<CctvController> cameraGroup;

        public void SendColToHurtBox()
        {
            killerHurtBox.GotEnterCollision();
        }
    }
}
