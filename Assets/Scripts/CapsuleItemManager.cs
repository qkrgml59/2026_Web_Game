using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapsuleItemManager : MonoBehaviour
{
    public static CapsuleItemManager Instance { get; private set; }

    [Header("필수 씬 컴포넌트 연결")]
    public TapMover tapMover;
    public Capsule targetCapsule;

    [Header("아이템 가격 설정 (Gold)")]
    public int holdPrice = 100;
    public int doubleTapPrice = 300;
    public int tripleTapPrice = 500;

    [Header("아이템 버프 지속시간 설정 (초)")]
    public float holdDuration = 5f;
    public float doubleTapDuration = 10f;
    public float tripleTapDuration = 10f;

    [Header("아이템 쿨타임 설정 (초)")]
    public float holdCooldown = 20f;
    public float doubleTapCooldown = 30f;
    public float tripleTapCooldown = 40f;

    [Header("홀드 속도 설정")]
    public float holdTapInterval = 0.15f; // 홀드 누르고 있을 때 타격 주기

    [Header("아이템 UI 버튼 연결")]
    public Button holdButton;
    public Button doubleTapButton;
    public Button tripleTapButton;

    [Header("아이템 남은 시간 UI 텍스트")]
    public TextMeshProUGUI statusText;

    // 현재 활성화된 버프 상태
    public bool IsHoldActive { get; private set; } = false;
    public bool IsDoubleTapActive { get; private set; } = false;
    public bool IsTripleTapActive { get; private set; } = false;

    // 쿨타임 진행 중 여부
    private bool isHoldOnCooldown = false;
    private bool isDoubleTapOnCooldown = false;
    private bool isTripleTapOnCooldown = false;

    private Coroutine holdPressCoroutine = null;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (tapMover == null) tapMover = FindFirstObjectByType<TapMover>();
        if (targetCapsule == null) targetCapsule = FindFirstObjectByType<Capsule>();

        if (holdButton != null) holdButton.onClick.AddListener(UseHoldItem);
        if (doubleTapButton != null) doubleTapButton.onClick.AddListener(UseDoubleTapItem);
        if (tripleTapButton != null) tripleTapButton.onClick.AddListener(UseTripleTapItem);
    }
    public void UseHoldItem()
    {
        if (IsHoldActive || isHoldOnCooldown) return;

        if (CurrencyManager.Instance != null)
        {
            if (!CurrencyManager.Instance.TrySpendGold(holdPrice))
            {
                UpdateStatusUI("골드가 부족합니다!");
                return;
            }
        }

        StartCoroutine(HoldBuffRoutine());
    }

    private IEnumerator HoldBuffRoutine()
    {
        IsHoldActive = true;
        isHoldOnCooldown = true;
        if (holdButton != null) holdButton.interactable = false;

        UpdateStatusUI("홀드 모드 (꾸우욱 누르세요!)");

        yield return new WaitForSeconds(holdDuration);

        // 버프 시간 만료 시 연속 누르기 강제 정지
        IsHoldActive = false;
        StopHoldPressing();
        UpdateStatusUI("");

        float remainCooldown = Mathf.Max(0f, holdCooldown - holdDuration);
        yield return new WaitForSeconds(remainCooldown);

        isHoldOnCooldown = false;
        if (holdButton != null) holdButton.interactable = true;
    }

    // 플레이어가 손가락으로 누르기 시작할 때 (TapMover에서 호출)
    public void StartHoldPressing(Vector2 pressPosition)
    {
        if (!IsHoldActive) return;

        // 기존 실행 중인 코루틴이 있다면 확실하게 정지
        StopHoldPressing();

        holdPressCoroutine = StartCoroutine(HoldPressingRoutine(pressPosition));
    }

    // 손가락을 뗐거나 버프가 끝났을 때
    public void StopHoldPressing()
    {
        if (holdPressCoroutine != null)
        {
            StopCoroutine(holdPressCoroutine);
            holdPressCoroutine = null;
        }
    }

    private IEnumerator HoldPressingRoutine(Vector2 pressPosition)
    {
        while (IsHoldActive && targetCapsule != null && !targetCapsule.IsDestroyed)
        {
            if (tapMover != null)
            {
                // 타격 및 효과음
                if (SoundManager.Instance != null) SoundManager.Instance.PlayTapSound();

                bool isDestroyed = targetCapsule.TakeDamage(tapMover.damagePerTap * 1.5f);
                tapMover.SpawnDamageText(pressPosition);
                tapMover.UpdateUI();

                // 캡슐이 부서진 경우 루프 탈출 및 파괴 연출 호출
                if (isDestroyed || targetCapsule.IsDestroyed)
                {
                    tapMover.OnCapsuleDestroyed();
                    break;
                }
            }

            yield return new WaitForSeconds(holdTapInterval);
        }

        holdPressCoroutine = null;
    }

    public void UseDoubleTapItem()
    {
        if (IsDoubleTapActive || isDoubleTapOnCooldown) return;

        if (CurrencyManager.Instance != null)
        {
            if (!CurrencyManager.Instance.TrySpendGold(doubleTapPrice))
            {
                UpdateStatusUI("골드가 부족합니다!");
                return;
            }
        }

        StartCoroutine(DoubleTapRoutine());
    }

    private IEnumerator DoubleTapRoutine()
    {
        IsDoubleTapActive = true;
        isDoubleTapOnCooldown = true;
        if (doubleTapButton != null) doubleTapButton.interactable = false;

        UpdateStatusUI("2배 연타 모드!");

        yield return new WaitForSeconds(doubleTapDuration);

        IsDoubleTapActive = false;
        UpdateStatusUI("");

        float remainCooldown = Mathf.Max(0f, doubleTapCooldown - doubleTapDuration);
        yield return new WaitForSeconds(remainCooldown);

        isDoubleTapOnCooldown = false;
        if (doubleTapButton != null) doubleTapButton.interactable = true;
    }


    public void UseTripleTapItem()
    {
        if (IsTripleTapActive || isTripleTapOnCooldown) return;

        if (CurrencyManager.Instance != null)
        {
            if (!CurrencyManager.Instance.TrySpendGold(tripleTapPrice))
            {
                UpdateStatusUI("골드가 부족합니다!");
                return;
            }
        }

        StartCoroutine(TripleTapRoutine());
    }

    private IEnumerator TripleTapRoutine()
    {
        IsTripleTapActive = true;
        isTripleTapOnCooldown = true;
        if (tripleTapButton != null) tripleTapButton.interactable = false;

        UpdateStatusUI("3배 폭풍 연타 모드!");

        yield return new WaitForSeconds(tripleTapDuration);

        IsTripleTapActive = false;
        UpdateStatusUI("");

        float remainCooldown = Mathf.Max(0f, tripleTapCooldown - tripleTapDuration);
        yield return new WaitForSeconds(remainCooldown);

        isTripleTapOnCooldown = false;
        if (tripleTapButton != null) tripleTapButton.interactable = true;
    }

    public void TriggerBonusTap(Vector2 clickPosition)
    {
        if (targetCapsule == null || targetCapsule.IsDestroyed || tapMover == null) return;

        if (IsTripleTapActive)
        {
            targetCapsule.TakeDamage(tapMover.damagePerTap);
            targetCapsule.TakeDamage(tapMover.damagePerTap);
        }
        else if (IsDoubleTapActive)
        {
            targetCapsule.TakeDamage(tapMover.damagePerTap);
        }
    }

    private void UpdateStatusUI(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
    }
}