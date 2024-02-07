using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class MainC360Attack : MonoBehaviour
{
    private MainCSounds _mainCSounds;
    PlayerInputActions _playerInputActions;
    private BastetController _bastet;

    private Tween _rotateTween;

    [SerializeField] private PlayerDamageData _playerDamageData;

    [SerializeField] private GameObject bastetObj;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private int rafagas = 5;
    private int _currentRafaga;

    private void Awake()
    {
        _bastet = bastetObj.GetComponent<BastetController>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.PlayerPC.BastetAttack.performed += InputPC;
        _playerInputActions.PlayerGamepad.BastetAttack.performed += InputGamepad;
        _mainCSounds = GetComponent<MainCSounds>();
    }

    private void InputPC(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc && GameManagerSingleton.Instance.currentAbility == CurrentAbility.ThreeSixtyAttack)
        {
            StartAttacking();
        }
            
    }

    private void InputGamepad(InputAction.CallbackContext context)
    {
        if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad && GameManagerSingleton.Instance.currentAbility == CurrentAbility.ThreeSixtyAttack)
        {
            StartAttacking();
        }
            
    }

    private void StartAttacking()
    {
        _mainCSounds.PlayBastetCall();
        bastetObj.SetActive(true);
        _bastet.HideScanner();
        var desiredPos = transform.position + transform.forward * 2 + transform.up * 1.5f;
        _bastet.transform.position = _bastet.playerRightArm.transform.position;
        _bastet.GoToDesiredPos(StartShooting, desiredPos, 1f, Ease.Linear);
    }

    private void StartShooting()
    {
        var desiredRot = new Vector3(0f, 360f, 0f);
        _rotateTween = bastetObj.transform.DORotate(desiredRot, .5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        StartCoroutine(Shooting());
    }

    private IEnumerator Shooting()
    {
        while (_currentRafaga < rafagas)
        {
            for (int i = 0; i < 20; i++)
            {
                float angulo = i * (360f / 20);
                float x = bastetObj.transform.position.x + Mathf.Cos(angulo * Mathf.Deg2Rad) * .2f;
                float z = bastetObj.transform.position.z + Mathf.Sin(angulo * Mathf.Deg2Rad) * .2f;
                var posicion = new Vector3(x, bastetObj.transform.position.y, z);
                var bullet = Instantiate(bulletPrefab, posicion, Quaternion.identity);

                // Calcula la dirección hacia la que se desplaza la bala
                var direccion = (posicion - bastetObj.transform.position).normalized;

                // Calcula la rotación basada en la dirección
                var rotacion = Quaternion.LookRotation(direccion);

                // Aplica la rotación a la bala
                bullet.transform.rotation = rotacion;

                // Aplica la velocidad a la bala
                bullet.GetComponent<Rigidbody>().velocity = direccion * 5f;
            }
            _currentRafaga++;
            yield return new WaitForSeconds(.5f);
        }
        _currentRafaga = 0;
        _rotateTween.Kill();
        _bastet.GoToDesiredPos(EndAttack, _bastet.playerRightArm.transform.position, 1f, Ease.Linear);

    }

    private void EndAttack()
    {
        bastetObj.SetActive(false);
    }
}
