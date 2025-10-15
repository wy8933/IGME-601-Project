using UnityEngine;

public class TestEventListener : MonoBehaviour
{
    private EventBinding<LevelLoadedTEST> _levelLoadedHigh;
    private EventBinding<LevelLoadedTEST> _levelLoadedLow;
    private EventBinding<CoinsChanged> _coinsChanged;
    private EventBinding<GamePaused> _pausedSticky;
    private EventBinding<HealthChanged> _healthScoped;

    private void OnEnable()
    {
        _levelLoadedHigh = new EventBinding<LevelLoadedTEST>(OnLevelLoadedHigh, pr: 100);
        EventBus<LevelLoadedTEST>.Register(_levelLoadedHigh);

        _levelLoadedLow = new EventBinding<LevelLoadedTEST>(OnLevelLoadedLow, pr: -10);
        EventBus<LevelLoadedTEST>.Register(_levelLoadedLow);

        _coinsChanged = new EventBinding<CoinsChanged>(OnCoinsChanged);
        EventBus<CoinsChanged>.Register(_coinsChanged);

        _pausedSticky = new EventBinding<GamePaused>(OnPausedChanged);
        EventBus<GamePaused>.RegisterSticky(_pausedSticky, global: true);

        _healthScoped = new EventBinding<HealthChanged>(OnHealthChangedScoped);
        EventBus<HealthChanged>.Register(_healthScoped, scope: gameObject);
    }

    private void OnDisable()
    {
        EventBus<LevelLoadedTEST>.DeRegister(_levelLoadedHigh);
        EventBus<LevelLoadedTEST>.DeRegister(_levelLoadedLow);
        EventBus<CoinsChanged>.DeRegister(_coinsChanged);
        EventBus<GamePaused>.DeRegister(_pausedSticky);

        EventBus<HealthChanged>.DeRegister(_healthScoped, scope: gameObject);
    }

    private void OnLevelLoadedHigh(LevelLoadedTEST e)
    {
        Debug.Log($"(High) LevelLoadedTEST Index={e.Index}");
    }

    private void OnLevelLoadedLow(LevelLoadedTEST e)
    {
        Debug.Log($"(Low ) LevelLoadedTEST Index={e.Index}");
    }

    private void OnCoinsChanged(CoinsChanged e)
    {
        Debug.Log($"CoinsChanged : {e.NewCoins}");
    }

    private void OnPausedChanged(GamePaused e)
    {
        Debug.Log($"GamePaused : IsPaused={e.IsPaused}");
    }

    private void OnHealthChangedScoped(HealthChanged e)
    {
        Debug.Log($"Scoped Health for {name} : {e.Value}");
    }
}
