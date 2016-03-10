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
    
    public NOWSELECT Nowselect;

    // Use this for initialization
    void Start () 
	{
        // 選択対象を初期化
        Nowselect = NOWSELECT.SHOT;
        		
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (Input.anyKeyDown)
        {
            Array k = Enum.GetValues(typeof(KeyCode));
            for (int i = 0; i < k.Length; i++)
            {
                if (Input.GetKeyDown((KeyCode)k.GetValue(i)))
                {
                    Debug.Log(k.GetValue(i).ToString());
                }
            }
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
}
