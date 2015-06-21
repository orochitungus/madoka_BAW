using UnityEngine;
using System.Collections;

// レーダーを描画する（PC以外は使用禁止）
public class DrawRadar : MonoBehaviour 
{
    public Texture2D m_radarBase;           // レーダーの背景
  

    public GameObject m_Player;             // レーダーの中心となるキャラクター

    public Camera     m_PerspectiveCamera;  // レーダーのもととなるカメラ 

    public Texture m_MarkerMine;          // マーカー自機（視点範囲つき）
    public Texture m_MarkerAlly;          // マーカー僚機
    public Texture m_MarkerEnemy;         // マーカー敵機

    public GameObject m_MarkerObject;       // マーカーが乗っているオブジェクト

    private GameObject m_playerObject = null;      // 画面に出現している「プレイヤーが制御するキャラ」

    private const float m_rotcalcbias = 1.0f;      // カメラの方向計算のときに、特異姿勢バグを起こさないようにするための閾値

    private Vector3 m_rotate_OR;            // レーダーカメラの元角度
    private Vector3 m_position_OR;          // レーダーカメラの元位置

    private Vector3 m_blackBoardRot_OR;     // BlackBoradの元の角度
    private Vector3 m_blackBoardPos_OR;     // BlackBoradの元の位置
    private Vector3 m_platePos_OR;          // Plateの元の位置

	void Start () 
    {
        // 属性「自機」以外は描画せず、カメラも無効
        var target = m_Player.GetComponentInChildren<CharacterControl_Base>();

        // マーカーのテクスチャを張り替え
        // 僚機
        if (target.m_isPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER_ALLY)
        {
            m_MarkerObject.renderer.material.mainTexture = (Texture)Instantiate(m_MarkerAlly);
        }
        // 敵機
        else if (target.m_isPlayer == CharacterControl_Base.CHARACTERCODE.ENEMY)
        {
            m_MarkerObject.renderer.material.mainTexture = (Texture)Instantiate(m_MarkerEnemy);
        }
        // カメラの元角度を取得
        m_rotate_OR = m_PerspectiveCamera.transform.rotation.eulerAngles;

        // カメラの元の位置を取得(本体との相対位置）
        m_position_OR = m_PerspectiveCamera.transform.localPosition;

        // BlackBoardの元の角度を取得
        var BB = m_Player.GetComponentInChildren<BlackBoard>();
        m_blackBoardRot_OR = BB.transform.rotation.eulerAngles;
        // 元の位置を取得(元位置のオフセットなので、相対）
        m_blackBoardPos_OR = BB.transform.localPosition;

        // Plateの元の位置を取得
        var Pl = m_Player.GetComponentInChildren<Marker>();
        // 元の位置を取得(元位置のオフセットなので、相対）
        m_platePos_OR = Pl.transform.localPosition;

        if (target.m_isPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
        {
            // プレイヤーキャラを取得する
            // 全オブジェクトを検索
            GameObject[] kouho = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (GameObject go in kouho)
            {
                // CharacterControl_Baseを持たないものも検索されるので、そういうのは除外
                if (go.GetComponentInChildren<CharacterControl_Base>() == null)
                {
                    continue;
                }
                if (go.GetComponentInChildren<CharacterControl_Base>().m_isPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER)
                {
                    m_playerObject = go;
                    break;
                }
            }
            m_PerspectiveCamera.enabled = false;
            return;
        }   
        
	}   

    // Update is called once per frame
    void Update() 
    {
        // 自身の高さを取得する
        Vector3 nowpos = m_Player.transform.position;
        // 敵機または僚機だった場合、マーカーの位置をPCの高さと同じにする
        if (m_playerObject != null)
        {           
            // プレイヤーキャラの高さを取得する
            Vector3 pcpos = m_playerObject.transform.position;
            // マーカーを取得する
            var marker = m_Player.GetComponentInChildren<Marker>();
            // 自身のマーカーの高さを変更する(レーダーマーカーの下になってしまうため）
            marker.transform.position = new Vector3(nowpos.x,pcpos.y + 20 ,nowpos.z);
        }      

	}

    // 描画
    public void OnGUI()
    {
        // 属性「自機」以外は描画しない
        var target = m_Player.GetComponentInChildren<CharacterControl_Base>();
        if (target.m_isPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
        {
            return;
        }
              
        Vector3 scale = new Vector3(Screen.width / MadokaDefine.SCREENWIDTH, Screen.height/ MadokaDefine.SCREENHIGET, 1);
        // GUIが解像度に合うように変換行列を設定(先にTRSの第1引数を0で埋めていくとリサイズにも対応可能ということらしい）
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
        Vector3 position = new Vector3(896.0f, 0.0f);
        GUI.BeginGroup(new Rect(position.x, position.y, 128.0f, 128.0f));
        // 背景描画
        GUI.DrawTexture(new Rect(0, 0, 128.0f, 128.0f), m_radarBase);
        
        
        GUI.EndGroup();
    }

    void LateUpdate()
    {
        // PCを取得
        var target = m_Player.GetComponentInChildren<CharacterControl_Base>();
        // プレイヤーキャラの位置を追跡する
        Vector3 pcposition = m_Player.transform.position;

        // マーカーの方向を取得
        var pcmarker = m_Player.GetComponentInChildren<Marker>();
        // メインカメラの方向を取得
        var pcmaincamera = m_Player.GetComponentInChildren<Player_Camera_Controller>();
        // メインカメラの向きを取得
        Vector3 maincamerarot = pcmaincamera.transform.rotation.eulerAngles;

        // BlackBoradと本体の相対位置を常に一定にする
        // BlackBoradの絶対角度を常に一定にする
        // plate（マーカーの乗っているオブジェクト）と本体の相対位置を常に一定にする
        // plateの絶対角度を常に一定にする

        // BlackBoradと本体の相対位置を常に一定にする(常に本体の下にする）
        var BB = m_Player.GetComponentInChildren<BlackBoard>();
        BB.transform.position = new Vector3(pcposition.x, pcposition.y + m_blackBoardPos_OR.y,pcposition.z);
        // BlackBoradの絶対角度をカメラに合わせる
        BB.transform.rotation = Quaternion.Euler(m_blackBoardRot_OR.x, m_blackBoardRot_OR.y + maincamerarot.y, m_blackBoardRot_OR.z);
       

        // 属性「自機」以外は描画しない       
        if (target.m_isPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
        {
            return;
        }

        // plate（マーカーの乗っているオブジェクト）と本体の相対位置を常に一定にする(敵や僚機のときはUpdateでやっているので二重操作を防ぐ）
        var Pl = m_Player.GetComponentInChildren<Marker>();
        Pl.transform.position = new Vector3(pcposition.x, pcposition.y + m_platePos_OR.y, pcposition.z);

        //Pl.transform.rotation = Quaternion.Euler();

        // ただし、カメラの高度はキャラやマーカーよりも高くなくてはならないので、そこは補正
        this.transform.position = new Vector3(pcposition.x, pcposition.y + 300.0f, pcposition.z);

        // 自機の場合、メインカメラの方向に自機にくっついているマーカーの向きを合わせる           
        // メインカメラの向き（+180?）をマーカーの向きとする
        pcmarker.transform.rotation = Quaternion.Euler(0, maincamerarot.y + 180, 0);

        // ここから自機の角度分レーダーカメラの角度を合わせる(マーカーは上で補正してるので勝手に付いてくる）
        // レーダーカメラの角度
        Vector3 radarrot = m_PerspectiveCamera.transform.rotation.eulerAngles;

        // レーダーカメラと本体との相対位置も合わせる(Yだけはm_position_ORにあわせる）
        //m_PerspectiveCamera.transform.localPosition = m_position_OR;
        // 絶対位置を合わせる
        m_PerspectiveCamera.transform.position = (pcposition + m_position_OR);
        

        // レーダーカメラと本体との相対角度を合わせる（Y除く）
        radarrot.x = m_rotate_OR.x;
        radarrot.z = m_rotate_OR.z;

        
        
        // 非ロック時
        if (!target.m_IsRockon)
        {
            m_PerspectiveCamera.transform.rotation = Quaternion.Euler(radarrot.x, m_rotate_OR.y + 180.0f, radarrot.z);
        }
        // ロック時
        else
        {
            m_PerspectiveCamera.transform.rotation = Quaternion.Euler(radarrot.x, m_rotate_OR.y+maincamerarot.y+180, radarrot.z);
        }
    }
}
