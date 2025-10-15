using UnityEngine;


public class EventTester : MonoBehaviour
{
    [Header("Initial State")]
    [SerializeField] private int _startLevelIndex = 0;
    [SerializeField] private int _startCoins = 0;
    [SerializeField] private float _startHealth = 100f;

    private int _levelIndex;
    private int _coins;
    private float _health;
    private bool _isPaused;

    private void Awake()
    {
        _levelIndex = _startLevelIndex;
        _coins = _startCoins;
        _health = _startHealth;
        _isPaused = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _levelIndex++;
            EventBus<LevelLoadedTEST>.Raise(new LevelLoadedTEST { Index = _levelIndex });
            Debug.Log($"Raised LevelLoaded : Index={_levelIndex}");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            _coins += 5;
            EventBus<CoinsChanged>.Raise(new CoinsChanged { NewCoins = _coins });
            Debug.Log($"Raised CoinsChanged : NewCoins={_coins}");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            EventBus<GamePaused>.RaiseSticky(new GamePaused { IsPaused = _isPaused });
            Debug.Log($"Raised Sticky GamePaused : IsPaused={_isPaused}");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            _health = Mathf.Max(0f, _health - 10f);
            EventBus<HealthChanged>.RaiseScoped(gameObject, new HealthChanged { Value = _health });
            Debug.Log($" Raised scoped HealthChanged (non-sticky) : {_health}");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            _health = Mathf.Clamp(_health + 7.5f, 0f, 100f);
            EventBus<HealthChanged>.RaiseStickyScoped(gameObject, new HealthChanged { Value = _health });
            Debug.Log($"Raised stick scoped HealthChanged : {_health}");
        }
    }
}
