using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 SORTING LAYER 자동 업데이트 (Y축)
/// </summary>
public class CharaSortingLayer : MonoBehaviour
{
    public SpriteRenderer sprRdr;

    void Start() {
        sprRdr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sprRdr.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
    }
}
