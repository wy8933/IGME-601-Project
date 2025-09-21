using UnityEngine;

public class TestEventListener : MonoBehaviour
{
    private EventBinding<LevelLoaded> _levelLoadedHigh;
    private EventBinding<LevelLoaded> _levelLoadedLow;
    private EventBinding<CoinsChanged> _coinsChanged;
    private EventBinding<GamePaused> _pausedSticky;
    private EventBinding<HealthChanged> _healthScoped;

    private void OnEnable()
    {
        _levelLoadedHigh = new EventBinding<LevelLoaded>(OnLevelLoadedHigh, pr: 100);
        EventBus<LevelLoaded>.Register(_levelLoadedHigh);

        _levelLoadedLow = new EventBinding<LevelLoaded>(OnLevelLoadedLow, pr: -10);
        EventBus<LevelLoaded>.Register(_levelLoadedLow);

        _coinsChanged = new EventBinding<CoinsChanged>(OnCoinsChanged);
        EventBus<CoinsChanged>.Register(_coinsChanged);

        _pausedSticky = new EventBinding<GamePaused>(OnPausedChanged);
        EventBus<GamePaused>.RegisterSticky(_pausedSticky, global: true);

        _healthScoped = new EventBinding<HealthChanged>(OnHealthChangedScoped);
        EventBus<HealthChanged>.Register(_healthScoped, scope: gameObject);
    }

    private void OnDisable()
    {
        EventBus<LevelLoaded>.DeRegister(_levelLoadedHigh);
        EventBus<LevelLoaded>.DeRegister(_levelLoadedLow);
        EventBus<CoinsChanged>.DeRegister(_coinsChanged);
        EventBus<GamePaused>.DeRegister(_pausedSticky);

        EventBus<HealthChanged>.DeRegister(_healthScoped, scope: gameObject);
    }

    private void OnLevelLoadedHigh(LevelLoaded e)
    {
        Debug.Log($"(High) LevelLoaded Index={e.Index}");
    }

    private void OnLevelLoadedLow(LevelLoaded e)
    {
        Debug.Log($"(Low ) LevelLoaded Index={e.Index}");
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
