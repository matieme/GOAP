using GOAP;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class EntityMovement : EnemyHealth
{
    public bool isMoving { get { return _isMoving; } }
    public bool canMove { get { return _canMove; } }
    public float movementSpeed;

    protected Rigidbody _rg;
    protected float _movementSpeed;
    protected bool _isMoving;
    protected bool _canMove;
    public abstract void Move();

    protected Coroutine _navCR;
    protected Node _gizmoRealTarget;
    protected Vector3 _vel;
    protected bool rotateInPath;
    public event Action<Node, bool> OnReachDestination = delegate { };

    public override void Start()
    {
        base.Start();
        _rg = GetComponent<Rigidbody>();
        _movementSpeed = movementSpeed;
        _canMove = true;
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
        OnReachDestination(reachedDst, reachedDst == dstNode);
    }


    public void RotateInPath(Vector3 target)
    {
        transform.LookAt(target);
    }

    Vector3 FloorPos(MonoBehaviour b)
    {
        return FloorPos(b.transform.position);
    }
    Vector3 FloorPos(Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }
}
