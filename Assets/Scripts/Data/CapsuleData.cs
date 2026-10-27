using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCapsuleData", menuName = "IdolClicker/Capsule Data")]
public class CapsuleData : ScriptableObject
{
    [Header("캡슐 기본 정보")]
    public string capsuleName = "기본 캡슐";

    [Header("내구도 설정 (랜덤 범위)")]
    public float minDurability = 50f;
    public float maxDurability = 150f;

    [Header("드롭 의상 정보")]
    public CostumeData costumeData;

    [Header("캡슐 기본 이미지 후보")]
    public List<Sprite> spriteCandidates = new List<Sprite>();

    [Header("캡슐 균열/깨짐 연출 이미지 목록")]
    [Tooltip("손상 정도에 따라 교체될 균열 스프라이트 목록입니다.")]
    public List<Sprite> crackSprites = new List<Sprite>();

    public Sprite GetRandomSprite()
    {
        if (spriteCandidates == null || spriteCandidates.Count == 0) return null;
        int randomIndex = Random.Range(0, spriteCandidates.Count);
        return spriteCandidates[randomIndex];
    }
}