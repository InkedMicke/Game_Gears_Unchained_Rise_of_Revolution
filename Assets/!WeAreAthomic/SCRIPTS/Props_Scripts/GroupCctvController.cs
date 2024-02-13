using System.Collections.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class GroupCctvController : MonoBehaviour
    {
        private LightKillerHurtBox _lightHurt;
        [SerializeField] private GameObject originalCam;

        [SerializeField] private List<GameObject> cameraGroup;

        private List<CctvController> allcctvs;

        private void Awake()
        {
            _lightHurt = transform.GetChild(0).GetChild(2).GetComponent<LightKillerHurtBox>();

            foreach (var x in cameraGroup)
            {
                var cctv = x.GetComponentInChildren<CctvController>();
                cctv.SetHasGroupCamera(true);
                allcctvs.Add(cctv);
            }
        }

        public void SendColToHurtBox()
        {
            _lightHurt.GotEnterCollision();
        }

        public void DisableTemporalyAllCamerasInChildren(float time)
        {
            originalCam.GetComponentInChildren<CctvController>().TurnOffCamera(time);

            foreach(var x in allcctvs)
            {
                x.TurnOffCamera(time);
            }
        }

        public void DisablePermanentAllCamerasInChildren()
        {
            originalCam.GetComponentInChildren<CctvController>().PermanentDisabled();

            foreach (var x in allcctvs)
            {
                x.PermanentDisabled();
            }
        }
    }
}
