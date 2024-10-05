using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class GameEffectManager : MonoBehaviour
{
    List<ObjectPool<GameObject>> pool = new List<ObjectPool<GameObject>>();

    public enum EFIDX
    {
        NULL = -1,
        OreBrokenEF1,
        OreBrokenEF2,
        OreBrokenEF3,
        OreBrokenEF4,
        OreBrokenEF5,
        OreBrokenEF6,
        OreBrokenEF7,
        OreBrokenEF8,
        TreasureBrokenEF,
        DmgTxtEF,
    }

    [field:Header("EFFECT")]
    [field:SerializeField] private GameObject[] OreBrokenEFArr;
    [field:SerializeField] private GameObject DmgTxtEF; // DmgTextEF Class

    void Awake()
    {
        pool.Add(Init(OreBrokenEFArr[0], max: 5));
        pool.Add(Init(OreBrokenEFArr[1], max: 5));
        pool.Add(Init(OreBrokenEFArr[2], max: 5));
        pool.Add(Init(OreBrokenEFArr[3], max: 5));
        pool.Add(Init(OreBrokenEFArr[4], max: 5));
        pool.Add(Init(OreBrokenEFArr[5], max: 5));
        pool.Add(Init(OreBrokenEFArr[6], max: 5));
        pool.Add(Init(OreBrokenEFArr[7], max: 5));
        pool.Add(Init(OreBrokenEFArr[8], max: 5));
        pool.Add(Init(DmgTxtEF, max: 50));
    }


#region POOL
    private ObjectPool<GameObject> Init(GameObject obj, int max)
    {
        return new ObjectPool<GameObject> (
            () => InstantiateEF(obj),
            OnGetEF,
            OnReleaseEF,
            Destroy,
            maxSize : max // 최대 생성 숫자
        );
    }
    /// <summary> /// 오브젝트 생성 /// </summary>
    private GameObject InstantiateEF(GameObject obj) => Instantiate(obj, transform);
    /// <summary> /// 오브젝트 가져오기 /// </summary>
    private void OnGetEF(GameObject obj) => obj.SetActive(true);
    /// <summary> /// 오브젝트 돌려놓기 /// </summary>
    private void OnReleaseEF(GameObject obj) => obj.SetActive(false);
#endregion

#region FUNC
    public void ShowEF(EFIDX efIdx, Vector2 pos)
        => StartCoroutine(CoShowEF(efIdx, pos));


    IEnumerator CoShowEF(EFIDX efIdx, Vector2 pos)
    {
        // 가져오기
        GameObject ins = pool[(int)efIdx].Get();
        ins.transform.position = pos;

        // 돌려놓기
        yield return Util.TIME1;
        pool[(int)efIdx].Release(ins);
    }

    /// <summary>
    /// 데미지 텍스트UI EF
    /// </summary>
    public void ShowDmgTxtEF(Vector2 pos, int dmg)
        => StartCoroutine(CoShowDmgTxtEF(pos, dmg));

    IEnumerator CoShowDmgTxtEF(Vector2 pos, int dmg)
    {
        // 가져오기
        GameObject ins = pool[(int)EFIDX.DmgTxtEF].Get();
        DmgTextEF dmgTxtEF = ins.GetComponent<DmgTextEF>();
        ins.transform.position = pos;
        dmgTxtEF.txt.text = $"{dmg}";
        dmgTxtEF.DOTAnim.DORestart();

        // 돌려놓기
        yield return Util.TIME1;
        pool[(int)EFIDX.DmgTxtEF].Release(ins);
    }
#endregion
}
