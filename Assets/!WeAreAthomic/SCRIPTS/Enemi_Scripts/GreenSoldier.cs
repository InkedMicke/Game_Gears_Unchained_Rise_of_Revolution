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

    [System.NonSerialized] public bool IsShooting;
    private bool _hasEndedShootAnim;

    private int _currentShoots;

    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private float speedShooting = 1.5f;
    [SerializeField] private float spreadAngle = 2f;
    [SerializeField] private float shootAngle = .6f;
    [SerializeField] private float maxTimeToStopShooting = 2.5f;
    [SerializeField] private float rotationSpeed = 5f;
    public float totalColdown;

    protected override void Awake()
    {
        _decalHurtBox = GetComponentInChildren<GreenDecalHurtBox>();
        base.Awake();
    }

    protected override void Update()
    {
        var distanceToPlayer = Vector3.Distance(transform.position, _playerTr.position);

        if (distanceToPlayer < 6f && !IsAttacking && !_soldierHurtBox.IsDeath && IsChasingPlayer)
        {
            _agent.isStopped = true;
            _soldierAnim.SetWalking(false);
            StartDecalToAttack();
            IsChasingPlayer = false;
            isPatrolling = false;
        }
        
/*        if(IsChasingPlayer && !IsAttacking)
        {
            if(distanceToPlayer < 2f)
            {
                _soldierAnim.SetBackWalking(true);
                IsChasingPlayer = false;
            }
        }*/

        base.Update();
    }

    private void StartDecalToAttack()
    {
        _agent.isStopped = true;
        IsAttacking = true;
        StartCoroutine(TurnToPlayer());
    }

    private IEnumerator ShootCoroutine()
    {
        IsShooting = true;
        while (IsShooting && !_soldierHurtBox.IsDeath)
        {
            _hasEndedShootAnim = false;
            _soldierAnim.ShootTrigger();
            _soldierAnim.SetAnimatorSpeed(speedShooting);
            _currentShoots++;
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
                    muzzle1.position.y + Random.Range(-1f, .5f) + Random.Range(-spreadAngle, spreadAngle),
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
            decal.uvBias = new(decal.uvBias.x, 0);
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

    // Comprueba si el personaje ha salido del decal para continuar o no atacando
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

    // Esta funcion se llama cuando animacion de ataque finaliza, Si el player ha salido del decal
    // y el soldado ha disparado X veces, entonces deja de disparar
    // Si no ha salido del decal reiniciamos los disparos actuales y sigue disparando
    public void EndShootAnim()
    {
        _hasEndedShootAnim = true;
        if (CheckForDisableShoot())
        {
            if (_currentShoots == 3)
            {
                _decalCoroutine = StartCoroutine(DecalSize(false));
                IsAttacking = false;
                IsShooting = false;
                StartChasingPlayer();
                _currentShoots = 0;
            }
        }
        else
            _currentShoots = 0;
    }
}
