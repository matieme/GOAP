using UnityEngine;

public abstract class EntityMovement : EntityLife
{
    public bool isMoving { get { return _isMoving; } }
    public bool canMove { get { return _canMove; } }
    public float movementSpeed;

    protected Rigidbody _rg;
    protected float _movementSpeed;
    protected bool _isMoving;
    protected bool _canMove;
    public abstract void Move();

    public override void Start()
    {
        base.Start();
        _rg = GetComponent<Rigidbody>();
        _movementSpeed = movementSpeed;
        _canMove = true;
    }
}
