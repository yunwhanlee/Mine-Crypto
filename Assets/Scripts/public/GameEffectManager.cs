using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class GameEffectManager : MonoBehaviour
{
    List<ObjectPool<GameObject>> pool = new List<ObjectPool<GameObject>>();

    public enum UI_EF
    {
        NULL = -1,
        DmgTxtEF,
    }

    [field:Header("UI EFFECT")]
    [field:SerializeField] private GameObject DmgTxtEF;

    void Awake()
    {
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
    /// <summary>
    /// 데미지 텍스트UI EF
    /// </summary>
    public void ShowDmgTxtEF(Vector2 pos, int dmg)
        => StartCoroutine(CoShowDmgTxtEF(pos, dmg));

    IEnumerator CoShowDmgTxtEF(Vector2 pos, int dmg)
    {
        // 가져오기
        GameObject ins = pool[(int)UI_EF.DmgTxtEF].Get();
        DmgTextEF dmgTxtEF = ins.GetComponent<DmgTextEF>();
        ins.transform.position = pos;
        dmgTxtEF.txt.text = $"{dmg}";
        dmgTxtEF.DOTAnim.DORestart();

        // 돌려놓기
        yield return Util.TIME1;
        pool[(int)UI_EF.DmgTxtEF].Release(ins);
    }
#endregion
}
