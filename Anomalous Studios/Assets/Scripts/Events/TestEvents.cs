using UnityEngine;

public struct LevelLoaded : IEvent { public int Index; }
public struct CoinsChanged : IEvent { public int NewCoins; }
public struct HealthChanged : IEvent { public float Value; }
public struct GamePaused : IEvent { public bool IsPaused; }
