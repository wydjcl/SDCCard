using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBarPiece : MonoBehaviour
{
    public Vector2 startPos;
    public Vector2 endPos;
    public float speed = 100f;
    public float height = 100f;      // 抛物线高度
    public float duration = 0.6f;    // 飞行时间
    public float lifeTime = 2f;    // 存活时间

    RectTransform rt;
    float timer;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Init(Vector2 start, Vector2 end)
    {
        startPos = start;
        endPos = end;
        timer = 0;
    }

    void Update()
    {
        rt.Rotate(0, 0, -speed * Time.deltaTime);
        timer += Time.deltaTime;

        float t = timer / duration;

        // 👉 抛物线公式
        Vector2 pos = Vector2.Lerp(startPos, endPos, t);
        pos.y += Mathf.Sin(t * Mathf.PI) * height;

        rt.anchoredPosition = pos;

        // 👉 结束后等待销毁
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
