using UnityEngine;
using System.Collections;

public class Arousal_Camera_Controller : MonoBehaviour {

    // 親元のゲームオブジェクト
    public GameObject m_insp_Master;
    // 追跡するカメラターゲット
    public GameObject m_inspCameraTarget;

    // 背景画像
    public Texture2D m_insp_BackGround;

    // カットイン画像
    public Texture2D m_insp_Cutin;

    // このスクリプトを実行するか否か
    public bool m_UseArousalCamera;

    // ポーズコントローラー
    private GameObject m_pausecontroller;

    void Awake()
    {
        m_UseArousalCamera = false;
        Camera camera = GetComponentInChildren<Camera>();
        camera.enabled = false;
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

    // 位置制御
    void LateUpdate()
    {
        // 本体の位置を取得
        Vector3 masterPos = m_inspCameraTarget.transform.position;
        
        // 常時相手を見る
        transform.rotation = Quaternion.LookRotation(new Vector3(masterPos.x,masterPos.y,masterPos.z) - transform.position);
       
    }

    private float   cutinXposint;
    private float   m_cutintime;                    // カットインが出現しきってからの累積時間
    private const float m_CutinAppearTime = 1.0f;   // カットインの出現時間
    public AudioClip m_insp_ArousalSE;
    private bool    m_playSE;

    // 背景及びカットイン
    void OnGUI()
    {
        if (m_UseArousalCamera)
        {            
            if (!m_playSE)
            {
                // SE再生
                AudioSource.PlayClipAtPoint(m_insp_ArousalSE, transform.position);
                m_playSE = true;
            }
            // カットインを描画
            GUI.DrawTexture(new Rect(cutinXposint, 0.0f, 1024.0f, 1024.0f), m_insp_Cutin);
            if (cutinXposint < 0)
            {
                cutinXposint += 8.0f;
                transform.localPosition = new Vector3(transform.localPosition.x + 0.05f, transform.localPosition.y, transform.localPosition.z);
            }
            else
            {
                m_cutintime += Time.deltaTime;
            }
            if (m_cutintime > m_CutinAppearTime)
            {                              
                // カットイン描画フラグを折る
                m_UseArousalCamera = false;
                // カメラを無効化する
                camera.enabled = false;
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
            }
        }
        // Startに置いておくと、2回目以降初期化してくれないため
        else
        {
            SetUpCutin();
        }
    }
    // カットインが入る前に配置位置をセット
    public void SetUpCutin()
    {
        cutinXposint = -1024.0f;
        transform.localPosition = new Vector3(-0.976237f, 1.120372f, 4.067078f);
        m_cutintime = 0;
        m_playSE = false;
    }
}
