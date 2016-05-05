using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class MainmenuController : MonoBehaviour 
{
	/// <summary>
	/// カーソル
	/// </summary>
	public Text StoryCursor;
	public Text ArcadeCusor;
	public Text FreeMatchCursor;
	public Text OptionCursor;
	public Text ExitCursor;

	public int NowSelect;

	// 1F前の上入力
	private bool _PreTopInput;
	// 1F前の下入力
	private bool _PreUnderInput;

	// 1F前の左入力
	private bool _PreLeftInput;
	// 1F前の右入力
	private bool _PreRightInput;

	/// <summary>
	/// モードチェンジしたばかりか否か
	/// </summary>
	public bool ModeChangeDone;

	public LoadSelectController Loadselectcontroller;
	public TitleController Titlecontroller;
	
	// Use this for initialization
	void Start () 
	{
		// アップキーストリーム（押下）
		var upkeydownstream = this.UpdateAsObservable().Where(_ => ControllerManager.Instance.Top);
		// アップキーダウンストリーム(解除)
		var upkeyupstream = this.UpdateAsObservable().Where(_ => !ControllerManager.Instance.Top);
		// ダウンキーストリーム(押下）
		var downkeydownstram = this.UpdateAsObservable().Where(_ => ControllerManager.Instance.Under);
		// ダウンキーストリーム（解除）
		var downkeyupstream = this.UpdateAsObservable().Where(_ => !ControllerManager.Instance.Under);
		// 長押し判定(1秒以上で上か下へ強制移動)
		// 上
		upkeydownstream.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.5)))
		// 途中で離されたらストリームをリセット
		.TakeUntil(upkeyupstream).RepeatUntilDestroy(this.gameObject).Subscribe(_ =>
		{
			_PreTopInput = false;
		});
		// 下
		downkeydownstram.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.5)))
		// 途中で離されたらストリームをリセット
		.TakeUntil(downkeyupstream).RepeatUntilDestroy(this.gameObject).Subscribe(_ =>
		{
			_PreUnderInput = false;
		});
		// 短押し判定(前フレーム押してない）		
		this.UpdateAsObservable().Subscribe(_ => 
		{
			// 上
			if(!_PreTopInput && ControllerManager.Instance.Top)
			{
				if (NowSelect > 0)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowSelect--;
				}
			}
			// 下
			if(!_PreUnderInput && ControllerManager.Instance.Under)
			{
				if (NowSelect < 4)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowSelect++;
				}
			}

			
			// 入力更新
			// 上
			_PreTopInput = ControllerManager.Instance.Top;
			// 下
			_PreUnderInput = ControllerManager.Instance.Under;
			// ボタンを離すとモードチェンジ瞬間フラグを折る(一旦ボタンを離さないと次へ行かせられない）
			if(!ControllerManager.Instance.Shot)
			{
				ModeChangeDone = false;
			}
		});		
				




		this.UpdateAsObservable().Subscribe(_ => 
		{			
			// カーソル位置制御
			switch (NowSelect)
			{
				case 0:
					StoryCursor.gameObject.SetActive(true);
					ArcadeCusor.gameObject.SetActive(false);
					FreeMatchCursor.gameObject.SetActive(false);
					OptionCursor.gameObject.SetActive(false);
					ExitCursor.gameObject.SetActive(false);
					break;
				case 1:
					StoryCursor.gameObject.SetActive(false);
					ArcadeCusor.gameObject.SetActive(true);
					FreeMatchCursor.gameObject.SetActive(false);
					OptionCursor.gameObject.SetActive(false);
					ExitCursor.gameObject.SetActive(false);
					break;
				case 2:
					StoryCursor.gameObject.SetActive(false);
					ArcadeCusor.gameObject.SetActive(false);
					FreeMatchCursor.gameObject.SetActive(true);
					OptionCursor.gameObject.SetActive(false);
					ExitCursor.gameObject.SetActive(false);
					break;
				case 3:
					StoryCursor.gameObject.SetActive(false);
					ArcadeCusor.gameObject.SetActive(false);
					FreeMatchCursor.gameObject.SetActive(false);
					OptionCursor.gameObject.SetActive(true);
					ExitCursor.gameObject.SetActive(false);
					break;
				case 4:
					StoryCursor.gameObject.SetActive(false);
					ArcadeCusor.gameObject.SetActive(false);
					FreeMatchCursor.gameObject.SetActive(false);
					OptionCursor.gameObject.SetActive(false);
					ExitCursor.gameObject.SetActive(true);
					break;
			}
			// ショットキーが押されたときに遷移
			if(ControllerManager.Instance.Shot && !ModeChangeDone)
			{
				switch (NowSelect)
				{
					case 0:         // ゲームモード選択
						Loadselectcontroller.gameObject.SetActive(true);
						Loadselectcontroller.NowSelect = 0;
						Loadselectcontroller.ModeSelectDone = false;
						AudioManager.Instance.PlaySE("OK");
						Titlecontroller.TitleCanvasAnimator.Play("Base Layer.LoadSelect");
						break;
					case 1:         // アーケード開始
						break;
					case 2:			// フリーバトル開始
						break;
					case 3:			// オプション開始
						break;
					case 4:         // 終了確認ポップアップ表示
						break;
				}
			}
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}

