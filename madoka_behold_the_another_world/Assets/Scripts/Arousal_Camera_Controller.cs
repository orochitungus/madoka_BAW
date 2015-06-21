using UnityEngine;
using System.Collections;

public class Arousal_Camera_Controller : MonoBehaviour {

    // 親元のゲームオブジェクト
    public GameObject m_insp_Master;
    // 追跡するカメラターゲット
    public GameObject m_inspCameraTarget;

    // ポーズコントローラー
    private GameObject m_pausecontroller;
    // カットインカメラアニメーション
    public Animation m_CutinCameraAnimation;

    // キャンバス
    public GameObject m_canvasOrigin;
    public CutinAnimation m_canvas;

    private float cutinXposint;
    public AudioClip m_insp_ArousalSE;
    private bool m_playSE;

    void Awake()
    {
        Camera camera = GetComponentInChildren<Camera>();
        camera.enabled = false;
        m_canvasOrigin.SetActive(false);
    }

	// Use this for initialization
	void Start () 
    {
        SetUpCutin();
        // ポーズコントローラー取得
        m_pausecontroller = GameObject.Find("Pause Controller");        
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    
        
    // 覚醒カットインカメラ起動
    public void UseAsousalCutinCamera()
    {
        // カメラを規定位置に配置
        SetUpCutin();
        // SE再生
        AudioSource.PlayClipAtPoint(m_insp_ArousalSE, transform.position);
        m_playSE = true;
        // Canvas有効化
        m_canvasOrigin.SetActive(true);
        m_canvas.StartAnimation();
        // カットインカメラ再生
        m_CutinCameraAnimation.Play("ArousalCameraAnimation");
    }

    // カットインが入る前に配置位置をセット
    public void SetUpCutin()
    {
        transform.localPosition = new Vector3(-0.976237f, 1.120372f, 4.067078f);
    }

    // カットイン終了後の処理
    public void CutinEnd()
    {       
        m_insp_Master.GetComponent<CharacterControl_Base>().ReleasePause();
        // 時間を再度動かす
        // ポーズコントローラーのインスタンスを取得
        var m_pausecontroller2 = m_pausecontroller.GetComponent<PauseControllerInputDetector>();
        m_pausecontroller2.pauseController.DeactivatePauseProtocol();
        // 位置固定を解除する
        m_insp_Master.GetComponent<CharacterControl_Base>().UnFreezePositionAll();
        // 消していたインターフェースを復活させる
        // CharacterControl_Base依存
        m_insp_Master.GetComponent<CharacterControl_Base>().m_DrawInterface = true;
        // Player_Camera_Controller依存
        var master = m_insp_Master.GetComponent<CharacterControl_Base>();
        master.m_MainCamera.GetComponent<Player_Camera_Controller>().m_DrawInterface = true;
        master.m_timstopmode = CharacterControl_Base.TimeStopMode.NORMAL;
        // カメラを切っておく
        Camera camera = GetComponentInChildren<Camera>();
        camera.enabled = false;
        // アニメーションを切っておく
        m_canvas.EndAnimation();
        // canvasを消しておく
        m_canvasOrigin.SetActive(false);
        // カメラの位置を戻しておく
        transform.localPosition = new Vector3(-0.976237f, 1.120372f, 4.067078f);
    }
}
