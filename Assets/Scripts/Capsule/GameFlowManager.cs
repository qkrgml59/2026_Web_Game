using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameFlowManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI noticeText;
    public TextMeshProUGUI durabilityText;
    public Image costumeImage;
    public Button boxButton;
    public GameObject capsuleObject;
    public TapMover tapMover;

    [Header("캡슐 뽑기 가격 설정")]
    [Tooltip("캡슐 1회 뽑기 시 차감될 골드 비용")]
    public int capsulePrice = 100; // 기본 100 골드 (인스펙터에서 수정 가능)

    [Header("공통 씬 이동 관리자")]
    public SceneMoveManager naviManager;

    [Header("의상 획득 선택 버튼")]
    public Button retryButton;
    public Button equipButton;

    [Header("씬 이동 설정")]
    [Tooltip("의상 입히기 버튼을 눌렀을 때 이동할 씬의 정확한 이름")]
    public string targetSceneName = "CostumeScene";

    [Header("연출 및 타임 설정")]
    public float noticeDisplayTime = 3.0f;
    public float popUpDuration = 0.4f;

    private RectTransform capsuleRect;
    private Vector3 originalCapsuleScale;
    private bool isStepProcessing = false;
    private CostumeData currentCapsuleCostume; // 현재 캡슐의 의상 데이터 임시 저장

    void Start()
    {
        if (capsuleObject != null)
        {
            capsuleRect = capsuleObject.GetComponent<RectTransform>();
            if (capsuleRect != null)
            {
                originalCapsuleScale = capsuleRect.localScale;
            }
        }

        if (retryButton != null) retryButton.onClick.AddListener(OnClickRetry);
        if (equipButton != null) equipButton.onClick.AddListener(OnClickEquip);

        SetStep1_Start();
    }

    // 게임 시작 및 초기화 단계
    private void SetStep1_Start()
    {
        if (noticeText != null)
        {
            noticeText.gameObject.SetActive(true);
            noticeText.text = $"캡슐 상자에서 캡슐을 뽑으세요! ({capsulePrice:N0} Gold)";
        }

        if (durabilityText != null) durabilityText.gameObject.SetActive(false);
        if (costumeImage != null) costumeImage.gameObject.SetActive(false);
        if (capsuleObject != null) capsuleObject.SetActive(false);

        if (retryButton != null) retryButton.gameObject.SetActive(false);
        if (equipButton != null) equipButton.gameObject.SetActive(false);

        if (boxButton != null) boxButton.gameObject.SetActive(true);
        if (tapMover != null) tapMover.canTap = false;

        if (naviManager != null)
        {
            naviManager.SetNavigationButtonsInteractable(true);
        }

        currentCapsuleCostume = null;
        isStepProcessing = false;
    }

    // 캡슐 상자 클릭 버튼 이벤트
    public void OnBoxClicked()
    {
        if (isStepProcessing) return;

        //
        if (CurrencyManager.Instance != null)
        {
            if (!CurrencyManager.Instance.TrySpendGold(capsulePrice))
            {
                if (noticeText != null)
                {
                    noticeText.gameObject.SetActive(true);
                    noticeText.text = $"골드가 부족합니다! (필요: {capsulePrice:N0} Gold)";
                }
                Debug.LogWarning($"[GameFlowManager] 골드가 부족하여 캡슐을 뽑지 못했습니다. 필요 골드: {capsulePrice}");
                return; // 골드가 부족하면 뽑기 진행하지 않음!
            }
        }

        StartCoroutine(Step2_DrawCapsuleRoutine());
    }

    // 캡슐 뽑기 등장 연출
    private IEnumerator Step2_DrawCapsuleRoutine()
    {
        isStepProcessing = true;

        if (naviManager != null)
        {
            naviManager.SetNavigationButtonsInteractable(false);
        }

        if (boxButton != null) boxButton.gameObject.SetActive(false);

        if (capsuleObject != null)
        {
            CapsuleAnimationController anim = capsuleObject.GetComponent<CapsuleAnimationController>();
            if (anim != null)
            {
                anim.ResetCapsuleImage();
            }

            Capsule capsuleScript = capsuleObject.GetComponent<Capsule>();
            if (capsuleScript != null)
            {
                capsuleScript.InitializeCapsule();

                if (capsuleScript.capsuleData != null && capsuleScript.capsuleData.costumeData != null)
                {
                    currentCapsuleCostume = capsuleScript.capsuleData.costumeData;

                    if (costumeImage != null)
                    {
                        costumeImage.sprite = currentCapsuleCostume.costumeSprite;
                        costumeImage.gameObject.SetActive(false); // 깨지기 전엔 숨김
                    }
                    Debug.Log($"[GameFlowManager] 캡슐 생성 완료! 뽑힐 의상: {currentCapsuleCostume.costumeName}");
                }
                else
                {
                    Debug.LogWarning("[GameFlowManager] 선택된 캡슐에 의상 데이터(CostumeData)가 없거나 설정되지 않았습니다!");
                }
            }

            capsuleObject.SetActive(true);
            if (capsuleRect != null)
            {
                capsuleRect.localScale = Vector3.zero;

                float elapsed = 0f;
                while (elapsed < popUpDuration)
                {
                    elapsed += Time.deltaTime;
                    capsuleRect.localScale = Vector3.Lerp(Vector3.zero, originalCapsuleScale, elapsed / popUpDuration);
                    yield return null;
                }
                capsuleRect.localScale = originalCapsuleScale;
            }
        }

        // 캡슐 커진 직후 바로 타격 터치 및 아이템 사용 허용
        if (tapMover != null)
        {
            tapMover.canTap = true;
            tapMover.UpdateUI();
        }

        isStepProcessing = false;

        StartCoroutine(Step3_ShowTapNoticeRoutine());
    }

    // 캡슐 타격 안내
    private IEnumerator Step3_ShowTapNoticeRoutine()
    {
        if (noticeText != null)
        {
            noticeText.gameObject.SetActive(true);
            noticeText.text = "꺼낸 캡슐을 눌러 캡슐을 까세요!";
        }

        yield return new WaitForSeconds(noticeDisplayTime);

        if (noticeText != null)
        {
            noticeText.gameObject.SetActive(false);
        }

        if (durabilityText != null)
        {
            durabilityText.gameObject.SetActive(false);
        }
    }

    // 캡슐 파괴 및 연출 완료 시 실행
    public void OnCostumeShowComplete()
    {
        // 1. 다 까진 캡슐 숨기기
        if (capsuleObject != null)
        {
            capsuleObject.SetActive(false);
        }

        // 2. 획득한 의상 연출 이미지 표시
        if (costumeImage != null)
        {
            costumeImage.gameObject.SetActive(true);
        }

        // 3. CostumeManager에 획득 의상 저장 처리
        if (CostumeManager.Instance != null && currentCapsuleCostume != null)
        {
            CostumeManager.Instance.AcquireCostume(currentCapsuleCostume);
            Debug.Log($"[GameFlowManager] 성공적으로 CostumeManager에 '{currentCapsuleCostume.costumeName}' 의상을 추가했습니다!");
        }
        else
        {
            if (CostumeManager.Instance == null)
                Debug.LogError("[GameFlowManager] CostumeManager.Instance가 씬에 존재하지 않습니다!");
            if (currentCapsuleCostume == null)
                Debug.LogWarning("[GameFlowManager] currentCapsuleCostume이 null이라 의상을 저장하지 못했습니다!");
        }

        // 4. 다시 뽑기 및 의상 입히기 버튼 활성화
        if (retryButton != null) retryButton.gameObject.SetActive(true);
        if (equipButton != null) equipButton.gameObject.SetActive(true);

        // 5. 안내 문구 변경 및 내비게이션 활성화
        if (noticeText != null)
        {
            noticeText.gameObject.SetActive(true);
            noticeText.text = "축하합니다! 의상을 획득하셨습니다!";
        }

        if (naviManager != null)
        {
            naviManager.SetNavigationButtonsInteractable(true);
        }
    }

    private void OnClickRetry()
    {
        Debug.Log("다시 뽑기 선택!");
        SetStep1_Start();
    }

    private void OnClickEquip()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"{targetSceneName} 씬으로 이동합니다.");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("이동할 씬 이름이 설정되지 않았습니다!");
        }
    }
}