using UnityEngine;
using System.Collections.Generic;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance;

    [Header("全局参数")]
    public float highlightDistance = 5f;   // 高亮距离
    public Material outlineMat;            // 高亮用的Shader Graph材质
    public LayerMask occlusionMask;        // 遮挡检测层

    [Header("渐变控制")]
    public float fadeSpeed = 5f;           // 渐变速度
    public float maxGlow = 3f;             // 最大发光强度

    private Transform player;
    private static List<HighlightTarget> targets = new List<HighlightTarget>();

    void Awake()
    {
        Instance = this;
        player = Camera.main.transform;
    }

    void Update()
    {
        foreach (var target in targets)
        {
            if (target == null || target.outlineMatInstance == null) continue;

            float dist = Vector3.Distance(player.position, target.transform.position);
            bool inRange = dist < highlightDistance;
            bool visible = IsVisible(target);

            // 目标Alpha（0 = 隐藏, 1 = 高亮）
            float targetAlpha = (inRange && visible) ? 1f : 0f;

            // 当前值
            float currentAlpha = target.outlineMatInstance.GetFloat("_Outline_Alpha");

            // 插值平滑
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

            // 更新 Shader 属性
            target.outlineMatInstance.SetFloat("_Outline_Alpha", newAlpha);
            target.outlineMatInstance.SetFloat("_OutlineGlowIntensity", newAlpha * maxGlow);
        }
    }

    bool IsVisible(HighlightTarget target)
    {
        Vector3 dir = (target.transform.position - player.position).normalized;
        float dist = Vector3.Distance(player.position, target.transform.position);

        if (Physics.Raycast(player.position, dir, out RaycastHit hit, dist, occlusionMask))
        {
            if (hit.collider.gameObject != target.gameObject)
                return false; // 有遮挡
        }
        return true;
    }

    public static void Register(HighlightTarget target)
    {
        if (!targets.Contains(target))
            targets.Add(target);
    }

    public static void Unregister(HighlightTarget target)
    {
        if (targets.Contains(target))
            targets.Remove(target);
    }
}
