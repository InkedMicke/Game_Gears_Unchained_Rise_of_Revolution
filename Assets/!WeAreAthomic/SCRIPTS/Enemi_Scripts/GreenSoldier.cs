using _WeAreAthomic.SCRIPTS.Enemi_Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GreenSoldier : Enemy
{
    private GreenDecalHurtBox _decalHurtBox;

    private Coroutine _shootCoroutine;
    private Coroutine _decalCoroutine;

    [SerializeField] private AnimationCurve moveToDecalCurve;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject decalAttack;
    private GameObject _currentDecal;

    [SerializeField] private Transform muzzle1;
    private Transform endDecalTr;
    private Transform _playerTr => GameObject.FindGameObjectWithTag("Player").transform;

    [System.NonSerialized] public bool IsShooting;
    private bool _hasEndedShootAnim;
    private bool _waitForDisableShoot;

    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private float speedShooting = 1.5f;
    [SerializeField] private float spreadAngle = 2f;
    [SerializeField] private float shootAngle = .6f;
    [SerializeField] private float maxTimeToStopShooting = 2.5f;
    [SerializeField] private float rotationSpeed = 5f;
    private float _timeToStopShooting;
    public float totalColdown;

    protected override void Awake()
    {
        _decalHurtBox = GetComponentInChildren<GreenDecalHurtBox>();
        base.Awake();
    }

    protected override void StartDecalToAttack()
    {
        _agent.isStopped = true;
        IsAttacking = true;
        StartCoroutine(TurnToPlayer());
    }

    protected override void Update()
    {
        TimerToDisableShoot();
        base.Update();
    }

    private IEnumerator ShootCoroutine()
    {
        IsShooting = true;
        while (IsShooting && !_soldierHurtBox.IsDeath)
        {
            _hasEndedShootAnim = false;
            _soldierAnim.ShootTrigger();
            _soldierAnim.SetAnimatorSpeed(speedShooting);

            while (!_hasEndedShootAnim)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void SpawnBullet()
    {
        for (int i = 0; i < 5; i++) // Cambia este valor seg�n la cantidad de proyectiles que quieres disparar
        {
            if (endDecalTr != null)
            {
                var desiredPos = new Vector3(
                    endDecalTr.position.x + Random.Range(-spreadAngle, spreadAngle),
                    (muzzle1.position.y - shootAngle) + Random.Range(-spreadAngle, spreadAngle),
                    endDecalTr.position.z + Random.Range(-spreadAngle, spreadAngle)
                    );
                var bulletObj = Instantiate(bullet, muzzle1.position, transform.rotation);
                bulletObj.transform.LookAt(desiredPos);
                var dir = desiredPos - bulletObj.transform.position;

                bulletObj.GetComponent<Rigidbody>().AddForce(dir.normalized * bulletObj.GetComponent<GBullet>().bulletForce, ForceMode.Impulse);
            }
        }

    }

    private IEnumerator TurnToPlayer()
    {
        while (!_soldierHurtBox.IsDeath)
        {
            var currentPlayerPos = _playerTr.transform.position;

            // Calcular la direcci�n hacia el objetivo
            Vector3 targetDirection = currentPlayerPos - transform.position;

            // Calcular la rotaci�n hacia el objetivo
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Rotar suavemente hacia el objetivo
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Si estamos casi mirando al objetivo, detener la rotaci�n
            if (Quaternion.Angle(transform.rotation, targetRotation) < 6f)
            {
                _decalCoroutine = StartCoroutine(DecalSize(true));
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator DecalSize(bool greaterOrSmaller)
    {
        yield return new WaitForSeconds(.2f);

        endDecalTr = decalAttack.transform.GetChild(1);

        var decal = decalAttack.transform.GetChild(0).GetComponent<DecalProjector>();

        if (greaterOrSmaller)
        {
            decal.uvBias = new(decal.uvBias.x, 1);

            while (decal.uvBias.y > 0)
            {
                decal.uvBias -= new Vector2(0, Time.deltaTime * 4);
                yield return new WaitForSeconds(.01f);
            }

            _shootCoroutine = StartCoroutine(ShootCoroutine());
        }
        else
        {
            decal.uvBias = new(decal.uvBias.x, 0);

            while (decal.uvBias.y < 1)
            {
                decal.uvBias += new Vector2(0, Time.deltaTime * 8);
                yield return new WaitForSeconds(.01f);
            }

            decal.uvBias = new(decal.uvBias.x, 1);
        }


    }

    /// <summary>
    /// Destroy decal if exists
    /// </summary>
    public void DestroyDecal()
    {
        if (_currentDecal != null)
        {
            Destroy(_currentDecal);
        }
    }

    private bool CheckForDisableShoot()
    {

        _decalHurtBox = decalAttack.GetComponentInChildren<GreenDecalHurtBox>();

        if (_decalHurtBox.HasPlayerLeft)
        {
            return true;
        }

        if (!_decalHurtBox.HasPlayerLeft && !_decalHurtBox.IsPlayerInside)
        {
            return true;
        }


        return false;
    }

    private void TimerToDisableShoot()
    {
        if (_waitForDisableShoot)
        {
            _timeToStopShooting -= Time.deltaTime;
            if (_timeToStopShooting <= 0f)
            {
                _decalCoroutine = StartCoroutine(DecalSize(false));
                IsAttacking = false;
                IsShooting = false;
                StartChasingPlayer();
                _waitForDisableShoot = false;
            }
        }
    }

    public void EndShootAnim()
    {
        _hasEndedShootAnim = true;
        if (CheckForDisableShoot())
        {
            if (!_waitForDisableShoot)
            {
                _timeToStopShooting = maxTimeToStopShooting;
            }
            _waitForDisableShoot = true;
        }
        else
            _waitForDisableShoot = false;
    }
}
