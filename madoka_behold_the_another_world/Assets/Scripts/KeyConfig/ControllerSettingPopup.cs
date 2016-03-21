using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class ControllerSettingPopup : MonoBehaviour
{
    public Animator Controllersettingpopup;
	public KeyConfigController Keyconfigcontroller;

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
    /// OKボタンを押した場合、スティック設定に入る
    /// </summary>
    public void OnClickOKButton()
    {
		// ついでに再出現しないように元のフラグも折っておく
		Controllersettingpopup.SetBool("ControllerSettingClose", false);
		Controllersettingpopup.SetBool("Conctroller2Appear", true);
		Keyconfigcontroller.Nowmode = KeyConfigController.NowMode.RIGHTSTICK2UPPERCHECK;
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
		
	}

	/// <summary>
	/// ２個目のポップアップが出ているときのキャンセルボタン
	/// </summary>
	public void OnClcikCancelButton2()
	{

	}


	// Use this for initialization
	void Start ()
    {
		if(Controllersettingpopup == null)
		{
			Controllersettingpopup = GameObject.Find("ControllerSelectPopUp").GetComponent<Animator>();
		}

		// 2個目のポップアップのタイトル&質問文字列
		this.UpdateAsObservable().Subscribe(_ =>
		{
			switch (Keyconfigcontroller.Nowmode)
			{
				case KeyConfigController.NowMode.RIGHTSTICK2UPPERCHECK:
					Popup2Title.text = "INPUT \n \"RIGHTSTICK UPPER\"";
					Popup2QuestionText.text = "右スティックを上に倒してください\n中止する場合はCANCELを押してください";
					break;
				case KeyConfigController.NowMode.RIGHTSTICK2DOWNCHECK:
					Popup2Title.text = "INPUT \n \"RIGHTSTICK DOWN\"";
					Popup2QuestionText.text = "右スティックを下に倒してください\n中止する場合はCANCELを押してください";
					break;
				case KeyConfigController.NowMode.RIGHTSTICK2LEFTCHECK:
					Popup2Title.text = "INPUT \n \"RIGHTSTICK LEFT\"";
					Popup2QuestionText.text = "右スティックを左に倒してください\n中止する場合はCANCELを押してください";
					break;
				case KeyConfigController.NowMode.RIGHTSTICK2RIGHTCHECK:
					Popup2Title.text = "INPUT \n \"RIGHTSTICK RIGHT\"";
					Popup2QuestionText.text = "右スティックを右に倒してください\n中止する場合はCANCELを押してください";
					break;
				case KeyConfigController.NowMode.RICHTSTICKFINALCHECK:
					Popup2Title.text = "SETTING COMPLETE!!";
					Popup2QuestionText.text = "右スティックの設定が完了しました\n確定する場合はOKを、やり直す場合はCANCELを押してください";
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
						Popup2AnswerText.text = "上動作が認識されました\nＯＫボタンを押してください";
						break;
					case KeyConfigController.NowMode.RIGHTSTICK2DOWNCHECK:
						Popup2AnswerText.text = "下動作が認識されました\nＯＫボタンを押してください";
						break;
					case KeyConfigController.NowMode.RIGHTSTICK2LEFTCHECK:
						Popup2AnswerText.text = "左動作が認識されました\nＯＫボタンを押してください";
						break;
					case KeyConfigController.NowMode.RIGHTSTICK2RIGHTCHECK:
						Popup2AnswerText.text = "右動作が認識されました\nＯＫボタンを押してください";
						break;
				}
			}
		});
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

	public string GetStickInput()
	{
		//右スティック（追加）
		// 3rd取得
		if (Input.GetAxisRaw("Vertical2") < 0)
		{
			return "3rdAxis PLUS";
		}
		else if (0 < Input.GetAxisRaw("Vertical2"))
		{			
			return "3rdAxis MINUS";
		}
		// 5th取得
		if (Input.GetAxisRaw("Horizontal2") < 0)
		{
			//左に傾いている
			return "5thAxis MINUS";
		}
		else if (0 < Input.GetAxisRaw("Horizontal2"))
		{
			//右に傾いている
			return "5thAxis PLUS";
		}

		// 4th取得
		if (Input.GetAxisRaw("Vertical3") < 0)
		{
			return "4thAxis PLUS";
		}
		else if (0 < Input.GetAxisRaw("Vertical3"))
		{
			return "4thAxis MINUS";
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
			return "7thAxis PLUS";
		}
		else if (0 < Input.GetAxisRaw("Vertical4"))
		{
			return "7thAxis MINUS";
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
}
