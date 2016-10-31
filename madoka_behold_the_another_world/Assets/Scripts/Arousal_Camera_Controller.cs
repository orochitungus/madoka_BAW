using UnityEngine;
using System.Collections;

public class Arousal_Camera_Controller : MonoBehaviour
{

    // 親元のゲームオブジェクト
    public GameObject m_insp_Master;
    // 追跡するカメラターゲット
    public GameObject m_inspCameraTarget;

    // ポーズマネージャー
    private GameObject Pausemanager;
    // カットインカメラアニメーション
    public Animation m_CutinCameraAnimation;

    // カットインシステム
	public CutinSystem m_CutinSystem;
	// カットインで表示されるグラフィックの種類
	public CutinSystem.CUTINNAME m_CutinName;

	/// <summary>
	/// インターフェース
	/// </summary>
	public BattleInterfaceController Battleinterfacecontroller;

    /// <summary>
    /// オーディオマネージャー
    /// </summary>
    public AudioManager Audiomanager;

	private float cutinXposint;

    void Awake()
    {
        Camera camera = GetComponentInChildren<Camera>();
        camera.enabled = false;
    }

	// Use this for initialization
	void Start () 
    {
        SetUpCutin();
		// ポーズマネージャー取得
		Pausemanager = GameObject.Find("PauseManager");
		// カットインシステム取得
		m_CutinSystem = GameObject.Find("CutinSystem").GetComponent<CutinSystem>();
		// インターフェース取得
		Battleinterfacecontroller = GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>();
        // オーディオマネージャー取得
        Audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    
        
    /// <summary>
    /// 覚醒カメラ起動＆カットイン有効化
    /// </summary>
    public void UseAsousalCutinCamera()
    {
        // カメラを規定位置に配置
        SetUpCutin();
        // SE再生
        Audiomanager.PlaySE("Arousal");
        // カットイン画像表示
		m_CutinSystem.ShowCutin(m_CutinName);
        // カットインカメラ再生
        m_CutinCameraAnimation.Play("ArousalCameraAnimation");
    }

    // カットインが入る前に配置位置をセット
    public void SetUpCutin()
    {
        transform.localPosition = new Vector3(-0.976237f, 1.120372f, 4.067078f);
    }

	/// <summary>
	/// カットイン終了後の処理
	/// </summary>
	public void CutinEnd()
    {
		// 時間を再度動かす
		Pausemanager.GetComponent<PauseManager>().ArousalPauseController.ProcessButtonPress();
        // 位置固定を解除する
        m_insp_Master.GetComponent<CharacterControlBase>().UnFreezePositionAll();
		// 消していたインターフェースを復活させる
		Battleinterfacecontroller.DrawInterface();
		m_insp_Master.GetComponent<CharacterControlBase>().Timestopmode = CharacterControlBase.TimeStopMode.NORMAL;
		// カメラを切っておく
		Camera camera = GetComponentInChildren<Camera>();
		camera.enabled = false;
		// カットイン画像消去
		m_CutinSystem.EraseCutin(m_CutinName);
		// カメラの位置を戻しておく
		transform.localPosition = new Vector3(-0.976237f, 1.120372f, 4.067078f);
        // アニメーションを巻き戻す
        m_CutinCameraAnimation.Rewind();
		// アニメーションを止める
		m_CutinCameraAnimation.Stop();
		// 覚醒発動キャラの全武装をリロードする
		switch (m_insp_Master.GetComponent<CharacterControlBase>().CharacterName)
		{
			// 弓ほむら
			case Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B:
				m_insp_Master.GetComponent<HomuraBowControl>().FullReload();
				break;
		}
	}

}
