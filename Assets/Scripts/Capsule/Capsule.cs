using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Capsule : MonoBehaviour
{
    [Header("캡슐 데이터 목록 (Inspector에서 여러 개 등록)")]
    public List<CapsuleData> capsuleDataList = new List<CapsuleData>();

    [Header("현재 뽑힌 캡슐 데이터 (자동 지정됨)")]
    public CapsuleData capsuleData;

    [Header("UI 연결")]
    public Image capsuleImage;

    public float MaxDurability { get; private set; }
    public float CurrentDurability { get; private set; }
    public bool IsDestroyed => CurrentDurability <= 0;

    private Sprite initialSprite; // 시작할 때 지정된 캡슐 기본 스프라이트

    public void InitializeCapsule()
    {
        if (capsuleDataList == null || capsuleDataList.Count == 0)
        {
            Debug.LogError("[Capsule 에러] capsuleDataList가 비어있습니다!");
            return;
        }

        int randomIndex = Random.Range(0, capsuleDataList.Count);
        capsuleData = capsuleDataList[randomIndex];

        if (capsuleData == null) return;

        MaxDurability = Random.Range(capsuleData.minDurability, capsuleData.maxDurability);
        if (MaxDurability <= 0) MaxDurability = 50f;
        CurrentDurability = MaxDurability;

        if (capsuleImage != null)
        {
            capsuleImage.enabled = true;

            Sprite randomSprite = capsuleData.GetRandomSprite();
            if (randomSprite != null)
            {
                initialSprite = randomSprite;
                capsuleImage.sprite = initialSprite;
            }
        }
    }

    // 타격 시 데미지 처리 및 균열 이미지 갱신
    public bool TakeDamage(float amount)
    {
        if (IsDestroyed) return true;

        CurrentDurability -= amount;
        if (CurrentDurability <= 0)
        {
            CurrentDurability = 0;
            return true; // 파괴 성공!
        }

        // 균열 이미지 갱신 연출 실행
        UpdateCrackVisual();

        return false;
    }

    // 내구도 비율에 따른 캡슐 균열 스프라이트 변경 함수
    private void UpdateCrackVisual()
    {
        if (capsuleImage == null || capsuleData == null || capsuleData.crackSprites == null || capsuleData.crackSprites.Count == 0)
            return;

        float damageRatio = 1f - (CurrentDurability / MaxDurability);

        int count = capsuleData.crackSprites.Count;
        int crackIndex = Mathf.FloorToInt(damageRatio * count);

        crackIndex = Mathf.Clamp(crackIndex, 0, count - 1);

        if (capsuleData.crackSprites[crackIndex] != null)
        {
            capsuleImage.sprite = capsuleData.crackSprites[crackIndex];
        }
    }
}