using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class OptionController : MonoBehaviour 
{
	// 現在の入力
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

	/// <summary>
	/// オプションボタン
	/// </summary>
	public OptionButtonController[] OptionButton;

	/// <summary>
	/// 説明文
	/// </summary>
	public Text Informationtext;

	public TitleCanvas Titlecanvas;

	/// <summary>
	/// BGMのゲージ
	/// </summary>
	public Image BGMGauge;

	/// <summary>
	/// SEのゲージ
	/// </summary>
	public Image SEGauge;

	/// <summary>
	/// Voiceのゲージ
	/// </summary>
	public Image VoiceGauge;

	// Use this for initialization
	void Start () 
	{			

		this.UpdateAsObservable().Subscribe(_ => 
		{
			// ゲージ変形量反映
			BGMGauge.fillAmount = PlayerPrefs.GetFloat("BGMVolume");
			SEGauge.fillAmount = PlayerPrefs.GetFloat("SEVolume");
			VoiceGauge.fillAmount = PlayerPrefs.GetFloat("VoiceVolue");

			for(int i=0; i<OptionButton.Length; i++)
			{
				if(i == NowSelect)
				{
					OptionButton[i].IsSelect = true;
				}
				else
				{
					OptionButton[i].IsSelect = false;
				}
			}

			// 方向キー上下で選択対象切り替え
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
				if (NowSelect < 3)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowSelect++;
				}
			}

			// BGM,SE,Voiceでは方向キー左右でボリューム調整
			// 左
			if(!_PreLeftInput && ControllerManager.Instance.Left)
			{
				switch (NowSelect)
				{
					case 0:
						if(PlayerPrefs.GetFloat("BGMVolume") <= 0.05f)
						{
							PlayerPrefs.SetFloat("BGMVolume", 0.01f);
						}
						else
						{
							float nextvalue = PlayerPrefs.GetFloat("BGMVolume") - 0.05f;
							PlayerPrefs.SetFloat("BGMVolume", nextvalue);
						}
						AudioManager.Instance.ChangeBGMVol(PlayerPrefs.GetFloat("BGMVolume"));
						break;
					case 1:
						if (PlayerPrefs.GetFloat("SEVolume") <= 0.05f)
						{
							PlayerPrefs.SetFloat("SEVolume", 0.01f);
						}
						else
						{
							float nextvalue = PlayerPrefs.GetFloat("SEVolume") - 0.05f;
							PlayerPrefs.SetFloat("SEVolume", nextvalue);
						}
						AudioManager.Instance.PlaySE("OK");
						break;
					case 2:
						if (PlayerPrefs.GetFloat("VoiceVolue") <= 0.05f)
						{
							PlayerPrefs.SetFloat("VoiceVolue", 0.01f);
						}
						else
						{
							float nextvalue = PlayerPrefs.GetFloat("VoiceVolue") - 0.05f;
							PlayerPrefs.SetFloat("VoiceVolue", nextvalue);
						}
						break;
				}
			}
			// 右
			if(!_PreRightInput && ControllerManager.Instance.Right)
			{
				switch (NowSelect)
				{
					case 0:
						if (PlayerPrefs.GetFloat("BGMVolume") >= 1.0f)
						{
							PlayerPrefs.SetFloat("BGMVolume", 1.0f);
						}
						else
						{
							float nextvalue = PlayerPrefs.GetFloat("BGMVolume") + 0.05f;
							PlayerPrefs.SetFloat("BGMVolume", nextvalue);
						}
						AudioManager.Instance.ChangeBGMVol(PlayerPrefs.GetFloat("BGMVolume"));
						break;
					case 1:
						if (PlayerPrefs.GetFloat("SEVolume") >= 1.0f)
						{
							PlayerPrefs.SetFloat("SEVolume", 1.0f);
						}
						else
						{
							float nextvalue = PlayerPrefs.GetFloat("SEVolume") + 0.05f;
							PlayerPrefs.SetFloat("SEVolume", nextvalue);
						}
						AudioManager.Instance.PlaySE("OK");
						break;
					case 2:
						if (PlayerPrefs.GetFloat("VoiceVolue") >= 1.0f)
						{
							PlayerPrefs.SetFloat("VoiceVolue", 1.0f);
						}
						else
						{
							float nextvalue = PlayerPrefs.GetFloat("VoiceVolue") + 0.05f;
							PlayerPrefs.SetFloat("VoiceVolue", nextvalue);
						}
						break;
				}
			}

			// 説明文表示
			switch (NowSelect)
			{
				case 0:
					Informationtext.text = "方向キー左右でBGMのボリュームを調整します\nシャンプでひとつ前の画面に戻ります";
					break;
				case 1:
					Informationtext.text = "方向キー左右でSEのボリュームを調整します\nシャンプでひとつ前の画面に戻ります";
					break;
				case 2:
					Informationtext.text = "方向キー左右でVOICEのボリュームを調整します\nシャンプでひとつ前の画面に戻ります";
					break;
				case 3:
					Informationtext.text = "ショットを押すとキーコンフィグ画面へ移動します\nシャンプでひとつ前の画面に戻ります";
					break;
			}

			// ショットで選択確定
			if (!ModeChangeDone && ControllerManager.Instance.Shot && NowSelect == 3)
			{
				AudioManager.Instance.PlaySE("OK");
				FadeManager.Instance.LoadLevel("KeyConfig", 1.0f);
			}

			// ジャンプで戻る
			if (!ModeChangeDone && ControllerManager.Instance.Jump)
			{
				AudioManager.Instance.PlaySE("cursor");
				Titlecanvas.TitleCanvasAnimator.Play(Titlecanvas.OptionClose);
			}

			// 上
			_PreTopInput = ControllerManager.Instance.Top;
			// 下
			_PreUnderInput = ControllerManager.Instance.Under;
			// 左
			_PreLeftInput = ControllerManager.Instance.Left;
			// 右
			_PreRightInput = ControllerManager.Instance.Right;
			// ボタンを離すとモードチェンジ瞬間フラグを折る(一旦ボタンを離さないと次へ行かせられない）
			if (!ControllerManager.Instance.Shot)
			{
				ModeChangeDone = false;
			}
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
