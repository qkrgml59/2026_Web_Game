using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TapMover : MonoBehaviour, IPointerDownHandler
{
    [Header("연결 요소")]
    public Capsule targetCapsule;                      
    public CapsuleAnimationController animController; 
    public TextMeshProUGUI durabilityText;
    public GameFlowManager gameFlowManager;

    [Header("타격 설정")]
    public float damagePerTap = 10f;

    public bool canTap = false;

    void Start()
    {
        if (gameFlowManager == null)
        {
            gameFlowManager = FindFirstObjectByType<GameFlowManager>();
        }

        UpdateUI();
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (!canTap) return;
        if (targetCapsule == null || targetCapsule.IsDestroyed) return;
        if (animController != null && animController.IsPlaying) return;

        bool isDestroyed = targetCapsule.TakeDamage(damagePerTap);
        UpdateUI();

        if (isDestroyed)
        {
            Debug.Log("[TapMover] 캡슐 파괴 판정!");

            canTap = false;

            if (animController != null)
            {
                Debug.Log("[TapMover] 파괴 연출 시작!");

                animController.PlayDestroySequence(() =>
                {
                    Debug.Log("[TapMover] 파괴 연출 완료!");

                    if (gameFlowManager == null)
                    {
                        gameFlowManager = FindFirstObjectByType<GameFlowManager>();
                    }

                    if (gameFlowManager != null)
                    {
                        gameFlowManager.OnCostumeShowComplete();
                    }
                    else
                    {
                        Debug.LogError("[TapMover] GameFlowManager를 찾을 수 없습니다!");
                    }
                });
            }
            else
            {
                Debug.LogError("[TapMover] Anim Controller가 연결되지 않았습니다!");
            }
        }
    }

    public void UpdateUI()
    {
        if (durabilityText != null && targetCapsule != null)
        {
            durabilityText.text = $"내구도: {targetCapsule.CurrentDurability} / {targetCapsule.MaxDurability}";
        }
    }
}