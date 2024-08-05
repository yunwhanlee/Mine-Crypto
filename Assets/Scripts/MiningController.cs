using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            // RECOVERTY, // <- Stamina 회복중 상태
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
        public Slider bagStorageSlider;                // カバン保管量 スライダーバー
        public TMP_Text bagStorageSliderTxt;           // カバン保管量 スライダーバー テキスト
        public Ore targetOre;

        //* VALUE
        const float ATTACK_SPEED_MAX_SEC = 1.5f;       // 공격속도 최대치
        const float REACH_TARGET_MIN_DIST = 0.375f;    // 타겟지점 도달판단 최소거리(집, 광석)

        // 攻撃力
        [SerializeField] int attackVal;
        public int AttackVal {
            get {
                float extraVal = 1 * GM._.ugm.upgAttack.Val;
                return Mathf.RoundToInt(attackVal * extraVal);
            } 
        }

        // 攻撃速度
        [SerializeField] [Range(1, 7.5f)] float attackSpeed;
        public float AttackSpeed {
            get {
                float extraVal = 1 + GM._.ugm.upgAttackSpeed.Val;
                return attackSpeed * extraVal;
            }
        }

        // 移動速度
        [SerializeField] float moveSpeed;
        public float MoveSpeed {
            get {
                float extraVal = 1 + GM._.ugm.upgMoveSpeed.Val;
                return moveSpeed * extraVal;
            }
        }

        // カバンサイズ保管量
        [SerializeField] int bagStorageSize;
        public int BagStorageSize {
            get {
                float extraVal = bagStorageSize * GM._.ugm.upgBagStorage.Val;
                return bagStorageSize + Mathf.RoundToInt(extraVal);
            }
        }

        float attackWaitTime; // 攻撃待機時間

        // カバン保管量
        [field:SerializeField] int bagStorage;  public int BagStorage {
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

        // public int staminaMax;                      // スタミナMAX
        // public int stamina;                         // スタミナ
        // public int staminaRecoveryVal;              // スタミナ回復量

        public void Start()
        {
            _animation = GetComponent<CharacterAnimation>();
            rigid = GetComponent<Rigidbody2D>();
            sprRdr = GetComponentInChildren<SpriteRenderer>();
            // _character = GetComponent<Character>();
            // _charaBuilder = GetComponent<CharacterBuilder>();
            // _controller = GetComponent<CharacterController2D>();

            // 実際の攻撃速度(秒)
            float attackSpeedSec = ATTACK_SPEED_MAX_SEC / AttackSpeed;
            Debug.Log($"attackSpeedSec= {attackSpeedSec}");

            BagStorage = 0;

            StartCoroutine(CoInitStatus());
            // stamina = staminaMax;
        }

        /// <summary>
        /// 첫 고블린 생성 시, 점프 애니메이션 0.2초 후에 이동시작
        /// </summary>
        public IEnumerator CoInitStatus()
        {
            status = Status.SPAWN;
            yield return Util.TIME0_2;
            status = Status.GO;
        }

        void FixedUpdate()
        {
            if(status == Status.SPAWN
            || status == Status.CLEARSTAGE)
            {
                Debug.Log($"status= {status}");
                return;
            }

            //* 採掘していないとき
            if(status == Status.GO)
            {
                // 一回 実行
                if(_animation.GetState() != CharacterState.Run)
                {
                    _animation.Run();
                }

                // ターゲット 指定
                if(!targetOre)
                {
                    float attackSpeedSec = ATTACK_SPEED_MAX_SEC / AttackSpeed; // 実際の攻撃速度(秒)
                    attackWaitTime = attackSpeedSec; // 最初一回すぐ攻撃

                    Transform oreGroupTf = GM._.mnm.oreGroupTf;
                    Ore ore = null;

                    // ターゲット探す
                    for(int i = 0; i < oreGroupTf.childCount; i++)
                    {   
                        ore = oreGroupTf.GetChild(i).GetComponent<Ore>();

                        // ターゲットにするため、ループ終了
                        int CurBalanceMiningCnt = 1 + GM._.mnm.CurTotalMiningCnt / oreGroupTf.childCount;
                        if(ore.MiningCnt < CurBalanceMiningCnt)
                        {
                            Debug.Log($"Calc MiningCnt Balance: {CurBalanceMiningCnt} <- {GM._.mnm.CurTotalMiningCnt} / {oreGroupTf.childCount}");
                            break;
                        }
                    }

                    // 途中で他のゴブリンが破壊したら
                    if(ore == null)
                    {
                        status = Status.BACKHOME; // 家に帰る
                        targetOre = null;
                        return;
                    }

                    // ターゲット指定
                    targetOre = ore;
                    targetOre.MiningCnt++;
                    GM._.mnm.CurTotalMiningCnt++;
                }

                // 方向 指定
                Vector2 dir = (targetOre.transform.position - transform.position).normalized;

                // キャラの向き
                sprRdr.flipX = dir.x < 0;

                Vector2 moveVec = dir * MoveSpeed * Time.fixedDeltaTime;

                // ターゲットへ行く
                rigid.MovePosition(rigid.position + moveVec);
                rigid.velocity = Vector2.zero;

                // 距離
                float distance = Vector2.Distance(targetOre.transform.position, transform.position);
                Debug.Log($"GO:: distance= {distance}");

                //* 鉱石についたら
                if(distance < REACH_TARGET_MIN_DIST)
                {
                    status = Status.MINING; // 採掘開始
                }
            }
            //* 家に帰る
            else if(status == Status.BACKHOME)
            {
                // 一回 実行
                if(_animation.GetState() != CharacterState.Crawl)
                {
                    _animation.Crawl(); // カバン持って帰る
                }

                Vector3 homePos = GM._.mnm.homeTf.position;

                // 方向 指定
                Vector2 dir = (homePos - transform.position).normalized;

                // キャラの向き
                sprRdr.flipX = dir.x < 0;

                const float WEIGHT_UP_SLOW_SPEED_PER = 0.65f;
                Vector2 moveVec = dir * (MoveSpeed * WEIGHT_UP_SLOW_SPEED_PER) * Time.fixedDeltaTime;

                // ターゲットへ行く
                rigid.MovePosition(rigid.position + moveVec);
                rigid.velocity = Vector2.zero;

                // 距離
                float distance = Vector2.Distance(homePos, transform.position);
                Debug.Log($"BACKHOME:: distance= {distance}");

                //* 家に到着
                if(distance < REACH_TARGET_MIN_DIST)
                {
                    Debug.Log("REACH HOME!");

                    // コインEF 増加
                    const int ratio = 50;
                    int effectPlayCnt = bagStorage / ratio;
                    Debug.Log($"bagStorage({bagStorage}) / ratio({ratio}) -> playCnt= {effectPlayCnt}");
                    StartCoroutine(GM._.ui.CoPlayCoinAttractionPtcUIEF(effectPlayCnt <= 0? 1 : effectPlayCnt));
                    GM._.mnm.Coin += bagStorage;
                    BagStorage = 0;

                    if(GM._.mnm.oreGroupTf.childCount > 0)
                    {
                        status = Status.GO; // 採掘しに行こう！
                        _animation.Idle();
                    }
                    else
                    {
                        status = Status.CLEARSTAGE; // 採掘しに行こう！
                        _animation.Die();

                        GM._.mnm.workerClearStageStatusCnt++;

                        if(GM._.mnm.workerClearStageStatusCnt >= GM._.epm.WorkerCnt)
                        {
                            GM._.mnm.workerClearStageStatusCnt = 0;
                            StartCoroutine(GM._.stm.CoNextStage());
                        }
                    }

                    _animation.moveDustParticle.Stop();

                    // スタミナ 減る
                    // stamina--;
                    // if(stamina <= 0)
                    // {
                    //     Debug.Log("REACH HOME! DIE");
                    //     status = Status.RECOVERTY;
                    //     _animation.Die();
                    // }
                    // else {
                    // status = Status.GO; // 採掘しに行こう！
                    // _animation.Idle();
                    // }
                }
            }
        }

        void Update() {
            if(status == Status.SPAWN)
            {
                Debug.Log("WAIT !");
                return;
            }

            //* 採掘しているとき
            if(status == Status.MINING) {
                attackWaitTime += Time.deltaTime;
                float attackSpeedSec = ATTACK_SPEED_MAX_SEC / AttackSpeed; // 実際の攻撃速度(秒)

                //* 走るアニメーション 停止
                if(_animation.GetState() == CharacterState.Run)
                {
                    _animation.Idle();
                }

                //* 鉱石 攻撃（採掘）
                if(attackWaitTime > attackSpeedSec)
                {
                    // カバン ストレージ量 増加
                    if(bagStorage < BagStorageSize)
                    {
                        BagStorage += AttackVal;
                    }
                    // カバン ストレージ量が埋めたら
                    else {
                        status = Status.BACKHOME; // 家に帰る
                        return;
                    }

                    attackWaitTime = 0;
                    _animation.Slash();

                    // 鉱石 HpBar 表示
                    if(targetOre && !targetOre.HpSlider.gameObject.activeSelf)
                        targetOre.HpSlider.gameObject.SetActive(true);

                    // 攻撃
                    targetOre.DecreaseHp(AttackVal);

                    // 破壊
                    if(targetOre.IsDestroied)
                    {
                        status = Status.BACKHOME; // 家に帰る
                        targetOre = null;
                    }
                }
            }
        }
    }
}
