using UnityEngine;
using UnityEngine.Splines;

namespace Player
{
    public class MainCRail : MonoBehaviour
    {
        private SplineContainer _splineContainer;
        private MainCAnimatorController _mainCAnim;
        private MainCDash _mainCDash;
        private MainCMovement _mainCMove;
        private CharacterController _cc;
        private MainCVFX m_mainCVFX;
        private MainCLayers m_mainClayers;
        private MainCSounds m_mainCSounds;
        private MainCJump m_mainCJump;
        private MainCCrouch m_mainCCrouch;

        [SerializeField] private LayerMask railLayer;

     
        [SerializeField] private GameObject PP;

        [SerializeField] private Transform railCheck;

        [System.NonSerialized] public bool IsSliding;

        [SerializeField] private float railSpeed = 5f;
        [SerializeField] private float jumpCooldown = 1f;
        private float _splineLength;
        private float _distancePercentage;
        private float _currentRailSpeed;
        private float m_railTotalCooldown;
        private float m_railCooldown = 1f;

  

        private void Awake()
        {
            _mainCAnim = GetComponent<MainCAnimatorController>();
            _mainCMove = GetComponent<MainCMovement>();
            _cc = GetComponent<CharacterController>();
            m_mainCVFX = GetComponent<MainCVFX>();
            m_mainClayers = GetComponent<MainCLayers>();
            _mainCDash = GetComponent<MainCDash>();
            m_mainCSounds = GetComponent<MainCSounds>();
            m_mainCJump = GetComponent<MainCJump>();
            m_mainCCrouch = GetComponent<MainCCrouch>();
        }


        private void Update()
        {
            if (IsOnRail() && !IsSliding && Time.time > m_railTotalCooldown)
            {

                StartSlide();

            }

            if (IsSliding)
            {

                _currentRailSpeed = railSpeed;
            }

            if (IsSliding)
            {
                var currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);
                Debug.DrawRay(currentPosition, Vector3.up * 20, Color.yellow);
            }

            if (IsSliding && !m_mainCJump.IsJumping && !_mainCMove.IsFalling)
            {
                m_mainCSounds.PlayRailSound();

                // Calcular la distancia recorrida en el mundo del juego
                float distanceTraveled = _currentRailSpeed * Time.deltaTime;

                // Calcular el nuevo _distancePercentage basado en la distancia recorrida
                float newDistancePercentage = _distancePercentage + distanceTraveled / _splineLength;

                // Asegurarse de que el nuevo _distancePercentage esté en el rango [0, 1]
                _distancePercentage = Mathf.Clamp01(newDistancePercentage);

                var currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);
                //transform.position = Vector3.MoveTowards(transform.position ,currentPosition, railSpeed * Time.deltaTime);
                var posVector = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
                var difference = posVector - transform.position;
                _cc.Move(_currentRailSpeed * Time.deltaTime * difference.normalized);



                if (_distancePercentage > 0.99f && IsSliding)
                {
                    _mainCAnim.SetSliding(false);
                    m_mainClayers.DisableSlideLayer();
                    _mainCDash.StartDash(false);
                    m_mainCVFX.SetActiveSparks(false);
                    m_mainCVFX.SetActiveSpeedlines(false);
                    m_railTotalCooldown = Time.time + m_railCooldown;
                    IsSliding = false;
                    m_mainCSounds.RemoveRailSounds();
                    m_mainCCrouch.SetIsCrouch(false);
                    _mainCAnim.SetCrouch(false);
                    m_mainClayers.DisableCrouchLayer();
                }

                var nextPosition = _splineContainer.EvaluatePosition(_distancePercentage + .001f);
                var direction = nextPosition - currentPosition;
                transform.rotation = Quaternion.LookRotation(direction, transform.up);
            }
            else if (IsSliding && m_mainCJump.IsJumping || IsSliding && _mainCMove.IsFalling)
            {
                _distancePercentage += _currentRailSpeed * Time.deltaTime / _splineLength;
                var currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);
                var posVector = new Vector3(currentPosition.x, transform.position.y, currentPosition.z);
                //transform.position = Vector3.MoveTowards(transform.position, currentPosition, railSpeed * Time.deltaTime);
                var difference = posVector - transform.position;
                _cc.Move(_currentRailSpeed * Time.deltaTime * difference.normalized);
                m_mainCSounds.RemoveRailSounds();
            }
        }

        private void StartSlide()
        {
            var cols = Physics.OverlapSphere(transform.position, 1f, railLayer);
            
            _splineContainer = cols[0].transform.GetChild(0).GetComponent<SplineContainer>();
            
            _distancePercentage = 0;
            m_mainClayers.EnableSlideLayer();
            _mainCAnim.SetSliding(true);
            _splineLength = _splineContainer.CalculateLength();
            IsSliding = true;
            _mainCAnim.SetMoveSpeed(0);
            m_mainCVFX.SetRailEffects(true);
            m_mainCSounds.PlayRailSound();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(railCheck.position, .1f);
        }

        public bool IsOnRail()
        {
            return Physics.CheckSphere(railCheck.position, .1f, railLayer);
        }

        public void SetIsSliding(bool isSliding)
        {
            IsSliding = isSliding;
        }

        public bool HigherJumpDueToInclination()
        {
            if (!IsSliding)
            {
                return false;
            }

            var distanceVector = new Vector3(_splineContainer.EvaluatePosition(_distancePercentage).x, _splineContainer.EvaluatePosition(_distancePercentage).y, _splineContainer.EvaluatePosition(_distancePercentage).z);
            var difference = distanceVector - transform.position;
            if (Vector3.Angle(difference, Vector3.up) > 60)
            {
                return false;
            }

            return true;
        }


    }
}
