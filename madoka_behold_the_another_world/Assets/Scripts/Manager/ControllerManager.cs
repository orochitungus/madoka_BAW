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
	/// 方向キー上を離した
	/// </summary>
	public bool TopUp;

	/// <summary>
	/// 方向キー上を長押した
	/// </summary>
	public bool TopLongPress;

	/// <summary>
	/// 方向キー/左スティック下
	/// </summary>
	public bool Under;

	/// <summary>
	/// 方向キー下を離した
	/// </summary>
	public bool UnderUp;

	/// <summary>
	/// 方向キー下を長押した
	/// </summary>
	public bool UnderLongPress;
	
	/// <summary>
	/// 方向キー/左スティック左
	/// </summary>
	public bool Left;

	/// <summary>
	/// 方向キー左を離した
	/// </summary>
	public bool LeftUp;

	/// <summary>
	/// 方向キー左を長押した
	/// </summary>
	public bool LeftLongPress;

	/// <summary>
	/// 方向キー/左スティック右
	/// </summary>
	public bool Right;

	/// <summary>
	/// 方向キー右を離した
	/// </summary>
	public bool RightUp;

	/// <summary>
	/// 方向キー右を長押した
	/// </summary>
	public bool RightLongPress;

	/// <summary>
	/// 左上を押した
	/// </summary>
	public bool LeftUpper;

	/// <summary>
	/// 左上を離した
	/// </summary>
	public bool LeftUpperUp;

	/// <summary>
	/// 左下を押した
	/// </summary>
	public bool LeftUnder;

	/// <summary>
	/// 左下を離した
	/// </summary>
	public bool LeftUnderUp;

	/// <summary>
	/// 右下を押した
	/// </summary>
	public bool RightUnder;

	/// <summary>
	/// 右下を離した
	/// </summary>
	public bool RightUnderUp;

	/// <summary>
	/// 右上を押した
	/// </summary>
	public bool RightUpper;

	/// <summary>
	/// 右上を押した
	/// </summary>
	public bool RightUpperUp;

	

	/// <summary>
	/// 射撃・決定
	/// </summary>
	public bool Shot;
    public bool ShotKeyboad;
    public bool ShotController;
	public bool ShotUp;

    /// <summary>
    /// 射撃長押し
    /// </summary>
    public bool Shotting;

	/// <summary>
	/// 格闘
	/// </summary>
	public bool Wrestle;
    public bool WrestleKeyboard;
    public bool WrestleController;
	public bool WrestleUp;

    /// <summary>
    /// 格闘長押し
    /// </summary>
    public bool Wrestling;

	/// <summary>
	/// ジャンプ・キャンセル
	/// </summary>
	public bool Jump;
    public bool JumpKeyboard;
    public bool JumpController;
	public bool JumpUp;

    /// <summary>
    /// ジャンプ長押し
    /// </summary>
    public bool Jumping;

	/// <summary>
	/// サーチ
	/// </summary>
	public bool Search;
    public bool SearchKeyboard;
    public bool SearchController;
	public bool SearchUp;

	/// <summary>
	/// コマンド
	/// </summary>
	public bool Command;
    public bool CommandKeyboard;
    public bool CommandController;
	public bool CommandUp;

	/// <summary>
	/// アイテム
	/// </summary>
	public bool Item;
    public bool ItemKeyboard;
    public bool ItemController;
	public bool ItemUp;

	/// <summary>
	/// メニュー
	/// </summary>
	public bool Menu;
    public bool MenuKeyboard;
    public bool MenuController;
	public bool MenuUp;

	/// <summary>
	/// サブ射撃
	/// </summary>
	public bool SubShot;
    public bool SubShotKeyboard;
    public bool SubShotController;
	public bool SubShotUp;

	/// <summary>
	/// 特殊射撃
	/// </summary>
	public bool EXShot;
    public bool EXShotKeyboard;
    public bool EXShotController;
	public bool EXShotUp;

	/// <summary>
	/// 特殊格闘
	/// </summary>
	public bool EXWrestle;
    public bool EXWrestleKeyboard;
    public bool EXWrestleController;
	public bool EXWrestleUp;

	/// <summary>
	/// 視点回転上(仰角）
	/// </summary>
	public bool ElevationAngleUpper;
    public bool ElevationAngleUpperKeyboard;
    public bool ElevationAngleUpperController;
	public bool ElevationAngleUpperUp;

	/// <summary>
	/// 視点回転下(仰角)
	/// </summary>
	public bool ElevationAngleDown;
    public bool ElevationAngleDownKeyboard;
    public bool ElevationAngleDownController;
	public bool ElevationAngleDownUp;

	/// <summary>
	/// 視点回転左(方位角)
	/// </summary>
	public bool AzimuthLeft;
    public bool AzimuthLeftKeyboard;
    public bool AzimuthLeftController;
	public bool AzimuthLeftUp;

	/// <summary>
	/// 視点回転右(方位角)
	/// </summary>
	public bool AzimuthRight;
    public bool AzimuthRightKeyboard;
    public bool AzimuthRightController;
	public bool AzimuthRightUp;

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

    /// <summary>
    /// 方向キーの入力ログ
    /// </summary>
    private InputDirection[] _Inputdirections = new InputDirection[60];

    //private bool[] Jumpdirections = new bool[60];

    // Use this for initialization
    void Start () 
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(gameObject);
        		

		// 右ジョイスティックからの入力を受け取る

		// 十字キー/ジョイスティック左入力方向検出
		this.UpdateAsObservable().Subscribe(_ =>
		{
			// 十字キー設定取得
			string Upkey = PlayerPrefs.GetString("KeyUP");
			string Downkey = PlayerPrefs.GetString("KeyDown");
			string Leftkey = PlayerPrefs.GetString("KeyLeft");
			string Rightkey = PlayerPrefs.GetString("KeyRight");

			float horizontal = Input.GetAxisRaw("Horizontal");      // 横入力を検出(左スティック)
			float vertical = Input.GetAxisRaw("Vertical");          // 縦入力を検出(左スティック)
			float horizontal2 = 0.0f;                               // 横入力を検出(十字キー）
			float vertical2 = 0.0f;									// 横入力を検出(十字キー)

			// 十字キーからの入力があればそっちを優先する
			// 上or下
			// 軸はAxis3～8(Horizontal2,3,4,Vertical2,3,4)のどれ？それともボタン？
			string verticalVector = "";
			// 上下どっちが＋？
			//bool isTopPlus = true;
			if (Rightkey.IndexOf("Button") >= 0)
			{
				verticalVector = "Button";
			}
			//// MINUSが入っていたらIsTopPlusをfalseにする(上入力がマイナスになるため）
			//if (Upkey.IndexOf("MINUS") >= 0)
			//{
			//	isTopPlus = false;
			//}
			// 左or右
			string horizontalVector = "";
			// 左右どっちが＋？
			//bool isRightPlus = true;
			if (Rightkey.IndexOf("Button") >= 0)
			{
				horizontalVector = "Button";
			}
			// MINUSが入っていたらIsRightPlusをfalseにする（右方向がマイナスになるため）
			//if (Rightkey.IndexOf("MINUS") >= 0)
			//{
			//	isRightPlus = false;
			//}

			// 軸入力だった場合
			if (Upkey.IndexOf("3rd") >= 0)
			{
				vertical2 = Input.GetAxisRaw("Vertical2");
			}
			else if (Upkey.IndexOf("4th") >= 0)
			{
				vertical2 = Input.GetAxisRaw("Vertical3");
			}
			else if (Upkey.IndexOf("5th") >= 0)
			{
				vertical2 = Input.GetAxisRaw("Horizontal2");
			}
			else if (Upkey.IndexOf("6th") >= 0)
			{
				vertical2 = Input.GetAxisRaw("Horizontal3");
			}
			else if (Upkey.IndexOf("7th") >= 0)
			{
				vertical2 = Input.GetAxisRaw("Vertical4");
			}
			else if (Upkey.IndexOf("8th") >= 0)
			{
				vertical2 = Input.GetAxisRaw("Horizontal4");
			}
			//if (!isTopPlus)
			//{
			//	vertical2 *= -1;
			//}

			if (Rightkey.IndexOf("3rd") >= 0)
			{
				horizontal2 = Input.GetAxisRaw("Vertical2");
			}
			else if (Rightkey.IndexOf("4th") >= 0)
			{
				horizontal2 = Input.GetAxisRaw("Vertical3");
			}
			else if (Rightkey.IndexOf("5th") >= 0)
			{
				horizontal2 = Input.GetAxisRaw("Horizontal2");
			}
			else if (Rightkey.IndexOf("6th") >= 0)
			{
				horizontal2 = Input.GetAxisRaw("Horizontal3");
			}
			else if (Rightkey.IndexOf("7th") >= 0)
			{
				horizontal2 = Input.GetAxisRaw("Vertical4");
			}
			else if (Rightkey.IndexOf("8th") >= 0)
			{
				horizontal2 = Input.GetAxisRaw("Horizontal4");
			}

			//if (!isRightPlus)
			//{
			//	horizontal2 *= -1;
			//}
			// ボタン入力だった場合
			if (verticalVector == "Button")
			{
				if (Input.GetButtonDown(Upkey))
				{
					vertical2 = 1.0f;
				}
				else if (Input.GetButtonDown(Downkey))
				{
					vertical2 = -1.0f;
				}
				else
				{
					vertical2 = 0.0f;
				}
			}
			if (horizontalVector == "Button")
			{
				if (Input.GetButtonDown(Rightkey))
				{
					horizontal2 = 1.0f;
				}
				else if (Input.GetButtonDown(Leftkey))
				{
					horizontal2 = -1.0f;
				}
				else
				{
					horizontal2 = 0.0f;
				}
			}

            // 入力ログを１F前に遷移させる
            for(int i=1; i<_Inputdirections.Length; i++)
            {
                _Inputdirections[i - 1] = _Inputdirections[i];
            }

            // ニュートラル
            if((Math.Abs(vertical) < 0.1f && Math.Abs(horizontal) < 0.1f) || (Math.Abs(vertical2) < 0.1f && Math.Abs(horizontal2) < 0.1f))
            {
                _Inputdirections[_Inputdirections.Length - 1] = InputDirection.NEUTRAL;
            }
			
			// 上押した
			if ((vertical > 0.0f && Math.Abs(horizontal) < 0.1f) || (vertical2 > 0.0f && Math.Abs(horizontal2) < 0.1f))
            {
                _Inputdirections[_Inputdirections.Length - 1] = InputDirection.TOP;
                Top = true;
				TopUp = false;
				UnderUp = false;
				LeftUp = false;
				RightUp = false;
                LeftUpperUp = false;
                LeftUnderUp = false;
                RightUpperUp = false;
                RightUnderUp = false;
            }
            else
            {
                Top = false;
            }

			// 上離した
			// ボタン
			if (verticalVector == "Button")
			{
				if(Input.GetButtonUp(Upkey))
				{
					TopUp = true;
				}
			}
			// 方向キー
			// 前にフレームに上を押していて今がニュートラルなら離したを有効にする
			if((_Inputdirections[_Inputdirections.Length - 8] == InputDirection.TOP || _Inputdirections[_Inputdirections.Length - 8] == InputDirection.LEFTTOP || _Inputdirections[_Inputdirections.Length - 8] == InputDirection.RIGHTTOP) &&
			   (_Inputdirections[_Inputdirections.Length - 7] == InputDirection.TOP || _Inputdirections[_Inputdirections.Length - 7] == InputDirection.LEFTTOP || _Inputdirections[_Inputdirections.Length - 7] == InputDirection.RIGHTTOP) &&
			   (_Inputdirections[_Inputdirections.Length - 6] == InputDirection.TOP || _Inputdirections[_Inputdirections.Length - 6] == InputDirection.LEFTTOP || _Inputdirections[_Inputdirections.Length - 6] == InputDirection.RIGHTTOP) &&
			   (_Inputdirections[_Inputdirections.Length - 5] == InputDirection.TOP || _Inputdirections[_Inputdirections.Length - 5] == InputDirection.LEFTTOP || _Inputdirections[_Inputdirections.Length - 5] == InputDirection.RIGHTTOP) &&
			     _Inputdirections[_Inputdirections.Length - 4] == InputDirection.NEUTRAL &&
				 _Inputdirections[_Inputdirections.Length - 3] == InputDirection.NEUTRAL &&
				 _Inputdirections[_Inputdirections.Length - 2] == InputDirection.NEUTRAL &&
				 _Inputdirections[_Inputdirections.Length - 1] == InputDirection.NEUTRAL)			
			{
				TopUp = true;
			}

			// 下押した
			if ((vertical < 0.0f && Math.Abs(horizontal) < 0.1f) || (vertical2 < 0.0f && Math.Abs(horizontal2) < 0.1f))
            {
                _Inputdirections[_Inputdirections.Length - 1] = InputDirection.UNDER;
                Under = true;
				TopUp = false;
				UnderUp = false;
				LeftUp = false;
				RightUp = false;
                LeftUpperUp = false;
                LeftUnderUp = false;
                RightUpperUp = false;
                RightUnderUp = false;
            }
            else
            {
                Under = false;
            }
			// 下離した
			// ボタン
			if(verticalVector == "Button")
			{
				if(Input.GetButtonUp(Downkey))
				{
					UnderUp = true;
				}
			}
			// 方向キー
			// 前フレームに下を押していて今がニュートラルなら離したを有効にする
			if ((_Inputdirections[_Inputdirections.Length - 8] == InputDirection.UNDER || _Inputdirections[_Inputdirections.Length - 8] == InputDirection.LEFTUNDER || _Inputdirections[_Inputdirections.Length - 8] == InputDirection.RIGHTUNDER) &&
				(_Inputdirections[_Inputdirections.Length - 7] == InputDirection.UNDER || _Inputdirections[_Inputdirections.Length - 7] == InputDirection.LEFTUNDER || _Inputdirections[_Inputdirections.Length - 7] == InputDirection.RIGHTUNDER) &&
				(_Inputdirections[_Inputdirections.Length - 6] == InputDirection.UNDER || _Inputdirections[_Inputdirections.Length - 6] == InputDirection.LEFTUNDER || _Inputdirections[_Inputdirections.Length - 6] == InputDirection.RIGHTUNDER) &&
				(_Inputdirections[_Inputdirections.Length - 5] == InputDirection.UNDER || _Inputdirections[_Inputdirections.Length - 5] == InputDirection.LEFTUNDER || _Inputdirections[_Inputdirections.Length - 5] == InputDirection.RIGHTUNDER) &&
				 _Inputdirections[_Inputdirections.Length - 4] == InputDirection.NEUTRAL &&
				 _Inputdirections[_Inputdirections.Length - 3] == InputDirection.NEUTRAL &&
				 _Inputdirections[_Inputdirections.Length - 2] == InputDirection.NEUTRAL &&
				 _Inputdirections[_Inputdirections.Length - 1] == InputDirection.NEUTRAL)			
			{
				UnderUp = true;
			}

			// 左押した
			if ((horizontal < 0.0f && Math.Abs(vertical) < 0.1f) || (horizontal2 < 0.0f && Math.Abs(vertical2) < 0.1f))
            {
                _Inputdirections[_Inputdirections.Length - 1] = InputDirection.LEFT;
                Left = true;
				TopUp = false;
				UnderUp = false;
				LeftUp = false;
				RightUp = false;
                LeftUpperUp = false;
                LeftUnderUp = false;
                RightUpperUp = false;
                RightUnderUp = false;
            }
            else
            {
                Left = false;
            }
			// 左離した
			// ボタン
			if (verticalVector == "Button")
			{
				if (Input.GetButtonUp(Leftkey))
				{
					LeftUp = true;
				}
			}
			// 方向キー
			// 前フレームに左を押していて今がニュートラルなら離したを有効にする
			if ((_Inputdirections[_Inputdirections.Length - 8] == InputDirection.LEFT || _Inputdirections[_Inputdirections.Length - 8] == InputDirection.LEFTUNDER || _Inputdirections[_Inputdirections.Length - 8] == InputDirection.LEFTTOP) &&
				(_Inputdirections[_Inputdirections.Length - 7] == InputDirection.LEFT || _Inputdirections[_Inputdirections.Length - 7] == InputDirection.LEFTUNDER || _Inputdirections[_Inputdirections.Length - 7] == InputDirection.LEFTTOP) &&
				(_Inputdirections[_Inputdirections.Length - 6] == InputDirection.LEFT || _Inputdirections[_Inputdirections.Length - 6] == InputDirection.LEFTUNDER || _Inputdirections[_Inputdirections.Length - 6] == InputDirection.LEFTTOP) &&
				(_Inputdirections[_Inputdirections.Length - 5] == InputDirection.LEFT || _Inputdirections[_Inputdirections.Length - 5] == InputDirection.LEFTUNDER || _Inputdirections[_Inputdirections.Length - 5] == InputDirection.LEFTTOP) &&
				_Inputdirections[_Inputdirections.Length - 4] == InputDirection.NEUTRAL &&
				_Inputdirections[_Inputdirections.Length - 3] == InputDirection.NEUTRAL &&
				_Inputdirections[_Inputdirections.Length - 2] == InputDirection.NEUTRAL &&
				_Inputdirections[_Inputdirections.Length - 1] == InputDirection.NEUTRAL)
			{
				LeftUp = true;
			}

			// 右押した
			if ((horizontal > 0.0f && Math.Abs(vertical) < 0.1f) || (horizontal2 > 0.0f && Math.Abs(vertical2) < 0.1f))
			{
                _Inputdirections[_Inputdirections.Length - 1] = InputDirection.RIGHT;
                Right = true;
				TopUp = false;
				UnderUp = false;
				LeftUp = false;
				RightUp = false;
                LeftUpperUp = false;
                LeftUnderUp = false;
                RightUpperUp = false;
                RightUnderUp = false;
            }
            else
            {
                Right = false;
            }
			// 右離した
			// ボタン
			if (verticalVector == "Button")
			{
				if (Input.GetButtonUp(Rightkey))
				{
					RightUp = true;
				}
			}
			// 方向キー
			// 前フレームに右を押していて今がニュートラルなら離したを有効にする
			if ((_Inputdirections[_Inputdirections.Length - 8] == InputDirection.RIGHT || _Inputdirections[_Inputdirections.Length - 8] == InputDirection.RIGHTUNDER || _Inputdirections[_Inputdirections.Length - 8] == InputDirection.RIGHTTOP) &&
				(_Inputdirections[_Inputdirections.Length - 7] == InputDirection.RIGHT || _Inputdirections[_Inputdirections.Length - 7] == InputDirection.RIGHTUNDER || _Inputdirections[_Inputdirections.Length - 7] == InputDirection.RIGHTTOP) &&
				(_Inputdirections[_Inputdirections.Length - 6] == InputDirection.RIGHT || _Inputdirections[_Inputdirections.Length - 6] == InputDirection.RIGHTUNDER || _Inputdirections[_Inputdirections.Length - 6] == InputDirection.RIGHTTOP) &&
				(_Inputdirections[_Inputdirections.Length - 5] == InputDirection.RIGHT || _Inputdirections[_Inputdirections.Length - 5] == InputDirection.RIGHTUNDER || _Inputdirections[_Inputdirections.Length - 5] == InputDirection.RIGHTTOP) &&
				 _Inputdirections[_Inputdirections.Length - 4] == InputDirection.NEUTRAL &&
				 _Inputdirections[_Inputdirections.Length - 3] == InputDirection.NEUTRAL &&
				 _Inputdirections[_Inputdirections.Length - 2] == InputDirection.NEUTRAL &&
				 _Inputdirections[_Inputdirections.Length - 1] == InputDirection.NEUTRAL)
			{
				RightUp = true;
			}

			// 左上押した
			if ((horizontal < -0.5f && vertical > 0.5f) || (horizontal2 < -0.5f && vertical2 > 0.5f))
			{
                _Inputdirections[_Inputdirections.Length - 1] = InputDirection.LEFTTOP;
                LeftUpper = true;
                TopUp = false;
                UnderUp = false;
                LeftUp = false;
                RightUp = false;
                LeftUpperUp = false;
                LeftUnderUp = false;
                RightUpperUp = false;
                RightUnderUp = false;
            }
			else
			{
				LeftUpper = false;
			}
			// 左上離した
			// ボタン
			if (verticalVector == "Button")
			{
				if (Input.GetButtonUp(Upkey) && Input.GetButtonUp(Leftkey))
				{
					LeftUpperUp = true;
				}
			}
			// 方向キー
            if(_Inputdirections[_Inputdirections.Length - 8] == InputDirection.LEFTTOP &&
			   _Inputdirections[_Inputdirections.Length - 7] == InputDirection.LEFTTOP &&
			   _Inputdirections[_Inputdirections.Length - 6] == InputDirection.LEFTTOP &&
			   _Inputdirections[_Inputdirections.Length - 5] == InputDirection.LEFTTOP &&
			   _Inputdirections[_Inputdirections.Length - 4] == InputDirection.NEUTRAL &&
			   _Inputdirections[_Inputdirections.Length - 3] == InputDirection.NEUTRAL &&
			   _Inputdirections[_Inputdirections.Length - 2] == InputDirection.NEUTRAL &&
			   _Inputdirections[_Inputdirections.Length - 1] == InputDirection.NEUTRAL)
            {
                LeftUpperUp = true;                
            }


			// 左下押した
			if ((horizontal < -0.5f && vertical < -0.5f) || (horizontal2 < -0.5f && vertical2 < -0.5f))
			{
                _Inputdirections[_Inputdirections.Length - 1] = InputDirection.LEFTUNDER;
                LeftUnder = true;
                TopUp = false;
                UnderUp = false;
                LeftUp = false;
                RightUp = false;
                LeftUpperUp = false;
                LeftUnderUp = false;
                RightUpperUp = false;
                RightUnderUp = false;
            }
			else
			{
				LeftUnder = false;
			}
			// 左下離した
			// ボタン
			if (verticalVector == "Button")
			{
				if (Input.GetButtonUp(Downkey) && Input.GetButtonUp(Leftkey))
				{
					LeftUnderUp = true;
				}
			}
            // 方向キー
            if(_Inputdirections[_Inputdirections.Length - 8] == InputDirection.LEFTUNDER &&
			   _Inputdirections[_Inputdirections.Length - 7] == InputDirection.LEFTUNDER &&
			   _Inputdirections[_Inputdirections.Length - 6] == InputDirection.LEFTUNDER &&
			   _Inputdirections[_Inputdirections.Length - 5] == InputDirection.LEFTUNDER &&
			   _Inputdirections[_Inputdirections.Length - 4] == InputDirection.NEUTRAL &&
			   _Inputdirections[_Inputdirections.Length - 3] == InputDirection.NEUTRAL &&
			   _Inputdirections[_Inputdirections.Length - 2] == InputDirection.NEUTRAL &&
			   _Inputdirections[_Inputdirections.Length - 1] == InputDirection.NEUTRAL)
            {
                LeftUnderUp = true;
            }

			// 右下押した
			if ((horizontal > 0.5f && vertical < -0.5f) || (horizontal2 > 0.5f && vertical2 < -0.5f))
			{
                _Inputdirections[_Inputdirections.Length - 1] = InputDirection.RIGHTUNDER;
                RightUnder = true;
                TopUp = false;
                UnderUp = false;
                LeftUp = false;
                RightUp = false;
                LeftUpperUp = false;
                LeftUnderUp = false;
                RightUpperUp = false;
                RightUnderUp = false;
            }
			else
			{
				RightUnder = false;
			}
			// 右下離した
			// ボタン
			if (verticalVector == "Button")
			{
				if (Input.GetButtonUp(Rightkey) && Input.GetButtonUp(Downkey))
				{
					RightUnderUp = true;
				}
			}
            // 方向キー
            if(_Inputdirections[_Inputdirections.Length - 2] == InputDirection.RIGHTUNDER && _Inputdirections[_Inputdirections.Length - 1] == InputDirection.NEUTRAL)
            {
                RightUnderUp = true;
            }

			// 右上押した
			if ((horizontal > 0.5f && vertical > 0.5f) || (horizontal2 > 0.5f && vertical2 > 0.5f))
			{
                _Inputdirections[_Inputdirections.Length - 1] = InputDirection.RIGHTTOP;
                RightUpper = true;
                TopUp = false;
                UnderUp = false;
                LeftUp = false;
                RightUp = false;
                LeftUpperUp = false;
                LeftUnderUp = false;
                RightUpperUp = false;
                RightUnderUp = false;
            }
			else
			{
				RightUpper = false;
			}
			// 右上離した
			// ボタン
			if (verticalVector == "Button")
			{
				if (Input.GetButtonUp(Rightkey) && Input.GetButtonUp(Upkey))
				{
					RightUpperUp = true;
				}
			}
            // 方向キー
            if(_Inputdirections[_Inputdirections.Length - 8] == InputDirection.RIGHTTOP &&
			   _Inputdirections[_Inputdirections.Length - 7] == InputDirection.RIGHTTOP &&
			   _Inputdirections[_Inputdirections.Length - 6] == InputDirection.RIGHTTOP &&
			   _Inputdirections[_Inputdirections.Length - 5] == InputDirection.RIGHTTOP &&
			   _Inputdirections[_Inputdirections.Length - 4] == InputDirection.NEUTRAL &&
			   _Inputdirections[_Inputdirections.Length - 3] == InputDirection.NEUTRAL &&
			   _Inputdirections[_Inputdirections.Length - 2] == InputDirection.NEUTRAL &&
			   _Inputdirections[_Inputdirections.Length - 1] == InputDirection.NEUTRAL)
            {
                RightUpperUp = true;
            }

			// ステップ入力検知
			// 前ステップ
			if (GetStepInput(InputDirection.TOP))
			{
				FrontStep = true;
			}
			else
			{
				FrontStep = false;
			}

			// 左前ステップ
			if (GetStepInput(InputDirection.LEFTTOP))
			{
				LeftFrontStep = true;
			}
			else
			{
				LeftFrontStep = false;
			}

			// 左ステップ
			if (GetStepInput(InputDirection.LEFT))
			{
				LeftStep = true;
			}
			else
			{
				LeftStep = false;
			}

			// 左後ステップ
			if (GetStepInput(InputDirection.LEFTUNDER))
			{
				LeftBackStep = true;
			}
			else
			{
				LeftBackStep = false;
			}

			// 後ステップ
			if (GetStepInput(InputDirection.UNDER))
			{
				BackStep = true;
			}
			else
			{
				BackStep = false;
			}

			// 右後ステップ
			if (GetStepInput(InputDirection.RIGHTUNDER))
			{
				RightBackStep = true;
			}
			else
			{
				RightBackStep = false;
			}

			// 右ステップ
			if (GetStepInput(InputDirection.RIGHT))
			{
				RightStep = true;
			}
			else
			{
				RightStep = false;
			}

			// 右前ステップ
			if (GetStepInput(InputDirection.RIGHTTOP))
			{
				RightFrontStep = true;
			}
			else
			{
				RightFrontStep = false;
			}

            // ブースト

		});


        // http://www.slideshare.net/torisoup/unity-unirx
        this.UpdateAsObservable().Subscribe(_ =>
		{
			// 射撃
			shotcode_keyboard = PlayerPrefs.GetString("Shot_Keyboard");
			shotcode_controller = PlayerPrefs.GetString("Shot_Controller");

			// 格闘
			wrestlecode_keyboard = PlayerPrefs.GetString("Wrestle_Keyboard");
			wrestlecode_controller = PlayerPrefs.GetString("Wrestle_Controller");

			// ジャンプ
			jump_keyboard = PlayerPrefs.GetString("Jump_Keyboard");
			jump_controller = PlayerPrefs.GetString("Jump_Controller");

			// サーチ
			search_keyboard = PlayerPrefs.GetString("Search_Keyboard");
			search_controller = PlayerPrefs.GetString("Search_Controller");

            // アイテム
            item_keyboard = PlayerPrefs.GetString("Item_Keyboard");
            item_controller = PlayerPrefs.GetString("Item_Controller");

			// サブ射撃
			subshot_keyboard = PlayerPrefs.GetString("SubShot_Keyboard");
			subshot_controller = PlayerPrefs.GetString("SubShot_Controller");

			// 特殊格闘 
			exwrestle_keyboard = PlayerPrefs.GetString("EXWrestle_Keyboad");
			exwrestle_controller = PlayerPrefs.GetString("EXWrestle_Controller");

			// 特殊射撃
			exshot_keyboard = PlayerPrefs.GetString("EXShot_Keyboard");
			exshot_controller = PlayerPrefs.GetString("EXShot_Controller");

			// メニュー
			menu_keyboard = PlayerPrefs.GetString("Menu_Keyboard");
			menu_controller = PlayerPrefs.GetString("Menu_Controller");

			// コマンド
			command_keyboard = PlayerPrefs.GetString("Command_Keyboard");
			command_controller = PlayerPrefs.GetString("Commnd_Controller");

			// 視点変更上
			viewchangeupper_keyboard = PlayerPrefs.GetString("ElevationUpper_Keyboard");
			viewchangeupper_controller = PlayerPrefs.GetString("ElevationUpper_Controller");

			// 視点変更下
			viewchangedown_keyboard = PlayerPrefs.GetString("ElevationDown_Keyboard");
			viewchangedown_controller = PlayerPrefs.GetString("ElevationDown_Controller");

			// 視点変更左
			viewchangeleft_keyboard = PlayerPrefs.GetString("AzimuthLeft_Keyboard");
			viewchangeleft_controller = PlayerPrefs.GetString("AzimuthLeft_Controller");

			// 視点変更右
			viewchangeright_keyboard = PlayerPrefs.GetString("AzimuthRight_Keyboard");
			viewchangeright_controller = PlayerPrefs.GetString("AzimuthRight_Controller");
		});

		// キー入力取得
		this.UpdateAsObservable().Subscribe(_ =>
        {
			// 入力取得
            //if (Input.anyKeyDown)
            {
                Array k = Enum.GetValues(typeof(KeyCode));
                for (int i = 0; i < k.Length; i++)
                {
                    // 入力キー取得
                    if (Input.GetKeyDown((KeyCode)k.GetValue(i)))
                    {
						GetKeyInput(i,k);
                    }
					// 長押し取得
                    else if(Input.GetKey((KeyCode)k.GetValue(i)))
                    {
                        GetKeyLongInput(i, k);
                    }
					// 離した瞬間取得
					else if(Input.GetKeyUp((KeyCode)k.GetValue(i)))
					{
						GetKeyUp(i, k);
					}
					// 非入力時解除
					else
					{
						GetKeyNotInput(i, k);
					}					
                }
                // 右スティック取得
                // 右スティック設定取得
                string up = PlayerPrefs.GetString("ElevationUpper_Controller");
                //string down = PlayerPrefs.GetString("ElevationDown_Controller");
                //string left = PlayerPrefs.GetString("AzimuthLeft_Controller");
                string right = PlayerPrefs.GetString("AzimuthRight_Controller");

                float rightVertical = 0.0f;
                float rightHorizontal = 0.0f;
                if (up.IndexOf("3rd") >= 0)
                {
                    rightVertical = Input.GetAxisRaw("Vertical2");
                }
                else if (up.IndexOf("4th") >= 0)
                {
                    rightVertical = Input.GetAxisRaw("Vertical3");
                }
                else if (up.IndexOf("5th") >= 0)
                {
                    rightVertical = Input.GetAxisRaw("Horizontal2");
                }
                else if (up.IndexOf("6th") >= 0)
                {
                    rightVertical = Input.GetAxisRaw("Horizontal3");
                }
                else if (up.IndexOf("7th") >= 0)
                {
                    rightVertical = Input.GetAxisRaw("Vertical4");
                }
                else if (up.IndexOf("8th") >= 0)
                {
                    rightVertical = Input.GetAxisRaw("Horizontal4");
                }

                if (right.IndexOf("3rd") >= 0)
                {
                    rightHorizontal = Input.GetAxisRaw("Vertical2");
                }
                else if (right.IndexOf("4th") >= 0)
                {
                    rightHorizontal = Input.GetAxisRaw("Vertical3");
                }
                else if (right.IndexOf("5th") >= 0)
                {
                    rightHorizontal = Input.GetAxisRaw("Horizontal2");
                }
                else if (right.IndexOf("6th") >= 0)
                {
                    rightHorizontal = Input.GetAxisRaw("Horizontal3");
                }
                else if (right.IndexOf("7th") >= 0)
                {
                    rightHorizontal = Input.GetAxisRaw("Vertical4");
                }
                else if (right.IndexOf("8th") >= 0)
                {
                    rightHorizontal = Input.GetAxisRaw("Horizontal4");
                }
                

                // 上
                if (rightVertical > 0.0f && Math.Abs(rightHorizontal) < 0.1f)
                {
                    ElevationAngleUpper = true;
                }
                else
                {
                    ElevationAngleUpper = false;
                }

                // 下
                if (rightVertical < 0.0f && Math.Abs(rightHorizontal) < 0.1f)
                {
                    ElevationAngleDown = true;
                }
                else
                {
                    ElevationAngleDown = false;
                }

                // 左
                if (rightHorizontal < 0.0f)
                {
                    AzimuthLeft = true;
                }
                else
                {
                    AzimuthLeft = false;
                }

                // 右
                if (rightHorizontal > 0.0f)
                {
                    AzimuthRight = true;
                }
                else
                {
                    AzimuthRight = false;
                }
            }			
        });


        // BD
        var boostdashstream = this.UpdateAsObservable().Where(_ => Jump);
        boostdashstream.Buffer(boostdashstream.Throttle(TimeSpan.FromMilliseconds(300))).Where(x => x.Count >= 2).Subscribe(_ => 
		{
            Debug.Log("BoostDashInput");
            BoostDash = true;
        });
		// BD入力解除はfall移行で折る
		this.UpdateAsObservable().Where(_ => BoostDash && !Jumping).Subscribe(_ =>
		{
            Debug.Log("BoostDashCancelDone!");
			BoostDash = false;
		});

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

	// 射撃
	string shotcode_keyboard;
	string shotcode_controller;

	// 格闘
	string wrestlecode_keyboard;
	string wrestlecode_controller;

	// ジャンプ
	string jump_keyboard;
	string jump_controller;

	// サーチ
	string search_keyboard;
	string search_controller;

	// アイテム
	string item_keyboard;
	string item_controller;

	// サブ射撃
	string subshot_keyboard;
	string subshot_controller;

	// 特殊格闘 
	string exwrestle_keyboard;
	string exwrestle_controller;

	// 特殊射撃
	string exshot_keyboard;
	string exshot_controller;

	// メニュー
	string menu_keyboard;
	string menu_controller;

	// コマンド
	string command_keyboard;
	string command_controller;

	// 視点変更上
	string viewchangeupper_keyboard;
	string viewchangeupper_controller;

	// 視点変更下
	string viewchangedown_keyboard;
	string viewchangedown_controller;

	// 視点変更左
	string viewchangeleft_keyboard;
	string viewchangeleft_controller;

	// 視点変更右
	string viewchangeright_keyboard;
	string viewchangeright_controller;

   

	/// <summary>
	/// キー入力取得(離したの無効化もしておく）
	/// </summary>
	public void GetKeyInput(int i,Array k)
	{
		// テンキーとマウス入力と画面クリック以外のキー入力を取得する
		if (k.GetValue(i).ToString().IndexOf("Arrow") < 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0 && k.GetValue(i).ToString().IndexOf("Joystick") < 0)
		{
			// 射撃取得
			if (k.GetValue(i).ToString() == shotcode_keyboard)
			{
				Shot = true;
                ShotKeyboad = true;
				ShotUp = false;
			}
			Debug.Log(k.GetValue(i).ToString() + jump_keyboard);
			
			// 格闘取得
			if (k.GetValue(i).ToString() == wrestlecode_keyboard)
			{
				Wrestle = true;
                WrestleKeyboard = true;
				WrestleUp = false;
			}
			// ジャンプ取得
			if (k.GetValue(i).ToString() == jump_keyboard)
			{
				Jump = true;
                JumpKeyboard = true;
				JumpUp = false;
			}
			// サーチ取得
			if (k.GetValue(i).ToString() == search_keyboard)
			{
				Search = true;
                SearchKeyboard = true;
				SearchUp = false;
			}
			// メニュー取得
			if (k.GetValue(i).ToString() == menu_keyboard)
			{
				Menu = true;
                MenuKeyboard = true;
				MenuUp = false;
				StartCoroutine(MenuButtonStopper());
			}
			// コマンド取得
			if (k.GetValue(i).ToString() == command_keyboard)
			{
				Command = true;
                CommandKeyboard = true;
				CommandUp = false;
			}
			// アイテム取得
			if (k.GetValue(i).ToString() == item_keyboard)
			{
				Item = true;
                ItemKeyboard = true;
				ItemUp = false;
			}
			// サブ射撃取得
			if (k.GetValue(i).ToString() == subshot_keyboard)
			{
				SubShot = true;
                SubShotKeyboard = true;
				SubShotUp = false;
			}
			// 特殊射撃取得
			if (k.GetValue(i).ToString() == exshot_keyboard)
			{
				EXShot = true;
                EXShotKeyboard = true;
				EXShotUp = false;
			}
			// 特殊格闘取得
			if (k.GetValue(i).ToString() == exwrestle_keyboard)
			{
				EXWrestle = true;
                EXWrestleKeyboard = true;
				EXWrestleUp = false;
			}
			// 視点変更上取得
			if (k.GetValue(i).ToString() == viewchangeupper_keyboard)
			{
				ElevationAngleUpper = true;
                ElevationAngleUpperKeyboard = true;
				ElevationAngleUpperUp = false;
			}
			// 視点変更下取得
			if (k.GetValue(i).ToString() == viewchangedown_keyboard)
			{
				ElevationAngleDown = true;
                ElevationAngleDownKeyboard = true;
				ElevationAngleDownUp = false;
			}
			// 視点変更左取得
			if (k.GetValue(i).ToString() == viewchangeleft_keyboard)
			{
				AzimuthLeft = true;
                AzimuthLeftKeyboard = true;
				AzimuthLeftUp = false;
			}
			// 視点変更右取得
			if (k.GetValue(i).ToString() == viewchangeright_keyboard)
			{
				AzimuthRight = true;
                AzimuthRightKeyboard = true;
				AzimuthRightUp = false;
			}
		}
		// キー入力取得（コントローラー）
		if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
		{
			// 射撃取得
			if (k.GetValue(i).ToString() == shotcode_controller)
			{
				Shot = true;
                ShotController = true;
				ShotUp = false;
			}
			// 格闘取得
			if (k.GetValue(i).ToString() == wrestlecode_controller)
			{
				Wrestle = true;
                WrestleController = true;
				WrestleUp = false;
			}
			// ジャンプ取得
			if (k.GetValue(i).ToString() == jump_controller)
			{
				Jump = true;
                JumpController = true;
				JumpUp = false;
			}
			// サーチ取得
			if (k.GetValue(i).ToString() == search_controller)
			{
				Search = true;
                SearchController = true;
				SearchUp = false;
			}
			// メニュー取得
			if (k.GetValue(i).ToString() == menu_controller)
			{
				Menu = true;
                MenuController = true;
				MenuUp = false;
				StartCoroutine(MenuButtonStopper());
			}
			// コマンド取得
			if (k.GetValue(i).ToString() == command_controller)
			{
				Command = true;
                CommandController = true;
				CommandUp = false;
			}
			// アイテム取得
			if (k.GetValue(i).ToString() == item_controller)
			{
				Item = true;
                ItemController = true;
				ItemUp = false;
			}
			// サブ射撃取得
			if (k.GetValue(i).ToString() == subshot_controller)
			{
				SubShot = true;
                SubShotController = true;
				SubShotUp = false;
			}
			// 特殊射撃取得
			if (k.GetValue(i).ToString() == exshot_controller)
			{
				EXShot = true;
                EXShotController = true;
				EXShotUp = false;
			}
			// 特殊格闘取得
			if (k.GetValue(i).ToString() == exwrestle_controller)
			{
				EXWrestle = true;
                EXWrestleController = true;
				EXWrestleUp = false;
			}
			// 視点変更上取得
			if (k.GetValue(i).ToString() == viewchangeupper_controller)
			{
				ElevationAngleUpper = true;
                ElevationAngleUpperController = true;
				ElevationAngleUpperUp = false;
			}
			// 視点変更下取得
			if (k.GetValue(i).ToString() == viewchangedown_controller)
			{
				ElevationAngleDown = true;
                ElevationAngleDownController = true;
				ElevationAngleDownUp = false;
			}
			// 視点変更左取得
			if (k.GetValue(i).ToString() == viewchangeleft_controller)
			{
				AzimuthLeft = true;
                AzimuthLeftController = true;
				AzimuthLeftUp = false;
			}
			// 視点変更右取得
			if (k.GetValue(i).ToString() == viewchangeright_controller)
			{
				AzimuthRight = true;
                AzimuthRightController = true;
				AzimuthRightUp = false;
			}
		}
	}

	/// <summary>
	/// フレーム終了時にメニューボタンのフラグを折る
	/// </summary>
	/// <returns></returns>
	public IEnumerator MenuButtonStopper()
	{
		yield return new WaitForEndOfFrame();
		Menu = false;
	}

    public IEnumerator JumpButtonStopper()
    {
        yield return new WaitForEndOfFrame();
        Jump = false;
    }

	/// <summary>
	/// 長押しを取得
	/// </summary>
	/// <param name="i"></param>
	/// <param name="k"></param>
    public void GetKeyLongInput(int i, Array k)
    {
        // テンキーとマウス入力と画面クリック以外のキー入力を取得する
        if (k.GetValue(i).ToString().IndexOf("Arrow") < 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0 && k.GetValue(i).ToString().IndexOf("Joystick") < 0)
        {
            // 射撃チャージ取得
            if (k.GetValue(i).ToString() == shotcode_keyboard)
            {
				Shot = false;
                Shotting = true;
            }
            // 格闘チャージ取得
            if (k.GetValue(i).ToString() == wrestlecode_keyboard)
            {
				Wrestle = false;
                Wrestling = true;
            }
            // ジャンプ取得
            if (k.GetValue(i).ToString() == jump_keyboard)
            {
				Jump = false;
                Jumping = true;
            }
        }
        // キー入力取得（コントローラー）
        if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
        {
            // 射撃取得
            if (k.GetValue(i).ToString() == shotcode_controller)
            {
				Shot = false;
                Shotting = true;
            }
            // 格闘取得
            if (k.GetValue(i).ToString() == wrestlecode_controller)
            {
				Wrestle = false;
                Wrestling = true;
            }
            // ジャンプ取得
            if (k.GetValue(i).ToString() == jump_controller)
            {
				// Jumpは折っておく
				Jump = false;
                Jumping = true;
            }
        }
    }


	/// <summary>
	/// キー入力解除を取得（解除の瞬間を取得する）
	/// </summary>
	/// <param name="i"></param>
	/// <param name="k"></param>
	public void GetKeyUp(int i, Array k)
	{
		// テンキーとマウス入力と画面クリック以外のキー入力を取得する
		if (k.GetValue(i).ToString().IndexOf("Arrow") < 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0 && k.GetValue(i).ToString().IndexOf("Joystick") < 0)
		{
			// 射撃取得
			if (k.GetValue(i).ToString() == shotcode_keyboard)
			{				
				ShotUp = true;
			}
			// 射撃アップ取得

			// 格闘取得
			if (k.GetValue(i).ToString() == wrestlecode_keyboard)
			{
				WrestleUp = true;
			}
			// ジャンプ取得
			if (k.GetValue(i).ToString() == jump_keyboard)
			{			
				JumpUp = true;
			}
			// サーチ取得
			if (k.GetValue(i).ToString() == search_keyboard)
			{
				SearchUp = true;
			}
			// メニュー取得
			if (k.GetValue(i).ToString() == menu_keyboard)
			{
				MenuUp = true;
			}
			// コマンド取得
			if (k.GetValue(i).ToString() == command_keyboard)
			{
				CommandUp = true;
			}
			// アイテム取得
			if (k.GetValue(i).ToString() == item_keyboard)
			{				
				ItemUp = true;
			}
			// サブ射撃取得
			if (k.GetValue(i).ToString() == subshot_keyboard)
			{
				SubShotUp = true;
			}
			// 特殊射撃取得
			if (k.GetValue(i).ToString() == exshot_keyboard)
			{				
				EXShotUp = true;
			}
			// 特殊格闘取得
			if (k.GetValue(i).ToString() == exwrestle_keyboard)
			{				
				EXWrestleUp = true;
			}
			// 視点変更上取得
			if (k.GetValue(i).ToString() == viewchangeupper_keyboard)
			{				
				ElevationAngleUpperUp = true;
			}
			// 視点変更下取得
			if (k.GetValue(i).ToString() == viewchangedown_keyboard)
			{
				ElevationAngleDownUp = true;
			}
			// 視点変更左取得
			if (k.GetValue(i).ToString() == viewchangeleft_keyboard)
			{
				AzimuthLeftUp = true;
			}
			// 視点変更右取得
			if (k.GetValue(i).ToString() == viewchangeright_keyboard)
			{
				AzimuthRightUp = true;
			}
		}
		// キー入力取得（コントローラー）
		if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
		{
			// 射撃取得
			if (k.GetValue(i).ToString() == shotcode_controller)
			{				
				ShotUp = true;
			}
			// 格闘取得
			if (k.GetValue(i).ToString() == wrestlecode_controller)
			{
				WrestleUp = true;
			}
			// ジャンプ取得
			if (k.GetValue(i).ToString() == jump_controller)
			{
				JumpUp = true;
			}
			// サーチ取得
			if (k.GetValue(i).ToString() == search_controller)
			{;
				SearchUp = true;
			}
			// メニュー取得
			if (k.GetValue(i).ToString() == menu_controller)
			{
				MenuUp = true;
			}
			// コマンド取得
			if (k.GetValue(i).ToString() == command_controller)
			{				
				CommandUp = true;
			}
			// アイテム取得
			if (k.GetValue(i).ToString() == item_controller)
			{
				ItemUp = true;
			}
			// サブ射撃取得
			if (k.GetValue(i).ToString() == subshot_controller)
			{				
				SubShotUp = true;
			}
			// 特殊射撃取得
			if (k.GetValue(i).ToString() == exshot_controller)
			{
				EXShotUp = true;
			}
			// 特殊格闘取得
			if (k.GetValue(i).ToString() == exwrestle_controller)
			{				
				EXWrestleUp = true;
			}
			// 視点変更上取得
			if (k.GetValue(i).ToString() == viewchangeupper_controller)
			{
				ElevationAngleUpperUp = true;
			}
			// 視点変更下取得
			if (k.GetValue(i).ToString() == viewchangedown_controller)
			{				
				ElevationAngleDownUp = true;
			}
			// 視点変更左取得
			if (k.GetValue(i).ToString() == viewchangeleft_controller)
			{				
				AzimuthLeftUp = true;
			}
			// 視点変更右取得
			if (k.GetValue(i).ToString() == viewchangeright_controller)
			{
				AzimuthRightUp = true;
			}
		}
	}

	/// <summary>
	/// 入力解除されたキーのフラグを折る
	/// </summary>
	/// <param name="i"></param>
	/// <param name="k"></param>
	public void GetKeyNotInput(int i, Array k)
	{
		// テンキーとマウス入力と画面クリック以外のキー入力を取得する
		if (k.GetValue(i).ToString().IndexOf("Arrow") < 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0 && k.GetValue(i).ToString().IndexOf("Joystick") < 0)
		{
			// 射撃取得
			if (k.GetValue(i).ToString() == shotcode_keyboard && ShotKeyboad)
			{
				Shot = false;
                Shotting = false;
                ShotKeyboad = false;
			}
			// 格闘取得
			if (k.GetValue(i).ToString() == wrestlecode_keyboard && WrestleKeyboard)
			{
				Wrestle = false;
                Wrestling = false;
                WrestleKeyboard = false;
			}
			// ジャンプ取得
			if (k.GetValue(i).ToString() == jump_keyboard && JumpKeyboard)
			{
				Jump = false;
                Jumping = false;
                JumpKeyboard = false;
			}
			// サーチ取得
			if (k.GetValue(i).ToString() == search_keyboard && SearchKeyboard)
			{
				Search = false;
                SearchKeyboard = false;
			}
			// メニュー取得
			if (k.GetValue(i).ToString() == menu_keyboard && MenuKeyboard)
			{
				Menu = false;
                MenuKeyboard = false;
			}
			// コマンド取得
			if (k.GetValue(i).ToString() == command_keyboard && CommandKeyboard)
			{
				Command = false;
                CommandKeyboard = false;
			}
			// サブ射撃取得
			if (k.GetValue(i).ToString() == subshot_keyboard && SubShotKeyboard)
			{
				SubShot = false;
                SubShotKeyboard = false;
			}
			// 特殊射撃取得
			if (k.GetValue(i).ToString() == exshot_keyboard && EXShotKeyboard)
			{
				EXShot = false;
                EXShotKeyboard = false;
			}
			// 特殊格闘取得
			if (k.GetValue(i).ToString() == exwrestle_keyboard && EXWrestleKeyboard)
			{
				EXWrestle = false;
                EXWrestleKeyboard = false;
			}
            // 視点変更上取得
            if (k.GetValue(i).ToString() == viewchangeupper_keyboard && ElevationAngleUpperKeyboard)
			{
				ElevationAngleUpper = false;
                ElevationAngleUpperKeyboard = false;
			}
			// 視点変更下取得
			if (k.GetValue(i).ToString() == viewchangedown_keyboard && ElevationAngleDownKeyboard)
			{
				ElevationAngleDown = false;
                ElevationAngleDownKeyboard = false;
			}
			// 視点変更左取得
			if (k.GetValue(i).ToString() == viewchangeleft_keyboard && AzimuthLeftKeyboard)
			{
				AzimuthLeft = false;
                AzimuthLeftKeyboard = false;
			}
			// 視点変更右取得
			if (k.GetValue(i).ToString() == viewchangeright_keyboard && AzimuthRightKeyboard)
			{
				AzimuthRight = false;
                AzimuthRightKeyboard = false;
			}
		}
		// キー入力取得（コントローラー）
		if (k.GetValue(i).ToString().IndexOf("Joystick1") >= 0 && k.GetValue(i).ToString().IndexOf("Mouse") < 0)
		{
			// 射撃取得
			if (k.GetValue(i).ToString() == shotcode_controller && ShotController)
			{
				Shot = false;
                Shotting = false;
                ShotController = false;
			}
			// 格闘取得
			if (k.GetValue(i).ToString() == wrestlecode_controller && WrestleController)
			{
				Wrestle = false;
                Wrestling = false;
                WrestleController = false;
			}
			// ジャンプ取得
			if (k.GetValue(i).ToString() == jump_controller && JumpController)
			{
				Jump = false;
                Jumping = false;
                JumpController = false;
			}
			// サーチ取得
			if (k.GetValue(i).ToString() == search_controller && SearchController)
			{
				Search = false;
                SearchController = false;
			}
			// メニュー取得
			if (k.GetValue(i).ToString() == menu_controller && MenuController)
			{
				Menu = false;
                MenuController = false;
			}
			// コマンド取得
			if (k.GetValue(i).ToString() == command_controller && CommandController)
			{
				Command = false;
                CommandController = false;
			}
			// サブ射撃取得
			if (k.GetValue(i).ToString() == subshot_controller && SubShotController)
			{
				SubShot = false;
                SubShotController = false;
			}
			// 特殊射撃取得
			if (k.GetValue(i).ToString() == exshot_controller && EXShotController)
			{
				EXShot = false;
                EXShotController = false;
			}
			// 特殊格闘取得
			if (k.GetValue(i).ToString() == exwrestle_controller && EXWrestleController)
			{
				EXWrestle = false;
                EXWrestleController = false;
			}
            // 視点変更上取得
            if (k.GetValue(i).ToString() == viewchangeupper_controller && ElevationAngleUpperController)
			{
				ElevationAngleUpper = false;
                ElevationAngleUpperController = false;
			}
			// 視点変更下取得
			if (k.GetValue(i).ToString() == viewchangedown_controller && ElevationAngleDownController)
			{
				ElevationAngleDown = false;
                ElevationAngleDownController = false;
			}
			// 視点変更左取得
			if (k.GetValue(i).ToString() == viewchangeleft_controller && AzimuthLeftController)
			{
				AzimuthLeft = false;
                AzimuthLeftController = false;
			}
			// 視点変更右取得
			if (k.GetValue(i).ToString() == viewchangeright_controller && AzimuthRightController)
			{
				AzimuthRight = false;
                AzimuthRightController = false;
			}
		}
	}

    /// <summary>
    /// 引数で指定した方向へステップ入力が成功しているか否か判定する
    /// </summary>
    /// <param name="inputdirection"></param>
    /// <returns></returns>
    public bool GetStepInput(InputDirection inputdirection)
    {
        // ニュートラル→inputdirectionが_InputDicretcionsに存在する？
        int appearindex = -1;
        for(int i=0; i<_Inputdirections.Length - 1; i++)
        {
            if(_Inputdirections[i] == InputDirection.NEUTRAL && _Inputdirections[i + 1] == inputdirection)
            {
                appearindex = i + 1;
                break;
            }            
        }
        // さらにその先にニュートラル→inputdirectionが存在する？
        if(appearindex != -1)
        {
            for(int i=appearindex; i<_Inputdirections.Length - 1; i++)
            {
                if (_Inputdirections[i] == InputDirection.NEUTRAL && _Inputdirections[i + 1] == inputdirection)
                {
                    InputDirectionFullClear();
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 入力ログをリセットする（連続ステップを防ぐ）
    /// </summary>
    public void InputDirectionFullClear()
    {
        for(int i=0; i<_Inputdirections.Length; i++)
        {
            _Inputdirections[i] = InputDirection.NEUTRAL;
        }
    }
}

/// <summary>
/// 方向キーの入力方向
/// </summary>
public enum InputDirection
{
    NEUTRAL,
    TOP,
    LEFTTOP,
    LEFT,
    LEFTUNDER,
    UNDER,
    RIGHTUNDER,
    RIGHT,
    RIGHTTOP
}
