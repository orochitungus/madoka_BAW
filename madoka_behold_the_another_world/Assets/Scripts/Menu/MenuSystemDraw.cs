using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class MenuSystemDraw : MonoBehaviour
{
    /// <summary>
	/// オプションボタン
	/// </summary>
	public OptionButtonController[] OptionButton;

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

    // 現在の入力
    public int NowSelect;

    /// <summary>
	/// 説明文
	/// </summary>
	public Text Informationtext;

	// 1F前の左入力
	private bool _PreLeftInput;
	// 1F前の右入力
	private bool _PreRightInput;

	// Use this for initialization
	void Start ()
    {
        this.UpdateAsObservable().Subscribe(_ =>
        {
            // ゲージ変形量反映
            BGMGauge.fillAmount = PlayerPrefs.GetFloat("BGMVolume");
            SEGauge.fillAmount = PlayerPrefs.GetFloat("SEVolume");
            VoiceGauge.fillAmount = PlayerPrefs.GetFloat("VoiceVolue");

            for (int i = 0; i < OptionButton.Length; i++)
            {
                if (i == NowSelect)
                {
                    OptionButton[i].IsSelect = true;
                }
                else
                {
                    OptionButton[i].IsSelect = false;
                }
            }

			// BGM,SE,Voiceでは方向キー左右でボリューム調整
			// 左
			if (!_PreLeftInput && ControllerManager.Instance.Left)
			{
				switch (NowSelect)
				{
					case 0:
						if (PlayerPrefs.GetFloat("BGMVolume") <= 0.05f)
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
			if (!_PreRightInput && ControllerManager.Instance.Right)
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

		});

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
