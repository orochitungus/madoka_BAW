using UnityEngine;
using System.Collections;

public class PlayerMotionUITest : MonoBehaviour
{

    public float duration = 0.4f;   // 移動にかかる時間（秒）
    // このスクリプトをインポートしたオブジェクトをX軸方向にdistanceの距離だけ移動させる
    public void MoveX(float distance)
    {
        StartCoroutine(MoveXCouroutine(distance));
    }
    public void MoveZ(float distance)
    {
        StartCoroutine(MoveZCouroutine(distance));
    }

    // durationの値の示す時間だけコルーチンによってX軸方向へ移動させる
    IEnumerator MoveXCouroutine(float distace)
    {
        float currentX = transform.position.x;
        float targetX = currentX + distace;
        float startTime = Time.time;
        float t = 0;

        while (t < 1.0f)
        {
            t = (Time.time - startTime) / duration;
            transform.position = new Vector3(Mathf.SmoothStep(currentX, targetX, t), 0, 0);
            yield return 0;
        }
    }

    // durationの値の示す時間だけコルーチンによってZ軸方向へ移動させる
    IEnumerator MoveZCouroutine(float distace)
    {
        float currentZ = transform.position.z;
        float targetZ = currentZ + distace;
        float startTime = Time.time;
        float t = 0;

        while (t < 1.0f)
        {
            t = (Time.time - startTime) / duration;
            transform.position = new Vector3(0, 0, Mathf.SmoothStep(currentZ, targetZ, t));
            yield return 0;
        }
    }
}
