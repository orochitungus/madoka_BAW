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

    /// <summary>
    /// サーチボタン長押しによるアンロック
    /// </summary>
    public bool Unlock;

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

            // 上
            if (vertical > 0.0f && Math.Abs(horizontal) < 0.1f)
            {
                Top = true;
            }
            else
            {
                Top = false;
            }

            // 下
            if (vertical < 0.0f && Math.Abs(horizontal) < 0.1f)
            {
                Under = true;
            }
            else
            {
                Under = false;
            }

            // 左
            if (horizontal > 0.0f && Math.Abs(vertical) < 0.1f)
            {
                Left = true;
            }
            else
            {
                Left = false;
            }

            // 右
            if(horizontal < 0.0f && Math.Abs(vertical) < 0.1f)
            {
                Right = true;
            }
            else
            {
                Right = false;
            }        
            
        });
        // ステップ入力検知
        // 前ステップ
        var frontstepstream = this.UpdateAsObservable().Where(_ => Top);
        frontstepstream.Buffer(frontstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count >= 2).Subscribe(_ => { FrontStep = true; });
        frontstepstream.Buffer(frontstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count < 2).Subscribe(_ => { FrontStep = false; });

        // 左前ステップ
        var leftfrontstepstream = this.UpdateAsObservable().Where(_ => Top && Left);
        leftfrontstepstream.Buffer(leftfrontstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count >= 2).Subscribe(_ => { LeftFrontStep = true; });
        leftfrontstepstream.Buffer(leftfrontstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count < 2).Subscribe(_ => { LeftFrontStep = false; });

        // 左ステップ
        var leftstepstream = this.UpdateAsObservable().Where(_ => Left);
        leftstepstream.Buffer(leftstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count >= 2).Subscribe(_ => { LeftStep = true; });
        leftstepstream.Buffer(leftstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count < 2).Subscribe(_ => { LeftStep = false; });

        // 左後ステップ
        var leftbackstepstream = this.UpdateAsObservable().Where(_ => Left && Under);
        leftbackstepstream.Buffer(leftbackstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count >= 2).Subscribe(_ => { LeftBackStep = true; });
        leftbackstepstream.Buffer(leftbackstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count < 2).Subscribe(_ => { LeftBackStep = false; });

        // 後ステップ
        var backstepstream = this.UpdateAsObservable().Where(_ => Under);
        backstepstream.Buffer(backstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count >= 2).Subscribe(_ => { BackStep = true; });
        backstepstream.Buffer(backstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count < 2).Subscribe(_ => { BackStep = false; });

        // 右後ステップ
        var rightbackstepstream = this.UpdateAsObservable().Where(_ => Right && Under);
        rightbackstepstream.Buffer(rightbackstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count >= 2).Subscribe(_ => { RightBackStep = true; });
        rightbackstepstream.Buffer(rightbackstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count < 2).Subscribe(_ => { RightBackStep = false; });

		// 右ステップ
		var rightstepstream = this.UpdateAsObservable().Where(_ => Right);
		rightstepstream.Buffer(rightstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count >= 2).Subscribe(_ => { RightStep = true; });
		rightstepstream.Buffer(rightstepstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count < 2).Subscribe(_ => { RightStep = false; });

		// 右前ステップ
		var rightfrontstepstrem = this.UpdateAsObservable().Where(_ => Right && Top);
		rightfrontstepstrem.Buffer(rightfrontstepstrem.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count >= 2).Subscribe(_ => { RightFrontStep = true; });
		rightfrontstepstrem.Buffer(rightfrontstepstrem.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count < 2).Subscribe(_ => { RightFrontStep = false; });

		// http://www.slideshare.net/torisoup/unity-unirx
		// 射撃
		string shotcode_keyboard = PlayerPrefs.GetString("Shot_Keyboard");
		string shotcode_controller = PlayerPrefs.GetString("Shot_Controller");

		// 格闘
		string wrestlecode_keyboard = PlayerPrefs.GetString("Wrestle_Keyboard");
		string wrestlecode_controller = PlayerPrefs.GetString("Wrestle_Controller");

		// ジャンプ
		string jump_keyboard = PlayerPrefs.GetString("Jump_Keyboard");
		string jump_controller = PlayerPrefs.GetString("Jump_Controller");

		// サーチ
		string search_keyboard = PlayerPrefs.GetString("Search_Keyboard");
		string search_controller = PlayerPrefs.GetString("Search_Controller");

		// サブ射撃
		string subshot_keyboard = PlayerPrefs.GetString("SubShot_Keyboard");
		string subshot_controller = PlayerPrefs.GetString("SubShot_Controller");

		// 特殊格闘 
		string exwrestle_keyboard = PlayerPrefs.GetString("EXWrestle_Keyboad");
		string exwrestle_controller = PlayerPrefs.GetString("EXWrestle_Controller");

		// 特殊射撃
		string exshot_keyboard = PlayerPrefs.GetString("EXShot_Keyboard");
		string exshot_controller = PlayerPrefs.GetString("EXShot_Controller");

		// メニュー
		string menu_keyboard = PlayerPrefs.GetString("Menu_Keyboard");
		string menu_controller = PlayerPrefs.GetString("Menu_Controller");

		// コマンド
		string command_keyboard = PlayerPrefs.GetString("Command_Keyboard");
		string command_controller = PlayerPrefs.GetString("Commnd_Controller");

		// 視点変更上
		string viewchangeupper_keyboard = PlayerPrefs.GetString("ElevationUpper_Keyboard");
		string viewchangeupper_controller = PlayerPrefs.GetString("ElevationUpper_Controller");

		// 視点変更下
		string viewchangedown_keyboard = PlayerPrefs.GetString("ElevationDown_Keyboard");
		string viewchangedown_controller = PlayerPrefs.GetString("ElevationDown_Controller");

		// 視点変更左
		string viewchangeleft_keyboard = PlayerPrefs.GetString("AzimuthLeft_Keyboard");
		string viewchangeleft_controller = PlayerPrefs.GetString("AzimuthLeft_Controller");

        // 視点変更右
        string viewchangeright_keyboard = PlayerPrefs.GetString("AzimuthRight_Keyboard");
        string viewchangeright_controller = PlayerPrefs.GetString("AzimuthRight_Controller");

        // キー入力取得
        this.UpdateAsObservable().Subscribe(_ =>
        {
            if (Input.anyKeyDown)
            {
                Array k = Enum.GetValues(typeof(KeyCode));
                for (int i = 0; i < k.Length; i++)
                {
                    // キー取得(キーボード）
                    if (Input.GetKeyDown((KeyCode)k.GetValue(i)))
                    {
                        // テンキーとマウス入力と画面クリック以外のキー入力を取得する
                        if (k.GetValue(i).ToString().IndexOf("Arrow") < 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0 && k.GetValue(i).ToString().IndexOf("Joystick") < 0)
                        {
                            // 射撃取得
                            if (k.GetValue(i).ToString() == shotcode_keyboard)
                            {
                                Shot = true;
                            }
                            else
                            {
                                Shot = false;
                            }
                            // 格闘取得
                            if (k.GetValue(i).ToString() == wrestlecode_keyboard)
                            {
                                Wrestle = true;
                            }
                            else
                            {
                                Wrestle = false;
                            }
                            // ジャンプ取得
                            if (k.GetValue(i).ToString() == jump_keyboard)
                            {
                                Jump = true;
                            }
                            else
                            {
                                Jump = false;
                            }
                            // サーチ取得
                            if (k.GetValue(i).ToString() == search_keyboard)
                            {
                                Search = true;
                            }
                            else
                            {
                                Search = false;
                            }
                            // メニュー取得
                            if (k.GetValue(i).ToString() == menu_keyboard)
                            {
                                Menu = true;
                            }
                            else
                            {
                                Menu = false;
                            }
                            // コマンド取得
                            if (k.GetValue(i).ToString() == command_keyboard)
                            {
                                Command = true;
                            }
                            else
                            {
                                Command = false;
                            }
                            // サブ射撃取得
                            if (k.GetValue(i).ToString() == subshot_keyboard)
                            {
                                SubShot = true;
                            }
                            else
                            {
                                Wrestle = false;
                            }
                            // 特殊射撃取得
                            if (k.GetValue(i).ToString() == exshot_keyboard)
                            {
                                EXShot = true;
                            }
                            else
                            {
                                EXShot = false;
                            }
                            // 特殊格闘取得
                            if (k.GetValue(i).ToString() == exwrestle_keyboard)
                            {
                                EXWrestle = true;
                            }
                            else
                            {
                                EXWrestle = false;
                            }
                            // 視点変更上取得
                            if (k.GetValue(i).ToString() == viewchangeupper_keyboard)
                            {
                                ElevationAngleUpper = true;
                            }
                            else
                            {
                                ElevationAngleUpper = false;
                            }
                            // 視点変更下取得
                            if (k.GetValue(i).ToString() == viewchangedown_keyboard)
                            {
                                ElevationAngleDown = true;
                            }
                            else
                            {
                                ElevationAngleDown = false;
                            }
                            // 視点変更左取得
                            if (k.GetValue(i).ToString() == viewchangeleft_keyboard)
                            {
                                AzimuthLeft = true;
                            }
                            else
                            {
                                AzimuthLeft = false;
                            }
                            // 視点変更右取得
                            if (k.GetValue(i).ToString() == viewchangeright_keyboard)
                            {
                                AzimuthRight = true;
                            }
                            else
                            {
                                AzimuthRight = false;
                            }
                        }
                        // キー入力取得（コントローラー）
                        if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
                        {
                            // 射撃取得
                            if (k.GetValue(i).ToString() == shotcode_controller)
                            {
                                Shot = true;
                            }
                            else
                            {
                                Shot = false;
                            }
                            // 格闘取得
                            if (k.GetValue(i).ToString() == wrestlecode_controller)
                            {
                                Wrestle = true;
                            }
                            else
                            {
                                Wrestle = false;
                            }
                            // ジャンプ取得
                            if (k.GetValue(i).ToString() == jump_controller)
                            {
                                Jump = true;
                            }
                            else
                            {
                                Jump = false;
                            }
                            // サーチ取得
                            if (k.GetValue(i).ToString() == search_controller)
                            {
                                Search = true;
                            }
                            else
                            {
                                Search = false;
                            }
                            // メニュー取得
                            if (k.GetValue(i).ToString() == menu_controller)
                            {
                                Menu = true;
                            }
                            else
                            {
                                Menu = false;
                            }
                            // コマンド取得
                            if (k.GetValue(i).ToString() == command_controller)
                            {
                                Command = true;
                            }
                            else
                            {
                                Command = false;
                            }
                            // サブ射撃取得
                            if (k.GetValue(i).ToString() == subshot_controller)
                            {
                                SubShot = true;
                            }
                            else
                            {
                                Wrestle = false;
                            }
                            // 特殊射撃取得
                            if (k.GetValue(i).ToString() == exshot_controller)
                            {
                                EXShot = true;
                            }
                            else
                            {
                                EXShot = false;
                            }
                            // 特殊格闘取得
                            if (k.GetValue(i).ToString() == exwrestle_controller)
                            {
                                EXWrestle = true;
                            }
                            else
                            {
                                EXWrestle = false;
                            }
                            // 視点変更上取得
                            if (k.GetValue(i).ToString() == viewchangeupper_controller)
                            {
                                ElevationAngleUpper = true;
                            }
                            else
                            {
                                ElevationAngleUpper = false;
                            }
                            // 視点変更下取得
                            if (k.GetValue(i).ToString() == viewchangedown_controller)
                            {
                                ElevationAngleDown = true;
                            }
                            else
                            {
                                ElevationAngleDown = false;
                            }
                            // 視点変更左取得
                            if (k.GetValue(i).ToString() == viewchangeleft_controller)
                            {
                                AzimuthLeft = true;
                            }
                            else
                            {
                                AzimuthLeft = false;
                            }
                            // 視点変更右取得
                            if (k.GetValue(i).ToString() == viewchangeright_controller)
                            {
                                AzimuthRight = true;
                            }
                            else
                            {
                                AzimuthRight = false;
                            }
                        }
                    }
                }

            }
        });


        // BD
        var boostdashstream = this.UpdateAsObservable().Where(_ => Jump);
        boostdashstream.Buffer(boostdashstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count >= 2).Subscribe(_ => { BoostDash = true; });
        boostdashstream.Buffer(boostdashstream.Throttle(TimeSpan.FromMilliseconds(200))).Where(x => x.Count < 2).Subscribe(_ => { BoostDash = false; });

        // ロック外し
        var unlockstream = this.UpdateAsObservable().Where(_ => Search);
        // ロック外しキャンセル
        var unlockcancelstream = this.UpdateAsObservable().Where(_ => !Search);
        // ロック外し実行（1秒以上長押しでロックが外れる）
        unlockstream.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(1.0f)))
        // 途中でロックボタンが離れるとリセット
        .TakeUntil(unlockcancelstream).RepeatUntilDestroy(this.gameObject).Subscribe(_ => { Unlock = true; });
        // ロック外しキャンセル
        unlockcancelstream.Timestamp().Zip(unlockcancelstream.Timestamp(), (d, u) => (u.Timestamp - d.Timestamp).TotalMilliseconds / 1000.0f).Where(time => time < 1.0f).Subscribe(t => Unlock = false);
    }

	// Update is called once per frame
	void Update () 
	{
	
	}

    
}
