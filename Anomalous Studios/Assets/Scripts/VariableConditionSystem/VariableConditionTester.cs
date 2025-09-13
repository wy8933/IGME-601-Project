using System.Collections.Generic;
using UnityEngine;

public class VariableConditionTester : MonoBehaviour
{
    public ConditionBlackboardSO blackboardAsset;

    void Awake()
    {
        Setup();
    }

    void Start()
    {
        RunAllTests();
    }

    void Setup()
    {
        if (VariableConditionManager.Instance == null)
        {
            var gameobejct = new GameObject("VariableConditionManager");
            gameobejct.AddComponent<VariableConditionManager>();
        }

        if (blackboardAsset == null)
            blackboardAsset = ScriptableObject.CreateInstance<ConditionBlackboardSO>();

        VariableConditionManager.Instance.conditionBlackboard = blackboardAsset;
    }

    public void RunAllTests()
    {
        Setup();
        int passed = 0, failed = 0;

        VariableConditionManager.Instance.Set("TestKey1", "1");
        Test(VariableConditionManager.Instance.TryGet("TestKey1", out var v1) && v1 == "1", "Set/TryGet string", ref passed, ref failed);

        Test(VariableConditionManager.Instance.Check(new VariableCondition("TestKey1", VariableCompareType.Equals, "1")), "Equals check", ref passed, ref failed);
        Test(VariableConditionManager.Instance.Check(new VariableCondition("TestKey1", VariableCompareType.NotEquals, "0")), "NotEquals check", ref passed, ref failed);

        VariableConditionManager.Instance.Set("TestKey2", "3.5");
        Test(VariableConditionManager.Instance.Check(new VariableCondition("TestKey2", VariableCompareType.GreaterThan, "2")), "GreaterThan numeric", ref passed, ref failed);
        Test(!VariableConditionManager.Instance.Check(new VariableCondition("TestKey2", VariableCompareType.LessThan, "2")), "LessThan negative case", ref passed, ref failed);

        var all = new List<VariableCondition> {
            new("TestKey1", VariableCompareType.Equals, "1"),
            new("TestKey2", VariableCompareType.GreaterThan, "3")
        };
        Test(VariableConditionManager.Instance.CheckAll(all), "CheckAll AND", ref passed, ref failed);

        Test(!VariableConditionManager.Instance.TryGet("MissingKey", out _), "TryGet missing key", ref passed, ref failed);
        Test(!VariableConditionManager.Instance.Check(new VariableCondition("MissingKey", VariableCompareType.Equals, "x")), "Check missing key fails", ref passed, ref failed);

        VariableConditionManager.Instance.ClearAll();
        Test(!VariableConditionManager.Instance.TryGet("TestKey1", out _), "ClearAll removes keys", ref passed, ref failed);

        Debug.Log($"Variable Condition Tester Done. Passed: {passed}, Failed: {failed}");
    }

    void Test(bool condition, string name, ref int passed, ref int failed)
    {
        if (condition) { Debug.Log($"PASS - {name}"); passed++; }
        else { Debug.Log($"FAIL - {name}"); failed++; }
    }
}
