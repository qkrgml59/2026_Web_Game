using System.Collections;
using UnityEngine;
using TMPro;

public class AutoRevenueManager : MonoBehaviour
{
    [Header("수익 기본 설정")]
    public float baseRevenue = 10f;          // 기본 수익 (매력도 0 기준)
    public float payInterval = 1.0f;         // 지급 간격 (1초)
    public float charmMultiplierCoeff = 0.01f; // 매력도 1당 1% 증가 (0.01)

    [Header("수익 획득 연출 UI")]
    public TextMeshProUGUI revenuePopText;

    void Start()
    {
        StartCoroutine(AutoRevenueRoutine());
    }

    private IEnumerator AutoRevenueRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(payInterval);

            // 1. 현재 총 매력도 확인
            int totalCharm = CharacterEquipManager.Instance != null ? CharacterEquipManager.Instance.CalculateTotalCharm() : 0;

            // 2. 최종 수익 공식: 기본수익 * (1 + 총매력도 * 0.01)
            float multiplier = 1f + (totalCharm * charmMultiplierCoeff);
            int finalRevenue = Mathf.Max(1, Mathf.FloorToInt(baseRevenue * multiplier)); // int 형변환 및 최소값 1 보장

            // 3. 자금 지급
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddGold(finalRevenue);
            }

            // 4. 팝업 연출
            if (revenuePopText != null)
            {
                StartCoroutine(ShowRevenuePop(finalRevenue));
            }
        }
    }

    private IEnumerator ShowRevenuePop(int amount)
    {
        revenuePopText.text = $"+{amount:N0}";
        revenuePopText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        revenuePopText.gameObject.SetActive(false);
    }
}