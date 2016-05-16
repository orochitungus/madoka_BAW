using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class KeyConfigController : MonoBehaviour
{
    /// <summary>
    /// 射撃・決定ボタン
    /// </summary>
    public Button Shot;

    /// <summary>
    /// 格闘ボタン
    /// </summary>
    public Button Wrestle;

    /// <summary>
    /// ジャンプ・キャンセルボタン
    /// </summary>
    public Button Jump;

    /// <summary>
    /// サーチボタン
    /// </summary>
    public Button Search;

    /// <summary>
    /// コマンドボタン
    /// </summary>
    public Button Command;

    /// <summary>
    /// メニューボタン
    /// </summary>
    public Button Menu;

    /// <summary>
    /// サブ射撃ボタン
    /// </summary>
    public Button SubShot;

    /// <summary>
    /// 特殊射撃ボタン
    /// </summary>
    public Button ExShot;

    /// <summary>
    /// 特殊格闘ボタン
    /// </summary>
    public Button ExWrestle;

    /// <summary>
    /// 視点回転上ボタン
    /// </summary>
    public Button ElevationAngleUpper;

    /// <summary>
    /// 視点回転下ボタン
    /// </summary>
    public Button ElevationAngleDown;

    /// <summary>
    /// 視点回転左ボタン
    /// </summary>
    public Button AzimuthLeft;

    /// <summary>
    /// 視点回転左ボタン
    /// </summary>
    public Button AzimuthRight;

	/// <summary>
	/// キャンセルボタン
	/// </summary>
	public Button CancelButton;

	/// <summary>
	/// アイテムボタン
	/// </summary>
	public Button Item;

    /// <summary>
    /// どれが選択されているか
    /// </summary>
    public enum NOWSELECT
    {
        SHOT,
        WRESTLE,
        JUMP,
        SEARCH,
        COMMAND,
        MENU,
		ITEM,
        SUBSHOT,
        EXSHOT,
        EXWRESTLE,
        ELEVATIONUPPER,
        ELEVETIONDOWN,
        AZIMUTHLEFT,
        AZIMUTHRIGHT
    }

    public enum NowMode
    {
        SETTINGSTANDBY,                 // 設定開始
        RIGHTSTICK2UPPERCHECK,          // 上入力の取得中
        RIGHTSTICK2DOWNCHECK,           // 下入力の取得中
        RIGHTSTICK2LEFTCHECK,           // 左入力の取得中
        RIGHTSTICK2RIGHTCHECK,          // 右入力の取得中
        RICHTSTICKFINALCHECK,           // ファイナルチェック
        POPUPCLOSE,                     // ポップアップ終了
    }

    public NowMode Nowmode;

    // 各カーソル
    public Image ShotKeyboard;
    public Image ShotController;

    public Image WrestleKeyboard;
    public Image WrestleController;

    public Image JumpKeyboad;
    public Image JumpController;

    public Image SearchKeyboard;
    public Image SearchController;

    public Image CommandKeyboard;
    public Image CommandController;

    public Image MenuKeyboard;
    public Image MenuController;

	public Image ItemKeyboard;
	public Image ItemController;

    public Image SubShotKeyboard;
    public Image SubShotController;

    public Image ExShotKeyboard;
    public Image ExShotController;

    public Image ExWrestleKeyboard;
    public Image ExWrestleController;

    public Image ElevationUpperKeyboard;
    public Image ElevationUpperController;

    public Image ElevetionDownKeyboard;
    public Image ElevationDownController;

    public Image AzimuthLeftKeyboard;
	public Image AzimuthLeftController;

	public Image AzimhthRightKeyboad;
	public Image AzimhthRightController;
    
	/// <summary>
	/// 現在選択中の項目
	/// </summary>
    public NOWSELECT Nowselect;

    /// <summary>
    /// 画面全体を制御するAnimator
    /// </summary>
    public Animator Controllersetting;


	/// <summary>
	/// 各テキスト
	/// </summary>
	public Text ShotKeyboardText;
	public Text ShotControllerText;

	public Text WrestleKeyboardText;
	public Text WrestleControllerText;

	public Text JumpKeyboadText;
	public Text JumpControllerText;

	public Text SearchKeyboardText;
	public Text SearchControllerText;

	public Text CommandKeyboardText;
	public Text CommandControllerText;

	public Text MenuKeyboardText;
	public Text MenuControllerText;

	public Text ItemKeyboardText;
	public Text ItemControllerText;

	public Text SubShotKeyboardText;
	public Text SubShotControllerText;

	public Text ExShotKeyboardText;
	public Text ExShotControllerText;

	public Text ExWrestleKeyboardText;
	public Text ExWrestleControllerText;

	public Text ElevationUpperKeyboardText;
	public Text ElevationUpperControllerText;

	public Text ElevetionDownKeyboardText;
	public Text ElevationDownControllerText;

	public Text AzimuthLeftKeyboardText;
	public Text AzimuthLeftControllerText;

	public Text AzimhthRightKeyboadText;
	public Text AzimhthRightControllerText;

	/// <summary>
	/// 各ステートの登録番号
	/// </summary>
	public int Standby;
	public int OpenControllerSetting;
	public int CloseControllerSetting;
	public int Controller1EraseAndController2Appear;
	public int CloseController2Setting;

	public ControllerSettingPopup Controllersettingpopup;

	void Awake()
	{
		// AudioManagerがあるか判定
		if (GameObject.Find("AudioManager") == null)
		{
			// なければ作る
			GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
			am.name = "AudioManager";   // このままだと名前にAudioManagerがつくので消しておく
		}
		// FadeManagerがあるか判定
		if (GameObject.Find("FadeManager") == null)
		{
			// 無ければ作る
			GameObject fadeManager = (GameObject)Instantiate(Resources.Load("FadeManager"));
			fadeManager.name = "FadeManager";
		}
		// LoadManagerがあるか判定
		if (GameObject.Find("LoadManager") == null)
		{
			// 無ければ作る
			GameObject loadManager = (GameObject)Instantiate(Resources.Load("LoadManager"));
			loadManager.name = "LoadManager";
		}
		// ControllerManagerがあるか判定
		if (GameObject.Find("ControllerManager") == null)
		{
			// 無ければ作る
			GameObject loadManager = (GameObject)Instantiate(Resources.Load("ControllerManager"));
			loadManager.name = "ControllerManager";
		}
        // PauseManagerがあるか判定
        //if (GameObject.Find("PauseManager") == null)
        //{
        //    // 無ければ作る
        //    GameObject pauseManager = (GameObject)Instantiate(Resources.Load("PauseManager"));
        //    pauseManager.name = "PauseManager";
        //}
        // ControllerSettingのモーション登録
        Standby = Animator.StringToHash("Base Layer.Standby");
		OpenControllerSetting = Animator.StringToHash("Base Layer.OpenControllerSetting");
		CloseControllerSetting = Animator.StringToHash("Base Layer.CloseControllerSetting");
		Controller1EraseAndController2Appear = Animator.StringToHash("Base Layer.Controller1EraseAndController2Appear");
		CloseController2Setting = Animator.StringToHash("Base Layer.CloseController2Setting");
	}

	// Use this for initialization
	void Start () 
	{
        // モード初期化
        Nowmode = NowMode.SETTINGSTANDBY;

        // 初期状態で十字キーと右コントローラーのセッティングが成されていなければ、セッティングに入る。キャンセルも切る
        this.UpdateAsObservable().Where(_ => Nowmode == NowMode.SETTINGSTANDBY).Subscribe(_AppDomain =>
        {
			// 十字キー未設定
            if (PlayerPrefs.GetInt("TenKeySetup") == 0)
            {
				Controllersettingpopup.Popup1Text.text = "コントローラーの十字キーの設定を行います\nこの操作を行わないと、コントローラーの十字キーが使えなくなります\nなお、オプション画面からの設定も可能となります\nアナログスティックのないコントローラーは設定不要です\n設定を行いますか？";
				Controllersettingpopup.SettingTarget = SETTINGTARGET.TENKEY;
				Controllersetting.SetBool("SetRightStick", true);
				CancelButton.interactable = false;
            }
			// 方向キー未設定
			else if (PlayerPrefs.GetInt("RightStickSetup") == 0)
			{
				Controllersettingpopup.Popup1Text.text = "コントローラーの右アナログスティックの設定を行います\nこの操作を行わないと、コントローラーの右スティックが使えなくなります\nなお、オプション画面からの設定も可能となります\n\n設定を行いますか？";
				Controllersettingpopup.SettingTarget = SETTINGTARGET.RIGHTSTICK;
				Controllersetting.SetBool("SetRightStick", true);
				CancelButton.interactable = false;
				// 各ステートを上書き(方向キー除く）
				OverWriteState(false);
			}
			// セッティングが成されていたらステートを切り替える（ポップアップを出さない）
			else
            {
                Nowmode = NowMode.POPUPCLOSE;
				Controllersetting.Play("Base Layer.CloseControllerSetting");
				// 各ステートを上書き
				OverWriteState(true);
            }
        });

		// 選択対象を初期化
		OnClickShotButton();
		


		// キー入力を取得
		this.UpdateAsObservable().Where(_ => Nowmode == NowMode.POPUPCLOSE).Subscribe(_ =>
		{
			GetKeyInput(Nowselect);			
		});
	}

	/// <summary>
	/// 各ボタンのステート表示を上書きする
	/// </summary>
	/// <param name="RightStick">右スティックの設定を書くか否か</param>
	public void OverWriteState(bool RightStick)
	{
		if (PlayerPrefs.GetString("Shot_Keyboard") != "")
		{
			ShotKeyboardText.text = PlayerPrefs.GetString("Shot_Keyboard");
		}
		else
		{
			ShotKeyboardText.text = "A";
		}
		if (PlayerPrefs.GetString("Shot_Controller") != "")
		{
			ShotControllerText.text = PlayerPrefs.GetString("Shot_Controller").Substring(9);
		}
		else
		{
			ShotControllerText.text = "Button0";
		}
		if(PlayerPrefs.GetString("Wrestle_Keyboard") != "")
		{
			WrestleKeyboardText.text = PlayerPrefs.GetString("Wrestle_Keyboard");
		}
		else
		{
			WrestleKeyboardText.text = "s";
		}
		if (PlayerPrefs.GetString("Wrestle_Controller") != "")
		{
			WrestleControllerText.text = PlayerPrefs.GetString("Wrestle_Controller").Substring(9);
		}
		else
		{
			WrestleControllerText.text = "Button1";
		}
		if (PlayerPrefs.GetString("Jump_Keyboard") != "")
		{
			JumpKeyboadText.text = PlayerPrefs.GetString("Jump_Keyboard");
		}
		else
		{
			JumpKeyboadText.text = "x";
		}
		if (PlayerPrefs.GetString("Jump_Controller") != "")
		{
			JumpControllerText.text = PlayerPrefs.GetString("Jump_Controller").Substring(9);
		}
		else
		{
			JumpControllerText.text = "Button2";
		}
		if (PlayerPrefs.GetString("Search_Keyboard") != "")
		{
			SearchKeyboardText.text = PlayerPrefs.GetString("Search_Keyboard");
		}
		else
		{
			SearchKeyboardText.text = "c";
		}
		if (PlayerPrefs.GetString("Search_Controller") != "")
		{
			SearchControllerText.text = PlayerPrefs.GetString("Search_Controller").Substring(9);
		}
		else
		{
			SearchControllerText.text = "Button3";
		}
		if (PlayerPrefs.GetString("Command_Keyboard") != "")
		{
			CommandKeyboardText.text = PlayerPrefs.GetString("Command_Keyboard");
		}
		else
		{
			CommandKeyboardText.text = "q";
		}
		if (PlayerPrefs.GetString("Commnd_Controller") != "")
		{
			CommandControllerText.text = PlayerPrefs.GetString("Commnd_Controller").Substring(9);
		}
		else
		{
			CommandControllerText.text = "Button4";
		}
		if (PlayerPrefs.GetString("Menu_Keyboard") != "")
		{
			MenuKeyboardText.text = PlayerPrefs.GetString("Menu_Keyboard");
		}
		else
		{
			MenuKeyboardText.text = "w";
		}
		if (PlayerPrefs.GetString("Menu_Controller") != "")
		{
			MenuControllerText.text = PlayerPrefs.GetString("Menu_Controller").Substring(9);
		}
		else
		{
			MenuControllerText.text = "Button5";
		}
		if (PlayerPrefs.GetString("Item_Keyboard") != "")
		{
			ItemKeyboardText.text = PlayerPrefs.GetString("Item_Keyboard");
		}
		else
		{
			ItemKeyboardText.text = "d";			
		}
		if (PlayerPrefs.GetString("Item_Controller") != "")
		{
			ItemControllerText.text = PlayerPrefs.GetString("Item_Controller").Substring(9);
		}
		else
		{
			ItemControllerText.text = "Button9";
		}
		if (PlayerPrefs.GetString("SubShot_Keyboard") != "")
		{
			SubShotKeyboardText.text = PlayerPrefs.GetString("SubShot_Keyboard");
		}
		else
		{
			SubShotKeyboardText.text = "e";
		}
		if (PlayerPrefs.GetString("SubShot_Controller") != "")
		{
			SubShotControllerText.text = PlayerPrefs.GetString("SubShot_Controller").Substring(9);
		}
		else
		{
			SubShotControllerText.text = "Button6";
		}
		if (PlayerPrefs.GetString("EXShot_Keyboard") != "")
		{
			ExShotKeyboardText.text = PlayerPrefs.GetString("EXShot_Keyboard");
		}
		else
		{
			ExShotKeyboardText.text = "r";
		}
		if(PlayerPrefs.GetString("EXShot_Controller") != "")
		{
			ExShotControllerText.text = PlayerPrefs.GetString("EXShot_Controller").Substring(9);
		}
		else
		{
			ExShotControllerText.text = "Button7";
		}
		if (PlayerPrefs.GetString("EXWrestle_Keyboad") != "")
		{
			ExWrestleKeyboardText.text = PlayerPrefs.GetString("EXWrestle_Keyboad");
		}
		else
		{
			ExWrestleKeyboardText.text = "t";
		}
		if(PlayerPrefs.GetString("EXWrestle_Controller") != "")
		{
			ExWrestleControllerText.text = PlayerPrefs.GetString("EXWrestle_Controller").Substring(9);
		}
		else
		{
			ExWrestleControllerText.text = "Button8";
		}

		if (!RightStick)
		{
			return; 
		}

		// キーボードの場合
		if (PlayerPrefs.GetString("ElevationUpper_Keyboard") != "")
		{
			ElevationUpperKeyboardText.text = PlayerPrefs.GetString("ElevationUpper_Keyboard");
		}
		else
		{
			ElevationUpperKeyboardText.text = "u";
		}

		// コントローラーの場合
		if (PlayerPrefs.GetString("ElevationUpper_Controller") != "")
		{
			// 軸設定
			if (ElevationUpperControllerText.text.IndexOf("Axis") >=0)
			{
				ElevationUpperControllerText.text =  PlayerPrefs.GetString("ElevationUpper_Controller");
			}
			// キー設定
			else
			{
				ElevationUpperControllerText.text =  PlayerPrefs.GetString("ElevationUpper_Controller").Substring(9);
			}
		}
		else
		{
			ElevationUpperControllerText.text = "-";
		}

		// キーボードの場合
		if (PlayerPrefs.GetString("ElevationDown_Keyboard") != "")
		{
			ElevetionDownKeyboardText.text = PlayerPrefs.GetString("ElevationDown_Keyboard");
		}
		else
		{
			ElevetionDownKeyboardText.text = "i";
		}
		// コントローラーの場合
		if (PlayerPrefs.GetString("ElevationDown_Controller") != "")
		{
			// 軸設定
			if (ElevationDownControllerText.text.IndexOf("Axis") >=0)
			{
				ElevationDownControllerText.text = PlayerPrefs.GetString("ElevationDown_Controller");
			}
			// キー設定
			else
			{
				ElevationDownControllerText.text = PlayerPrefs.GetString("ElevationDown_Controller").Substring(9);
			}
		}
		else
		{
			ElevationDownControllerText.text = "-";
		}

		// キーボードの場合
		if (PlayerPrefs.GetString("AzimuthLeft_Keyboard") != "")
		{
			AzimuthLeftKeyboardText.text = PlayerPrefs.GetString("AzimuthLeft_Keyboard");
		}
		else
		{
			AzimuthLeftKeyboardText.text = "o";
		}
		// コントローラーの場合
		if (PlayerPrefs.GetString("AzimuthLeft_Controller") != "")
		{
			// 軸設定
			if (ElevationDownControllerText.text.IndexOf("Axis") >=0)
			{
				AzimuthLeftControllerText.text = PlayerPrefs.GetString("AzimuthLeft_Controller");
			}
			// キー設定
			else
			{
				AzimuthLeftControllerText.text = PlayerPrefs.GetString("AzimuthLeft_Controller").Substring(9);
			}
		}
		else
		{
			AzimuthLeftControllerText.text = "-";
		}

		// キーボードの場合
		if(PlayerPrefs.GetString("AzimuthRight_Keyboard") != "")
		{
			AzimhthRightKeyboadText.text = PlayerPrefs.GetString("AzimuthRight_Keyboard");
		}
		else
		{
			AzimhthRightKeyboadText.text = "p";
		}
		// コントローラーの場合
		if (PlayerPrefs.GetString("AzimuthRight_Controller") != "")
		{
			// 軸設定
			if (ElevationDownControllerText.text.IndexOf("Axis") >=0)
			{
				AzimhthRightControllerText.text = PlayerPrefs.GetString("AzimuthRight_Controller");
			}
			// キー設定
			else
			{
				AzimhthRightControllerText.text = PlayerPrefs.GetString("AzimuthRight_Controller").Substring(9);
			}
		}
		else
		{
			AzimhthRightControllerText.text = "";
		}
	}

	public void GetKeyInput(NOWSELECT nowselect)
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
						// ダブり対策
						OverlapCheckKeyboard(k.GetValue(i).ToString(), nowselect);
						switch (nowselect)
						{
							case NOWSELECT.SHOT:
								ShotKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.WRESTLE:
								WrestleKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.SEARCH:
								SearchKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.JUMP:
								JumpKeyboadText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.COMMAND:
								CommandKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.MENU:
								MenuKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.ITEM:
								ItemKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.SUBSHOT:
								SubShotKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.EXSHOT:
								ExShotKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.EXWRESTLE:
								ExWrestleKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.ELEVATIONUPPER:
								ElevationUpperKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.ELEVETIONDOWN:
								ElevetionDownKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.AZIMUTHLEFT:
								AzimuthLeftKeyboardText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.AZIMUTHRIGHT:
								AzimhthRightKeyboadText.text = k.GetValue(i).ToString();
								break;
						}
					}
					// ジョイスティック
					// ジョイスティック1以外のボタンおよび軸入力は無視
					else if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
					{
						// ダブり対策
						OverlapCheckController(k.GetValue(i).ToString().Substring(9), nowselect);
						// ボタン取得
						switch (nowselect)
						{
							case NOWSELECT.SHOT:
								ShotControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.WRESTLE:
								WrestleControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.SEARCH:
								SearchControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.JUMP:
								JumpControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.COMMAND:
								CommandControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.MENU:
								MenuControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.ITEM:
								ItemControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.SUBSHOT:
								SubShotControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.EXSHOT:
								ExShotControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.EXWRESTLE:
								ExWrestleControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.ELEVATIONUPPER:
								ElevationUpperControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.ELEVETIONDOWN:
								ElevationUpperControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.AZIMUTHLEFT:
								AzimuthLeftControllerText.text = k.GetValue(i).ToString().Substring(9);
								break;
							case NOWSELECT.AZIMUTHRIGHT:
								AzimhthRightKeyboadText.text = k.GetValue(i).ToString().Substring(9);
								break;
						}
					}					
				}
			}
		}
	}
	
	/// <summary>
	/// 重複していた場合入れ替える(キーボード）
	/// </summary>
	/// <param name="insert">入力された文字列</param>
	/// <param name="select">現在選択中の項目</param>
	public void OverlapCheckKeyboard(string insert,NOWSELECT select)
	{
        // もともと入っていたボタンを取得
        string originalbuton = "";
        switch(select)
        {
            case NOWSELECT.SHOT:
                originalbuton = ShotKeyboardText.text;
                break;
            case NOWSELECT.WRESTLE:
                originalbuton = WrestleKeyboardText.text;
                break;
            case NOWSELECT.AZIMUTHLEFT:
                originalbuton = AzimuthLeftKeyboardText.text;
                break;
            case NOWSELECT.AZIMUTHRIGHT:
                originalbuton = AzimhthRightKeyboadText.text;
                break;
            case NOWSELECT.COMMAND:
                originalbuton = CommandKeyboardText.text;
                break;
			case NOWSELECT.ITEM:
				originalbuton = ItemKeyboardText.text;
				break;
            case NOWSELECT.ELEVATIONUPPER:
                originalbuton = ElevationUpperKeyboardText.text;
                break;
            case NOWSELECT.ELEVETIONDOWN:
                originalbuton = ElevetionDownKeyboardText.text;
                break;
            case NOWSELECT.EXSHOT:
                originalbuton = ExShotKeyboardText.text;
                break;
            case NOWSELECT.EXWRESTLE:
                originalbuton = ExWrestleKeyboardText.text;
                break;
            case NOWSELECT.JUMP:
                originalbuton = JumpKeyboadText.text;
                break;
            case NOWSELECT.MENU:
                originalbuton = MenuKeyboardText.text;
                break;
            case NOWSELECT.SEARCH:
                originalbuton = SearchKeyboardText.text;
                break;
            case NOWSELECT.SUBSHOT:
                originalbuton = SubShotKeyboardText.text;
                break;            
        }
        // 射撃に割り振っているのと同じボタンが入力された
        if (ShotKeyboardText.text == insert)
        {
            // もともと使われていたボタンを射撃ボタンに割り振る
            OverlapDone(originalbuton, NOWSELECT.SHOT);
        }
        // 格闘に割り振っているものと同じボタンが押された
        else if (WrestleKeyboardText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.WRESTLE);
        }
        // ジャンプに割り振っているものと同じボタンが押された
        else if (JumpKeyboadText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.JUMP);
        }
        // サーチに割り振っているものと同じボタンが押された
        else if (SearchKeyboardText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.SEARCH);
        }
        // コマンドに割り振っているものと同じボタンが押された
        else if (CommandKeyboardText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.COMMAND);
        }
		// アイテムに割り振っているものと同じボタンが押された
		else if(ItemKeyboardText.text == insert)
		{
			OverlapDone(originalbuton, NOWSELECT.ITEM);
		}
        // サブ射撃に割り振っているものと同じボタンが押された
        else if (SubShotKeyboardText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.SUBSHOT);
        }
        // 特殊射撃に割り振っているものと同じボタンが押された
        else if (ExShotKeyboardText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.EXSHOT);
        }
        // 特殊格闘に割り振っているものと同じボタンが押された
        else if (ExWrestleKeyboardText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.EXWRESTLE);
        }
        // 視点回転上に入っているものと同じボタンが押された
        else if (ElevationUpperKeyboardText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.ELEVATIONUPPER);
        }
        // 視点回転下に入っているものと同じボタンが押された
        else if(ElevetionDownKeyboardText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.ELEVETIONDOWN);
        }
        // 視点回転左に入っているものと同じボタンが押された
        else if(AzimuthLeftControllerText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.AZIMUTHLEFT);
        }
        // 視点回転右に入っているものと同じボタンが押された
        else if(AzimhthRightKeyboadText.text == insert)
        {
            OverlapDone(originalbuton, NOWSELECT.AZIMUTHRIGHT);
        }
    }

	/// <summary>
	/// 入れ替えがあるとき、元あったものを入れ替え先へ移動する(キーボード）
	/// </summary>
	/// <param name="origin">元あったもの</param>
	public void OverlapDone(string origin, NOWSELECT select)
	{
		// selectの文字列に入っていたものを射撃に割り振る
		switch (select)
		{
			case NOWSELECT.SHOT:
				ShotKeyboardText.text = origin;
				break;
			case NOWSELECT.WRESTLE:
				WrestleKeyboardText.text = origin;
				break;
			case NOWSELECT.SEARCH:
				SearchKeyboardText.text = origin;
				break;
			case NOWSELECT.JUMP:
				JumpKeyboadText.text = origin;
				break;
			case NOWSELECT.COMMAND:
				CommandKeyboardText.text = origin;
				break;
			case NOWSELECT.MENU:
				MenuKeyboardText.text = origin;
				break;
			case NOWSELECT.ITEM:
				ItemKeyboardText.text = origin;
				break;
			case NOWSELECT.SUBSHOT:
				SubShotKeyboardText.text = origin;
				break;
			case NOWSELECT.EXSHOT:
				ExShotKeyboardText.text = origin;
				break;
			case NOWSELECT.EXWRESTLE:
				ExWrestleKeyboardText.text = origin;
				break;
			case NOWSELECT.ELEVATIONUPPER:
				ElevationUpperKeyboardText.text = origin;
				break;
			case NOWSELECT.ELEVETIONDOWN:
				ElevetionDownKeyboardText.text = origin;
				break;
			case NOWSELECT.AZIMUTHLEFT:
				AzimuthLeftKeyboardText.text = origin;
				break;
			case NOWSELECT.AZIMUTHRIGHT:
				AzimhthRightKeyboadText.text = origin;
				break;
		}
	}

	/// <summary>
	/// 重複していた場合入れ替える（コントローラー）
	/// </summary>
	/// <param name="insert"></param>
	/// <param name="select"></param>
	public void OverlapCheckController(string insert, NOWSELECT select)
	{
		// もともと入っていたボタンを取得
		string originalbuton = "";
		switch (select)
		{
			case NOWSELECT.SHOT:
				originalbuton = ShotControllerText.text;
				break;
			case NOWSELECT.WRESTLE:
				originalbuton = WrestleControllerText.text;
				break;
			case NOWSELECT.AZIMUTHLEFT:
				originalbuton = AzimuthLeftControllerText.text;
				break;
			case NOWSELECT.AZIMUTHRIGHT:
				originalbuton = AzimhthRightControllerText.text;
				break;
			case NOWSELECT.COMMAND:
				originalbuton = CommandControllerText.text;
				break;
			case NOWSELECT.ITEM:
				originalbuton = ItemControllerText.text;
				break;
			case NOWSELECT.ELEVATIONUPPER:
				originalbuton = ElevationUpperControllerText.text;
				break;
			case NOWSELECT.ELEVETIONDOWN:
				originalbuton = ElevationDownControllerText.text;
				break;
			case NOWSELECT.EXSHOT:
				originalbuton = ExShotControllerText.text;
				break;
			case NOWSELECT.EXWRESTLE:
				originalbuton = ExWrestleControllerText.text;
				break;
			case NOWSELECT.JUMP:
				originalbuton = JumpControllerText.text;
				break;
			case NOWSELECT.MENU:
				originalbuton = MenuControllerText.text;
				break;
			case NOWSELECT.SEARCH:
				originalbuton = SearchControllerText.text;
				break;
			case NOWSELECT.SUBSHOT:
				originalbuton = SubShotControllerText.text;
				break;
		}
		// 射撃に割り振っているのと同じボタンが入力された
		if (ShotControllerText.text == insert)
		{
			// もともと使われていたボタンを射撃ボタンに割り振る
			OverlapDoneController(originalbuton, NOWSELECT.SHOT);
		}
		// 格闘に割り振っているものと同じボタンが押された
		else if (WrestleControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.WRESTLE);
		}
		// ジャンプに割り振っているものと同じボタンが押された
		else if (JumpControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.JUMP);
		}
		// サーチに割り振っているものと同じボタンが押された
		else if (SearchControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.SEARCH);
		}
		// コマンドに割り振っているものと同じボタンが押された
		else if (CommandControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.COMMAND);
		}
		else if(ItemControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.ITEM);
		}
		// サブ射撃に割り振っているものと同じボタンが押された
		else if (SubShotControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.SUBSHOT);
		}
		// 特殊射撃に割り振っているものと同じボタンが押された
		else if (ExShotControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.EXSHOT);
		}
		// 特殊格闘に割り振っているものと同じボタンが押された
		else if (ExWrestleControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.EXWRESTLE);
		}
		// 視点回転上に入っているものと同じボタンが押された
		else if (ElevationUpperControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.ELEVATIONUPPER);
		}
		// 視点回転下に入っているものと同じボタンが押された
		else if (ElevationDownControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.ELEVETIONDOWN);
		}
		// 視点回転左に入っているものと同じボタンが押された
		else if (AzimuthLeftControllerText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.AZIMUTHLEFT);
		}
		// 視点回転右に入っているものと同じボタンが押された
		else if (AzimhthRightKeyboadText.text == insert)
		{
			OverlapDoneController(originalbuton, NOWSELECT.AZIMUTHRIGHT);
		}
	}


	/// <summary>
	/// 入れ替えがあるとき、元あったものを入れ替え先へ移動する（コントローラー）
	/// </summary>
	/// <param name="origin"></param>
	/// <param name="select"></param>
	public void OverlapDoneController(string origin, NOWSELECT select)
	{
		// selectの文字列に入っていたものを射撃に割り振る
		switch (select)
		{
			case NOWSELECT.SHOT:
				ShotControllerText.text = origin;
				break;
			case NOWSELECT.WRESTLE:
				WrestleControllerText.text = origin;
				break;
			case NOWSELECT.SEARCH:
				SearchControllerText.text = origin;
				break;
			case NOWSELECT.JUMP:
				JumpControllerText.text = origin;
				break;
			case NOWSELECT.COMMAND:
				CommandControllerText.text = origin;
				break;
			case NOWSELECT.MENU:
				MenuControllerText.text = origin;
				break;
			case NOWSELECT.ITEM:
				ItemControllerText.text = origin;
				break;
			case NOWSELECT.SUBSHOT:
				SubShotControllerText.text = origin;
				break;
			case NOWSELECT.EXSHOT:
				ExShotControllerText.text = origin;
				break;
			case NOWSELECT.EXWRESTLE:
				ExWrestleControllerText.text = origin;
				break;
			case NOWSELECT.ELEVATIONUPPER:
				ElevationUpperControllerText.text = origin;
				break;
			case NOWSELECT.ELEVETIONDOWN:
				ElevationDownControllerText.text = origin;
				break;
			case NOWSELECT.AZIMUTHLEFT:
				AzimuthLeftControllerText.text = origin;
				break;
			case NOWSELECT.AZIMUTHRIGHT:
				AzimhthRightControllerText.text = origin;
				break;
		}

	}

	// Update is called once per frame
	void Update () 
	{
		        
    }

	/// <summary>
	/// 射撃ボタンが押された時の処理
	/// </summary>
	public void OnClickShotButton()
	{
		// 選択対象をSHOTにする
		Nowselect = NOWSELECT.SHOT;
        // 選択対象のカーソル色を赤にする
        ShotKeyboard.color = new Color(1, 0, 0);
        ShotController.color = new Color(1, 0, 0);
        // それ以外の時のカーソルの色を白にする
        WrestleKeyboard.color = new Color(1, 1, 1);
        WrestleController.color = new Color(1, 1, 1);

        JumpKeyboad.color = new Color(1, 1, 1);
        JumpController.color = new Color(1, 1, 1);

        SearchKeyboard.color = new Color(1, 1, 1);
        SearchController.color = new Color(1, 1, 1);

        CommandKeyboard.color = new Color(1, 1, 1);
        CommandController.color = new Color(1, 1, 1);

        MenuKeyboard.color = new Color(1, 1, 1);
        MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

        SubShotKeyboard.color = new Color(1, 1, 1);
        SubShotController.color = new Color(1, 1, 1);

        ExShotKeyboard.color = new Color(1, 1, 1);
        ExShotController.color = new Color(1, 1, 1);

        ExWrestleKeyboard.color = new Color(1, 1, 1);
        ExWrestleController.color = new Color(1, 1, 1);

        ElevationUpperKeyboard.color = new Color(1, 1, 1);
        ElevationUpperController.color = new Color(1, 1, 1);

        ElevetionDownKeyboard.color = new Color(1, 1, 1);
        ElevationDownController.color = new Color(1, 1, 1);

        AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
    }

    /// <summary>
    /// 格闘ボタンが押されたときの処理
    /// </summary>
    public void OnClickWrestleButton()
    {
        Nowselect = NOWSELECT.WRESTLE;

        // 選択対象のカーソルを赤にし、それ以外を白にする
        ShotKeyboard.color = new Color(1, 1, 1);
        ShotController.color = new Color(1, 1, 1);
        
        WrestleKeyboard.color = new Color(1, 0, 0);
        WrestleController.color = new Color(1, 0, 0);

        JumpKeyboad.color = new Color(1, 1, 1);
        JumpController.color = new Color(1, 1, 1);

        SearchKeyboard.color = new Color(1, 1, 1);
        SearchController.color = new Color(1, 1, 1);

        CommandKeyboard.color = new Color(1, 1, 1);
        CommandController.color = new Color(1, 1, 1);

        MenuKeyboard.color = new Color(1, 1, 1);
        MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
        SubShotController.color = new Color(1, 1, 1);

        ExShotKeyboard.color = new Color(1, 1, 1);
        ExShotController.color = new Color(1, 1, 1);

        ExWrestleKeyboard.color = new Color(1, 1, 1);
        ExWrestleController.color = new Color(1, 1, 1);

        ElevationUpperKeyboard.color = new Color(1, 1, 1);
        ElevationUpperController.color = new Color(1, 1, 1);

        ElevetionDownKeyboard.color = new Color(1, 1, 1);
        ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}    

    /// <summary>
    /// ジャンプボタンが押されたときの処理
    /// </summary>
    public void OnClickJump()
    {
        Nowselect = NOWSELECT.JUMP;
        // 選択対象のカーソルを赤にし、それ以外を白にする
        ShotKeyboard.color = new Color(1, 1, 1);
        ShotController.color = new Color(1, 1, 1);

        WrestleKeyboard.color = new Color(1, 1, 1);
        WrestleController.color = new Color(1, 1, 1);

        JumpKeyboad.color = new Color(1, 0, 0);
        JumpController.color = new Color(1, 0, 0);

        SearchKeyboard.color = new Color(1, 1, 1);
        SearchController.color = new Color(1, 1, 1);

        CommandKeyboard.color = new Color(1, 1, 1);
        CommandController.color = new Color(1, 1, 1);

        MenuKeyboard.color = new Color(1, 1, 1);
        MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
        SubShotController.color = new Color(1, 1, 1);

        ExShotKeyboard.color = new Color(1, 1, 1);
        ExShotController.color = new Color(1, 1, 1);

        ExWrestleKeyboard.color = new Color(1, 1, 1);
        ExWrestleController.color = new Color(1, 1, 1);

        ElevationUpperKeyboard.color = new Color(1, 1, 1);
        ElevationUpperController.color = new Color(1, 1, 1);

        ElevetionDownKeyboard.color = new Color(1, 1, 1);
        ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// サーチボタンが押された時の処理
	/// </summary>
	public void OnClickSearch()
	{
		Nowselect = NOWSELECT.SEARCH;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 0, 0);
		SearchController.color = new Color(1, 0, 0);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// コマンドボタンが押された時の処理
	/// </summary>
	public void OnClickCommand()
	{
		Nowselect = NOWSELECT.COMMAND;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 0, 0);
		CommandController.color = new Color(1, 0, 0);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// メニューボタンが押された時の処理
	/// </summary>
	public void OnClickMenu()
	{
		Nowselect = NOWSELECT.MENU;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 0, 0);
		MenuController.color = new Color(1, 0, 0);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// アイテムボタンが押された時の処理
	/// </summary>
	public void OnClickItem()
	{
		Nowselect = NOWSELECT.ITEM;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 0, 0);
		ItemController.color = new Color(1, 0, 0);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// サブ射撃が押された時の処理
	/// </summary>
	public void OnClickSubShot()
	{
		Nowselect = NOWSELECT.SUBSHOT;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 0, 0);
		SubShotController.color = new Color(1, 0, 0);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// 特殊射撃が押された時の処理
	/// </summary>
	public void OnClickExShot()
	{
		Nowselect = NOWSELECT.EXSHOT;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 0, 0);
		ExShotController.color = new Color(1, 0, 0);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// 特殊格闘を押された時の処理
	/// </summary>
	public void OnClickExWrestle()
	{
		Nowselect = NOWSELECT.EXWRESTLE;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 0, 0);
		ExWrestleController.color = new Color(1, 0, 0);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// 視点回転上を押された時の処理
	/// </summary>
	public void OnClickElevationUpper()
	{
		Nowselect = NOWSELECT.ELEVATIONUPPER;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 0, 0);
		ElevationUpperController.color = new Color(1, 0, 0);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// 視点回転下を押された時の処理
	/// </summary>
	public void OnClickElevetionDown()
	{
		Nowselect = NOWSELECT.ELEVETIONDOWN;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 0, 0);
		ElevationDownController.color = new Color(1, 0, 0);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// 視点回転左を押された時の処理
	/// </summary>
	public void OnClickAzimuthLeft()
	{
		Nowselect = NOWSELECT.AZIMUTHLEFT;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 0, 0);
		AzimuthLeftController.color = new Color(1, 0, 0);

		AzimhthRightKeyboad.color = new Color(1, 1, 1);
		AzimhthRightController.color = new Color(1, 1, 1);
	}

	/// <summary>
	/// 視点回転右を押された時の処理
	/// </summary>
	public void OnClickAzimthRight()
	{
		Nowselect = NOWSELECT.AZIMUTHRIGHT;
		// 選択対象のカーソルを赤にし、それ以外を白にする
		ShotKeyboard.color = new Color(1, 1, 1);
		ShotController.color = new Color(1, 1, 1);

		WrestleKeyboard.color = new Color(1, 1, 1);
		WrestleController.color = new Color(1, 1, 1);

		JumpKeyboad.color = new Color(1, 1, 1);
		JumpController.color = new Color(1, 1, 1);

		SearchKeyboard.color = new Color(1, 1, 1);
		SearchController.color = new Color(1, 1, 1);

		CommandKeyboard.color = new Color(1, 1, 1);
		CommandController.color = new Color(1, 1, 1);

		MenuKeyboard.color = new Color(1, 1, 1);
		MenuController.color = new Color(1, 1, 1);

		ItemKeyboard.color = new Color(1, 1, 1);
		ItemController.color = new Color(1, 1, 1);

		SubShotKeyboard.color = new Color(1, 1, 1);
		SubShotController.color = new Color(1, 1, 1);

		ExShotKeyboard.color = new Color(1, 1, 1);
		ExShotController.color = new Color(1, 1, 1);

		ExWrestleKeyboard.color = new Color(1, 1, 1);
		ExWrestleController.color = new Color(1, 1, 1);

		ElevationUpperKeyboard.color = new Color(1, 1, 1);
		ElevationUpperController.color = new Color(1, 1, 1);

		ElevetionDownKeyboard.color = new Color(1, 1, 1);
		ElevationDownController.color = new Color(1, 1, 1);

		AzimuthLeftKeyboard.color = new Color(1, 1, 1);
		AzimuthLeftController.color = new Color(1, 1, 1);

		AzimhthRightKeyboad.color = new Color(1, 0, 0);
		AzimhthRightController.color = new Color(1, 0, 0);
	}

	/// <summary>
	/// 右スティック設定ボタンが押された時の処理
	/// </summary>
	public void OnClickSetRightStick()
	{
		Controllersetting.SetBool("ControllerSettingClose", false);
		Controllersetting.SetBool("CloseController2", false);
		Controllersetting.Play("OpenControllerSetting");
		PlayerPrefs.SetInt("ControllerSetting", 11);
		Controllersettingpopup.Popup1Text.text = "コントローラーの右アナログスティックの設定を行います\nこの操作を行わないと、コントローラーの右スティックが使えなくなります\nなお、オプション画面からの設定も可能となります\n\n設定を行いますか？";
		Controllersettingpopup.SettingTarget = SETTINGTARGET.RIGHTSTICK;
	}

	/// <summary>
	/// 十字キー設定ボタンが押された時の処理
	/// </summary>
	public void OnClickSetTenkey()
	{
		Controllersetting.SetBool("ControllerSettingClose", false);
		Controllersetting.SetBool("CloseController2", false);
		Controllersetting.Play("OpenControllerSetting");
		PlayerPrefs.SetInt("ControllerSetting", 1);
		Controllersettingpopup.Popup1Text.text = "コントローラーの十字キーの設定を行います\nこの操作を行わないと、コントローラーの十字キーが使えなくなります\nなお、オプション画面からの設定も可能となります\nアナログスティックのないコントローラーは設定不要です\n設定を行いますか？";
		Controllersettingpopup.SettingTarget = SETTINGTARGET.TENKEY;
	}

	/// <summary>
	/// OKボタンが押された時の処理
	/// </summary>
	public void OnClickOKButton()
	{
		// 保持内容をセーブする
		// 射撃・決定
		// キーボード
		PlayerPrefs.SetString("Shot_Keyboard", ShotKeyboardText.text);
		// コントローラー
		PlayerPrefs.SetString("Shot_Controller", "Joystick1" + ShotControllerText.text);

		// 格闘
		// キーボード
		PlayerPrefs.SetString("Wrestle_Keyboard", WrestleKeyboardText.text);
		// コントローラー
		PlayerPrefs.SetString("Wrestle_Controller", "Joystick1" + WrestleControllerText.text);

		// ジャンプ
		// キーボード
		PlayerPrefs.SetString("Jump_Keyboard", JumpKeyboadText.text);
		// コントローラー
		PlayerPrefs.SetString("Jump_Controller", "Joystick1" + JumpControllerText.text);

		// サーチ
		// キーボード
		PlayerPrefs.SetString("Search_Keyboard", SearchKeyboardText.text);
		// コントローラー
		PlayerPrefs.SetString("Search_Controller", "Joystick1" + SearchControllerText.text);

		// コマンド
		// キーボード
		PlayerPrefs.SetString("Command_Keyboard", CommandKeyboardText.text);
		// コントローラー
		PlayerPrefs.SetString("Commnd_Controller", "Joystick1" + CommandControllerText.text);

		// メニュー
		// キーボード
		PlayerPrefs.SetString("Menu_Keyboard", MenuKeyboardText.text);
		// コントローラー
		PlayerPrefs.SetString("Menu_Controller", "Joystick1" + MenuControllerText.text);

		// アイテム
		PlayerPrefs.SetString("Item_Keyboard", ItemKeyboardText.text);
		PlayerPrefs.SetString("Item_Controller", "Joystick1" + ItemControllerText.text);

		// サブ射撃
		// キーボード
		PlayerPrefs.SetString("SubShot_Keyboard", SubShotKeyboardText.text);
		// コントローラー
		PlayerPrefs.SetString("SubShot_Controller", "Joystick1" + SubShotControllerText.text);

		// 特殊射撃
		// キーボード
		PlayerPrefs.SetString("EXShot_Keyboard", ExShotKeyboardText.text);
		// コントローラー
		PlayerPrefs.SetString("EXShot_Controller", "Joystick1" + ExShotControllerText.text);

		// 特殊格闘
		// キーボード
		PlayerPrefs.SetString("EXWrestle_Keyboad", ExWrestleKeyboardText.text);
		// コントローラー
		PlayerPrefs.SetString("EXWrestle_Controller", "Joystick1" + ExWrestleControllerText.text);

		// 視点回転上
		// キーボード
		PlayerPrefs.SetString("ElevationUpper_Keyboard", ElevationUpperKeyboardText.text);
		// コントローラー
		// 軸指定
		if (ElevationUpperControllerText.text.IndexOf("Axis") >= 0)
		{
			PlayerPrefs.SetString("ElevationUpper_Controller", ElevationUpperControllerText.text);
		}
		// キー設定
		else
		{
			PlayerPrefs.SetString("ElevationUpper_Controller", "Joystick1" + ElevationUpperControllerText.text);
		}
		// 視点回転下
		// キーボード
		PlayerPrefs.SetString("ElevationDown_Keyboard", ElevetionDownKeyboardText.text);
		// コントローラー
		// 軸設定
		if(ElevationDownControllerText.text.IndexOf("Axis") >=0)
		{
			PlayerPrefs.SetString("ElevationDown_Controller", ElevationDownControllerText.text);
		}
		// キー設定
		else
		{
			PlayerPrefs.SetString("ElevationDown_Controller", "Joystick1" + ElevationDownControllerText.text);
		}

		// 視点回転左
		// キーボード
		PlayerPrefs.SetString("AzimuthLeft_Keyboard", AzimuthLeftKeyboardText.text);
		// コントローラー
		// 軸設定
		if(AzimuthLeftControllerText.text.IndexOf("Axis") >= 0)
		{
			PlayerPrefs.SetString("AzimuthLeft_Controller", AzimuthLeftControllerText.text);
		}
		// キー設定
		else
		{
			PlayerPrefs.SetString("AzimuthLeft_Controller", "Joystick1" + ElevationDownControllerText.text);
		}

		// 視点回転右
		// キーボード
		PlayerPrefs.SetString("AzimuthRight_Keyboard", AzimhthRightKeyboadText.text);
		// コントローラー
		// 軸設定
		if(AzimhthRightControllerText.text.IndexOf("Axis") >= 0)
		{
			PlayerPrefs.SetString("AzimuthRight_Controller", AzimhthRightControllerText.text);
		}
		// キー設定
		else
		{
			PlayerPrefs.SetString("AzimuthRight_Controller", "Joystick1" +  AzimhthRightControllerText.text);
		}

		// Cancelボタンが非アクティブ（初回）→セーブしてタイトルへ遷移
		if (!CancelButton.interactable)
		{
			// 保持情報をセーブ
			PlayerPrefs.SetInt("ControllerSetting", 20);
			// タイトルへ遷移
			FadeManager.Instance.LoadLevel("title", 1.0f);
		}
		// Cancelボタンがアクティブ（オプションから来た）→セーブしてオプションへ遷移
		else
		{
			// 保持情報をセーブ
			PlayerPrefs.SetInt("ControllerSetting", 20);
			// タイトルへ遷移
			FadeManager.Instance.LoadLevel("title", 1.0f);
		}
	}

	public void OnClickCancelButton()
	{
		// タイトルへ遷移する
		FadeManager.Instance.LoadLevel("title", 1.0f);
	}
}
