using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class LoadSelectController : MonoBehaviour 
{
	public TitleController Titlecontroller;
	public TitleCanvas Titlecanvas;
	public Text LoadGameCursor;
	public Text NewGameCursor;

	public int NowSelect;

	// 1F前の上入力
	private bool _PreTopInput;
	// 1F前の下入力
	private bool _PreUnderInput;

	// 1F前の左入力
	private bool _PreLeftInput;
	// 1F前の右入力
	private bool _PreRightInput;

	public bool ModeSelectDone;

	public LoadFileSelect Loadfileselect;

	// Use this for initialization
	void Start () 
	{
		// 短押し判定(前フレーム押してない）		
		this.UpdateAsObservable().Where(_ => Titlecanvas.Animatorstate.fullPathHash == Animator.StringToHash("Base Layer.LoadSelect")).Subscribe(_ =>
		{
			// 上
			if (!_PreTopInput && ControllerManager.Instance.Top)
			{
				if (NowSelect > 0)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowSelect--;
				}
			}
			// 下
			if (!_PreUnderInput && ControllerManager.Instance.Under)
			{
				if (NowSelect < 1)
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
			if (!ControllerManager.Instance.Shot)
			{
				ModeSelectDone = false;
			}

			// カーソル位置制御
			switch (NowSelect)
			{
				case 0:
					LoadGameCursor.gameObject.SetActive(true);
					NewGameCursor.gameObject.SetActive(false);
					break;
				case 1:
					LoadGameCursor.gameObject.SetActive(false);
					NewGameCursor.gameObject.SetActive(true);
					break;
			}
			// 選択確定時の処理
			if (ControllerManager.Instance.Shot && !ModeSelectDone)
			{
				AudioManager.Instance.PlaySE("OK");
				// saveフォルダがない場合、saveフォルダを作る
				if (!System.IO.Directory.Exists("save"))
				{
					//System.IO.DirectoryInfo di = 
					System.IO.Directory.CreateDirectory(@"save");
				}
				// save.savがない場合、save.savを作る
				if (!System.IO.File.Exists(@"save\save.sav"))
				{
					// データ作成
					string[] savedata = new string[MadokaDefine.SAVEDATANUM];
					// 以下のように埋める
					// 01:----/--/--/--:-- ----------------------
					// 02:----/--/--/--:-- ----------------------
					for (int i = 0; i < MadokaDefine.SAVEDATANUM; ++i)
					{
						savedata[i] = (i + 1).ToString("D3") + ":----/--/-- --:-- ----------------------";
					}
					// savedataを保存する
					System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
					//書き込むファイルを開く
					System.IO.StreamWriter sr = new System.IO.StreamWriter(@"save\save.sav", false, enc);
					for (int i = 0; i < MadokaDefine.SAVEDATANUM; ++i)
					{
						sr.WriteLine(savedata[i]);
					}
					sr.Close();
				}
				// LOAD GAMEの場合セーブファイル一覧を出して描きこむ
				if(NowSelect == 0)
				{
					Titlecanvas.TitleCanvasAnimator.Play(Titlecanvas.LoadFileSelectAppear);
					// 初期化する
					Loadfileselect.Initialize();
				}
				// NEW GAMEの場合ステートを初期化してプロローグへ移行
				else
				{
					FadeManager.Instance.LoadLevel("Prologue01", 1.0f);
				}
			}
			// 選択キャンセル時の処理
			else if(ControllerManager.Instance.Jump && !ModeSelectDone)
			{
				// ひとつ前に戻る
				Titlecanvas.TitleCanvasAnimator.Play(Titlecanvas.LogoFullAppear);
				AudioManager.Instance.PlaySE("cursor");
			}	
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
		
}
