using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Controller_Agent : MonoBehaviour
{
    protected NavMeshAgent _agent;
    Animator _animator;
    [SerializeField] Vector3 _targetPosition;
    [SerializeField] GameObject _targetGO;
    float _speed;

    float _followDistance;
    WanderData _wanderData;
    bool _canMove = false;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        SubscribeToEvents();
    }

    protected virtual void SubscribeToEvents()
    {

    }

    public void ToggleAgent(bool toggle)
    {
        _agent.enabled = toggle;
        this.enabled = toggle;
    }

    public void SetAgentDetails(Vector3? targetPosition = null, GameObject targetGO = null, float speed = 1, float followDistance = 0, WanderData wanderData = null)
    {
        _targetPosition = targetPosition ?? transform.position;
        _targetGO = targetGO;
        _speed = speed;
        _followDistance = followDistance;
        wanderData = _wanderData;
        _canMove = true;
    }

    protected virtual void Update()
    {
        if (!_canMove) return;

        if (_wanderData != null) Wander();
        else Follow();
        AnimationAndDirection();
    }

    void Follow()
    {
        if (_targetGO != null) _targetPosition = _targetGO.transform.position;
        else if (_targetPosition == transform.position) return;

        if (Vector2.Distance(transform.position, _targetPosition) > _followDistance) { _agent.isStopped = false; _agent.SetDestination(_targetPosition); }
        else _agent.isStopped = true;
    }

    void Wander()
    {
        if (!_wanderData.IsWandering && !_wanderData.IsWanderWaiting)
        {
            Bounds wanderBounds = _wanderData.WanderRegion.bounds;

            float x = UnityEngine.Random.Range(wanderBounds.min.x, wanderBounds.max.x);
            float y = UnityEngine.Random.Range(wanderBounds.min.y, wanderBounds.max.y);

            _wanderData.WanderTargetPosition = new Vector3(x, y, transform.position.z);

            _agent.SetDestination(_wanderData.WanderTargetPosition);
            _agent.speed = _wanderData.WanderSpeed;

            _wanderData.IsWandering = true;
        }

        if (_wanderData.IsWandering && !_wanderData.IsWanderingCoroutineRunning)
        {
            _wanderData.IsWanderingCoroutineRunning = true;
            StartCoroutine(WanderCoroutine());
        }
    }

    IEnumerator WanderCoroutine()
    {
        yield return new WaitForSeconds(_wanderData.GetRandomWanderTime());
        StartCoroutine(WaitAtWanderPoint());
        _wanderData.IsWanderingCoroutineRunning = false;
    }

    IEnumerator WaitAtWanderPoint()
    {
        _wanderData.IsWandering = false;
        _wanderData.IsWanderWaiting = true;
        yield return new WaitForSeconds(_wanderData.GetRandomWanderWaitTime());
        _wanderData.IsWanderWaiting = false;
    }

    void AnimationAndDirection()
    {
        if (_animator.runtimeAnimatorController != null) _animator.SetFloat("Speed", _agent.velocity.magnitude);

        if (_agent.velocity != Vector3.zero)
        {
            transform.localScale = new Vector3(Mathf.Sign(_agent.velocity.x), transform.localScale.y, transform.localScale.z);
            
            //ActorScripts.Actor_VFX.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
    }

    public void ResetAgent()
    {
        _agent.isStopped = true;
        _speed = 0;
        _targetPosition = transform.position;
        _followDistance = 0;
        _wanderData = null;
        _canMove = false;
    }
}

[System.Serializable]
public class WanderData
{
    public Vector3 WanderTargetPosition;
    public BoxCollider2D WanderRegion;
    public float WanderSpeed;
    public float MinWanderTime;
    public float MaxWanderTime;
    public float MinWanderWaitTime;
    public float MaxWanderWaitTime;
    public bool IsWandering = false;
    public bool IsWanderingCoroutineRunning = false;
    public bool IsWanderWaiting = false;

    public WanderData(BoxCollider2D wanderRegion, float wanderSpeed, float minWanderTime, float maxWanderTime, float minWanderWaitTime, float maxWanderWaitTime)
    {
        WanderTargetPosition = new Vector3(0, 0, 0);
        WanderRegion = wanderRegion;
        WanderSpeed = wanderSpeed;
        MinWanderTime = minWanderTime;
        MaxWanderTime = maxWanderTime;
        MinWanderWaitTime = minWanderWaitTime;
        MaxWanderWaitTime = maxWanderWaitTime;
        IsWandering = false;
        IsWanderingCoroutineRunning = false;
        IsWanderWaiting = false;
    }

    public float GetRandomWanderTime()
    {
        return UnityEngine.Random.Range(MinWanderTime, MaxWanderTime);
    }

    public float GetRandomWanderWaitTime()
    {
        return UnityEngine.Random.Range(MinWanderWaitTime, MaxWanderWaitTime);
    }
}
