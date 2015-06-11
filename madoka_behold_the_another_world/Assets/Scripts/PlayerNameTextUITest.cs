using UnityEngine;
using System.Collections;

public class PlayerNameTextUITest : MonoBehaviour 
{
    [SerializeField] private Transform target;
    private Vector3 screenPoint;

    void LateUpdate()
    {
        // メインカメラで撮影されているターゲットのポジションをスクリーン座標に変換
        screenPoint = Camera.main.WorldToScreenPoint(target.position);
        // 得られたスクリーン座標のXYの値をそのままUIのポジションに代入
        transform.position = new Vector3(screenPoint.x, screenPoint.y, 0);
    }
}
