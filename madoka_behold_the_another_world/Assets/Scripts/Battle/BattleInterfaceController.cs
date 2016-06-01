using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public class BattleInterfaceController : MonoBehaviour
{
    /// <summary>
    /// キャラクター名表示部分
    /// </summary>
    public Text CharacterName;

    /// <summary>
    /// キャラクターグラフィック表示部分（順にプレイヤー・僚機1・僚機2）
    /// </summary>
    public PlayerBG[] Playerbg;

	/// <summary>
	/// プレイヤーのHP表示部分（順にプレイヤー・僚機1・僚機2）
	/// </summary>
	public Text[] PlayerHP;

	/// <summary>
	/// 現在のプレイヤーHP（順にプレイヤー・僚機1・僚機2）
	/// </summary>
	public int[] NowPlayerHP = new int[3];

	/// <summary>
	/// 現在のプレイヤー最大HP（順にプレイヤー・僚機1・僚機2）
	/// </summary>
	public int[] MaxPlayerHP = new int[3];

	/// <summary>
	/// 僚機１の命令の文字列
	/// </summary>
	public Text Ally1Command;

	/// <summary>
	/// 僚機２の命令の文字列
	/// </summary>
	public Text Ally2Command;

    /// <summary>
    /// プレイヤーのレベル表示部分
    /// </summary>
    public Text PlayerLv;

    public int NowPlayerLV;
	   

    /// <summary>
    /// Information表示部分
    /// </summary>
    public Text InformationText;

    /// <summary>
    /// 覚醒ゲージ
    /// </summary>
    public Image ArousalGauge;

    /// <summary>
    /// 現在の覚醒ゲージ
    /// </summary>
    public float NowArousal;

    /// <summary>
    /// 覚醒ゲージ最大量
    /// </summary>
    public float MaxArousal;

    /// <summary>
    /// 覚醒可能最小値
    /// </summary>
    public float BiasArousal;

    /// <summary>
    /// 覚醒使用可否表示
    /// </summary>
    public Text OK;

    /// <summary>
    /// ブーストゲージ
    /// </summary>
    public Image BoostGauge;

    /// <summary>
    /// 現在のブースト量
    /// </summary>
    public float NowBoost;

    /// <summary>
    /// 最大のブースト量
    /// </summary>
    public float MaxBoost;

    /// <summary>
    /// ウェポンゲージ最下段
    /// </summary>
    public WeaponGauge Weapon1;

    /// <summary>
    /// 以下下から順
    /// </summary>
    public WeaponGauge Weapon2;

    public WeaponGauge Weapon3;

    public WeaponGauge Weapon4;

    public WeaponGauge Weapon5;

    /// <summary>
    /// アイテム名表示部
    /// </summary>
    public Text ItemName;

    /// <summary>
    /// 装備アイテム名
    /// </summary>
    public string Itemname;

    /// <summary>
    /// アイテム個数
    /// </summary>
    public Text ItemNumber;

    public int Itemnumber;

	// Use this for initialization
	void Start ()
    {
        this.UpdateAsObservable().Subscribe(_ => 
        {
            // 覚醒ゲージ表示
            ArousalGauge.fillAmount = NowArousal / MaxArousal;
            // 閾値を超えていたらOKを表示
            if(NowArousal > BiasArousal)
            {
                OK.gameObject.SetActive(true);
            }
            else
            {
                OK.gameObject.SetActive(false);
            }
            // ブーストゲージを表示
            BoostGauge.fillAmount = NowBoost / MaxBoost;


			for (int i = 0; i<Playerbg.Length; i++)
			{
				// HP表示
				PlayerHP[i].text = NowPlayerHP[i].ToString();

				// HPが1/4を割っていたら赤色
				if (NowPlayerHP[i] < 0.25f * MaxPlayerHP[i])
				{
					PlayerHP[i].color = new Color(1.0f, 0.0f, 0.0f);
				}
				// HPが1/2を割っていたら黄色
				else if (NowPlayerHP[i] < 0.5f * MaxPlayerHP[i])
				{
					PlayerHP[i].color = new Color(0.96f, 1.0f, 0.21f);
				}
				// それ以外なら黒
				else
				{
					PlayerHP[i].color = new Color(0.0f, 0.0f, 0.0f);
				}
			}

        });
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
