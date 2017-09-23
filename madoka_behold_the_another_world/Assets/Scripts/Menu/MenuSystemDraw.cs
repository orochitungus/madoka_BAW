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


        });

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
