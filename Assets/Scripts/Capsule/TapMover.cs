using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapMover : MonoBehaviour, IPointerDownHandler
{
    [Header("연결 요소")]
    public Capsule targetCapsule;          // 타격할 캡슐
    public TextMeshProUGUI durabilityText; // 내구도 표시 UI

    [Header("타격 설정")]
    public float damagePerTap = 10f;       // 1회 타격당 피해량

    void Start()
    {
        UpdateUI();
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (targetCapsule == null || targetCapsule.IsDestroyed) return;

        targetCapsule.TakeDamage(damagePerTap);


        UpdateUI();
    }


    public void UpdateUI()
    {
        if (durabilityText != null && targetCapsule != null)
        {
            durabilityText.text = $"내구도: {targetCapsule.CurrentDurability} / {targetCapsule.MaxDurability}";
        }
    }
}