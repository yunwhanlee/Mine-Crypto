using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SoundManager : MonoBehaviour
{
    static public SoundManager _;

    public enum BGM {
        Home,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Stage6,
        Stage7,
        Stage8,
        Challenge,
    }

    public enum SFX {
        AttackSFX,                  // 공격1회(곡괭이질)(공용)
        AutoMiningTakeSFX,          // 자동채굴획득
        BlessResetSFX,              // 축복재설정
        BrokenOreSFX,               // 광석깨짐
        ClickBagSFX,                // 가방열기닫기
        EnterSFX,                   // 광산입장
        ErrorSFX,                   // 에러
        FameCompleteSFX,            // 명예임무완료
        FameLvUpSFX,                // 명예레벨업
        GamePlaySFX,                // 인게임시작 효과음
        InvSlotClickSFX,            // 아이템정보열기닫기
        ItemDrop1SFX,               // 인게임 광석 파티클1
        ItemDrop2SFX,               // 인게임 광석 파티클2
        Jump1SFX,                   // 캐릭터점프등장1
        Jump2SFX,                   // 캐릭터점프등장2
        Jump3SFX,                   // 캐릭터점프등장3
        Metal1SFX,                  // 광석 HIT1
        Metal2SFX,                  // 광석 HIT2
        Metal3SFX,                  // 광석 HIT3
        OpenMushBoxSFX,             // 버섯상자오픈
        OpenOreChestSFX,            // 광석상자오픈
        OpenTreasureChestSFX,       // 보물상자오픈
        ProductionSFX,              // 제작버튼
        ProficiencyFinishSFX,       // 숙련도완료
        SummonLegendSFX,            // 전설캐릭터소환(개별)
        SummonMythSFX,              // 신화캐릭터소환(개별)
        SummonNormalSFX,            // 일반캐릭터소환(개별)
        SummonRareUniqueSFX,        // 희귀영웅캐릭터소환(개별)
        Tap1SFX,                    // 메뉴열기닫기,입장버튼, 카테고리선택
        Tap2SFX,                    // 연금술 목록선택, 
        TranscendUpgradeSFX,        // 초월강화
        TreasureChestOrePickSFX,    // 인게임 보물상자 획득
        UnlockSFX,                  // 광산개방
        UpgradeMushSFX,             // 버섯도감강화
        UpgradeSFX,                 // 일반강화,자동채굴강화
    }

    [field: Header("BGM")]
    GameObject bgmObj;
    public AudioClip[] bgmClips;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [field: Header("SFX")]
    GameObject sfxObj;
    public AudioClip[] sfxClips;
    public float sfxVolume;
    AudioSource[] sfxPlayers;
    public int channels;
    int channelIndex;

    void Awake()
    {
        _ = this;
        Init();
    }

#region EVENT
    /// <summary>
    /// 화면 닫을때 효과음 이벤트처리
    /// </summary>
    public void OnClickClosePopUpSFX()
    {
        PlaySfx(SFX.Tap2SFX);
    }
#endregion

#region FUNC
    /// <summary>
    /// 초기화
    /// </summary>
    private void Init()
    {
        // BGM 초기화
        bgmObj = new GameObject("BgmPlayer");
        bgmObj.transform.parent = transform;
        bgmPlayer = bgmObj.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        SetBgmVolume(bgmVolume);
        GM._.stm.bgmSlider.value = bgmVolume;

        // SFX 초기화
        sfxObj = new GameObject("SfxPlayer");
        sfxObj.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
        for(int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObj.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
        }
        SetSfxVolume(sfxVolume);
        GM._.stm.sfxSlider.value = sfxVolume;
    }

    public void SetBgmVolume(float value)
    {
        bgmObj.SetActive(value > 0);
        if(value > 0 && !bgmPlayer.isPlaying)
            bgmPlayer.Play();

        bgmPlayer.volume = value;
    }

    public void SetSfxVolume(float value)
    {
        sfxObj.SetActive(value > 0);

        for(int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i].volume = value;
        }
    }

    /// <summary>
    /// 배경음 재생 및 정지 
    /// </summary>
    /// <param name="isOn">재생 or 정지 트리거</param>
    public void PauseBgm(bool isOn)
    {
        if(isOn)
            bgmPlayer.Pause();
        else
            bgmPlayer.Play();
    }

    /// <summary>
    /// 배경음 재생
    /// </summary>
    /// <param name="bgm">배경음 종류</param>
    /// <param name="isLoop">루프 트리거</param>
    public void PlayBgm(BGM bgm, bool isLoop = true)
    {
        bgmPlayer.clip = bgmClips[(int)bgm];
        bgmPlayer.loop = isLoop;
        bgmPlayer.Play();
    }

    /// <summary>
    /// 효과음 재생
    /// </summary>
    /// <param name="sfx">효과음 종류</param>
    public void PlaySfx(SFX sfx)
    {
        // 재생중이지 않은 오디오소스 채널로 재생
        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            // 맨 마지막에 재생을 했던 오디오 플레이어 인덱스
            int loopIdx = (index + channelIndex) % sfxPlayers.Length;

            //* 재생중인 오디오인 경우
            if(sfxPlayers[loopIdx].isPlaying)
            {
                continue; // 다음 인덱스로 이동
            }
            //* 대기중인 오디오인 경우
            else
            {
                // 현재 채널인덱스 최신화
                channelIndex = loopIdx; 

                // 현재 오디오플레이어로 재생
                sfxPlayers[loopIdx].clip = sfxClips[(int)sfx];
                sfxPlayers[loopIdx].Play();

                break; // for문 종료
            }
        }
    }
#endregion
}
