using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class ControllerSettingPopup : MonoBehaviour
{
    public Animator Controllersettingpopup;
	public KeyConfigController Keyconfigcontroller;

	/// <summary>
	/// 1個目のポップアップの文字列
	/// </summary>
	public Text Popup1Text;

	/// <summary>
	/// ２個目のポップアップのタイトル表示部
	/// </summary>
	public Text Popup2Title;

	/// <summary>
	/// ２個目のポップアップの質問文字列
	/// </summary>
	public Text Popup2QuestionText;

	/// <summary>
	/// ２個目のポップアップの結果文字列
	/// </summary>
	public Text Popup2AnswerText;

	/// <summary>
	/// 上方向の入力
	/// </summary>
	private string _UpperInput;

	/// <summary>
	/// 下方向の入力
	/// </summary>
	private string _DownInput;

	/// <summary>
	/// 左方向の入力
	/// </summary>
	private string _LeftInput;

	/// <summary>
	/// 右方向の入力
	/// </summary>
	private string _RightInput;

	/// <summary>
	/// 1F前のOK入力
	/// </summary>
	private bool _PreOKInput;

	/// <summary>
	/// 1F前のキャンセル入力
	/// </summary>
	private bool _PreCancelInput;

	/// <summary>
	/// 各ステートの登録番号
	/// </summary>
	public int Standby;
	public int OpenControllerSetting;
	public int CloseControllerSetting;
	public int Controller1EraseAndController2Appear;
	public int CloseController2Setting;

	public SETTINGTARGET SettingTarget;


	void Awake()
	{
		// モーション登録
		Standby = Animator.StringToHash("Base Layer.Standby");
		OpenControllerSetting = Animator.StringToHash("Base Layer.OpenControllerSetting");
		CloseControllerSetting = Animator.StringToHash("Base Layer.CloseControllerSetting");
		Controller1EraseAndController2Appear = Animator.StringToHash("Base Layer.Controller1EraseAndController2Appear");
		CloseController2Setting = Animator.StringToHash("Base Layer.CloseController2Setting");
	}

	/// <summary>
	/// 右スティック設定済みでここに来た場合、最初のポップアップを強制的に閉じる
	/// </summary>
	public void CloseThisPopUp()
	{
		Controllersettingpopup.Play("Base Layer.CloseControllerSetting");
	}

	

    /// <summary>
    /// OKボタンを押した場合、スティック設定に入る
    /// </summary>
    public void OnClickOKButton()
    {
		// ついでに再出現しないように元のフラグも折っておく
		Controllersettingpopup.SetBool("Conctroller2Appear", true);
		Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.RIGHTSTICK2UPPERCHECK;
		Popup2AnswerText.text = "";
	}

    /// <summary>
    /// キャンセルボタンを押した場合、キーコンフィグに移行する
    /// </summary>
    public void OnClickCancelButton()
    {
		// ついでに再出現しないように元のフラグも折っておく
		Controllersettingpopup.SetBool("SetRightStick", false);
		Controllersettingpopup.SetBool("ControllerSettingClose", true);
		Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.POPUPCLOSE;
	}

	
	/// <summary>
	/// ２個目のポップアップが出ているときのOKボタン
	/// </summary>
	public void OnClickOKButton2()
	{
		if(Keyconfigcontroller.Nowmode == KeyConfigController.NowMode.RIGHTSTICK2UPPERCHECK && Popup2AnswerText.text != "")
		{
			Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.RIGHTSTICK2DOWNCHECK;
			Popup2AnswerText.text = "";
		}
		else if(Keyconfigcontroller.Nowmode == KeyConfigController.NowMode.RIGHTSTICK2DOWNCHECK && Popup2AnswerText.text != "")
		{
			Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.RIGHTSTICK2LEFTCHECK;
			Popup2AnswerText.text = "";
		}
		else if (Keyconfigcontroller.Nowmode == KeyConfigController.NowMode.RIGHTSTICK2LEFTCHECK && Popup2AnswerText.text != "")
		{
			Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.RIGHTSTICK2RIGHTCHECK;
			Popup2AnswerText.text = "";
		}
		else if (Keyconfigcontroller.Nowmode == KeyConfigController.NowMode.RIGHTSTICK2RIGHTCHECK && Popup2AnswerText.text != "")
		{
			Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.RICHTSTICKFINALCHECK;
			Popup2Title.text = "";			
			Popup2AnswerText.text = "↑：" + _UpperInput + "\n↓：" + _DownInput + "\n←：" + _LeftInput + "\n→：" + _RightInput;
		}
		else if(Keyconfigcontroller.Nowmode == KeyConfigController.NowMode.RICHTSTICKFINALCHECK)
		{
			// 方向キーの場合
			if (SettingTarget == SETTINGTARGET.TENKEY)
			{
				// 設定情報保存
				PlayerPrefs.SetString("KeyUP",_UpperInput);
				PlayerPrefs.SetString("KeyDown", _DownInput);
				PlayerPrefs.SetString("KeyLeft", _LeftInput);
				PlayerPrefs.SetString("KeyRight", _RightInput);
				PlayerPrefs.SetInt("TenKeySetup",1);
				// 右スティック未設定の場合、右スティック選択画面表示
				if (PlayerPrefs.GetInt("RightStickSetup") < 1)
				{
					Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.SETTINGSTANDBY;
					Controllersettingpopup.Play("Base Layer.OpenControllerSetting");
					Controllersettingpopup.SetBool("Conctroller2Appear", false);
					SettingTarget = SETTINGTARGET.RIGHTSTICK;
				}
				else
				{
					// ポップアップを消す
					Controllersettingpopup.SetBool("Conctroller2Appear", false);
					Controllersettingpopup.SetBool("CloseController2", true);
					Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.POPUPCLOSE;
					
				}
			}
			// 右スティックの場合
			else if (SettingTarget == SETTINGTARGET.RIGHTSTICK)
			{
				// 下の枠に登録
				// ↑
				Keyconfigcontroller.ElevationUpperControllerText.text = _UpperInput;
				// ↓
				Keyconfigcontroller.ElevationDownControllerText.text = _DownInput;
				// ←
				Keyconfigcontroller.AzimuthLeftControllerText.text = _LeftInput;
				// →
				Keyconfigcontroller.AzimhthRightControllerText.text = _RightInput;
				// ポップアップを消す
				Controllersettingpopup.SetBool("Conctroller2Appear", false);
				Controllersettingpopup.SetBool("CloseController2", true);
				Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.POPUPCLOSE;
				PlayerPrefs.SetInt("RightStickSetup", 1);			
			}
		}
	}

	/// <summary>
	/// ２個目のポップアップが出ているときのキャンセルボタン
	/// </summary>
	public void OnClcikCancelButton2()
	{
		switch (Keyconfigcontroller.Nowmode)
		{
			// キャンセルによってクローズ
			case KeyConfigController.NowMode.RIGHTSTICK2UPPERCHECK:
			case KeyConfigController.NowMode.RIGHTSTICK2DOWNCHECK:
			case KeyConfigController.NowMode.RIGHTSTICK2LEFTCHECK:
			case KeyConfigController.NowMode.RIGHTSTICK2RIGHTCHECK:
				Controllersettingpopup.SetBool("Conctroller2Appear", false);
				Controllersettingpopup.SetBool("CloseController2", true);
				Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.POPUPCLOSE;
				break;
			// 最初からやり直し
			case KeyConfigController.NowMode.RICHTSTICKFINALCHECK:
				Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.RIGHTSTICK2UPPERCHECK;
				Popup2AnswerText.text = "";
				break;
		}
	}


	// Use this for initialization
	void Start ()
    {
		if(Controllersettingpopup == null)
		{
			Controllersettingpopup = GameObject.Find("ControllerSelectPopUp").GetComponent<Animator>();
		}

		//// OKキー入力ストリーム
		//var okkeystream = this.UpdateAsObservable().Where(_ => GetOKInput());
		//// OKキー解除ストリーム
		//var okkeyupstream = this.UpdateAsObservable().Where(_ => !GetOKInput());
		//// キャンセルキー入力ストリーム
		//var cancelkeystream = this.UpdateAsObservable().Where(_ => GetCancelInput());
		//// キャンセルキー解除ストリーム
		//var cancelkeyupstream = this.UpdateAsObservable().Where(_ => !GetCancelInput());

		//// 途中で離されたら解除
		//okkeystream.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.5)))
		//.TakeUntil(okkeyupstream).RepeatUntilDestroy(this.gameObject).Subscribe(_ =>
		//{
		//	_PreOKInput = false;
		//});
		//cancelkeystream.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.5)))
		//.TakeUntil(cancelkeyupstream).RepeatUntilDestroy(this.gameObject).Subscribe(_ =>
		//{
		//	_PreCancelInput = false;
		//});


		//// OKとキャンセルの入力受け取り
		//// 方向キー
		//this.UpdateAsObservable().Where(_=> _PreOKInput && SettingTarget == SETTINGTARGET.TENKEY).Subscribe(_ =>
		//{
		//	if (GetOKInput())
		//	{
		//		OnClickOKButton();
		//	}
		//});

		//// 右スティック
		//this.UpdateAsObservable().Where(_ => SettingTarget == SETTINGTARGET.RIGHTSTICK).Subscribe(_ =>
		//{
		//	if (GetOKInput() && !_PreOKInput)
		//	{
		//		OnClickOKButton2();
		//	}
		//	if (GetCancelInput() && !_PreCancelInput)
		//	{
		//		OnClickCancelButton();
		//	}
		//});

		// 2個目のポップアップのタイトル&質問文字列
		this.UpdateAsObservable().Subscribe(_ =>
		{
			switch (Keyconfigcontroller.Nowmode)
			{
				case KeyConfigController.NowMode.RIGHTSTICK2UPPERCHECK:
					if (SettingTarget == SETTINGTARGET.RIGHTSTICK)
					{
						Popup2Title.text = "INPUT \n \"RIGHTSTICK UPPER\"";
						Popup2QuestionText.text = "右スティックを上に倒してください\n中止する場合はCANCELをクリックしてください";
					}
					else if(SettingTarget == SETTINGTARGET.TENKEY)
					{
						Popup2Title.text = "INPUT \n \"TENKEY UPPER\"";
						Popup2QuestionText.text = "十字キーを上に倒してください\n中止する場合はCANCELをクリックしてください";
					}
					break;
				case KeyConfigController.NowMode.RIGHTSTICK2DOWNCHECK:
					if (SettingTarget == SETTINGTARGET.RIGHTSTICK)
					{
						Popup2Title.text = "INPUT \n \"RIGHTSTICK DOWN\"";
						Popup2QuestionText.text = "右スティックを下に倒してください\n中止する場合はCANCELをクリックしてください";
					}
					else if (SettingTarget == SETTINGTARGET.TENKEY)
					{
						Popup2Title.text = "INPUT \n \"TENKEY DOWN\"";
						Popup2QuestionText.text = "十字キーを下に倒してください\n中止する場合はCANCELをクリックしてください";
					}
					break;
				case KeyConfigController.NowMode.RIGHTSTICK2LEFTCHECK:
					if (SettingTarget == SETTINGTARGET.RIGHTSTICK)
					{
						Popup2Title.text = "INPUT \n \"RIGHTSTICK LEFT\"";
						Popup2QuestionText.text = "右スティックを左に倒してください\n中止する場合はCANCELをクリックしてください";
					}
					else if (SettingTarget == SETTINGTARGET.TENKEY)
					{
						Popup2Title.text = "INPUT \n \"TENKEY LEFT\"";
						Popup2QuestionText.text = "十字キーを左に倒してください\n中止する場合はCANCELをクリックしてください";
					}
					break;
				case KeyConfigController.NowMode.RIGHTSTICK2RIGHTCHECK:
					if (SettingTarget == SETTINGTARGET.RIGHTSTICK)
					{
						Popup2Title.text = "INPUT \n \"RIGHTSTICK RIGHT\"";
						Popup2QuestionText.text = "右スティックを右に倒してください\n中止する場合はCANCELをクリックしてください";
					}
					else if (SettingTarget == SETTINGTARGET.TENKEY)
					{
						Popup2Title.text = "INPUT \n \"TENKEY RIGHT\"";
						Popup2QuestionText.text = "十字キーを右に倒してください\n中止する場合はCANCELをクリックしてください";
					}
					break;
				case KeyConfigController.NowMode.RICHTSTICKFINALCHECK:
					if (SettingTarget == SETTINGTARGET.RIGHTSTICK)
					{
						Popup2Title.text = "SETTING COMPLETE!!";
						Popup2QuestionText.text = "右スティックの設定が完了しました\n確定する場合はOKを、やり直す場合はCANCELをクリックしてください";
					}
					else if (SettingTarget == SETTINGTARGET.TENKEY)
					{
						Popup2Title.text = "SETTING COMPLETE!!";
						Popup2QuestionText.text = "方向キーの設定が完了しました\n確定する場合はOKを、やり直す場合はCANCELをクリックしてください";
					}
					break;
					
			}	
		});

		// ジョイスティック入力を取得
		this.UpdateAsObservable().Subscribe(_ =>
		{
			if (GetStickInput() != "")
			{
				switch (Keyconfigcontroller.Nowmode)
				{
					case KeyConfigController.NowMode.RIGHTSTICK2UPPERCHECK:
						_UpperInput = GetStickInput();
						Popup2AnswerText.text = "上動作：" + _UpperInput + "\n上動作が認識されました\nＯＫボタンをクリックしてください";
						break;
					case KeyConfigController.NowMode.RIGHTSTICK2DOWNCHECK:
						_DownInput = GetStickInput();
						Popup2AnswerText.text = "下動作：" + _DownInput + "\n下動作が認識されました\nＯＫボタンをクリックしてください";
						break;
					case KeyConfigController.NowMode.RIGHTSTICK2LEFTCHECK:
						_LeftInput = GetStickInput();
						Popup2AnswerText.text = "左動作：" + _LeftInput + "\n左動作が認識されました\nＯＫボタンをクリックしてください";
						break;
					case KeyConfigController.NowMode.RIGHTSTICK2RIGHTCHECK:
						_RightInput = GetStickInput();
						Popup2AnswerText.text = "右動作：" + _RightInput + "\n右動作が認識されました\nＯＫボタンをクリックしてください";
						break;
				}
			}
		});
	}
	

	void LateUpdate ()
    {		
		_PreCancelInput = GetCancelInput();
	}

	public string GetStickInput()
	{
		//右スティック（追加）
		// 3rd取得
		if (Input.GetAxisRaw("Vertical2") < 0)
		{
			return "3rd PLUS";
		}
		else if (0 < Input.GetAxisRaw("Vertical2"))
		{			
			return "3rd MINUS";
		}
		// 5th取得
		if (Input.GetAxisRaw("Horizontal2") < 0)
		{
			//左に傾いている
			return "5th MINUS";
		}
		else if (0 < Input.GetAxisRaw("Horizontal2"))
		{
			//右に傾いている
			return "5th PLUS";
		}

		// 4th取得
		if (Input.GetAxisRaw("Vertical3") < 0)
		{
			return "4th PLUS";
		}
		else if (0 < Input.GetAxisRaw("Vertical3"))
		{
			return "4th MINUS";
		}
		else
		{
			//上下方向には傾いていない
		}
		// 6th取得
		if (Input.GetAxisRaw("Horizontal3") < 0)
		{
			return "6th MINUS";
		}
		else if (0 < Input.GetAxisRaw("Horizontal3"))
		{		
			return "6th PLUS";
		}

		// 7th取得
		if (Input.GetAxisRaw("Vertical4") < 0)
		{
			return "7th PLUS";
		}
		else if (0 < Input.GetAxisRaw("Vertical4"))
		{
			return "7th MINUS";
		}

		// 8th取得
		if (Input.GetAxisRaw("Horizontal4") < 0)
		{
			//左に傾いている
			return "8th MINUS";
		}
		else if (0 < Input.GetAxisRaw("Horizontal4"))
		{
			//右に傾いている
			return "8th PLUS";
		}

		return "";
	}

	/// <summary>
	/// 現段階で設定されているOKキーを取得する
	/// </summary>
	/// <returns></returns>
	public bool GetOKInput()
	{
		// キー入力
		if (Input.anyKeyDown)
		{
			Array k = Enum.GetValues(typeof(KeyCode));
			for (int i = 0; i < k.Length; i++)
			{
				// キー取得
				if (Input.GetKeyDown((KeyCode)k.GetValue(i)))
				{
					Debug.Log(k.GetValue(i).ToString());
					//キーボード
					//入力がテンキーとマウスクリックだったら無視
					if (k.GetValue(i).ToString().IndexOf("Arrow") < 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0 && k.GetValue(i).ToString().IndexOf("Joystick") < 0)
					{
						if (k.GetValue(i).ToString() == Keyconfigcontroller.ShotKeyboardText.text)
						{
							return true;
						}

					}
					// ジョイスティック
					// ジョイスティック1以外のボタンおよび軸入力は無視
					else 
					if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
					{
						if (Keyconfigcontroller.ShotControllerText.text == k.GetValue(i).ToString().Substring(9))
						{
							_PreOKInput = true;
							return true;
						}
					}
				}
				
			}
		}
		_PreOKInput = false;
		return false;
	}

	/// <summary>
	/// 現段階で設定されているOKキーの解除を取得する
	/// </summary>
	/// <returns></returns>
	public bool GetOKPull()
	{
		if(Input.anyKey)
		{
			Array k = Enum.GetValues(typeof(KeyCode));
			for (int i = 0; i < k.Length; i++)
			{
				if(Input.GetKeyUp((KeyCode)k.GetValue(i)))
				{
					// キーボード
					// 入力がテンキーとマウスクリックだったら無視
					if (k.GetValue(i).ToString().IndexOf("Arrow") < 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0 && k.GetValue(i).ToString().IndexOf("Joystick") < 0)
					{
						if (k.GetValue(i).ToString() == Keyconfigcontroller.ShotKeyboardText.text)
						{
							return true;
						}
					}
					// ジョイスティック
					// ジョイスティック1以外のボタンおよび軸入力は無視
					else if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
					{
						if (Keyconfigcontroller.ShotControllerText.text == k.GetValue(i).ToString().Substring(9))
						{
							return true;
						}
					}

					if (i == k.Length - 1)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	

	/// <summary>
	/// 現段階で設定されているキャンセルキーを受け取る
	/// </summary>
	/// <returns></returns>
	public bool GetCancelInput()
	{
		// キー入力
		if (Input.anyKeyDown)
		{
			Array k = Enum.GetValues(typeof(KeyCode));
			for (int i = 0; i < k.Length; i++)
			{
				// キー取得
				if (Input.GetKeyDown((KeyCode)k.GetValue(i)))
				{
					//Debug.Log(k.GetValue(i).ToString());
					// キーボード
					// 入力がテンキーとマウスクリックだったら無視
					if (k.GetValue(i).ToString().IndexOf("Arrow") < 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0 && k.GetValue(i).ToString().IndexOf("Joystick") < 0)
					{
						if (k.GetValue(i).ToString() == Keyconfigcontroller.JumpKeyboadText.text)
						{
							
							return true;
						}
					}
					// ジョイスティック
					// ジョイスティック1以外のボタンおよび軸入力は無視
					else if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
					{
						if (Keyconfigcontroller.JumpControllerText.text == k.GetValue(i).ToString().Substring(9))
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public bool GetPullCancel()
	{
		// キー入力
		if (Input.anyKey)
		{
			Array k = Enum.GetValues(typeof(KeyCode));
			for (int i = 0; i < k.Length; i++)
			{
				// キー取得
				if (Input.GetKeyUp((KeyCode)k.GetValue(i)))
				{
					//Debug.Log(k.GetValue(i).ToString());
					// キーボード
					// 入力がテンキーとマウスクリックだったら無視
					if (k.GetValue(i).ToString().IndexOf("Arrow") < 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0 && k.GetValue(i).ToString().IndexOf("Joystick") < 0)
					{
						if (k.GetValue(i).ToString() == Keyconfigcontroller.JumpKeyboadText.text)
						{
							return true;
						}
					}
					// ジョイスティック
					// ジョイスティック1以外のボタンおよび軸入力は無視
					else if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
					{
						if (Keyconfigcontroller.JumpControllerText.text == k.GetValue(i).ToString().Substring(9))
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}
}

public enum SETTINGTARGET
{
	TENKEY,
	RIGHTSTICK
}
