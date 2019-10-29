using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;
using GameUtils;
using System;
using System.Linq;

public class ZombunnyEnemy : EntityMovement
{
    public GameObject shootParticle;
    public bool isLaserLoaded;
    public float rushingSpeed;
    public float rotationSpeed;
    public int meleeDamage;
    public float meleeDistance;

    public float pickUpDistance;
    public int clipSize;
    public LayerMask batteryLayerMask;
    public LayerMask mediKitLayerMask;

    [SerializeField]
    private ParticleSystem biteEffect;

    [SerializeField]
    private EnemyShooting enemyShooting;

    private Queue<string> _queueActions = new Queue<string>();
    private Dictionary<string, Func<bool>> concreteActions = new Dictionary<string, Func<bool>>();
    private bool hasRun;

    private Transform _target;
    private Transform _player;
    private Transform _closestBattery;
    private Transform _closestMediKit;
    private Animator _anim;
    private bool _finishAnimation;
    private int _clipSizeMax;

    public override void Start()
    {
        base.Start();
        _anim = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag(StringTagManager.tagPlayer).transform;
        _clipSizeMax = clipSize;
        SetTarget(_player);
        CreateConcreteActionsDict();
        GetGOAPPlaning();
        _vel = Vector3.zero;
        OnReachDestination += OnReachDestinationStopMoving;
        OnDeath += OnZombunnyDeath;
    }

    private void FixedUpdate()
    {
        if (_target == null || _isDeath) return;

        if(!rotateInPath)
            Rotate();

        if(_canMove)
            transform.position += _vel * Time.deltaTime * _movementSpeed;
    }

    #region GOAPPlaning Functions
    List<IGOAPAction> CreateActions()
    {
        var _availableActions = new List<IGOAPAction>();

        _availableActions.Add(
            new GOAPAction(GOAPActionKeyEnum.Shoot)
                .SetPrecondition(GOAPKeyEnum.isLaserLoaded, x => (bool)x == true)
                .SetPrecondition(GOAPKeyEnum.isInWeaponRange, x => (bool)x == true)
                .SetPrecondition(GOAPKeyEnum.battery, x => (int)x > 0)
                .SetEffect(GOAPKeyEnum.isAttacking, x => x = true)
                .SetCost(1));

        _availableActions.Add(
            new GOAPAction(GOAPActionKeyEnum.PositionToShoot)
                .SetPrecondition(GOAPKeyEnum.battery, x => (int)x > 0)
                .SetEffect(GOAPKeyEnum.isInWeaponRange, x => x = true)
                .SetCost(Math.Max(Vector3.Distance(_player.position, transform.position) - enemyShooting.shootDistance, 1)));

        _availableActions.Add(
            new GOAPAction(GOAPActionKeyEnum.Reload)
                .SetPrecondition(GOAPKeyEnum.battery, x => (int)x > 0)
                .SetEffect(GOAPKeyEnum.isLaserLoaded, x => x = true)
                .SetCost(1));

        _availableActions.Add(
            new GOAPAction(GOAPActionKeyEnum.GrabBattery)
                .SetPrecondition(GOAPKeyEnum.batteryNearby, x => (bool)x == true)
                .SetEffect(GOAPKeyEnum.battery, x => x = (int)x + 1)
                .SetCost(GetCostForBatteryNearby()));

        _availableActions.Add(
            new GOAPAction(GOAPActionKeyEnum.Melee)
                .SetPrecondition(GOAPKeyEnum.isInMeleeRange, x => (bool)x == true)
                .SetEffect(GOAPKeyEnum.isAttacking, x => x = true)
                .SetCost(50));

        _availableActions.Add(
            new GOAPAction(GOAPActionKeyEnum.Rush)
                .SetPrecondition(GOAPKeyEnum.life, x => (float)x > 5)
                .SetEffect(GOAPKeyEnum.isInMeleeRange, x => x = true)
                .SetCost(Vector3.Distance(_player.position, transform.position)));

        _availableActions.Add(
            new GOAPAction(GOAPActionKeyEnum.Forward)
                .SetEffect(GOAPKeyEnum.isInMeleeRange, x => x = true)
                .SetCost(Vector3.Distance(_player.position, transform.position) + 5));

        _availableActions.Add(
            new GOAPAction(GOAPActionKeyEnum.GetHealed)
                .SetPrecondition(GOAPKeyEnum.medikitNearby, x => (bool)x == true)
                .SetEffect(GOAPKeyEnum.life, x => x = (float)x + 20)
                .SetCost(GetCostForMedikitNearby()));

        return _availableActions;
    }

    private void GetGOAPPlaning()
    {
        StopAllCoroutines();
        _queueActions.Clear();
        var actions = CreateActions();
        var initialState = new Map<GOAPKeyEnum, object>()
        {
            { GOAPKeyEnum.battery , battery},
            { GOAPKeyEnum.life , Life},
            { GOAPKeyEnum.isAttacking , false},
            { GOAPKeyEnum.isLaserLoaded , isLaserLoaded},
            { GOAPKeyEnum.isInMeleeRange , IsInMeleeRange() },
            { GOAPKeyEnum.isInWeaponRange , IsInShootingRange() },
            { GOAPKeyEnum.batteryNearby , IsBatteryNearby() },
            { GOAPKeyEnum.medikitNearby , IsMedikitNearby() },
            { GOAPKeyEnum.playerIsAlive , IsPlayerAlive() }
        };
        var goal = new Map<GOAPKeyEnum, Func<object, bool>>() {
            { GOAPKeyEnum.isAttacking , x => (bool) x == true},
            { GOAPKeyEnum.playerIsAlive , x => (bool) x == true}
        };

        var plan = new GOAPPlan(actions, initialState, goal, heuristic);

        foreach (var action in plan.Execute())
        {
            _queueActions.Enqueue(action.Name.ToString());
        }
        GoGoGOAP();
    }

    private void GoGoGOAP()
    {
        if (_isDeath) return;

        foreach (string item in _queueActions)
        {
            Debug.LogError(item);
        }

        Debug.LogError("-----------------------------");

        if (_queueActions.Count > 0) StartCoroutine(ExecuteAction(concreteActions[_queueActions.Dequeue()]));
        else GetGOAPPlaning();
    }

    private float heuristic(IGraphNode<GOAPState> curr)
    {
        float total = 0;
        GOAPState state = (GOAPState)curr;

        if (state.goalValues.ContainsKey(GOAPKeyEnum.isAttacking)) total += (bool)state.currentValues[GOAPKeyEnum.isAttacking] ? 0 : 1;
        if (state.goalValues.ContainsKey(GOAPKeyEnum.isLaserLoaded)) total += (bool)state.currentValues[GOAPKeyEnum.isLaserLoaded] ? 0 : 1;
        if (state.goalValues.ContainsKey(GOAPKeyEnum.isInMeleeRange)) total += (bool)state.currentValues[GOAPKeyEnum.isInMeleeRange] ? 0 : 1;
        if (state.goalValues.ContainsKey(GOAPKeyEnum.isInWeaponRange)) total += (bool)state.currentValues[GOAPKeyEnum.isInWeaponRange] ? 0 : 1;
        if (state.goalValues.ContainsKey(GOAPKeyEnum.batteryNearby)) total += (bool)state.currentValues[GOAPKeyEnum.batteryNearby] ? 0 : 1;
        if (state.goalValues.ContainsKey(GOAPKeyEnum.medikitNearby)) total += (bool)state.currentValues[GOAPKeyEnum.medikitNearby] ? 0 : 1;
        if (state.goalValues.ContainsKey(GOAPKeyEnum.battery)) total += (int)state.currentValues[GOAPKeyEnum.battery] > 0 ? 0 : 1;
        if (state.goalValues.ContainsKey(GOAPKeyEnum.life)) total += (float)state.currentValues[GOAPKeyEnum.life] > 5 ? 0 : 1;

        return total;
    }

    private Dictionary<String, Func<bool>> CreateConcreteActionsDict()
    {
        concreteActions.Add(GOAPActionKeyEnum.Rush.ToString(), Rush);
        concreteActions.Add(GOAPActionKeyEnum.Melee.ToString(), Melee);
        return concreteActions;
    }

    #endregion

    #region Event Functions
    public void AnimShootLaser()
    {
        if (battery <= 0) return;
        enemyShooting.Shoot();
        battery--;
        _clipSizeMax--;
    }

    public void AnimGrab()
    {     
        if (_target == null) return;

        if (_target.GetComponent<DuracellBattery>() != null)
        {
            ReplenishBattery(_target.GetComponent<DuracellBattery>().GetBattery());
        }
        else if (_target.GetComponent<MedikitBox>() != null)
        {
            Healed(_target.GetComponent<MedikitBox>().GetHealed());
        }
        _finishAnimation = true;
    }

    public void AnimReload()
    {
        isLaserLoaded = true;
        _clipSizeMax = clipSize;
    }

    public void AnimMeleeAttack()
    {
        biteEffect.Play();

        var victim = _target.GetComponent<PlayerHealth>();

        if (victim != null && Vector3.Distance(_target.position, transform.position) <= meleeDistance * 2)
        {
            victim.TakeDamage(meleeDamage);
        }
    }

    public void AnimDead()
    {
        Destroy(gameObject);
    }

    public void AnimFinishAnimation()
    {
        _finishAnimation = true;
    }
    #endregion

    #region Normal Functions
    public override void Move()
    {
        GoTo(_target.transform.position);
    }

    public void GoTo(Vector3 destination)
    {
        _navCR = StartCoroutine(Navigate(destination));
    }

    private void OnReachDestinationStopMoving(Node arg2, bool arg3)
    {
        rotateInPath = false;
        hasRun = false;
        GetGOAPPlaning();
    }

    public void StopMoving()
    {
        if (_navCR != null) StopCoroutine(_navCR);
        _vel = Vector3.zero;
    }


    private void OnZombunnyDeath()
    {
        _isDeath = true;
    }

    public void Rotate()
    {
        transform.LookAt(_target);
        /*
        var vec = _target.transform.position - transform.position;
        vec.Normalize();
        Quaternion quack = Quaternion.LookRotation(vec);
        quack.x = 0;
        quack.z = 0;
        _rg.MoveRotation(Quaternion.RotateTowards(transform.rotation, quack, rotationSpeed * Time.fixedDeltaTime));    
        */
    }

    private void SetMove(bool value)
    {
        _canMove = value;
    }

    private void SetTarget(Transform value)
    {
        _target = value;
    }

    public override void Destroyed()
    {
        base.Destroyed();
        _anim.SetTrigger(StringTagManager.animDead);
    }

    private bool IsPlayerAlive()
    {
        return PlayerHealth.Instance.currentHealth > 0; 
    }

    private bool IsInMeleeRange()
    {
        return _target != null ? Vector3.Distance(_target.position, transform.position) <= meleeDistance : false;
    }

    private bool IsInShootingRange()
    {
        bool targetInSight = true;
        Vector3 dirToTarget = _target.transform.position - transform.position;
        RaycastHit[] rch;
        rch = Physics.RaycastAll(transform.position, dirToTarget, enemyShooting.shootDistance);
        for (int i = 0; i < rch.Length; i++)
        {
            RaycastHit hit = rch[i];
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(StringTagManager.maskShootable))
                targetInSight = false;
        }

        return _target != null ? Vector3.Distance(_target.position, transform.position) <= enemyShooting.shootDistance && targetInSight: false;
    }

    private bool IsInPickUpRange()
    {
        return _target != null ? Vector3.Distance(_target.position, transform.position) <= 1 : false;
    }

    private bool IsBatteryNearby()
    {
        return _closestBattery != null;
    }

    private float GetCostForBatteryNearby()
    {
        var duracellBattery = Physics.OverlapSphere(transform.position, pickUpDistance, batteryLayerMask, QueryTriggerInteraction.Collide);
        if (duracellBattery.Length > 0)
        {
            _closestBattery = duracellBattery[0].transform;
            return Vector3.Distance(_closestBattery.position, transform.position);
        }
        else
        {
            _closestBattery = null;
            return 99;
        }
    }

    private bool IsMedikitNearby()
    {
        return _closestMediKit != null;
    }

    private float GetCostForMedikitNearby()
    {
        var mediKitBoxes = Physics.OverlapSphere(transform.position, pickUpDistance, mediKitLayerMask, QueryTriggerInteraction.Collide);
        if (mediKitBoxes.Length > 0)
        {
            _closestMediKit = mediKitBoxes[0].transform;
            return Vector3.Distance(_closestMediKit.position, transform.position);
        }
        else
        {
            _closestMediKit = null;
            return 99;
        }
    }
    #endregion

    #region GOAP Interfaces
    IEnumerator ExecuteAction(Func<bool> action)
    {
        while (!action()) yield return null;
        GetGOAPPlaning();
    }

    private bool Rush()
    {
        Action initial = () => {
            Move();
            _movementSpeed = rushingSpeed;
            _anim.speed = 1.7f;
            SetTarget(_player);
            SetMove(true);
        };

        RunOnceAction(initial);

        if(!IsInMeleeRange())
        {
            return false;
        }
        else{
            _movementSpeed = movementSpeed;
            _anim.speed = 1;
            hasRun = false;
            return true;
        }
    }

    IEnumerator Forward()
    {
        _movementSpeed = movementSpeed;
        SetTarget(_player);
        SetMove(true);
        while (true)
        {
            if (IsInMeleeRange()) break;
            yield return null;
        }
        GoGoGOAP();
    }

    private bool Melee()
    {
        Action initial = () => {
            SetMove(false);
            Rotate();
            _finishAnimation = false;
            _anim.SetTrigger(StringTagManager.animAttack);
        };
        Debug.LogError(_finishAnimation);
        RunOnceAction(initial);

        if (_finishAnimation)
        {
            hasRun = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator GetHealed()
    {
        var mediKitBoxes = Physics.OverlapSphere(transform.position, pickUpDistance, mediKitLayerMask, QueryTriggerInteraction.Collide);
        mediKitBoxes.OrderBy(x => Vector3.Distance(x.transform.position, transform.position));

        if (mediKitBoxes.Length > 0)
        {
            _closestMediKit = mediKitBoxes[0].transform;
            var triggerSet = false;
            SetTarget(_closestMediKit);
            SetMove(true);
            _finishAnimation = false;

            while (true)
            {
                if (IsInPickUpRange() && !_finishAnimation && !triggerSet)
                {
                    SetMove(false);
                    _anim.SetTrigger(StringTagManager.animPickUp);
                    triggerSet = true;
                }
                else if (_finishAnimation)
                {
                    _anim.ResetTrigger(StringTagManager.animPickUp);
                    break;
                }
                else if (_closestMediKit == null)
                {
                    GetGOAPPlaning();
                }
                yield return null;
            }
        }
        else
        {
            GetGOAPPlaning();
        }


        GoGoGOAP();
    }

    IEnumerator GrabBattery()
    {
        var batteries = BatteryManager.Instance.batteries;
        batteries.OrderBy(x => Vector3.Distance(x.transform.position, transform.position));

        if (batteries.Count > 0)
        {
            _closestBattery = batteries[0].transform;
            var triggerSet = false;
            SetTarget(_closestBattery);
            SetMove(true);
            Move();
            _finishAnimation = false;

            while (true)
            {
                if (IsInPickUpRange() && !_finishAnimation && !triggerSet)
                {
                    SetMove(false);
                    StopMoving();
                    AnimGrab();
                    _anim.SetTrigger(StringTagManager.animPickUp);
                    triggerSet = true;
                }
                else if (_finishAnimation)
                {
                    _anim.ResetTrigger(StringTagManager.animPickUp);
                    break;
                }
                else if (_closestBattery == null)
                {
                    GetGOAPPlaning();
                }
                yield return null;
            }
        }
        else
        {
            GetGOAPPlaning();
        }

        GoGoGOAP();
    }

    IEnumerator Reload()
    {
        SetMove(false);
        Rotate();
        _finishAnimation = false;
        _anim.SetTrigger(StringTagManager.animReload);

        while (!_finishAnimation)
        {
            yield return null;
        }
        GoGoGOAP();
    }

    IEnumerator PositionToShoot()
    {
        SetTarget(_player);
        SetMove(true);
        Move();
        while (true)
        {
            if (IsInShootingRange()) break;
            yield return null;
        }
        GetGOAPPlaning();
    }

    IEnumerator Shoot()
    {
        if(IsInShootingRange())
        {
            SetTarget(_player);
            Rotate();
            SetMove(false);
            //_anim.SetBool(weapon.ToString(), true);
            while (battery > 0 && _clipSizeMax > 0)
            {
                yield return null;
            }
            isLaserLoaded = false;
            AnimShootLaser();
            //_anim.SetBool(weapon.ToString(), false);
        }
        GetGOAPPlaning();
    }

    private void RunOnceAction(Action f)
    {
        if (hasRun) return;
        f();
        hasRun = true;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickUpDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyShooting.shootDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeDistance);

        if (_gizmoRealTarget != null)
            Gizmos.DrawCube(_gizmoRealTarget.transform.position + Vector3.up * 1f, Vector3.one * 0.3f);
    }
}
