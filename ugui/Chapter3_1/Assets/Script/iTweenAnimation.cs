using UnityEngine;
using System.Collections;

public class iTweenAnimation : MonoBehaviour 
{
    // 目的地の相対座標
    public Vector3 moveTarget = Vector3.zero;

    // 目的地に着くまでに要する時間
    public float duration = 1.0f;

    void StartAnimation()
    {
        iTween.MoveTo(gameObject, iTween.Hash(
            // 目的地の相対座標
            "position", moveTarget,
            // 相対座標の場合はtrueに、UIの移動で使用する場合はほぼこのオプションを有効にする必要がある
            "islocal", true,
            // Tween終了まで要する時間
            "time", duration,
            // イージングのタイプ。この場合は開始速度は速く、徐々にゆっくりとした動きをシミュレート
            "easetype", iTween.EaseType.easeOutQuad
            ));
    }
}
