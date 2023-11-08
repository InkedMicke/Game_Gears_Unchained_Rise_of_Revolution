using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Genericos
{
    public class GIntroToLoopSound : MonoBehaviour
    {
        [SerializeField] private AudioClip intro;
        [SerializeField] private AudioSource loop;

        private PlayerInputActions _playerInput;

        private void Start()
        {
            var desiredLength = intro.length - 2.35f;
            Invoke(nameof(PlayLoop), desiredLength);
        }

        private void PlayLoop()
        {
            loop.Play();
        }
    }
}
