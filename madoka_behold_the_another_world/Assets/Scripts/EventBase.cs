#define DEBUGMODE           // このシーン単独でビルドするときはgamsettingのインスタンスを作る必要があるのでフラグとしておく。シーン間移動を実装した場合、コメントアウトする

using UnityEngine;
using System.Collections;

public class EventBase : MonoBehaviour 
{
    // gamesetting
#if DEBUGMODE
    public static gamesetting gs;
#endif

    public float m_Accumulated_time; // 累積時間

    // 1F前の時間
    protected float m_oldtime;

    // 現在の時間
    protected float m_nowtime;

    // タイトルロゴを入れるので、GUISkinを有効にする
    public GUISkin m_guiskin;

    // 描画対象
    // 名前背景
    public Texture2D m_Name_JP;
    public Texture2D m_Name_EN;
    // セリフ背景
    public Texture2D m_Serifback;
    // 名前
    public string m_drawname_jp;
    public string m_drawname_en;
    // セリフ
    public string[] m_serif;

    // キャラクター顔
    public Texture2D m_characterfece;


    // カメラ関係は場面ごとに使用数が異なるので、ここでは定義しない
    public GameObject[] m_camera;

    //縦方向の解像度
    public float nativeVerticalResolution_h;
    //横方向の解像度
    public float nativeVerticalResolution_w;

    // タイトルロゴの配置位置
    private Vector2 m_serifbackpos;
    // 文字列の配置位置
    private Vector2 m_serifpos;

    // 表示する顔の番号
    protected CharacterFace m_facetype;
    protected CharacterFace m_facetype_old;

    // キーの入力の是非を問う変数(trueなら入力不可）
    protected bool keybreak;
    // シナリオの進行度
    protected int xstory;

    // 出現している文字の数
    protected int m_word_len = 0;

    // 前の文字が出現してからの累積時間
    protected int m_wordappeartime = 0;

    // 次の文字が出現するまでの時間
    protected int m_nextwordappeartime = 5;

    // キャラクターの表情
    public enum CharacterFace 
    {
        // なし
        NONE                            = 0,
        // まどか制服（100番台）
        MADOKA_NORMAL                   = 100,
        MADOKA_SMILE                    = 101,
        MADOKA_SADNESS                  = 102,
        MADOKA_HATE                     = 103,
        MADOKA_SURPRISE                 = 104,
        MADOKA_BITTER_SMILE             = 105,
        MADOKA_DETERMINATION            = 106,
        MADOKA_PAIN                     = 107,
        // まどか魔法少女（150番台）
        MADOKA_MAGICA_NORMAL            = 150,
        MADOKA_MAGICA_SMILE             = 151,
        MADOKA_MAGICA_SADNESS           = 152,
        MADOKA_MAGICA_HATE              = 153,
        MADOKA_MAGICA_SURPRISE          = 154,
        MADOKA_MAGICA_BITTER_SMILE      = 155,
        MADOKA_MAGICA_DETERMINATION     = 156,
        MADOKA_MAGICA_PAIN              = 157,
        // さやか制服（200番台）
        SAYAKA_NORMAL                   = 200,
        SAYAKA_SMILE                    = 201,
        SAYAKA_SADNESS                  = 202,
        SAYAKA_HATE                     = 203,
        SAYAKA_SURPRISE                 = 204,
        SAYAKA_PAIN                     = 207,
        SAYAKA_SHY                      = 208,
        SAYAKA_DESPAIR                  = 209,
        // さやか魔法少女（250番台）
        SAYAKA_MAGICA_NORMAL            = 250,
        SAYAKA_MAGICA_SMILE             = 251,
        SAYAKA_MAGICA_SADNESS           = 252,
        SAYAKA_MAGICA_HATE              = 253,
        SAYAKA_MAGICA_SURPRISE          = 254,
        SAYAKA_MAGICA_PAIN              = 257,
        SAYAKA_MAGICA_SHY               = 258,
        SAYAKA_MAGICA_DESPAIR           = 259,
        // マミ制服(300番台)

        // マミ魔法少女（350番台）

        // ほむら制服（400番台）
        HOMURA_NORMAL                   = 400,
        HOMURA_SMILE                    = 401,
        HOMURA_SADNESS                  = 402,
        HOMURA_HATE                     = 403,
        HOMURA_SURPRISE                 = 404,
        HOMURA_BITTER_SMILE             = 405,
        HOMURA_DETERMINATION            = 406,
        HOMURA_PAIN                     = 407,
        // ほむらリボン制服（420番台）
        HOMURA_RIBON_NORMAL             = 420,
        HOMURA_RIBON_SMILE              = 421,
        HOMURA_RIBON_SADNESS            = 422,
        HOMURA_RIBON_HATE               = 423,
        HOMURA_RIBON_SURPRISE           = 424,
        HOMURA_RIBON_BITTER_SMILE       = 425,
        HOMURA_RIBON_DETERMINATION      = 426,
        HOMURA_RIBON_PAIN               = 427,
        // ほむら魔法少女（450番台）
        HOMURA_MAGICA_NORMAL            = 450,
        HOMURA_MAGICA_SMILE             = 451,
        HOMURA_MAGICA_SADNESS           = 452,
        HOMURA_MAGICA_HATE              = 453,
        HOMURA_MAGICA_SURPRISE          = 454,
        HOMURA_MAGICA_DETERMINATION     = 456,
        HOMURA_MAGICA_PAIN              = 457,
        // ほむらリボン魔法少女（480番台）
        HOMURA_RIBON_MAGICA_NORMAL      = 480,
        HOMURA_RIBON_MAGICA_SMILE       = 481,
        HOMURA_RIBON_MAGICA_SADNESS     = 482,
        HOMURA_RIBON_MAGICA_HATE        = 483,
        HOMURA_RIBON_MAGICA_SURPRISE    = 484,
        HOMURA_RIBON_MAGICA_DETERMINATION=486,
        HOMURA_RIBON_MAGICA_PAIN        = 487,
        // 杏子

        // ゆま

        // 織莉子

        // キリカ

        // スコノシュート魔法少女（900番台）
        SCONOSCIUTO_NORMAL              = 900,
        SCONOSCIUTO_PAIN                = 907,
        // スコノシュート白衣（950番台）
        SCONOSCIUTO_WRITE_NORMAL        = 950,
        SCONOSCIUTO_WRITE_PAIN          = 957,
        // アルティメットまどか（1000番台）
        ULTIMATE_MADOKA_NORMAL          = 1000,
        ULTIMATE_MADOKA_SMILE           = 1001,
        ULTIMATE_MADOKA_SADNESS         = 1002,
        ULTIMATE_MADOKA_HATE            = 1003,
        ULTIMATE_MADOKA_SURPRISE        = 1004,
        ULTIMATE_MADOKA_BITTER_SMILE    = 1005,
        ULTIMATE_MADOKA_DETERMINATION   = 1006,
        ULTIMATE_MADOKA_PAIN            = 1007,
        // さやかゴッドシブ

        // 恭介制服（1200番台）
        KYOSUKE_NORMAL                  = 1200,
        KYOSUKE_SMILE                   = 1201,
        KYOSUKE_SADNESS                 = 1202,
        KYOSUKE_HATE                    = 1203,
        KYOSUKE_WAILING                 = 1211,
        // 恭介病院服（1250番台）
        KYOSUKE_HOSPITAL_NORMAL         = 1250,
        KYOSUKE_HOSPITAL_SMILE          = 1251,
        KYOSUKE_HOSPITAL_SADNESS        = 1252,        
        KYOSUKE_HOSPITAL_HATE           = 1253,
        KYOSUKE_HOSPITAL_WAILING        = 1261,
        // 仁美

        // 詢子
        
        // 知久

        // タツヤ

        // 和子

        // 中沢

        // ミッチェル

        // キュゥべえ（2000番台）
        KYUBEY_NORMAL                   = 2000,
        KYUBEY_DARK                     = 2010
    };

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // startとUpdateは継承先に持たせる。共通ステートの初期化はここで行う
    protected void EventInitialize()
    {
        // FPSを固定
        //Application.targetFrameRate = 60;
        // 累積時間をリセット
        m_Accumulated_time = 0;

        // 過去時間をリセット
        m_oldtime = 0;
        // 現在時間を取得(Time.timeは秒で取得する）
        m_nowtime = 0;
        // 進行度をリセット
        xstory = 0;
        // 共通ステート（セリフの配置位置など）を初期化
#if DEBUGMODE
        gs = new gamesetting();
        gs.setupgamesetting();
       
#endif
        
        m_serif = new string[gs.selif_lines];
        // 全セリフをクリア
        for (int i = 0; i < gs.selif_lines; i++)
        {
            m_serif[i] = "";            
        }
        m_wordappeartime = 0;
        m_word_len = 0;
    }

    // 共通ステートの実行
    protected void EventUpdate()
    {
        // 現在時間を取得（初回除く）
        //m_nowtime = Time.time;
        // 現在時間と過去時間の差を求め
        // 累積時間をインクリメント
        //m_Accumulated_time = m_Accumulated_time + (m_nowtime - m_oldtime);
        m_Accumulated_time += Time.deltaTime;
        // 現在時間を過去時間に
        //m_oldtime = m_nowtime;

        // 文字出現時間をインクリメント
        m_wordappeartime++;


        // キー制御を行う
        KeyControl_Event();
    }

    // キー制御
    public void KeyControl_Event()
    {
        if (!keybreak)
        {
            if(Input.GetButtonDown("Enter"))
            {                
                // 最終的な文字の数
                int finalwords = 0;
                for (int i = 0; i < m_serif.Length; i++)
                {
                    finalwords += m_serif[i].Length;
                }
                // セリフが出きっていない場合は全部強制出現
                if (m_word_len < finalwords)
                {
                    m_wordappeartime = m_nextwordappeartime * 1000;
                }
                // そうでなければ次へ
                else
                {
                    m_wordappeartime = 0;   // 台詞の出現時間をリセット
                    m_word_len = 0;
                    m_facetype_old = m_facetype;    // xstory1つまえの顔
                    m_facetype = CharacterFace.NONE;
                    xstory++;
                }
            }
        }
    }

    // 描画関連
    void OnGUI()
    {
        // GUIスキンを設定
        if (m_guiskin)
        {
            GUI.skin = m_guiskin;
        }
        else
        {
            Debug.Log("No GUI skin has been set!");
        }
        //GUIが解像度に合うように変換行列を設定
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / nativeVerticalResolution_w, Screen.height / nativeVerticalResolution_h, 1));


        // セリフがあれば描画
        if (checkserifexist())
        {
            DrawCharacterFace(m_facetype);
            DrawImage(gs.serifboard_pos, m_Serifback);
            
            // 何文字目まで出すか判定
            m_word_len = m_wordappeartime / m_nextwordappeartime;
            // セリフ1行目
            if (m_serif[0] != "")
            {
                // word_lenまで文字列を出力する
                // 但し最大値を超えたらそのまま
                if (m_word_len < m_serif[0].Length)
                {
                    DrawText(gs.serifpos[0], m_serif[0].Substring(0, m_word_len), "serif");
                }
                else
                {
                    DrawText(gs.serifpos[0], m_serif[0], "serif");
                }
                
            }
            // 台詞2行目以降
            for (int i = 1; i < m_serif.Length; i++)
            {
                if (m_serif[i] != "")
                {
                    // 前の行が終わっている
                    // 前の行までの長さ
                    int checklen = 0;
                    for (int j = 0; j < i; j++)
                    {
                        checklen += m_serif[j].Length;
                    }

                    if (m_word_len > checklen)
                    {
                        // word_lenまで文字列を出力する
                        // 但し最大値を超えたらそのまま
                        if (m_word_len - checklen < m_serif[i].Length)
                        {
                            DrawText(gs.serifpos[i], m_serif[i].Substring(0, m_word_len - checklen), "serif");
                        }
                        else
                        {
                            DrawText(gs.serifpos[i], m_serif[i], "serif");
                        }
                    }
                }
            }
        }
        // 日本語名があれば描画
        if (m_drawname_jp != "")
        {
            DrawImage(gs.characterboard_pos_jp, m_Name_JP);
            // 名前日本語
            DrawText(gs.characternamePos_jp, m_drawname_jp, "serif");
        }
        // 英語名があれば描画
        if (m_drawname_en != "")
        {
            DrawImage(gs.characterboard_pos_en, m_Name_EN);
            // 名前英語
            DrawText(gs.characternamePos_en, m_drawname_en, "charaname_en");
        }

    }
    // 描画の関数
    // 第1引数：配置位置
    // 第2引数：描画対象
    void DrawImage(Vector2 pos, Texture2D image)
    {
        GUIStyle backgroundStyle = new GUIStyle();
        backgroundStyle.normal.background = image;
        GUI.Label(new Rect(pos.x, pos.y, image.width, image.height), image);
        // スタイルを設定すれば配置位置の調整及びリサイズ可能
        // Rectの第3引数が幅、第4引数が高さ
        //GUI.Label(new Rect(pos.x, pos.y, image.width, 256), "", backgroundStyle);
    }

    // タイトル文字列
    // 第1引数：配置位置
    // 第2引数：描画する文字列 
    // 第3引数：表示するフォントの種類（セットしているGUISkinのCustomStyles/Nameの名前)
    void DrawText(Vector2 pos, string text, string style)
    {
        // 配置位置とスタイルを有効化する。スタイルはGUISkinのStyleの名前
        GUI.Label(new Rect(pos.x, pos.y, 1500, 100), text, style);
    }
    // キー制御の関数
    void InputCheck()
    {
        // Enterか接続されているコントローラーのボタン0で入力確定
        // ここでの引数の値はInputManagerで設定した名前を直で取れる
        if (Input.GetButtonDown("Enter"))
        {
            m_facetype_old = m_facetype;    // xstory1つまえの顔
            xstory++;
        }
        // バックログとか入れるならここで
    }

    // 現在セリフが出ているかを問う
    // trueなら出ている
    bool checkserifexist()
    {
        for (int i = 0; i < m_serif.Length; i++)
        {
            if (m_serif[i] != "")
            {
                return true;
            }
        }
        return false;
    }
    // 出ている台詞とキャラ名を全て消す
    protected void FullClear()
    {
        m_drawname_jp = "";
        m_drawname_en = "";
        for (int i = 0; i < m_serif.Length; i++)
        {
            m_serif[i] = "";
        }
    }
    // xstory加算
    protected void IncrementXstory()
    {
        m_word_len = 0;
        m_wordappeartime = 0;   // 台詞の出現時間をリセット
        m_facetype_old = m_facetype;    // xstory1つまえの顔
        xstory++;
    }

    // キャラのバストアップを動画。0番台が通常。50番台が変身後。
    // 例外1　ほむら：20番台がリボン、80番台がリボン魔法少女
    // 例外2　スコノ：0番台が白衣なし、50番台が白衣あり
    protected void DrawCharacterFace(CharacterFace characterface)
    {
        // NONEだったらなにもしない
        if (characterface == CharacterFace.NONE)
        {
            return;
        }
        // enumをファイル名へ変換
        int facecode = (int)characterface;
        string filename = facecode.ToString();
        // 顔の種類が変わっていたら書き換える（同じものを入れようとするとなぜか落ちる）
        if (m_facetype != m_facetype_old)
        {
            m_characterfece = (Texture2D)Instantiate(Resources.Load(filename));     
            m_facetype_old = m_facetype;
        }
        DrawImage(gs.characterfacePos, m_characterfece);
    }

    // 歩行時の経過時間
    private float m_walkTimer;

    // 歩行音再生
    // 第1引数：歩行音
    // 第2引数：再生間隔
    // 第3引数：発生場所
    protected void FootSE(AudioClip footSE,float walkTime,Vector3 position)
    {
        if(m_walkTimer > 0)
        {
            m_walkTimer -= Time.deltaTime;
        }
        if(m_walkTimer <= 0)
        {
            AudioSource.PlayClipAtPoint(footSE, position);
            m_walkTimer = walkTime;
        }        
    }

    // 一定時間ウェイトしてからxstoryを増やす(呼び出し方と後処理に注意）
    // 第1引数：待ち時間
    public IEnumerator TimeWait(float time)
    {
        yield return new WaitForSeconds(time);
        IncrementXstory();        
    }
}
