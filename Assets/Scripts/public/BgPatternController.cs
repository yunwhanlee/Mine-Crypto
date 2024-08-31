using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgPatternController : MonoBehaviour {
    const float speed = 1.5f;
    Vector2 dir = new Vector2(-1, -1);
    Vector2 startLocalPos;
    float minY;

    void Start() {
        startLocalPos = transform.localPosition;
        minY = -startLocalPos.y;
    }

    void Update() {
        //* 斜面に動ける
        transform.Translate(speed * Time.deltaTime * dir);
        //* 位置を初期に戻せる
        if(transform.localPosition.y < minY)
            transform.localPosition = startLocalPos;
    }
}
