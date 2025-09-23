using UnityEngine;
using System.Collections.Generic;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance;

    [Header("ȫ�ֲ���")]
    public float highlightDistance = 5f;   // ��������
    public Material outlineMat;            // �����õ�Shader Graph����
    public LayerMask occlusionMask;        // �ڵ�����

    [Header("�������")]
    public float fadeSpeed = 5f;           // �����ٶ�
    public float maxGlow = 3f;             // ��󷢹�ǿ��

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

            // Ŀ��Alpha��0 = ����, 1 = ������
            float targetAlpha = (inRange && visible) ? 1f : 0f;

            // ��ǰֵ
            float currentAlpha = target.outlineMatInstance.GetFloat("_Outline_Alpha");

            // ��ֵƽ��
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

            // ���� Shader ����
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
                return false; // ���ڵ�
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
