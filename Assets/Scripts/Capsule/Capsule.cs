using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Capsule : MonoBehaviour
{
    [Header("캡슐 데이터")]
    public List<CapsuleData> capsuleDataList = new List<CapsuleData>();

    [Header("현재 뽑힌 캡슐 데이터 (자동 지정됨)")]
    public CapsuleData capsuleData;

    [Header("UI 연결")]
    public Image capsuleImage;

    public float CurrentDurability { get; private set; }
    public float MaxDurability { get; private set; }
    public bool IsDestroyed { get; private set; }

    public void InitializeCapsule()
    {
        if (capsuleDataList == null || capsuleDataList.Count == 0)
        {
            Debug.LogError("[Capsule 에러] capsuleDataList가 비어있습니다! Inspector에서 CapsuleData SO들을 넣어주세요.");
            return;
        }
        int randomIndex = Random.Range(0, capsuleDataList.Count);
        capsuleData = capsuleDataList[randomIndex];

        if (capsuleData == null)
        {
            Debug.LogError("[Capsule 에러] 선택된 capsuleData가 null입니다.");
            return;
        }

        MaxDurability = Random.Range(capsuleData.minDurability, capsuleData.maxDurability);


        if (MaxDurability <= 0) MaxDurability = 50f;

        CurrentDurability = MaxDurability;

        if (capsuleImage != null)
        {
            capsuleImage.enabled = true; 

            Sprite randomSprite = capsuleData.GetRandomSprite();
            if (randomSprite != null)
            {
                capsuleImage.sprite = randomSprite;
            }
        }

        Debug.Log($"[Capsule] 캡슐 생성 완료! (설정된 내구도: {CurrentDurability:F0}, 의상: {capsuleData.costumeData?.costumeName ?? "없음"})");
    }

    public bool TakeDamage(float amount)
    {
        if (IsDestroyed) return true;

        CurrentDurability -= amount;
        if (CurrentDurability <= 0)
        {
            CurrentDurability = 0;
            return true; 
        }

        return false;
    }
}