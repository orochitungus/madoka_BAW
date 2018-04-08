#define DEBUGMODE           // このシーン単独でビルドするときはgamsettingのインスタンスを作る必要があるのでフラグとしておく。シーン間移動を実装した場合、コメントアウトする

using UnityEngine;
using System.Collections;



public class PrologueController : MonoBehaviour 
{
//    // モードのenumを用意(カメラパン、ほむQ、ほむらアップ、キュゥべえアップ、魔獣出現、ほむら弓構え)
//    public enum m_modecode { INTRO, TITLE, HOMURA, KYUBEY, MAJYU, HOMURA_WEAPON };
//    public m_modecode m_animechanged;    // 現在のモード

//    // カメラ１(開幕と同時に降りてくる)
//    public GameObject Camera1;
//    // カメラ２(タイトル画面が描画された時、くるくるとタワーの周囲を回り全員を映す)
//    public GameObject Camera2;

//    public float m_Accumulated_time; // 累積時間

//    // 1F前の時間
//    public float m_oldtime;

//    // 現在の時間
//    public float m_nowtime;

//    // タイトルロゴを入れるので、GUISkinを有効にする
//    public GUISkin m_guiskin;

//    // 描画対象
//    // 名前背景
//    public Texture2D m_Name_JP;
//    public Texture2D m_Name_EN;
//    // セリフ背景
//    public Texture2D m_Serifback;
//    // 名前
//    public string m_drawname_jp;
//    public string m_drawname_en;
//    // セリフ
//    public string[] m_serif = new string[4];

//    // gamesetting
//#if DEBUGMODE
//    public static gamesetting gs;
//    public static savingparameter sp;
//#endif

//    //縦方向の解像度
//    public float nativeVerticalResolution_h;
//    //横方向の解像度
//    public float nativeVerticalResolution_w;

//    // タイトルロゴの配置位置
//    private Vector2 m_serifbackpos;


//    // キーの入力の是非を問う変数(trueなら入力不可）
//    protected bool keybreak;


//    // Use this for initialization
//    void Start()
//    {
//        // カメラ関連のアニメーションファイルは１個しか登録できないらしい
//        // そのためここで呼び制御する       
//        // カメラワーク変更フラグを初期化
//        m_animechanged = m_modecode.INTRO;
//        // FPSを固定
//        Application.targetFrameRate = 60;
//        // 一応カメラ2を無効化する
//        Camera2.SetActiveRecursively(false);
//        // 累積時間をリセット
//        m_Accumulated_time = 0;

//        // 過去時間をリセット
//        m_oldtime = 0;
//        // 現在時間を取得(Time.timeは秒で取得する）
//        m_nowtime = 0;

//#if DEBUGMODE
//        gs = new gamesetting();
//        gs.setupgamesetting();
//        sp = new savingparameter();
//#endif

//        // 画面解像度を取得する(固定すること.機体の設定によって異なるため、使用機体に対してどれだけずれているかで判定を行う)
//        nativeVerticalResolution_h = gs.height; //576; // Screen.currentResolution.height;
//        nativeVerticalResolution_w = gs.width;  // Screen.currentResolution.width;

//        // 画面サイズは解像度の何倍？
//        float density_w = Screen.width / nativeVerticalResolution_w;

//        // 配置座標を決める(X方向左右に均等配置,Y方向はとりあえず0)
//        float imagepos_x = m_Serifback.width;
//        float screensize_X = Screen.width;
//        // （スクリーンサイズ-画像サイズ）/2*拡大率で決定とはできない（右にずれる）
//        m_serifbackpos.x = gs.serifboard_pos.x;
//        m_serifbackpos.y = gs.serifboard_pos.y;

//        // セリフとキャラ名を初期化
//        m_drawname_en = m_drawname_jp = "";
//        for (int i = 0; i < 4; i++)
//        {
//            m_serif[i] = "";
//        }

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // 基本Kanonの時と同様に（このクラスを継承させてUPDATEのみ書く）
//        switch (sp.xstory)
//        {
//            case 0:
//                break;
//            default:
//                break;
//        }
//        if (m_Accumulated_time > 5.0f && m_animechanged == m_modecode.INTRO)
//        {
//            // カメラ１のアニメを止める
//            Camera1.SetActiveRecursively(false);
//            // カメラ２を実行する
//            Camera2.SetActiveRecursively(true);

//            // カメラワーク変更フラグを立てておく
//            m_animechanged = m_modecode.TITLE;
//        }

//        // キー入力を取得する
//        InputCheck();

//        // 現在時間を取得（初回除く）
//        m_nowtime = Time.time;
//        // 現在時間と過去時間の差を求め
//        // 累積時間をインクリメント
//        m_Accumulated_time = m_Accumulated_time + (m_nowtime - m_oldtime);
//        // 現在時間を過去時間に
//        m_oldtime = m_nowtime;
//    }

//    // 描画関連
//    void OnGUI()
//    {
//        // GUIスキンを設定
//        if (m_guiskin)
//        {
//            GUI.skin = m_guiskin;
//        }
//        else
//        {
//            Debug.Log("No GUI skin has been set!");
//        }
//        //GUIが解像度に合うように変換行列を設定
//        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / nativeVerticalResolution_w, Screen.height / nativeVerticalResolution_h, 1));


//        // セリフがあれば描画
//        if (checkserifexist())
//        {
//            DrawImage(m_serifbackpos, m_Serifback);
//            // セリフ1行目
//            for (int i = 0; i < 4; i++)
//            {
//                DrawText(gs.serifpos[i], m_serif[i], "serif");
//            }
//        }
//        // 日本語名があれば描画
//        if (m_drawname_jp != "")
//        {
//            DrawImage(gs.characterboard_pos_jp, m_Name_JP);
//            // 名前日本語
//            DrawText(gs.characternamePos_jp, m_drawname_jp, "serif");
//        }
//        // 英語名があれば描画
//        if (m_drawname_en != "")
//        {
//            DrawImage(gs.characterboard_pos_en, m_Name_EN);
//            // 名前英語
//            DrawText(gs.characternamePos_en, m_drawname_en, "charaname_en");
//        }        

//    }

//    // 描画の関数
//    // 第1引数：配置位置
//    // 第2引数：描画対象
//    void DrawImage(Vector2 pos, Texture2D image)
//    {
//        GUIStyle backgroundStyle = new GUIStyle();
//        backgroundStyle.normal.background = image;
//        GUI.Label(new Rect(pos.x, pos.y, image.width, image.height), image);
//        // スタイルを設定すれば配置位置の調整及びリサイズ可能
//        // Rectの第3引数が幅、第4引数が高さ
//        //GUI.Label(new Rect(pos.x, pos.y, image.width, 256), "", backgroundStyle);
//    }

//    // タイトル文字列
//    // 第1引数：配置位置
//    // 第2引数：描画する文字列 
//    // 第3引数：表示するフォントの種類（セットしているGUISkinのCustomStyles/Nameの名前)
//    void DrawText(Vector2 pos, string text,string style)
//    {
//        float scaledResolutionWidth = nativeVerticalResolution_w / Screen.height * Screen.width;
//        // 配置位置とスタイルを有効化する。スタイルはGUISkinのStyleの名前
//        GUI.Label(new Rect(pos.x, pos.y, 1500, 100), text, style);
//    }

//    // キー制御の関数
//    void InputCheck()
//    {
//        // Enterか接続されているコントローラーのボタン0で入力確定
//        // ここでの引数の値はInputManagerで設定した名前を直で取れる
//        if (Input.GetButtonDown("Enter"))
//        {
//            Debug.Log("test");
//        }
//        // バックログとか入れるならここで
//    }

//    // 現在セリフが出ているかを問う
//    // trueなら出ている
//    bool checkserifexist()
//    {
//        for (int i = 0; i < 4; i++)
//        {
//            if (m_serif[i] != "")
//            {
//                return true;
//            }
//        }
//        return false;
//    }






}
