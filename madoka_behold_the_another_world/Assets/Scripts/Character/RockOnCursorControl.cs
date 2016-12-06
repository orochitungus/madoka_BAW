using UnityEngine;
using System.Collections;

public class RockOnCursorControl : MonoBehaviour 
{
	/// <summary>
	/// ロックオンする相手の座標
	/// </summary>
	public Transform targetTrans; 
    public Vector3 offset = Vector3.zero;
	
	/// <summary>
	/// 表示するグラフィックの位置
	/// </summary>
	public Vector2 RockonCursorPos;

	/// <summary>
	/// 表示するグラフィックの親オブジェクト
	/// </summary>
	public RectTransform parentRectTrans;
	public Camera uiCamera;

	void Awake()
	{
		var canvasArr = GetComponentsInParent<Canvas>();
		for (int i = 0; i < canvasArr.Length; i++)
		{
			if (canvasArr[i].isRootCanvas)
			{
				uiCamera = canvasArr[i].worldCamera;
			}
		}
	}

	void Update()
	{
		if (targetTrans != null && parentRectTrans != null && uiCamera != null)
		{
			UpdateUiLocalPosFromTargetPos();
		}
	}

	/// <summary>
	/// 指定した画像の位置を決める
	/// </summary>
	public void UpdateUiLocalPosFromTargetPos()
	{
		// メインカメラのスクリーンポジションを取得
		var screenPos = Camera.main.WorldToScreenPoint(targetTrans.position + offset);
		var localPos = Vector2.zero;
		// 表示したいUI（この場合ロックカーソルなど）の表示したいUIの親RectTransform配下のローカルポジションを取得する
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTrans, screenPos, uiCamera, out localPos);
		RockonCursorPos = localPos;
	}
}
