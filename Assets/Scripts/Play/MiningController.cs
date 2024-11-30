using System;
using System.Collections;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;
using static SoundManager;
using Random = UnityEngine.Random;

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
        private MineManager mnm;
        private CharacterAnimation _animation;
        private Rigidbody2D rigid;
        private SpriteRenderer sprRdr;

        //* ELEMENT
        public Sprite iconCharaImg;                    // UI표시 전용 아이콘 이미지
        public Slider bagStorageSlider;                // カバン保管量 スライダーバー
        public TMP_Text bagStorageSliderTxt;           // カバン保管量 スライダーバー テキスト
        public Ore targetOre;                          // 타겟광석
        public RSC targetOreType;                      // 타겟광석 타입 (타겟이 파괴되어도 타입으로 확인하기 위해)
        public GameObject[] buffFireEFArr;             // 버프 파이어 이펙트 배열 

        //* VALUE
        const float TARGET_Y_UNDER_MINUS = 0.5f;      // 고블린을 광석보다 앞으로 배치하기 위해, 타겟위치 Y값 낮출 값
        const float ATTACK_SPEED_MAX_SEC = 1.5f;       // 공격속도 최대치
        const float REACH_TARGET_MIN_DIST = 0.225f;    // 타겟지점 도달판단 최소거리(집, 광석)

        // 이름
        [field: SerializeField] public string Name {get; private set;}

        // 등급
        [field: SerializeField] public Enum.GRADE Grade {get; private set;}

        [Header("능력치")]
        // 공격력
        [SerializeField] int attackVal;
        public int AttackVal {
            get {
                int extraVal = GM._.sttm.ExtraAtk;
                float extraPer = 1 + GM._.sttm.ExtraAtkPer;

                int result = Mathf.RoundToInt((attackVal + extraVal) * extraPer);
                Debug.Log($"AttackVal: ({attackVal} + {extraVal}) * {extraPer}=" + result);
                return result;
            }
        }

        // 공격속도
        [SerializeField] [Range(0.5f, 7.5f)] float attackSpeed;
        public float AttackSpeed {
            get {
                float extraPer = 1 + GM._.sttm.ExtraAtkSpdPer;

                float result = attackSpeed * extraPer;
                Debug.Log($"AttackSpeed: {attackSpeed} * {extraPer}=" + result);
                return result;
            }
        }

        // 이동속도
        [SerializeField] float moveSpeed;
        public float MoveSpeed {
            get {
                float extraPer = 1 + GM._.sttm.ExtraMovSpdPer;

                float result = moveSpeed * extraPer;
                Debug.Log($"{this.name}:: MoveSpeed: {moveSpeed} * <color=yellow>{extraPer}</color>=" + result);
                return result;
            }
        }

        // 가방 수용량
        [SerializeField] int bagStorageSize;
        public int BagStorageSize {
            get {
                float extraPer = 1 + GM._.sttm.ExtraBagStgPer;

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
            mnm = GM._.mnm;
            _animation = GetComponent<CharacterAnimation>();
            rigid = GetComponent<Rigidbody2D>();
            sprRdr = GetComponentInChildren<SpriteRenderer>();

            // 버프이펙트 비표시 초기화
            SetBuffFireEF(isActive: false);

            // 첫 시작시 등장SFX
            int randomSFX = Random.Range((int)SFX.Jump1SFX, (int)SFX.Jump3SFX + 1);
            SoundManager._.PlaySfx((SFX)randomSFX);

            // 実際の攻撃速度(秒)
            float attackSpeedSec = ATTACK_SPEED_MAX_SEC / AttackSpeed;
            Debug.Log($"attackSpeedSec= {attackSpeedSec}");

            BagStorage = 0;

            StartCoroutine(CoInitStatus());
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

            if(status == Status.SPAWN || status == Status.CLEARSTAGE)
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
                    // 타겟타입 지정 완료
                    targetOreType = ore.OreType;

                    targetOre.MiningCnt++;
                    GM._.mnm.CurTotalMiningCnt++;
                }

                // Sorting Layer 앞 배치를 위해, 타겟 Y좌표를 조금아래로 설정
                var pos = targetOre.transform.position;
                Vector3 underYTargetPos = new (pos.x, pos.y - TARGET_Y_UNDER_MINUS, pos.z);

                // 타겟광석과의 거리
                float distance = Move(underYTargetPos);

                // 캐릭터의 이동 속도에 비례해 동적 거리 조건 생성
                float dynamicReachDist = REACH_TARGET_MIN_DIST + (MoveSpeed * 0.1f); // 보정 비율

                //* 타겟광석에 도착했을 경우
                if(distance < dynamicReachDist)
                {
                    status = Status.MINING; // 채굴시작!
                }
            }
            //* 귀가하는 경우
            else if(status == Status.BACKHOME)
            {
                // 가방에 채광한 광석이 있다면
                if(bagStorage > 0)
                {
                    //* 가방들고 돌아오기 애니메이션 실행 (1회)
                    if(_animation.GetState() != CharacterState.Crawl)
                        _animation.Crawl();
                }
                // 없다면
                else
                {
                    // 가방없이 달리기 애니메이션 실행 (1회)
                    if(_animation.GetState() != CharacterState.Run)
                        _animation.Run();
                }

                Vector3 homePos = GM._.mnm.homeTf.position;

                // 집과의 거리
                float distance = Move(homePos);

                // 캐릭터의 이동 속도에 비례해 동적 거리 조건 생성
                float dynamicReachDist = REACH_TARGET_MIN_DIST + (MoveSpeed * 0.05f); // 보정 비율

                if(MoveSpeed > 40)
                    Debug.Log($"moveSpeed={MoveSpeed}, distance({distance}) < dynamicReachDist({dynamicReachDist})");

                //* 집 도착
                if(distance < dynamicReachDist)
                {
                    Debug.Log("REACH HOME!");

                    if(BagStorage > 0) {
                        // 재화 수령
                        AcceptRsc(targetOreType, BagStorage);
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
                        if(GM._.mnm.workerClearStageStatusCnt >= GM._.mnm.workerGroupTf.childCount)
                        {
                            //* 시련의광산 층 돌파 성공
                            if(GM._.stgm.IsChallengeMode)
                            {
                                // 스테이지 클리어
                                GM._.gameState = GameState.TIMEOVER;
                                GM._.pm.StopCorTimer();
                                GM._.pm.Timeover();
                                GM._.clm.BestFloor++;
                            }
                            //* 일반광산 다음층으로
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
                //* 도중에 타겟광석이 파괴된다면
                if(targetOre.IsDestroied) // targetOre == null
                {
                    attackWaitTime = ATTACK_SPEED_MAX_SEC; // 공격대기시간 제거

                    //* 일반광석인 경우
                    if(targetOreType != RSC.CRISTAL)
                    {
                        // 수용량이 다 안차도 타겟파괴시 바로귀가
                        // status = Status.BACKHOME; // 귀가

                        // 수용량이 덜 채워져있다면
                        if(BagStorage < BagStorageSize)
                            status = Status.GO; // 다음광석을 찾으러 이동
                        else
                            status = Status.BACKHOME; // 귀가
                        return;
                    }
                    //* 보물상자인 경우
                    else
                    {
                        status = Status.GO; // 다음광석을 찾으러 이동
                    }

                }

                attackWaitTime += Time.deltaTime;

                // 実際の攻撃速度(秒)
                float attackSpeedSec = ATTACK_SPEED_MAX_SEC / AttackSpeed; 

                // 달리는 애니메이션 정지
                if(_animation.GetState() == CharacterState.Run)
                    _animation.Idle();

                //* 광석채굴 (공격)
                if(attackWaitTime > attackSpeedSec)
                {
                    SoundManager._.PlaySfx(SFX.AttackSFX);
                    attackWaitTime = 0;

                    //* 일반광석인 경우
                    if(targetOreType != RSC.CRISTAL)
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

                    // 광석 체력감소
                    DecreaseOreHpBar(targetOre, AttackVal);

                    // 광석체력 0이라면, 파괴
                    if(targetOre.IsDestroied) // targetOre == null
                    {
                        //* 일반광석인 경우
                        if(targetOreType != RSC.CRISTAL)
                        {
                            // 수용량이 다 안차도 타겟파괴시 바로귀가
                            // status = Status.BACKHOME; // 귀가

                            // 수용량이 덜 채워져있다면
                            if(BagStorage < BagStorageSize)
                                status = Status.GO; // 다음광석을 찾으러 이동
                            else
                                status = Status.BACKHOME; // 귀가

                            // 채굴한 광석이 없다면, 바로 광석타켓 삭제
                            if(bagStorage == 0)
                                targetOre = null;
                        }
                        //* 보물상자인 경우
                        else
                        {
                            status = Status.GO;
                        }
                    }
                }
            }
        }

#region FUNC
        /// <summary>
        /// 인게임 채굴재화 수령
        /// </summary>
        public static void AcceptRsc(RSC targetOreType, int val)
        {
            Debug.Log($"AcceptRsc(targetOre= {targetOreType}, val= {val}):: ");

            _.PlayRandomSfxs(SFX.ItemDrop1SFX, SFX.ItemDrop2SFX);

            //* 재화 이펙트 재생
            GM._.ui.PlayOreAttractionPtcUIEF(targetOreType);
            //* 재화 획득(타겟재화 증가)
            DM._.DB.statusDB.SetRscArr((int)targetOreType, val);
            // 게임결과 획득한 보상 중 재화에 반영
            GM._.pm.playResRwdArr[(int)targetOreType] += val;
        }

        /// <summary>
        /// 타겟광석 HP감소 및 HP Bar표시
        /// </summary>
        public static void DecreaseOreHpBar(Ore targetOre, int val)
        {
            // 광석 체력감소
            targetOre.DecreaseHp(val);

            // 광석 HpBar 표시
            if(targetOre && !targetOre.HpSlider.gameObject.activeSelf)
                targetOre.HpSlider.gameObject.SetActive(true);
        }


        /// <summary>
        /// 캐릭터 이동
        /// </summary>
        /// <param name="tgPos">타겟위치</param>
        /// <returns>타겟과의 거리</returns>
        private float Move(Vector3 tgPos)
        {
            // 방향
            Vector2 dir = (tgPos - transform.position).normalized;

            // 캐릭터 좌・우
            sprRdr.flipX = dir.x < 0;

            // 방향벡터
            Vector2 moveVec = dir * MoveSpeed * Time.fixedDeltaTime;

            // 타겟으로 이동
            rigid.MovePosition(rigid.position + moveVec);
            rigid.velocity = Vector2.zero;

            // 타겟과의 거리
            float distance = Vector2.Distance(tgPos, transform.position);

            return distance;
        }

        /// <summary>
        /// 첫 캐릭터 생성 시, 점프 애니메이션 0.2초 후에 이동시작
        /// </summary>
        public IEnumerator CoInitStatus()
        {
            status = Status.SPAWN;
            yield return mnm.waitSpawnToGoSec;
            status = Status.GO;
        }

        /// <summary>
        /// 캐릭터 버프파이어이펙트 활성화
        /// </summary>
        /// <param name="isActive">활성화, 비활성화</param>
        /// <param name="buffGradeIdx">버프파이어 색상 인덱스</param>
        public void SetBuffFireEF(bool isActive, int buffGradeIdx = 0)
        {
            if(isActive)
            {
                buffFireEFArr[buffGradeIdx].gameObject.SetActive(true);
            }
            else 
            {
                Array.ForEach(buffFireEFArr, buffFireEF => buffFireEF.gameObject.SetActive(false));
            }
        }
#endregion
    }
}
