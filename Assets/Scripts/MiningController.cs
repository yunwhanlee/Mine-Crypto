using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
        private float attackSpeedSec;               // 実際の攻撃速度(秒)
        private float attackWaitTime;               // 攻撃待機時間

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
            bagStorage = 0;
            stamina = staminaMax;
        }

        void FixedUpdate()
        {
            //* 採掘していないとき
            if(status == Status.GO)
            {
                // 一回 実行
                if(_animation.GetState() != CharacterState.Run)
                {
                    _animation.Run();
                    _animation.moveDustParticle.Play();
                }

                // ターゲット 指定
                if(!targetOre)
                {
                    // 最初一回すぐ攻撃
                    attackWaitTime = attackSpeedSec;

                    Ore ore = null;
                    for(int i = 0; i < GM._.mm.oreGroupTf.childCount; i++)
                    {   
                        ore = GM._.mm.oreGroupTf.GetChild(i).GetComponent<Ore>();
                        if(!ore.IsMining)
                            break;
                    }

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
                if(_animation.GetState() != CharacterState.Run)
                {
                    _animation.Run();
                    _animation.moveDustParticle.Play();
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
                    status = Status.GO; // 採掘しに行こう！
                    _animation.Idle();
                    _animation.moveDustParticle.Stop();
                    GM._.ui.coinAttractionPtcImg.Play();

                    // コイン 増加
                    GM._.mm.Coin += bagStorage;
                    bagStorage = 0;

                    // カバンイメージ 非表示
                    _charaBuilder.Back = "";
                    _charaBuilder.Rebuild();

                    // スタミナ 減る
                    stamina--;
                    if(stamina < 1)
                    {
                        status = Status.RECOVERTY;
                        _animation.Die();
                    }
                }
            }
        }

        void Update() {
            //* 採掘しているとき
            if(status == Status.MINING) {
                attackWaitTime += Time.deltaTime;

                //* 走るアニメーション 停止
                if(_animation.GetState() == CharacterState.Run)
                {
                    _animation.Idle();
                    _animation.moveDustParticle.Stop();
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

                        // カバンイメージ 表示
                        _charaBuilder.Back = "LargeBackpack";
                        _charaBuilder.Rebuild();

                        return;
                    }

                    attackWaitTime = 0;
                    _animation.Slash();

                    // 鉱石 HpBar 表示
                    if(!targetOre.HpSlider.gameObject.activeSelf)
                        targetOre.HpSlider.gameObject.SetActive(true);

                    // 攻撃
                    bool isDestory = targetOre.DecreaseHp(attackVal);
                    if(isDestory)
                    {
                        status = Status.BACKHOME; // 家に帰る
                        targetOre = null;

                        // カバンイメージ 表示
                        _charaBuilder.Back = "LargeBackpack";
                        _charaBuilder.Rebuild();
                    }
                }
            }
        }
    }
}
