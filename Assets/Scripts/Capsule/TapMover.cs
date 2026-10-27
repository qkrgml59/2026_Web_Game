using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TapMover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("연결 요소")]
    public Capsule targetCapsule;
    public CapsuleAnimationController animController;
    public TextMeshProUGUI durabilityText;
    public GameFlowManager gameFlowManager;

    [Header("데미지 팝업 설정")]
    public GameObject damageTextPrefab; // 둥둥 뜨는 숫자 프리팹
    public Transform canvasTransform;   // 팝업이 생성될 Canvas Transform

    [Header("타격 설정")]
    public float damagePerTap = 10f;

    public bool canTap = false;

    void Start()
    {
        if (gameFlowManager == null)
        {
            gameFlowManager = FindFirstObjectByType<GameFlowManager>();
        }

        if (animController == null && targetCapsule != null)
        {
            animController = targetCapsule.GetComponent<CapsuleAnimationController>();
        }

        UpdateUI();
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (!canTap) return;
        if (targetCapsule == null || targetCapsule.IsDestroyed) return;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayTapSound();
        }

        // 1. 홀드 모드 연동
        if (CapsuleItemManager.Instance != null && CapsuleItemManager.Instance.IsHoldActive)
        {
            CapsuleItemManager.Instance.StartHoldPressing(e.position);
            return;
        }

        // 2. 기본 1타 적용 & 데미지 팝업
        SpawnDamageText(e.position);
        bool isDestroyed = targetCapsule.TakeDamage(damagePerTap);

        // 3. 2배/3배 아이템 연동 & 팝업 연출
        if (!isDestroyed && CapsuleItemManager.Instance != null)
        {
            if (CapsuleItemManager.Instance.IsTripleTapActive)
            {
                CapsuleItemManager.Instance.TriggerBonusTap(e.position);
                SpawnDamageText(e.position + new Vector2(-25f, 25f));
                SpawnDamageText(e.position + new Vector2(25f, 25f));
            }
            else if (CapsuleItemManager.Instance.IsDoubleTapActive)
            {
                CapsuleItemManager.Instance.TriggerBonusTap(e.position);
                SpawnDamageText(e.position + new Vector2(20f, 20f));
            }
        }

        UpdateUI();

        // 4. 캡슐이 파괴되었을 때 연출 및 완료 콜백 실행!
        if (targetCapsule.IsDestroyed)
        {
            OnCapsuleDestroyed();
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (CapsuleItemManager.Instance != null)
        {
            CapsuleItemManager.Instance.StopHoldPressing();
        }
    }

    // 캡슐 파괴 시 실행되는 처리 함수
    public void OnCapsuleDestroyed()
    {
        canTap = false; // 파괴된 동안 추가 클릭 방지

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayDestroySound();
        }

        if (animController != null)
        {
            // 애니메이션 종료 후 GameFlowManager.OnCostumeShowComplete 호출!
            animController.PlayDestroySequence(() =>
            {
                if (gameFlowManager != null)
                {
                    gameFlowManager.OnCostumeShowComplete();
                }
            });
        }
        else
        {
            if (gameFlowManager != null)
            {
                gameFlowManager.OnCostumeShowComplete();
            }
        }
    }

    public void SpawnDamageText(Vector2 spawnPosition)
    {
        if (damageTextPrefab == null || canvasTransform == null) return;

        Vector2 randomOffset = new Vector2(Random.Range(-20f, 20f), Random.Range(-10f, 10f));
        Vector2 finalPos = spawnPosition + randomOffset;

        GameObject popObj = Instantiate(damageTextPrefab, canvasTransform);
        popObj.transform.position = finalPos;

        DamageText popScript = popObj.GetComponent<DamageText>();
        if (popScript != null)
        {
            popScript.Setup(damagePerTap);
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