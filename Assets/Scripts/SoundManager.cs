using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("오디오 소스 연결")]
    public AudioSource sfxSource; // 효과음 전용 AudioSource
    public AudioSource bgmSource; // BGM 전용 AudioSource

    [Header("클리커 효과음 클립 (AudioClip)")]
    public AudioClip tapSound;      // 기본 캡슐 타격음
    public AudioClip destroySound;  // 캡슐 깨지는(파괴) 소리
    public AudioClip itemUseSound;  // 아이템 버튼 누를 때 소리
    public AudioClip getCostumeSound; // 의상 획득 팝업 소리
    public AudioClip buttonClickSound; // 일반 UI 버튼 클릭 소리

    [Header("배경음악 (AudioClip)")]
    public AudioClip bgmClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // BGM 자동 재생
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    //효과음 재생 핵심 함수 (PlayOneShot을 써야 연타해도 소리가 안 끊기고 중첩됨)
    public void PlaySFX(AudioClip clip, float pitchRandomness = 0.1f)
    {
        if (sfxSource == null || clip == null) return;

        // 연타 시 소리가 다채롭도록 피치(음높이)에 약간의 미세한 피치 변화 부여
        sfxSource.pitch = 1f + Random.Range(-pitchRandomness, pitchRandomness);
        sfxSource.PlayOneShot(clip);
    }

    // Convenience 매소드들
    public void PlayTapSound() => PlaySFX(tapSound, 0.08f);
    public void PlayDestroySound() => PlaySFX(destroySound, 0.0f);
    public void PlayItemUseSound() => PlaySFX(itemUseSound, 0.0f);
    public void PlayGetCostumeSound() => PlaySFX(getCostumeSound, 0.0f);
    public void PlayButtonClickSound() => PlaySFX(buttonClickSound, 0.0f);
}