using UnityEngine;

public class Capsule : MonoBehaviour
{
    [Header("ƒ∏Ω∂ µ•¿Ã≈Õ")]
    public CapsuleData capsuleData;

    public float CurrentDurability { get; private set; }
    public float MaxDurability { get; private set; }
    public bool IsDestroyed { get; private set; }

    void Awake()
    {
        InitializeCapsule();
    }


    public void InitializeCapsule()
    {
        if (capsuleData != null)
        {

            MaxDurability = Random.Range(capsuleData.minDurability, capsuleData.maxDurability);

            MaxDurability = Mathf.Round(MaxDurability);
        }
        else
        {
            MaxDurability = 100f;
        }

        CurrentDurability = MaxDurability;
        IsDestroyed = false;
    }

    public bool TakeDamage(float damage)
    {
        if (IsDestroyed) return false;

        CurrentDurability -= damage;

        if (CurrentDurability <= 0f)
        {
            CurrentDurability = 0f;
            IsDestroyed = true;
            Debug.Log($"{capsuleData?.capsuleName ?? "ƒ∏Ω∂"}¿Ã(∞°) ∆ƒ±´µ«æ˙Ω¿¥œ¥Ÿ!");
            return true;
        }

        return false;
    }
}