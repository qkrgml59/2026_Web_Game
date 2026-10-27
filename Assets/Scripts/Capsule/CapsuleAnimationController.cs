using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CapsuleAnimationController : MonoBehaviour
{
    [Header("´ë»ó UI ¿¬°á")]
    public RectTransform capsuleRect;
    public Image costumeImage;

    [Header("Ä¸½¶ Èçµé¸² ¼³Á¤")]
    public float shakeDuration = 0.3f;
    public float shakeAmount = 15f;
    public float shakeSpeed = 50f;

    [Header("Ä¸½¶ È®´ë ¼³Á¤")]
    public float popDuration = 0.2f;
    public float popScale = 1.3f;

    [Header("Ä¸½¶ Ãà¼Ò ¼³Á¤")]
    public float shrinkDuration = 0.15f;

    [Header("ÀÇ»ó µîÀå ¼³Á¤")]
    public float costumeStartScale = 0.2f;
    public float costumeAppearDuration = 0.3f;

    public bool IsPlaying { get; private set; } = false;

    private RectTransform costumeRect;
    private Vector3 originalCapsuleScale;
    private Vector2 originalCapsulePos;
    private Image capsuleImage;

    void Start()
    {
        if (capsuleRect != null)
        {
            originalCapsuleScale = capsuleRect.localScale;
            originalCapsulePos = capsuleRect.anchoredPosition;
            capsuleImage = capsuleRect.GetComponent<Image>();
        }

        if (costumeImage != null)
        {
            costumeRect = costumeImage.GetComponent<RectTransform>();
            costumeImage.gameObject.SetActive(false);
        }
    }

    public void PlayDestroySequence(Action onComplete = null)
    {
        if (IsPlaying) return;
        StartCoroutine(DestroyAndRewardSequence(onComplete));
    }

    private IEnumerator DestroyAndRewardSequence(Action onComplete)
    {
        IsPlaying = true;

        // 1. Ä¸½¶ Èçµé¸² ¿¬Ãâ
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsed * shakeSpeed) * shakeAmount;
            if (capsuleRect != null) capsuleRect.anchoredPosition = originalCapsulePos + new Vector2(xOffset, 0f);
            yield return null;
        }
        if (capsuleRect != null) capsuleRect.anchoredPosition = originalCapsulePos;

        // 2. Ä¸½¶ È®´ë ¿¬Ãâ
        elapsed = 0f;
        Vector3 targetScale = originalCapsuleScale * popScale;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            if (capsuleRect != null) capsuleRect.localScale = Vector3.Lerp(originalCapsuleScale, targetScale, elapsed / popDuration);
            yield return null;
        }
        if (capsuleRect != null) capsuleRect.localScale = targetScale;

        // 3. Ä¸½¶ Ãà¼Ò(ÆÄ±«) ¿¬Ãâ
        elapsed = 0f;
        Vector3 currentScale = capsuleRect != null ? capsuleRect.localScale : Vector3.one;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            if (capsuleRect != null) capsuleRect.localScale = Vector3.Lerp(currentScale, Vector3.zero, elapsed / shrinkDuration);
            yield return null;
        }
        if (capsuleRect != null) capsuleRect.localScale = Vector3.zero;

        if (capsuleImage != null)
        {
            capsuleImage.enabled = false;
        }

        // 4. ÀÇ»ó ÆË¾÷ µîÀå ¿¬Ãâ
        if (costumeImage != null && costumeRect != null)
        {
            costumeImage.gameObject.SetActive(true);

            Vector3 costumeInitialScale = Vector3.one * costumeStartScale;
            Vector3 costumeFinalScale = Vector3.one;
            costumeRect.localScale = costumeInitialScale;

            elapsed = 0f;
            while (elapsed < costumeAppearDuration)
            {
                elapsed += Time.deltaTime;
                costumeRect.localScale = Vector3.Lerp(costumeInitialScale, costumeFinalScale, elapsed / costumeAppearDuration);
                yield return null;
            }
            costumeRect.localScale = costumeFinalScale;
        }

        IsPlaying = false;

        // ¡Ú [¿Ï·á ÄÝ¹é ½ÇÇà]
        onComplete?.Invoke();
    }

    public void ResetCapsuleImage()
    {
        if (capsuleImage != null)
        {
            capsuleImage.enabled = true;
        }
        if (capsuleRect != null)
        {
            capsuleRect.localScale = originalCapsuleScale;
            capsuleRect.anchoredPosition = originalCapsulePos;
        }
    }
}