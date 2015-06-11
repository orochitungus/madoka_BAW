using UnityEngine;
using System.Collections;

public class ScriptAnimation : MonoBehaviour 
{
	// スタートとゴールはTimeStart = 0, ValueStart = 0, TimeEnd = 1, ValueEnd = 1に固定
    // ただし、スタートからゴールまでの過程はインスペクター上で自由に編集可能とする
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    // 移動する値をベクトルで指定
    public Vector3 moveDirection = Vector3.zero;
    // 目的地に着くまでに要する時間
    public float duration = 1.0f;

    void StartAnimation()
    {
        StartCoroutine(AnimationCoroutine()); 
    }

    IEnumerator AnimationCoroutine()
    {
        float startTime = Time.time;
        Vector3 initPosition = transform.position;
        // コルーチンにより、関数が呼ばれてからduration時間が経過するまでは、グラフに従った座標にpositionを変更
        while (Time.time - startTime < duration)
        {
            transform.position = initPosition + moveDirection * curve.Evaluate((Time.time - startTime) / duration);
            yield return 0;
        }
        // 目的地到達後は、オブジェクトの座標を目的地に代入
        transform.position = initPosition + moveDirection;
    }

    // プレイ中以外はギズモを表示してアニメーションの目的位置を視覚的に表示させておく
    void OnDrawGizimoSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + moveDirection, 5f);
            Gizmos.DrawLine(transform.position, transform.position + moveDirection);
        }
    }
}
