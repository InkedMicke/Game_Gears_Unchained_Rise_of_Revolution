using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts
{
    public class CameraStayed : CameraFollower
    {
        [SerializeField] private Transform aimPosPlayer;
        private protected override void Update()
        {
            Look();
            transform.position = aimPosPlayer.position;
            base.Update();
        }

        private void Look()
        {
            transform.rotation = LocalRotation;
        }
    }
}
