using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;

public class ControllerManager : SingletonMonoBehaviour<ControllerManager> 
{
	// 各種キーの入力の有無

	/// <summary>
	/// 方向キー/左スティック上（コントローラー.斜めは複数同時）
	/// </summary>	
	public bool Top;

	/// <summary>
	/// 方向キー/左スティック下
	/// </summary>
	public bool Under;

	/// <summary>
	/// 方向キー/左スティック左
	/// </summary>
	public bool Left;

	/// <summary>
	/// 方向キー/左スティック右
	/// </summary>
	public bool Right;
	
	/// <summary>
	/// 射撃・決定
	/// </summary>
	public bool Shot;

	/// <summary>
	/// 格闘
	/// </summary>
	public bool Wrestle;

	/// <summary>
	/// ジャンプ・キャンセル
	/// </summary>
	public bool Jump;

	/// <summary>
	/// サーチ
	/// </summary>
	public bool Search;

	/// <summary>
	/// コマンド
	/// </summary>
	public bool Command;

	/// <summary>
	/// メニュー
	/// </summary>
	public bool Menu;

	/// <summary>
	/// サブ射撃
	/// </summary>
	public bool SubShot;

	/// <summary>
	/// 特殊射撃
	/// </summary>
	public bool EXShot;

	/// <summary>
	/// 特殊格闘
	/// </summary>
	public bool EXWrestle;

	/// <summary>
	/// 視点回転上(仰角）
	/// </summary>
	public bool ElevationAngleUpper;

	/// <summary>
	/// 視点回転下(仰角)
	/// </summary>
	public bool ElevationAngleDown;

	/// <summary>
	/// 視点回転左(方位角)
	/// </summary>
	public bool AzimuthLeft;

	/// <summary>
	/// 視点回転右(方位角)
	/// </summary>
	public bool AzimuthRight;

	/// <summary>
	/// ブーストダッシュ（ジャンプボタン２連打）検出
	/// </summary>
	public bool BoostDash;

	/// <summary>
	/// マジックバースト（射撃・格闘・ジャンプ）検出
	/// </summary>
	public bool MagicBurst;

	
	/// <summary>
	/// フロントステップ（前2回）検出
	/// </summary>
	public bool FrontStep;

	/// <summary>
	/// 左前ステップ（左上2回）検出
	/// </summary>
	public bool LeftFrontStep;

	/// <summary>
	/// 左ステップ（左2回）検出
	/// </summary>
	public bool LeftStep;

	/// <summary>
	/// 左後ステップ（左下2回）検出
	/// </summary>
	public bool LeftBackStep;

	/// <summary>
	/// 後ステップ（下2回）検出
	/// </summary>
	public bool BackStep;

	/// <summary>
	/// 右後ステップ（右下2回）検出
	/// </summary>
	public bool RightBackStep;

	/// <summary>
	/// 右ステップ（右2回）検出
	/// </summary>
	public bool RightStep;

	/// <summary>
	/// 右前ステップ（右上2回）検出
	/// </summary>
	public bool RightFrontStep;

	// Use this for initialization
	void Start () 
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);

		// 十字キー入力方向検出
		this.UpdateAsObservable().Subscribe(_ =>
		{
			float horizontal = Input.GetAxisRaw("Horizontal");        // 横入力を検出
			float vertical = Input.GetAxisRaw("Vertical");            // 縦入力を検出

			if (vertical > 0.0f && Math.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f)
			{
				Top = true;
			}
			Top = false;


		});


		// http://www.slideshare.net/torisoup/unity-unirx
		// 射撃検知

		// 格闘検知

		// 
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
