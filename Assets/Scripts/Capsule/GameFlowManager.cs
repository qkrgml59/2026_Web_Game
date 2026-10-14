using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameFlowManager : MonoBehaviour
{

    //TODO : 나중에 연속적으로 진행되어야 할 때가 있으니 그 때는 이 문구는 튜토리얼에만 뜨게 하기
    [Header("UI 연결")]
    public TextMeshProUGUI noticeText;    
    public TextMeshProUGUI durabilityText; 
    public Image costumeImage;
    public Button boxButton;              
    public GameObject capsuleObject;      
    public TapMover tapMover;

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
    private CostumeData currentCapsuleCostume; //현재 캡슐의 의상 데이터 임시 저장

    void Start()
    {
        if (capsuleObject != null)
        {
            capsuleRect = capsuleObject.GetComponent<RectTransform>();
            originalCapsuleScale = capsuleRect.localScale;
        }

        if (retryButton != null) retryButton.onClick.AddListener(OnClickRetry);
        if (equipButton != null) equipButton.onClick.AddListener(OnClickEquip);

        SetStep1_Start();
    }

    //게임 시작 및 안내 문구
    private void SetStep1_Start()
    {
        if (noticeText != null)
        {
            noticeText.gameObject.SetActive(true);
            noticeText.text = "캡슐 상자에서 캡슐을 뽑으세요!";
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

    public void OnBoxClicked()
    {
        if (isStepProcessing) return;
        StartCoroutine(Step2_DrawCapsuleRoutine());
    }

    //캡슐 뽑기 등장 연출
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
                    }
                }
                else
                {
                    Debug.LogWarning("[GameFlowManager] 선택된 캡슐에 의상 데이터(CostumeData)가 없거나 설정되지 않았습니다.");
                }
            }

            capsuleObject.SetActive(true);
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

        StartCoroutine(Step3_ShowTapNoticeRoutine());
    }

    //캡슐 타격 안내
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
            durabilityText.gameObject.SetActive(true);
        }

        if (tapMover != null)
        {
            tapMover.canTap = true;
            tapMover.UpdateUI();
        }

        isStepProcessing = false;
    }

    public void OnCostumeShowComplete()
    {
        if (CostumeManager.Instance != null && currentCapsuleCostume != null)
        {
            CostumeManager.Instance.AcquireCostume(currentCapsuleCostume);
        }
        else
        {
            Debug.LogWarning("[GameFlowManager] currentCapsuleCostume이 null이라 의상을 저장하지 못했습니다!");
        }

        if (retryButton != null) retryButton.gameObject.SetActive(true);
        if (equipButton != null) equipButton.gameObject.SetActive(true);

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