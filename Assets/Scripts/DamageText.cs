using System.Collections;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("연출 설정")]
    public float moveSpeed = 100f;       // 위로 올라가는 속도
    public float fadeSpeed = 2f;         // 투명해지는 속도
    public float destroyTime = 0.6f;     // 자동 삭제 시간

    private TextMeshProUGUI textMesh;
    private Color textColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textColor = textMesh.color;
        }
    }

    // 데미지 수치 설정 및 연출 시작
    public void Setup(float damageAmount)
    {
        if (textMesh != null)
        {
            textMesh.text = $"-{damageAmount:F0}"; // 예: "-10" 형태로 표시
        }

        StartCoroutine(FadeOutAndDestroyRoutine());
    }

    private IEnumerator FadeOutAndDestroyRoutine()
    {
        float elapsed = 0f;

        while (elapsed < destroyTime)
        {
            elapsed += Time.deltaTime;

            // 1. 위로 이동
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // 2. 투명도(Alpha) 점점 낮추기
            if (textMesh != null)
            {
                textColor.a = Mathf.Lerp(1f, 0f, elapsed / destroyTime);
                textMesh.color = textColor;
            }

            yield return null;
        }

        // 연출 종료 후 오브젝트 삭제
        Destroy(gameObject);
    }
}