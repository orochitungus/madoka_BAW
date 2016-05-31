using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class WeaponGauge : MonoBehaviour 
{
	/// <summary>
	/// 武装の画像
	/// </summary>
	public Image WeaponGraphic;

	/// <summary>
	/// チャージゲージ
	/// </summary>
	public Image Charge;

	/// <summary>
	/// チャージを使うか否か
	/// </summary>
	public bool UseChargeGauge;

    /// <summary>
    /// 現在のチャージ量（最大１）
    /// </summary>
    public float NowChargeValue;

	/// <summary>
	/// リロードゲージ
	/// </summary>
	public Image Reload;

	/// <summary>
	/// 残弾数表示
	/// </summary>
	public Text BulletNumber;

	/// <summary>
	/// 最大弾数
	/// </summary>
	public float MaxBulletNumber;

	/// <summary>
	/// 現在の弾数(リロードは小数で行い、フレームごとにいくつ回復するか決める）
	/// </summary>
	public float NowBulletNumber;

    /// <summary>
    /// 使用可能か否か（撃ちきりフルリロードなどの場合、このフラグを折っておく）
    /// </summary>
    public bool Use;

    /// <summary>
    /// 残弾数の表示桁数
    /// </summary>
    public int Digits;

    public Text Kind;

	// Use this for initialization
	void Start () 
	{
        this.UpdateAsObservable().Subscribe(_ => 
        {
            // 使用可能の場合、文字列を白に
            if(Use)
            {
                BulletNumber.color = new Color(1, 1, 1);
            }
            // 使用不能の場合、文字列を赤に
            else
            {
                BulletNumber.color = new Color(1, 0, 0);
            }
            // 現在の弾数を表示
            BulletNumber.text = NowBulletNumber.ToString(Digits.ToString());
            // リロード分を表示
            Reload.fillAmount = NowBulletNumber / MaxBulletNumber;
            // チャージを使う場合
            if (UseChargeGauge)
            {
                Charge.gameObject.SetActive(true);
                // 現在のチャージ量を表示
                Charge.fillAmount = NowChargeValue;
            }
            else
            {
                Charge.gameObject.SetActive(false);
            }
        });
	    
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
