using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;

// アクションステージで使うキャラの基底クラス
public class CharacterControlBase : MonoBehaviour
{
    /// <summary>
    /// 本体を追従するカメラ
    /// </summary>
    public Camera MainCamera;

    /// <summary>
    /// 覚醒演出用カメラ
    /// </summary>
    public Camera ArousalCamera;

    /// <summary>
    /// 地面設置確認用のレイの発射位置(コライダの中心とオブジェクトの中心)
    /// </summary>
    public Vector3 LayOriginOffs;

    /// <summary>
    /// レイの長さ
    /// </summary>
    private float _Laylength;

    /// <summary>
    /// ground属性をもったレイヤー（この場合8）
    /// </summary>
    protected int LayMask;

    /// <summary>
    /// このオブジェクトのRigidbody
    /// </summary>
    protected Rigidbody RigidBody;

	// 哨戒モードにおける起点(僚機の時はPC）
	public GameObject StartingPoint;
	public GameObject EndingPoint;

	// 歩き撃ちフラグ
	protected bool RunShotDone;

	/// <summary>
	/// 覚醒技演出中フラグ
	/// </summary>
	public bool ArousalAttackProduction;

	// プレイヤーであるか否か（これがfalseであると描画系の関数全カット）
	// それぞれの意味は以下の通り
	public enum CHARACTERCODE
	{
		PLAYER,         // プレイヤー操作
		PLAYER_ALLY,    // プレイヤー僚機
		ENEMY           // 敵
	};
	public CHARACTERCODE IsPlayer;

	// 弾の種類(CharacterSpecの配置順に合わせること）
	public enum ShotType
	{
		NORMAL_SHOT,    // 通常射撃
		CHARGE_SHOT,    // チャージ射撃
		SUB_SHOT,       // サブ射撃
		EX_SHOT,        // 特殊射撃        
	};

	// 射撃時の射出モード
	public enum ShotMode
	{
		NORMAL,     // 通常状態
		RELORD,     // 構え
		SHOT,       // 射出
		SHOTDONE,   // 射出完了
		AIRDASHSHOT // 射出（空中ダッシュ射撃）
	};
	public ShotMode Shotmode;



	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
