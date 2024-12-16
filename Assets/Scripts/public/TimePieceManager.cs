using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimePieceManager : MonoBehaviour
{
    public Coroutine CorActiveTimerID;
    const int TIMEPOTION_FILL_VAL = 500;
    int WAIT_TIME = 60; // 1분당 회복

    public Sprite activeBtnSpr;
    public Sprite inActiveBtnSpr;

    public DOTweenAnimation DOTAnim;
    public DOTweenAnimation iconDOTAnim;
    public DOTweenAnimation iconrotateDOTAnim;
    public DOTweenAnimation rotateDOTAnim;

    //* ELEMENT
    public GameObject windowObj;
    public GameObject alertRedDotObj;
    public GameObject ingameActiveBtnObj;
    public Slider iconStorageSlider;
    public TMP_Text myLightStoneTxt;
    public TMP_Text timerTxt;
    public Image activeBtnImg;
    public TMP_Text gaugeValTxt;
    public Slider gaugeSlider;
    public TMP_Text timeScaleTxt;
    public TMP_Text decreaseValTxt;              // 1초당 소모량 표시텍스트
    public TMP_Text timePotionCntTxt;            // 시간의포션 수량 표시텍스트
    public ParticleImage timePotionChargeParticleEF; // 시간의포션 충전 이펙트
    public Button autoActiveToggleBtn;           // 자동사용 토글버튼
    public TMP_Text autoActiveToggleBtnTxt;      // 자동사용 토글버튼 텍스트

    [Header("업그레이드 UI")]
    public UpgradeUIFormat upgFillValUI;         // 1분당 회복UI
    public UpgradeUIFormat upgIncStorageUI;      // 보관량 증가UI
    public UpgradeUIFormat upgIncTimeScaleUI;    // 시간속도증가UI

    //* VALUE
    // 자동시작 트리거
    public bool IsOnAutoActive {get => DM._.DB.timePieceDB.isAutoActiveTimePiece;}
    // 발동 트리거
    public bool isActive;
    // 회복 타이머시간
    private int time;                            
    // 시간의조각 최대보관량
    public int MaxStorage {get => upgIncStorage.Val;}
    // 시간의조각 현재보관량         
    public int curStorage;                       

    [Header("업그레이드 데이터")]
    public UpgradeFormatInt upgFillVal;          // 1분당 회복 데이터
    public UpgradeFormatInt upgIncStorage;       // 보관량 증가 데이터
    public UpgradeFormatFloat upgIncTimeScale;   // 시간속도증가 데이터

    IEnumerator Start()
    {
        ingameActiveBtnObj.SetActive(false);

        // 데이터가 먼저 로드될때까지 대기
        yield return new WaitUntil(() => DM._.DB != null);
        // yield return new WaitUntil(() => DM._.DB.timePieceDB != null);
        
        // timePieceDB가 null일 경우 새로 초기화
        if (DM._.DB.timePieceDB == null)
        {
            Debug.Log("<color=red>데이터가 없음으로 자체 초기화</color>");
            DM._.DB.timePieceDB = new TimePieceDB();
            DM._.DB.timePieceDB.Init();
        }

        // 현재보관량 데이터
        curStorage = DM._.DB.timePieceDB.curStorage;
        // 1분당 회복 데이터
        upgFillVal = DM._.DB.timePieceDB.upgFillVal;
        // 보관량 증가 데이터
        upgIncStorage = DM._.DB.timePieceDB.upgIncStorage;
        // 시간속도증가 데이터  
        upgIncTimeScale = DM._.DB.timePieceDB.upgIncTimeScale;

        //* 오프라인 자동획득 결과처리
        yield return new WaitForSeconds(1); // 저장된 추가획득량 데이터가 로드안되는 문제가 있어 1초 대기

        OfflineAutoFill();
        SetSliderUI();
        UpdateToggleBtnUI(IsOnAutoActive);
    }

    // void Update() {
    //     //! TEST 자동채굴 대기시간 5초 <-> 1분
    //     if(GM._.stm.testMode.activeSelf && Input.GetKeyDown(KeyCode.B))
    //     {
    //         if(WAIT_TIME == 60)
    //             WAIT_TIME = 5;
    //         else
    //             WAIT_TIME = 60;

    //         time = WAIT_TIME;
    //     }
    // }

#region EVENT
    /// <summary>
    /// 자동시작 토글버튼
    /// </summary>
    public void OnToogleAutoActiveToggleBtn()
    {
        SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
        DM._.DB.timePieceDB.isAutoActiveTimePiece = !IsOnAutoActive;
        UpdateToggleBtnUI(IsOnAutoActive);
    }

    /// <summary>
    /// 발동버튼
    /// </summary>
    public void OnClickActiveBtn()
    {
        // ACTIVE -> STOP
        if(isActive)
        {
            isActive = false;
            ActiveProcess(isActive);
        }
        // STOP -> ACTIVE
        else
        {
            if(curStorage <= 0)
            {   // 아이템이 부족합니다.
                GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
                return;
            }

            isActive = true;
            ActiveProcess(isActive);
        }
    }
    /// <summary>
    /// (업그레이드) 1분당 회복 버튼
    /// </summary>
    public void OnClickUpgradeFillValBtn() => Upgrade(upgFillVal);
    /// <summary>
    /// (업그레이드) 보관량 증가 버튼
    /// </summary>
    public void OnClickUpgradeIncStorageBtn() => Upgrade(upgIncStorage);
    /// <summary>
    /// (업그레이드) 시간속도 증가
    /// </summary>
    public void OnClickUpgradeIncTimeScaleBtn() => Upgrade(upgIncTimeScale);
    /// <summary>
    /// 시간의포션 사용 버튼
    /// </summary>
    public void OnClickUseTimePotion()
    {
        if(curStorage >= MaxStorage)
        {   // 현재 최대치입니다.
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.MaxValMsg));
            return;
        }

        // 수량체크
        if(DM._.DB.statusDB.GetInventoryItemVal(Enum.INV.TIMEPOTION) <= 0) 
        {
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
            return;
        }

        SoundManager._.PlaySfx(SoundManager.SFX.ItemDrop1SFX);
        timePotionChargeParticleEF.Play();

        // 시간의포션 수량감소
        DM._.DB.statusDB.SetInventoryItemVal(Enum.INV.TIMEPOTION, -1);
        // curStorage += TIMEPOTION_FILL_VAL;
        SetStorage(+TIMEPOTION_FILL_VAL);

        UpdateDataAndUI();
    }
#endregion

#region FUNC
    /// <summary>
    /// 팝업표시
    /// </summary>
    public void ShowPopUp()
    {
        windowObj.SetActive(true);
        DOTAnim.DORestart();
        UpdateDataAndUI();
    }

    private void UpdateToggleBtnUI(bool isOn)
    {
        autoActiveToggleBtnTxt.text = isOn? "ON" : "OFF";
        autoActiveToggleBtnTxt.color = isOn? Color.green : Color.white;
    }

    /// <summary>
    /// 시간의조각 활성화 및 비활성화 처리
    /// </summary>
    public void ActiveProcess(bool isActive)
    {
        if(!isActive)
        {
            SoundManager._.PlaySfx(SoundManager.SFX.Tap1SFX);
            activeBtnImg.sprite = inActiveBtnSpr;
            iconDOTAnim.DOPause();
            rotateDOTAnim.DOPause();
            iconrotateDOTAnim.DOPause();
            if(CorActiveTimerID != null)
                StopCoroutine(CorActiveTimerID);
            Time.timeScale = 1;
        }
        else
        {
            SoundManager._.PlaySfx(SoundManager.SFX.BlessResetSFX);
            activeBtnImg.sprite = activeBtnSpr;
            iconDOTAnim.DORestart();
            rotateDOTAnim.DORestart();
            iconrotateDOTAnim.DORestart();
            CorActiveTimerID = StartCoroutine(CoActiveTimer());
            Time.timeScale = upgIncTimeScale.Val;
        }

        timeScaleTxt.text = $"x {Time.timeScale}";
    }

    /// <summary>
    /// 업그레이드 처리
    /// </summary>
    /// <param name="upgDt">업그레이드할 데이터</param>
    private void Upgrade(UpgradeFormat upgDt) {
        var sttDB = DM._.DB.statusDB;

        if(upgDt.IsMaxLv)
        {
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.MaxLvMsg));
            return;
        }

        if(sttDB.GetInventoryItemVal(upgDt.NeedRsc) >= upgDt.Price)
        {
            SoundManager._.PlaySfx(SoundManager.SFX.TranscendUpgradeSFX);
            GM._.ui.ShowNoticeMsgPopUp(LM._.Localize(LM.UpgradeCompleteMsg));

            // 제작에 필요한 아이템 수량 감소
            sttDB.SetInventoryItemVal(upgDt.NeedRsc, -upgDt.Price);
            upgDt.Lv++;

            UpdateDataAndUI();

            // 이미 실행중인  시간의조각 활성화가 있다면 정지하여 초기화
            if(CorActiveTimerID != null)
            {
                StopCoroutine(CorActiveTimerID);
                CorActiveTimerID = null;
            }

            ActiveProcess(isActive); // 적용데이터 최신화
        }
        else
            GM._.ui.ShowWarningMsgPopUp(LM._.Localize(LM.NotEnoughItemMsg));
    }

    /// <summary>
    /// 업그레이드 결과 최신화
    /// </summary>
    private void UpdateDataAndUI()
    {
        upgFillVal.UpdatePrice(upgFillVal.PriceDef / 2);
        upgIncStorage.UpdatePrice(upgIncStorage.PriceDef / 2);
        upgIncTimeScale.UpdatePrice();

        upgFillValUI.UpdateUI(upgFillVal);
        upgIncStorageUI.UpdateUI(upgIncStorage);
        upgIncTimeScaleUI.UpdateUI(upgIncTimeScale);

        myLightStoneTxt.text = $"{DM._.DB.statusDB.LightStone}";
        timePotionCntTxt.text = $"{DM._.DB.statusDB.TimePotion}";
        decreaseValTxt.text = $"{LM._.Localize(LM.ConsumePerSec)} : {upgIncTimeScale.Val * 10}";

        // 슬라이더UI 최신화
        SetSliderUI();
    }

    /// <summary>
    /// 오프라인 자동회복 결과처리
    /// </summary>
    private void OfflineAutoFill()
    {
        //* 어플시작시 이전까지 경과한시간
        int passedTime = DM._.DB.autoMiningDB.GetPassedSecData();

        // 자동획득량 계산
        int cnt = passedTime / WAIT_TIME;

        // 결과
        int resVal = curStorage + cnt * GetProductionVal();

        // 최대수량보다 높다면 최대수량만큼으로 수정
        if(resVal > MaxStorage)
            resVal = MaxStorage;

        Debug.Log($"<color=white>자동획득 오프라인 처리: 이전수량= {curStorage} / {MaxStorage}, 획득량: {resVal} (경과시간: {passedTime} / {WAIT_TIME} = {cnt})</color>");
        curStorage = resVal;

        time = WAIT_TIME;
    }

    /// <summary>
    /// 자동회복량 계산 및 반환
    /// </summary>
    private int GetProductionVal() => upgFillVal.Val;

    /// <summary>
    /// 시간의결정 현재보관량 설정
    /// </summary>
    private void SetStorage(int val)
    {
        // 보관량 증가 또는 감소
        curStorage += val;

        // 시간의결정이 0인경우 정지
        if(curStorage <= 0)
        {
            curStorage = 0;
            isActive = false;
            ActiveProcess(isActive);
        }
    }

    private void SetSliderUI()
    {
        gaugeValTxt.text = $"{curStorage} / {MaxStorage}";
        
        // 분모가 0일경우, 나누기 에러 발생하는 부분 대응
        if (MaxStorage != 0)
        {
            gaugeSlider.value = (float)curStorage / MaxStorage;
            iconStorageSlider.value = (float)curStorage / MaxStorage;
        }
        else
        {
            gaugeSlider.value = 0;
            iconStorageSlider.value = 0;
        }

        // 🔴알람 (조건1: 시간의조각 최대치이상 보유시 + 조건2:시간의조각이 미사용중일때)
        alertRedDotObj.SetActive(curStorage >= MaxStorage && !isActive);
    }

    public IEnumerator CoActiveTimer()
    {
        // 소수점 나머지
        float remain = 0;

        while(isActive)
        {
            if(!isActive)
                break;

            yield return Util.TIME0_1;

            // 시간속도 업글시, 그 수치만큼 %로 더 감속이 빨리되도록 나머지값 적용
            float remainVal = (int)upgIncTimeScale.Val - upgIncTimeScale.Val;
            remain += remainVal;
            // 소수점 나머지값이 합산되어 1보다높다면 추가적으로 게이지 감소
            if(remain >= 1)
            {
                SetStorage(-(int)upgIncTimeScale.Val - 1);
                remain -= 1;
            }
            else
            {
                SetStorage(-(int)upgIncTimeScale.Val); // 게이지 감소
            }

            SetSliderUI();
        }
    }

    /// <summary>
    /// 타이머 (1분)
    /// </summary>
    public void SetTimer()
    {
        // 최대수량 넘어갈시 처리안함
        if(curStorage >= MaxStorage)
        {
            if(timerTxt.text != "00 : 00")
                timerTxt.text = "00 : 00";
            return;
        }
        
        time--;
        string timeFormat = Util.ConvertTimeFormat(time);
        timerTxt.text = timeFormat;

        // 리셋
        if(time < 1)
        {
            time = WAIT_TIME;
            SetStorage(GetProductionVal());
            SetSliderUI();
        }
    }
#endregion
}