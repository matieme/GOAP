using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;
using FP;
using System;
using System.Linq;

public class ZombunnyEnemy : EntityMovement
{
    public GameObject shootParticle;
    public GameObject barrel;
    public bool isLaserLoaded;
    public float rushingSpeed;
    public float rotationSpeed;
    public float meleeDamage;
    public float meleeDistance;

    public float pickUpDistance;
    public int clipSize;
    public LayerMask batteryLayerMask;
    public LayerMask mediKitLayerMask;

    [SerializeField]
    private EnemyShooting enemyShooting;

    private Queue<string> _queueActions = new Queue<string>();
    private Transform _target;
    private Transform _player;
    private Transform _closestBattery;
    private Transform _closestMediKit;
    private Animator _anim;
    private bool _finishAnimation;
    private int _clipSizeMax;

    Coroutine _navCR;
    Node _gizmoRealTarget;
    Vector3 _vel;
    bool rotateInPath;
    public event Action<ZombunnyEnemy, Node, bool> OnReachDestination = delegate { };

    public override void Start()
    {
        base.Start();
        _anim = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag(StringTagManager.tagPlayer).transform;
        _clipSizeMax = clipSize;
        SetTarget(_player);
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

        transform.position += _vel * Time.deltaTime * movementSpeed;
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
                .SetCost(10));

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
            { GOAPKeyEnum.medikitNearby , IsMedikitNearby() }
        };
        var goal = new Map<GOAPKeyEnum, Func<object, bool>>() {
            { GOAPKeyEnum.isAttacking , x => (bool) x == true}
        };

        var plan = new GOAPPlan(actions, initialState, goal, heuristic);

        //var count = 0;
        foreach (var action in plan.Execute())
        {
            //count++;
            _queueActions.Enqueue(action.Name.ToString());
            //Debug.Log(count + ": " + action.Name.ToString());
        }
        GoGoGOAP();
    }

    private void GoGoGOAP()
    {
        foreach (string item in _queueActions)
        {
            Debug.LogError(item);
        }

        Debug.LogError("-----------------------------");

        if (_queueActions.Count > 0) StartCoroutine(_queueActions.Dequeue());
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

    public void AnimPunch()
    {
        var victim = _target.GetComponent<EntityLife>();

        if (victim != null && Vector3.Distance(_target.position, transform.position) <= meleeDistance * 2)
        {
            victim.Damaged(meleeDamage);
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

    private void OnReachDestinationStopMoving(ZombunnyEnemy arg1, Node arg2, bool arg3)
    {
        rotateInPath = false;
        Debug.LogError("Hookear otro estado");
        //StopMoving();
        GoGoGOAP();
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

    protected virtual IEnumerator Navigate(Vector3 destination)
    {
        Node srcNode = Navigation.Instance.NearestTo(transform.position);
        Node dstNode = Navigation.Instance.NearestTo(destination);

        _gizmoRealTarget = dstNode;
        Node reachedDst = srcNode;

        if (srcNode != dstNode)
        {
            rotateInPath = true;
            var path = GraphOperations.AStar(srcNode, dstNode).ToList();

            if (path != null)
            {
                foreach (var next in path.Select(w => FloorPos(w.Content)))
                {
                    RotateInPath(next);
                    while ((next - FloorPos(this)).sqrMagnitude >= 0.05f)
                    {
                        _vel = (next - FloorPos(this)).normalized;
                        yield return null;
                    }
                }
            }
            reachedDst = path.Last().Content;
        }

        if (reachedDst == dstNode)
        {
            rotateInPath = false;
            _vel = (FloorPos(destination) - FloorPos(this)).normalized;
            yield return new WaitUntil(() => (FloorPos(destination) - FloorPos(this)).sqrMagnitude < 0.05f);
        }

        _vel = Vector3.zero;
        OnReachDestination(this, reachedDst, reachedDst == dstNode);
    }

    Vector3 FloorPos(MonoBehaviour b)
    {
        return FloorPos(b.transform.position);
    }
    Vector3 FloorPos(Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
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

    public void RotateInPath(Vector3 target)
    {
        transform.LookAt(target);
    }

    private void SetMove(bool value)
    {
        _canMove = value;
        //_anim.SetBool(StringTagManager.animWalking, value);
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

    private bool IsInMeleeRange()
    {
        return _target != null ? Vector3.Distance(_target.position, transform.position) <= meleeDistance : false;
    }

    private bool IsInShootingRange()
    {
        return _target != null ? Vector3.Distance(_target.position, transform.position) <= enemyShooting.shootDistance : false;
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
    IEnumerator Rush()
    {
        _anim.SetBool(StringTagManager.animRunning, true);
        Move();
        _movementSpeed = rushingSpeed;
        SetTarget(_player);
        SetMove(true);
        while (true)
        {
            if (IsInMeleeRange()) break;
            yield return null;
        }
        _movementSpeed = movementSpeed;
        _anim.SetBool(StringTagManager.animRunning, false);
        GoGoGOAP();
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

    IEnumerator Melee()
    {
        SetMove(false);
        _finishAnimation = false;
        _anim.SetTrigger(StringTagManager.animAttack);
        while (!_finishAnimation)
        {
            yield return null;
        }
        GoGoGOAP();
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
        GoGoGOAP();
    }

    IEnumerator Shoot()
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
        GoGoGOAP();
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
