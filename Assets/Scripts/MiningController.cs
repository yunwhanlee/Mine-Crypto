using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts
{
    public class MiningController : MonoBehaviour
    {
        public enum Status {
            GO,
            MINING,
            BACKHOME,
            RECOVERTY,
        }

        private Character _character;
        private CharacterBuilder _charaBuilder;
        private CharacterController2D _controller;
        private CharacterAnimation _animation;
        private Rigidbody2D rigid;
        private SpriteRenderer sprRdr;

        public Status status;

        public int attackVal;                       // 攻撃力
        public float moveSpeed;                     // 移動速度

        [Range(1, 5)] public float attackSpeed;     // 攻撃速度値 -> MIN 1(1SEC : 1秒1回) ~ MAX 5(0.2SEC : 1秒1回))
        public float attackSpeedSec;               // 実際の攻撃速度(秒)
        public float attackWaitTime;               // 攻撃待機時間

        public int bagStorageMax;                   // カバンMAX保管量
        public int bagStorage;                      // カバン保管量

        public int staminaMax;                      // スタミナMAX
        public int stamina;                         // スタミナ
        public int staminaRecoveryVal;              // スタミナ回復量

        public Ore targetOre;


        public void Start()
        {
            _character = GetComponent<Character>();
            _charaBuilder = GetComponent<CharacterBuilder>();
            _controller = GetComponent<CharacterController2D>();
            _animation = GetComponent<CharacterAnimation>();
            rigid = GetComponent<Rigidbody2D>();
            sprRdr = GetComponentInChildren<SpriteRenderer>();

            status = Status.GO;
            attackSpeedSec = 1 / attackSpeed;
            Debug.Log($"attackSpeedSec= {attackSpeedSec}");
            bagStorage = 0;
            stamina = staminaMax;
        }

        void FixedUpdate()
        {
            if(status == Status.RECOVERTY)
            {
                Debug.Log("RECOVERY !");
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
                    attackWaitTime = attackSpeedSec; // 最初一回すぐ攻撃

                    Ore ore = null;

                    // ターゲット探す
                    for(int i = 0; i < GM._.mm.oreGroupTf.childCount; i++)
                    {   
                        ore = GM._.mm.oreGroupTf.GetChild(i).GetComponent<Ore>();

                        // ターゲットにするため、ループ終了
                        if(ore.IsMining == false)
                            break;
                    }

                    // 途中で他のゴブリンが破壊したら
                    if(ore == null) {
                        status = Status.BACKHOME; // 家に帰る
                        targetOre = null;
                        return;
                    }

                    // ターゲット指定
                    targetOre = ore;
                    targetOre.IsMining = true;
                }

                // 方向 指定
                Vector2 dir = (targetOre.transform.position - transform.position).normalized;

                // キャラの向き
                sprRdr.flipX = dir.x < 0;

                Vector2 moveVec = dir * moveSpeed * Time.fixedDeltaTime;

                // ターゲットへ行く
                rigid.MovePosition(rigid.position + moveVec);
                rigid.velocity = Vector2.zero;

                // 距離
                float distance = Vector2.Distance(targetOre.transform.position, transform.position);
                Debug.Log($"GO:: distance= {distance}");

                //* 鉱石についたら
                if(distance < 0.1f)
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

                Vector3 homePos = GM._.mm.homeTf.position;

                // 方向 指定
                Vector2 dir = (homePos - transform.position).normalized;

                // キャラの向き
                sprRdr.flipX = dir.x < 0;

                const float WEIGHT_UP_SLOW_SPEED_PER = 0.7f;
                Vector2 moveVec = dir * (moveSpeed * WEIGHT_UP_SLOW_SPEED_PER) * Time.fixedDeltaTime;

                // ターゲットへ行く
                rigid.MovePosition(rigid.position + moveVec);
                rigid.velocity = Vector2.zero;

                // 距離
                float distance = Vector2.Distance(homePos, transform.position);
                Debug.Log($"BACKHOME:: distance= {distance}");

                //* 家に到着
                if(distance < 0.1f)
                {
                    Debug.Log("REACH HOME!");

                    // コイン 増加
                    const int ratio = 50;
                    int playCnt = bagStorage / ratio;
                    Debug.Log($"bagStorage({bagStorage}) / ratio({ratio}) -> playCnt= {playCnt}");
                    StartCoroutine(GM._.ui.CoPlayCoinAttractionPtcUIEF(playCnt <= 0? 1 : playCnt));
                    GM._.mm.Coin += bagStorage;
                    bagStorage = 0;

                    // スタミナ 減る
                    stamina--;
                    if(stamina <= 0)
                    {
                        Debug.Log("REACH HOME! DIE");
                        status = Status.RECOVERTY;
                        _animation.Die();
                    }
                    else {
                        status = Status.GO; // 採掘しに行こう！
                        _animation.Idle();
                    }

                    _animation.moveDustParticle.Stop();
                }
            }
        }

        void Update() {
            if(status == Status.RECOVERTY)
            {
                Debug.Log("RECOVERY !");
                return;
            }

            //* 採掘しているとき
            if(status == Status.MINING) {
                attackWaitTime += Time.deltaTime;

                //* 走るアニメーション 停止
                if(_animation.GetState() == CharacterState.Run)
                {
                    _animation.Idle();
                }

                //* 鉱石 攻撃（採掘）
                if(attackWaitTime > attackSpeedSec)
                {
                    // カバン ストレージ量 増加
                    if(bagStorage < bagStorageMax)
                    {
                        bagStorage += attackVal;
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
                    targetOre.DecreaseHp(attackVal);

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
