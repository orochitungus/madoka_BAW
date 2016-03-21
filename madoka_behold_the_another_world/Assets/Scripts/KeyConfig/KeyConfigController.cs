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

	// Use this for initialization
	void Start () 
	{
        // モード初期化
        Nowmode = NowMode.SETTINGSTANDBY;

        // 初期状態で右コントローラーのセッティングが成されていなければ、セッティングに入る
        this.UpdateAsObservable().Where(_ => Nowmode == NowMode.SETTINGSTANDBY).Subscribe(_AppDomain =>
        {
            if (PlayerPrefs.GetInt("ControllerSetting") < 1)
            {
				Controllersetting.SetBool("SetRightStick", true);
            }
            // セッティングが成されていたらステートを切り替える（ポップアップを出さない）
            else
            {
                Nowmode = NowMode.POPUPCLOSE;
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
					if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
					{
						// ダブり対策
						OverlapCheckController(k.GetValue(i).ToString(), nowselect);
						// ボタン取得
						switch (nowselect)
						{
							case NOWSELECT.SHOT:
								ShotControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.WRESTLE:
								WrestleControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.SEARCH:
								SearchControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.JUMP:
								JumpControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.COMMAND:
								CommandControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.MENU:
								CommandControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.SUBSHOT:
								SubShotControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.EXSHOT:
								ExShotControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.EXWRESTLE:
								ExWrestleControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.ELEVATIONUPPER:
								ElevationUpperControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.ELEVETIONDOWN:
								ElevationUpperControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.AZIMUTHLEFT:
								AzimuthLeftControllerText.text = k.GetValue(i).ToString();
								break;
							case NOWSELECT.AZIMUTHRIGHT:
								AzimhthRightKeyboadText.text = k.GetValue(i).ToString();
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
            case NOWSELECT.ELEVATIONUPPER:
                originalbuton = ElevationUpperControllerText.text;
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
		else if (JumpKeyboadText.text == insert)
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
		
        if (Input.anyKeyDown)
        {
            
        }
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
		Controllersetting.Play("SetRightStick");
	}
}
