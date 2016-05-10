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

    /// <summary>
    /// 哨戒モードにおける起点(僚機の時はPC）
    /// </summary>
    public GameObject StartingPoint;
	public GameObject EndingPoint;

    /// <summary>
    /// 歩き撃ちフラグ
    /// </summary>
    protected bool RunShotDone;

	/// <summary>
	/// 覚醒技演出中フラグ
	/// </summary>
	public bool ArousalAttackProduction;

    /// <summary>
    /// プレイヤーであるか否か（これがfalseであると描画系の関数全カット）
    /// それぞれの意味は以下の通り
    /// </summary>
    public enum CHARACTERCODE
	{
		PLAYER,         // プレイヤー操作
		PLAYER_ALLY,    // プレイヤー僚機
		ENEMY           // 敵
	};
	public CHARACTERCODE IsPlayer;

    /// <summary>
    /// 弾の種類(CharacterSpecの配置順に合わせること）
    /// </summary>
    public enum ShotType
	{
		NORMAL_SHOT,    // 通常射撃
		CHARGE_SHOT,    // チャージ射撃
		SUB_SHOT,       // サブ射撃
		EX_SHOT,        // 特殊射撃        
	};

    /// <summary>
    /// 射撃時の射出モード
    /// </summary>
    public enum ShotMode
	{
		NORMAL,     // 通常状態
		RELORD,     // 構え
		SHOT,       // 射出
		SHOTDONE,   // 射出完了
		AIRDASHSHOT // 射出（空中ダッシュ射撃）
	};
	public ShotMode Shotmode;

    /// <summary>
    /// 誰であるか
    /// </summary>
    public Character_Spec.CHARACTER_NAME CharacterName;

    /// <summary>
    /// 頭部オブジェクト
    /// </summary>
    public GameObject HeadObject;

    /// <summary>
    /// 胸部オブジェクト
    /// </summary>
    public GameObject BrestObject;

    /// <summary>
    /// 覚醒時エフェクト
    /// </summary>
    public GameObject ArousalEffect;

    /// <summary>
    /// ポーズコントローラー
    /// </summary>
    private GameObject Pausecontroller;

    // CPU
    /// <summary>
    /// ルーチンを拾う
    /// </summary>
    AIControl_Base.CPUMODE CPUMode;

    /// <summary>
    /// テンキー入力を拾う
    /// </summary>
    AIControl_Base.TENKEY_OUTPUT CPUTenkey;

    /// <summary>
    /// キー入力を拾う
    /// </summary>
    AIControl_Base.KEY_OUTPUT CPUKey;



    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
