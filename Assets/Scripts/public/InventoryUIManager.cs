using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enum;

public class InventoryUIManager : MonoBehaviour
{
    //* ELEMENT
    public GameObject windowObj;                // 인벤토리 팝업
    public InvSlotUI[] invSlotUIArr;      // 인벤토리 슬롯UI 객체배열

    [Header("모든 인벤토리 아이템슬롯을 ContentTf안에 미리 만들고 IDX로 처리")]
    public Transform contentTf;

    void Start() {
        InitDataAndUI();
    }

#region FUNC
    /// <summary>
    /// 인벤토리 모든 아이템 데이터 및 객체 초기화 (1회)
    /// </summary>
    private void InitDataAndUI() {
        // 인벤토리 슬롯UI를 배열로 저장 (미리 ContentTf에 슬롯UI 추가하여 준비)
        invSlotUIArr = new InvSlotUI[contentTf.childCount];
        InitElement();
    }

    /// <summary>
    /// 모든 아이템슬롯UI 객체 초기화 (1회)
    /// </summary>
    private void InitElement() {
        for(int i = 0; i < contentTf.childCount; i++)
        {
            var itemSlotUI = contentTf.GetChild(i);

            invSlotUIArr[i] = new InvSlotUI
            {
                name = INV_ITEM_INFO[i].name,
                invType = (INV)i,
                obj = itemSlotUI.gameObject,
                itemSpr = itemSlotUI.GetChild(1).GetComponent<Image>().sprite,
                cntTxt = itemSlotUI.GetComponentInChildren<TMP_Text>(),
                contentMsg = INV_ITEM_INFO[i].content
            };

            var bgBtn = invSlotUIArr[i].obj.GetComponentInChildren<Button>();
            int copyIdx = i;

            //* 아이템슬롯 클릭이벤트 등록 => Description팝업 표시
            bgBtn.onClick.AddListener(() =>  GM._.idm.OnClickInvItemSlot((Enum.INV)copyIdx));
        }

        
    }

    public void ShowInventory() {
        windowObj.SetActive(true);
        UpdateSlotUI();
    }

    /// <summary>
    /// 인벤토리 슬롯UI 업데이트 (수량에 따른 슬롯 표시・비표시)
    /// </summary>
    public void UpdateSlotUI() {
        StatusDB sttDB = DM._.DB.statusDB;

        invSlotUIArr[(int)INV.ORE1].Active(sttDB.RscArr[(int)INV.ORE1]);
        invSlotUIArr[(int)INV.ORE2].Active(sttDB.RscArr[(int)INV.ORE2]);
        invSlotUIArr[(int)INV.ORE3].Active(sttDB.RscArr[(int)INV.ORE3]);
        invSlotUIArr[(int)INV.ORE4].Active(sttDB.RscArr[(int)INV.ORE4]);
        invSlotUIArr[(int)INV.ORE5].Active(sttDB.RscArr[(int)INV.ORE5]);
        invSlotUIArr[(int)INV.ORE6].Active(sttDB.RscArr[(int)INV.ORE6]);
        invSlotUIArr[(int)INV.ORE7].Active(sttDB.RscArr[(int)INV.ORE7]);
        invSlotUIArr[(int)INV.ORE8].Active(sttDB.RscArr[(int)INV.ORE8]);
        invSlotUIArr[(int)INV.CRISTAL].Active(sttDB.RscArr[(int)INV.CRISTAL]);
        invSlotUIArr[(int)INV.ORE_TICKET].Active(sttDB.OreTicket);
        invSlotUIArr[(int)INV.RED_TICKET].Active(sttDB.RedTicket);
        invSlotUIArr[(int)INV.TREASURE_CHEST].Active(sttDB.TreasureChest);
        invSlotUIArr[(int)INV.ORE_CHEST].Active(sttDB.OreChest);
        //TODO 연금술 재료 등록 및 오브젝트도 추가하기!
    }
#endregion
}
