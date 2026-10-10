using UnityEngine;

[CreateAssetMenu(fileName = "NewCapsuleData", menuName = "IdolClicker/Capsule Data")]
public class CapsuleData : ScriptableObject
{
    [Header("캡슐 기본 정보")]
    public string capsuleName = "기본 캡슐";

    [Header("내구도 설정 (랜덤 범위)")]
    public float minDurability = 50f;  // 최소 내구도
    public float maxDurability = 150f; // 최대 내구도
}