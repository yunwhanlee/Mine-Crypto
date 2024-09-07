using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

namespace Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts
{
    public class MiningController : MonoBehaviour
    {
        public enum Status {
            SPAWN,
            GO,
            MINING,
            BACKHOME,
            CLEARSTAGE,
        }

        public Status status;

        //* COMPONENT
        private CharacterAnimation _animation;
        private Rigidbody2D rigid;
        private SpriteRenderer sprRdr;
        // private Character _character;
        // private CharacterBuilder _charaBuilder;
        // private CharacterController2D _controller;

        //* ELEMENT
        public Sprite iconCharaImg;                    // UI표시 전용 아이콘 이미지
        public Slider bagStorageSlider;                // カバン保管量 スライダーバー
        public TMP_Text bagStorageSliderTxt;           // カバン保管量 スライダーバー テキスト
        public Ore targetOre;

        //* VALUE
        const float TARGET_Y_UNDER_MINUS = 0.5f;      // 고블린을 광석보다 앞으로 배치하기 위해, 타겟위치 Y값 낮출 값
        const float ATTACK_SPEED_MAX_SEC = 1.5f;       // 공격속도 최대치
        const float REACH_TARGET_MIN_DIST = 0.375f;    // 타겟지점 도달판단 최소거리(집, 광석)

        // 이름
        [field: SerializeField] public string Name {get; private set;}

        // 등급
        [field: SerializeField] public Enum.GRADE Grade {get; private set;}

        [Header("능력치")]
        // 攻撃力
        [SerializeField] int attackVal;
        public int AttackVal {
            get {
                int extraVal = GM._.ugm.upgAttack.Val;
                float extraPer = 1
                    + GM._.obm.GetAbilityValue(OREBLESS_ABT.ATK_PER)
                    + GM._.pfm.totalAttackPer;

                int result = Mathf.RoundToInt((attackVal + extraVal) * extraPer);
                Debug.Log($"AttackVal: ({attackVal} + {extraVal}) * {extraPer}=" + result);
                return result;
            }
        }

        // 攻撃速度
        [SerializeField] [Range(1, 7.5f)] float attackSpeed;
        public float AttackSpeed {
            get {
                float extraPer = 1
                    + GM._.ugm.upgAttackSpeed.Val
                    + GM._.obm.GetAbilityValue(OREBLESS_ABT.ATKSPD_PER);

                float result = attackSpeed * extraPer;
                Debug.Log($"AttackSpeed: {attackSpeed} * {extraPer}=" + result);
                return result;
            }
        }

        // 移動速度
        [SerializeField] float moveSpeed;
        public float MoveSpeed {
            get {
                float extraPer = 1
                    + GM._.ugm.upgMoveSpeed.Val
                    + GM._.obm.GetAbilityValue(OREBLESS_ABT.MOVSPD_PER);
                
                float result = moveSpeed * extraPer;
                Debug.Log($"MoveSpeed: {moveSpeed} * {extraPer}=" + result);
                return result;
            }
        }

        // カバンサイズ保管量
        [SerializeField] int bagStorageSize;
        public int BagStorageSize {
            get {
                float extraPer = 1
                    + GM._.ugm.upgBagStorage.Val
                    + GM._.obm.GetAbilityValue(OREBLESS_ABT.BAG_STG_PER);

                int result = Mathf.RoundToInt(bagStorageSize * extraPer);
                Debug.Log($"BagStorageSize: {bagStorageSize} * {extraPer}=" + result);
                return result;
            }
        }

        // 攻撃待機時間
        float attackWaitTime; 

        // カバン保管量
        [field:SerializeField] int bagStorage;
        public int BagStorage {
            get => bagStorage;
            set {
                bagStorage = value;

                // ０以上なら スライダーバー 表示
                bagStorageSlider.gameObject.SetActive(bagStorage > 0);

                // スライダーバー UI最新化
                if(bagStorageSlider.gameObject.activeSelf) {
                    bagStorageSlider.value = (float)bagStorage / BagStorageSize;
                    bagStorageSliderTxt.text = bagStorage.ToString();
                }
            }
        }

        public void Start()
        {
            _animation = GetComponent<CharacterAnimation>();
            rigid = GetComponent<Rigidbody2D>();
            sprRdr = GetComponentInChildren<SpriteRenderer>();

            // 実際の攻撃速度(秒)
            float attackSpeedSec = ATTACK_SPEED_MAX_SEC / AttackSpeed;
            Debug.Log($"attackSpeedSec= {attackSpeedSec}");

            BagStorage = 0;

            StartCoroutine(CoInitStatus());
        }

        /// <summary>
        /// 첫 캐릭터 생성 시, 점프 애니메이션 0.2초 후에 이동시작
        /// </summary>
        public IEnumerator CoInitStatus()
        {
            status = Status.SPAWN;
            yield return Util.TIME0_1;
            status = Status.GO;
        }

        /// <summary>
        /// 타겟 이동관련 처리
        /// </summary>
        void FixedUpdate()
        {
            if(GM._.gameState == GameState.TIMEOVER)
            {
                if(_animation.GetState() != CharacterState.Die)
                    _animation.Die();
                return;
            }

            if(status == Status.SPAWN
            || status == Status.CLEARSTAGE)
            {
                Debug.Log($"status= {status}");
                return;
            }

            //* 채굴하러 이동중인 경우
            if(status == Status.GO)
            {
                // 달리기 애니메이션 실행 (1회)
                if(_animation.GetState() != CharacterState.Run)
                    _animation.Run();

                // 타겟광석이 없을 경우
                if(!targetOre)
                {
                    Ore ore = null;
                    Transform oreGroupTf = GM._.mnm.oreGroupTf;

                    float attackSpeedSec = ATTACK_SPEED_MAX_SEC / AttackSpeed; // 実際の攻撃速度(秒)
                    attackWaitTime = attackSpeedSec; // 처음 1회는 바로 공격하도록

                    // 타겟광석 설정
                    for(int i = 0; i < oreGroupTf.childCount; i++)
                    {
                        // 순서대로 타겟광석을 배분하기위한 균형카운트
                        int CurBalanceMiningCnt = 1 + GM._.mnm.CurTotalMiningCnt / oreGroupTf.childCount;

                        // i번째 광석대입
                        ore = oreGroupTf.GetChild(i).GetComponent<Ore>();

                        // 위 광석을 채굴중인 캐릭이 균형카운트보다 작다면, 루프 종료
                        if(ore.MiningCnt < CurBalanceMiningCnt)
                        {
                            Debug.Log($"Calc MiningCnt Balance: {CurBalanceMiningCnt} <- {GM._.mnm.CurTotalMiningCnt} / {oreGroupTf.childCount}");
                            break;
                        }
                    }

                    //* 도중에 다른 캐릭터가 타겟광석을 파괴했을 경우
                    if(ore == null)
                    {
                        status = Status.BACKHOME; // 귀가
                        targetOre = null;
                        return;
                    }

                    // 타겟 지정 완료
                    targetOre = ore;

                    targetOre.MiningCnt++;
                    GM._.mnm.CurTotalMiningCnt++;
                }

                // Sorting Layer 앞 배치를 위해, 타겟 Y좌표를 조금아래로 설정
                var pos = targetOre.transform.position;
                Vector3 underYTargetPos = new (pos.x, pos.y - TARGET_Y_UNDER_MINUS, pos.z);

                // 타겟 방향
                Vector2 dir = (underYTargetPos - transform.position).normalized;

                // 캐릭터 좌・우
                sprRdr.flipX = dir.x < 0;

                Vector2 moveVec = dir * MoveSpeed * Time.fixedDeltaTime;

                // 타겟으로 이동
                rigid.MovePosition(rigid.position + moveVec);
                rigid.velocity = Vector2.zero;

                // 타겟과의 거리
                float distance = Vector2.Distance(underYTargetPos, transform.position);
                Debug.Log($"GO:: distance= {distance}");

                //* 타겟광석에 도착했을 경우
                if(distance < REACH_TARGET_MIN_DIST)
                {
                    status = Status.MINING; // 채굴시작!
                }
            }
            //* 가방이 꽉차서 귀가하는 경우 (또는 광석이 채굴도중 파괴될 경우)
            else if(status == Status.BACKHOME)
            {
                // 가방들고 돌아오기 애니메이션 실행 (1회)
                if(_animation.GetState() != CharacterState.Crawl)
                    _animation.Crawl();

                Vector3 homePos = GM._.mnm.homeTf.position;

                // 집 방향
                Vector2 dir = (homePos - transform.position).normalized;

                // 캐릭터 좌・우
                sprRdr.flipX = dir.x < 0;

                // 가방이 무거우니까 속도낮춘 방향벡터
                const float WEIGHT_UP_SLOW_SPEED_PER = 0.9f;
                Vector2 moveVec = dir * (MoveSpeed * WEIGHT_UP_SLOW_SPEED_PER) * Time.fixedDeltaTime;

                // 집으로 이동
                rigid.MovePosition(rigid.position + moveVec);
                rigid.velocity = Vector2.zero;

                // 집과의 거리
                float distance = Vector2.Distance(homePos, transform.position);
                Debug.Log($"BACKHOME:: distance= {distance}");

                //* 집 도착
                if(distance < REACH_TARGET_MIN_DIST)
                {
                    Debug.Log("REACH HOME!");

                    // 재화이펙트 수량
                    const int ratio = 50;
                    int effectPlayCnt = bagStorage / ratio;
                    Debug.Log($"bagStorage({bagStorage}) / ratio({ratio}) -> playCnt= {effectPlayCnt}");

                    if(targetOre != null) {
                        // 재화 이펙트 재생
                        StartCoroutine(GM._.ui.CoPlayCoinAttractionPtcUIEF(
                            (effectPlayCnt <= 0)? 1 : effectPlayCnt, targetOre.OreType
                        ));
                    }

                    //* 재화 획득
                    if(targetOre != null) {
                        // 타겟재화 증가
                        DM._.DB.statusDB.SetRscArr((int)targetOre.OreType, BagStorage);
                        // 게임결과 획득한 보상 중 재화에 반영
                        GM._.pm.playResRwdArr[(int)targetOre.OreType] += BagStorage;
                    }

                    // 가방 비우기
                    BagStorage = 0;

                    //* 맵에 광석이 남아있는 경우
                    if(GM._.mnm.oreGroupTf.childCount > 0)
                    {
                        status = Status.GO; // 채굴하러 이동
                        _animation.Idle();
                    }
                    //* 맵에 광석이 없는 경우
                    else
                    {
                        status = Status.CLEARSTAGE; // 스테이지 클리어
                        _animation.Die(); // 쓰러짐 애니메이션

                        // 캐릭터 클리어 카운트 ++
                        GM._.mnm.workerClearStageStatusCnt++;

                        // 모든 캐릭터가 클리어 카운트 됬다면
                        if(GM._.mnm.workerClearStageStatusCnt >= GM._.ugm.upgIncPopulation.Val)
                        {
                            // 시련의광산 층 돌파 성공
                            if(GM._.stgm.OreType == RSC.CRISTAL)
                            {
                                GM._.gameState = GameState.TIMEOVER;
                                GM._.pm.StopCorTimer();
                                GM._.pm.timerTxt.text = "CLEAR!";
                                GM._.pm.Timeover(); // 타임오버
                                GM._.clm.BestFloor++;
                            }
                            // 일반광산 다음층으로
                            else
                            {
                                GM._.mnm.workerClearStageStatusCnt = 0; // 클리어카운트 초기화
                                StartCoroutine(GM._.stgm.CoNextStage()); // 다음 스테이지 이동
                            }
                        }
                    }

                    _animation.moveDustParticle.Stop();
                }
            }
        }

        /// <summary>
        /// 광석 채굴관련 처리
        /// </summary>
        void Update() {
            if(GM._.gameState == GameState.TIMEOVER)
                return;
            if(status == Status.SPAWN)
                return;

            //* 채굴중인 경우
            if(status == Status.MINING) {
                attackWaitTime += Time.deltaTime;

                // 実際の攻撃速度(秒)
                float attackSpeedSec = ATTACK_SPEED_MAX_SEC / AttackSpeed; 

                // 달리는 애니메이션 정지
                if(_animation.GetState() == CharacterState.Run)
                    _animation.Idle();

                //* 광석채굴 (공격)
                if(attackWaitTime > attackSpeedSec)
                {
                    attackWaitTime = 0;

                    // 타겟광석이 보물상자가 아닌 광석인 경우
                    if(targetOre.OreType != RSC.CRISTAL)
                    {
                        // 가방용량 증가
                        if(bagStorage < BagStorageSize)
                        {
                            BagStorage += AttackVal;
                        }
                        // 가방이 꽉찬 경우
                        else {
                            status = Status.BACKHOME; // 귀가
                            return;
                        }
                    }

                    // 채굴 애니메이션
                    _animation.Slash();

                    // 광석 HpBar 표시
                    if(targetOre && !targetOre.HpSlider.gameObject.activeSelf)
                        targetOre.HpSlider.gameObject.SetActive(true);

                    // 광석 체력감소
                    targetOre.DecreaseHp(AttackVal);

                    // 광석체력 0이라면, 파괴
                    if(targetOre.IsDestroied)
                    {
                        // 광석인 경우
                        if(targetOre.OreType != RSC.CRISTAL)
                        {
                            status = Status.BACKHOME; // 귀가
                        }
                        // 보물상자인 경우
                        else
                        {
                            GM._.fm.missionArr[(int)MISSION.MINING_CHEST_CNT].Exp++;
                            status = Status.GO;
                        }

                        targetOre = null;
                        GM._.fm.missionArr[(int)MISSION.MINING_ORE_CNT].Exp++;
                    }
                }
            }
        }
    }
}
