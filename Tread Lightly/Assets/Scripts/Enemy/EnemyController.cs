using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private EnemyState _state;
    [SerializeField] private Transform _homeLocation;
    [SerializeField] private float _sightDistance;
    [SerializeField] private float _timeToWaitBeforeReturningHome;
    [SerializeField] private LayerMask _terrainLayerMask;

    private const float LedgeLookAheadAmount = 0.5f;
    private const float MaxDistanceFromHomeLocation = 0.5f;

    private float _waitingCooldown = 0;

    private GameObject _player;
    private Rigidbody2D _rigidbody;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody = GetComponent<Rigidbody2D>();
        _state = EnemyState.Sleeping;
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
        {
            return;
        }

        var currentState = _state;
        if (RunBehaviour())
        {
            Debug.Log($"State Changed {currentState} => {_state}");
        }
    }

    #region Behaviours

    // Behaviours will return true if they have triggered a change in state
    private bool RunBehaviour() => _state switch
    {
        EnemyState.Sleeping => RunSleepingBehaviour(),
        EnemyState.Chasing => RunChasingBehaviour(),
        EnemyState.Searching => RunSearchingBehaviour(),
        EnemyState.Returning => RunReturningBehaviour(),
        _ => throw new System.NotImplementedException(),
    };

    private bool RunSleepingBehaviour()
    {
        if (CanHearPlayer())
        {
            WakeUp();
            return true;
        }
        return false;
    }

    private bool RunReturningBehaviour()
    {
        if (IsAtHomeLocation())
        {
            Sleep();
            return true;
        }
        MoveTowards(_homeLocation.position, _walkSpeed);
        return false;
    }

    private bool RunChasingBehaviour()
    {
        if (!CanSeePlayer())
        {
            Search();
            return true;
        }
        if (IsOnLedge(DirectionTowardsPlayer))
        {
            StopMoving();
        }
        else
        {
            MoveTowards(_player.transform.position, _chaseSpeed);
        }
        return false;
    }

    private bool RunSearchingBehaviour()
    {
        _waitingCooldown -= Time.deltaTime;
        if (_waitingCooldown <= 0)
        {
            _waitingCooldown = 0;
            ReturnToHome();
            return true;
        }
        return false;
    }

    private void WakeUp()
        => _state = CanSeePlayer() ? EnemyState.Chasing : EnemyState.Searching;
    
    private void Search()
    {
        _state = EnemyState.Searching;
        _waitingCooldown = _timeToWaitBeforeReturningHome;
    }

    private void Sleep()
        => _state = EnemyState.Sleeping;

    private void ReturnToHome()
        => _state = EnemyState.Returning;

    private bool IsAtHomeLocation()
        => (transform.position - _homeLocation.position).sqrMagnitude < MaxDistanceFromHomeLocation;

    #endregion

    #region Detection

    private bool CanSeePlayer()
    {
        var directionVector = _player.transform.position - transform.position;
        var distanceToCast = Mathf.Min(_sightDistance, directionVector.magnitude);
        var hit = Physics2D.Raycast(transform.position, directionVector, distanceToCast, _terrainLayerMask);
        return !hit;
    }

    private bool CanHearPlayer()
        => SoundEmitter.CanListenerHearASound(transform.position);
    
    private bool IsOnLedge(int lookingDirection)
    {
        var positionInFront = new Vector2(transform.position.x + lookingDirection * LedgeLookAheadAmount, transform.position.y);
        var hit = Physics2D.Raycast(positionInFront, Vector2.down, 2f, _terrainLayerMask);
        Debug.DrawRay(positionInFront, Vector2.down, Color.black);
        return !hit;
    }

    #endregion

    #region Utility Functions

    private void MoveTowards(Vector3 target, float speed)
    {
        var xVelocity = Mathf.Clamp(target.x - transform.position.x, -speed, speed);
        _rigidbody.velocity = new Vector2(xVelocity, _rigidbody.velocity.y);
    }

    private void StopMoving()
        =>  _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);

    private int DirectionTowardsPlayer
        => (int)Mathf.Sign(_player.transform.position.x - transform.position.x);

    #endregion
}
