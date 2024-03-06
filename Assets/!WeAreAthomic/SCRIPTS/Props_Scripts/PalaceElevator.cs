using UnityEngine;
using Player;
using DG.Tweening;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class PalaceElevator : MonoBehaviour
    {
        private MainCMovement _mainCMovement;

        [SerializeField] private Transform playerTr;


        [Header("Elevator")]

        [SerializeField] private Transform trToFinish;
        [SerializeField] private float elevatorDuration = 15f;


        private bool isMoving = false;
        private void Start()
        {
            _mainCMovement = playerTr.GetComponent<MainCMovement>();

        }
        public void GoUpElevator()
        {
            if (!isMoving)
            {
                isMoving = true;
                playerTr.SetParent(transform);
                playerTr.GetComponent<CharacterController>().enabled = false;
                _mainCMovement.SetCanApplyGravity(false);
                transform.DOMove(trToFinish.position, elevatorDuration).SetEase(Ease.Linear).OnComplete(() =>
                {

                    playerTr.SetParent(null);
                    _mainCMovement.SetCanApplyGravity(true);
                    playerTr.GetComponent<CharacterController>().enabled = true;

                    isMoving = false;
                });
            }

        }
    }
}
