using UnityEngine;

namespace Player.CameraS
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
