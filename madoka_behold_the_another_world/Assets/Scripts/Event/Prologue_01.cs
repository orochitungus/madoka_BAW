using UnityEngine;
using System.Collections;

// イベントシーンはEventBaseを継承すること（固定ステートはEventBaseで制御）
public class Prologue_01 : EventBase
{
    // カメラモードのenumを用意(カメラパン、ほむQ、ほむらアップ、キュゥべえアップ、魔獣出現、ほむら弓構え)
    public enum m_modecode { INTRO, HOM_KYU, HOMURA, KYUBEY, MAJYU, HOMURA_WEAPON };
    public m_modecode m_animechanged;    // 現在のモード

    // カメラ１(開幕と同時にパン)
    // カメラ２(ほむらとQB)
    // カメラ３（ほむらアップ）
    // カメラ４（キュゥべえアップ）
    // カメラ５（魔獣出現）
    // カメラ６（ほむら弓構え）

    // 制御対象となるオブジェクト(フィールドに配置してあるオブジェクトを指定する)
    public GameObject Homura;   // ほむら
    public GameObject Kyubey;   // キュゥべえ

    // 魔獣
    public GameObject[] Majyu;

    // 魔獣の顔のパーティクル
    public GameObject[] Majyu_particle;

    // 魔獣出現時の専用カウンタ
    private float m_appearmajyu;

    // 魔獣出現時のフラグ
    // 未出現・出現中・出現完了
    private enum m_majyuflag { ERASE, APPEAR, IDLE };
    private m_majyuflag[] m_majyuappear = new m_majyuflag[5];

    // 魔獣の顔にかかるパーティクルの座標
    private Vector3[] m_majyu_effect = new Vector3[5];

    // 魔獣出現時のSE
    public AudioClip m_appearmajyu_se;
    // 弓抜刀時のSE
    public AudioClip m_drawn_bow;
    // パーティクルの角度
    private Quaternion particlerotate;

    // ほむらの弓のフック
    public GameObject Homura_Bow;
    // 弓の弦のフック
    public GameObject BowString;
    // 矢のフック
    public GameObject Homura_Arrow;

    // Use this for initialization
    void Awake()
    {
        // 魔獣のインスタンスを取得
        for (int i = 0; i < 5; i++)
        {
            m_majyuappear[i] = m_majyuflag.ERASE;
        }

        // パーティクル角度
        particlerotate = new Quaternion(0, 1.5f, 0, 0);

        // カメラワーク変更フラグを初期化
        m_animechanged = m_modecode.INTRO;
        // FPSを固定
        //Application.targetFrameRate = 60;
        // 一応カメラ2以降を無効化する
        for (int i = 1; i < m_camera.Length; i++)
        {
            this.m_camera[i].SetActive(false);
        }

        // パーティクルの座標を決定
        //par1
        m_majyu_effect[0] = new Vector3(945.1056f, 62.70607f, 676.0916f);
        //par2
        m_majyu_effect[1] = new Vector3(972.9453f, 64.13898f, 718.3557f);
        //par3
        m_majyu_effect[2] = new Vector3(1060.531f, 66.54268f, 718.3557f);
        //par4
        m_majyu_effect[3] = new Vector3(1029.509f, 64.36892f, 669.7842f);
        //par5
        m_majyu_effect[4] = new Vector3(998.0921f, 64.36892f, 687.845f);


        // 共通ステートの初期化を行う
        EventInitialize();
        // 魔獣出現時の専用カウンタを初期化する
        m_appearmajyu = 0;

       
    }

    // Update is called once per frame
    void Update()
    {
        switch (xstory)
        {
            case 0:
                keybreak = true;
                if (m_Accumulated_time > 7.5f && m_animechanged == m_modecode.INTRO)
                {
                    // カメラ１のアニメを止める
                    this.m_camera[0].SetActive(false);
                    // カメラ２を実行する
                    this.m_camera[1].SetActive(true);

                    // カメラワーク変更フラグを立てておく
                    m_animechanged = m_modecode.INTRO;
                    IncrementXstory();
                }
                break;
            case 1:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_NORMAL;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "・・・・・・";
                break;
            case 2:
                keybreak = true;
                // カメラ2のアニメを止める
                this.m_camera[1].SetActive(false);
                // カメラ4を実行する
                this.m_camera[3].SetActive(true);
                // キュゥべえにアニメーションを行わせる
                Kyubey.GetComponent<Animation>().Play("see_ru_copy");
                IncrementXstory();
                break;
            case 3:
                keybreak = false;
                m_facetype = CharacterFace.KYUBEY_NORMAL;
                m_drawname_jp = "キュゥべえ";
                m_drawname_en = "KYUBEY";
                m_serif[0] = "ついにこの星の人類の歴史も終焉を迎えるね。お疲れさまというべ";
                m_serif[1] = "きかい、暁美ほむら";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 4:
                keybreak = true;
                // カメラ4のアニメを止める
                this.m_camera[3].SetActive(false);
                // カメラ3を実行する
                this.m_camera[2].SetActive(true);
                Homura.GetComponent<Animation>().Play("homura_see_lu_copy");
                IncrementXstory();
                break;
            case 5:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_NORMAL;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "あなたこそご苦労なことね。ヒトと呼ばれたこの星の知的生命はも";
                m_serif[1] = "ういなくなる。あなたが集めるべき魔獣のエネルギーもグリーフシ";
                m_serif[2] = "ードも、もう手に入れることはできないわ。だのに今更何の用だと";
                m_serif[3] = "いうの？";
                break;
            case 6:
                keybreak = true;
                this.m_camera[2].SetActive(false);
                this.m_camera[3].SetActive(true);
                Kyubey.GetComponent<Animation>().Play("see_ru_r_copy");
                IncrementXstory();
                break;
            case 7:
                keybreak = false;
                m_facetype = CharacterFace.KYUBEY_NORMAL;
                m_drawname_jp = "キュゥべえ";
                m_drawname_en = "KYUBEY";
                m_serif[0] = "深い意味はないよ。ただまあ、僕が今まで育ててきた種の終焉を見";
                m_serif[1] = "届けたいと思うのはおかしいかい？";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 8:
                keybreak = false;
                m_facetype = CharacterFace.KYUBEY_NORMAL;
                m_drawname_jp = "キュゥべえ";
                m_drawname_en = "KYUBEY";
                m_serif[0] = "僕たちインキュベーターは君たち人類がこの地上に発生してから、";
                m_serif[1] = "少女達を奇跡と引き替えに魔法少女とすることで、君たちの世界を";
                m_serif[2] = "発展させてきた。君たちは僕たちの与えた奇跡で繁栄の境地を築き";
                m_serif[3] = "上げ、そして滅んでしまった";
                break;
            case 9:
                keybreak = false;
                this.m_camera[3].SetActive(false);
                this.m_camera[1].SetActive(true);
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_HATE;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "ふん、人間に火を与えたプロメテウスを気取るつもりかしら．だっ";
                m_serif[1] = "たらおとなしく永遠に喰われ続けてほしいものだけど";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 10:
                keybreak = false;
                this.m_camera[1].SetActive(false);
                this.m_camera[3].SetActive(true);
                m_facetype = CharacterFace.KYUBEY_NORMAL;
                m_drawname_jp = "キュゥべえ";
                m_drawname_en = "KYUBEY";
                m_serif[0] = "そうだね、確かに僕は君たちに文明の火を与えてきた。だけど少し";
                m_serif[1] = "残念ではある。君たちが僕たちによって与えられた文明の火をつか";
                m_serif[2] = "って、何れ宇宙での『対話』の場所に参加する日を僕はそれなりに";
                m_serif[3] = "は楽しみにしていたんだよ？";
                break;
            case 11:
                keybreak = false;
                m_facetype = CharacterFace.KYUBEY_NORMAL;
                m_drawname_jp = "キュゥべえ";
                m_drawname_en = "KYUBEY";
                m_serif[0] = "手塩にかけて育てた子供の死を悼む親の気持ち、というのかもしれ";
                m_serif[1] = "ないね";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 12:
                keybreak = false;
                this.m_camera[3].SetActive(false);
                this.m_camera[2].SetActive(true);
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_NORMAL;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "戯言ね。あなただって見方を変えれば、誤った薬を処方して患者を";
                m_serif[1] = "死なせた医者のようなものよ";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 13:
                keybreak = false;
                m_facetype = CharacterFace.KYUBEY_NORMAL;
                this.m_camera[2].SetActive(false);
                this.m_camera[3].SetActive(true);
                m_drawname_jp = "キュゥべえ";
                m_drawname_en = "KYUBEY";
                m_serif[0] = "そうかもしれない。だがいまさらそれを論じても無駄だろう。それ";
                m_serif[1] = "に君だっておかしいんじゃないかい？君は人類がいるこの世界を守";
                m_serif[2] = "るために戦ってきたんだよね？守るべきものがなくなった君に、今";
                m_serif[3] = "更戦う理由があるのかい？";
                break;
            case 14:
                keybreak = true;
                this.m_camera[3].SetActive(false);
                this.m_camera[2].SetActive(true);
                Homura.GetComponent<Animation>().Play("homura_see_lu_back_copy");
                IncrementXstory();
                break;
            case 15:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_NORMAL;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "そうね・・・確かに私は無意味なことをしているように見えるわね";
                m_serif[1] = "これはもう意地よ．まどかに会うときに、最後まで戦ったと胸を張";
                m_serif[2] = "れるように";
                m_serif[3] = "";
                break;
            case 16:
                keybreak = false;
                this.m_camera[2].SetActive(false);
                this.m_camera[1].SetActive(true);
                m_facetype = CharacterFace.KYUBEY_NORMAL;
                m_drawname_jp = "キュゥべえ";
                m_drawname_en = "KYUBEY";
                m_serif[0] = "君の言う魔法少女の神様か．魔法少女たちの間では円環の理と呼ば";
                m_serif[1] = "れていたね";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 17:    // 魔獣たちが連続でテレポートして出現する(時間を全初期化)
                keybreak = true;
                m_appearmajyu = 0;
                // カメラを5に切り替える
                this.m_camera[1].SetActive(false);
                this.m_camera[4].SetActive(true);
                // セリフとキャラ名をすべて消す
                FullClear();
                IncrementXstory();
                break;
            case 18:    // 0.5秒ごとに新しい魔獣を出現させ、次のcaseへ移る
                keybreak = true;
                // 最初
                if (m_majyuappear[0] == m_majyuflag.ERASE && m_appearmajyu > 60)
                {
                    // 魔獣出現時のアニメーションを再生
                    Majyu[0].GetComponent<Animation>().Play("majyu_appear");
                    // 魔獣出現時のSEを再生
                    AudioSource.PlayClipAtPoint(m_appearmajyu_se, transform.position);
                    // 出現フラグを折っておく
                    m_majyuappear[0] = m_majyuflag.APPEAR;
                }
                // それ以外
                for (int i = 0; i < 4; i++)
                {
                    Appear_Majyu_Control(i, 5);
                }
                // 最後
                if (m_majyuappear[4] == m_majyuflag.APPEAR && m_appearmajyu > 150 + 90 * 4)
                {
                    // 魔獣出現時のアニメーションを再生
                    Majyu[4].GetComponent<Animation>().Play("idle_majyu");
                    // パーティクルを召喚
                    Majyu_particle[4].transform.position = m_majyu_effect[4];
                    Majyu_particle[4].transform.rotation = particlerotate;
                    // 出現フラグを折っておく
                    m_majyuappear[4] = m_majyuflag.IDLE;
                    // 何か回りきらないので一応固定
                    Majyu[4].transform.rotation = new Quaternion(0, 180, 0, 0);
                    IncrementXstory();
                }
                m_appearmajyu++;
                break;
            case 19:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_HATE;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "お喋りは終わりのようね";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 20:
                keybreak = true;
                // カメラを6に切り替える
                this.m_camera[4].SetActive(false);
                this.m_camera[5].SetActive(true);
                // ほむらの左手に弓を持たせる
                // 他のスクリプトを呼び出す（この場合はHomura_Bowに設定されているHockControl)                
                var C_Weapon = Homura_Bow.GetComponent<HockControl>();
                C_Weapon.EquipWeapon();
                // 抜刀時のSEを鳴らす
                AudioSource.PlayClipAtPoint(m_drawn_bow, transform.position);
                // ほむらに抜刀ポーズをとらせる
                Homura.GetComponent<Animation>().Play("reload_arrow_homura_ribon_event2_copy");
                // 表情を変える
                var Face = Homura.GetComponentInChildren<FaceSelecter>(); // 対象オブジェクトに取り付けたスクリプト名を指定する              
                Face.ChangeFaceTexture("homura_seriousface");
                IncrementXstory();
                break;
            case 21:
                keybreak = false;
                m_facetype = CharacterFace.KYUBEY_NORMAL;
                m_drawname_jp = "キュゥべえ";
                m_drawname_en = "KYUBEY";
                m_serif[0] = "残念だよ。遺言は可能な限り聞こうと思ったんだけどね";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 22:
                keybreak = true;
                // ほむらに弓を引くポーズをとらせる
                Homura.GetComponent<Animation>().Play("set_bow_homura_ribon_event2_copy");
                // 弓の弦を出す
                var C_Bow_String = BowString.GetComponent<HockControl>();
                C_Bow_String.EquipWeapon();     // この状態で元のフックはMissingになる（実質解放されたと同じで、アクセスしようとするとヌルポインタバグになる）
                // 弦を引いた状態にする
                // HockControlにアニメ再生を着けて表情変化と同じ要領でそのスクリプト内の関数を呼ぶ
                //var C_Bow_String_True = C_Bow_String.GetComponent<AssetAnimation_Done>();   // 切り替えた新しいオブジェクトもヌルになる
                // 親元のHomuraから拾ってみると・・・？
                var C_Bow_String_True2 = Homura.GetComponentInChildren<AssetAnimation_Done>();  // こっちはいける。ルートオブジェクトからGetComponentInChildrenで対象のクラスを呼べばヌルにならん
                C_Bow_String_True2.GetComponent<Animation>().Play("homura_bow_string_set");      // 再生
                // 矢を出す
                var C_Arrow = Homura_Arrow.GetComponent<HockControl>();
                C_Arrow.EquipWeapon();
                // SEを鳴らす
                AudioSource.PlayClipAtPoint(m_appearmajyu_se, transform.position);
                IncrementXstory();
                break;
            case 23:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_DETERMINATION;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "余計なお世話よ。消えなさい藪医者";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 24:
                keybreak = false;
                m_facetype = CharacterFace.KYUBEY_NORMAL;
                m_drawname_jp = "キュゥべえ";
                m_drawname_en = "KYUBEY";
                m_serif[0] = "やれやれ、ではさよならだ最後の魔法少女・・・そして『対話』の";
                m_serif[1] = "場所に来ることができなかった最後の地球人";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
            case 25:
                keybreak = true;
                // カメラを5に切り替える
                this.m_camera[1].SetActive(false);
                this.m_camera[4].SetActive(true);
                // セリフとキャラ名をすべて消す
                FullClear();
                IncrementXstory();
                break;
            case 26:
                keybreak = false;
                m_facetype = CharacterFace.HOMURA_RIBON_MAGICA_DETERMINATION;
                m_drawname_jp = "暁美ほむら";
                m_drawname_en = "AKEMI HOMURA";
                m_serif[0] = "ふん・・・さて、行くわよ！";
                m_serif[1] = "";
                m_serif[2] = "";
                m_serif[3] = "";
                break;
			case 27:
				keybreak = true;
				FullClear();
				StartCoroutine(ExFadeout(1.0f));
				break;
			case 28:
				keybreak = true;
				foreach(GameObject c in m_camera)
				{
					c.SetActive(false);
				}
				xstory++;
				break;
            default:
                // 必要ステートを初期化
                savingparameter.savingparameter_Init();
                savingparameter.beforeField = 0;
				FadeManager.Instance.LoadLevel("Broken_Mitakihara", 1.0f);
				break;
        }
        EventUpdate();
    }

    // 魔獣出現時の処理
    // 第1引数：魔獣のインデックス
    // 第2引数：最大値
    // 出力   ：全部出し切ったか否か
    private void Appear_Majyu_Control(int i, int max)
    {
        if (m_majyuappear[i] == m_majyuflag.APPEAR && m_appearmajyu > 150 + 90 * i)
        {
            // 魔獣出現時のアニメーションを再生
            Majyu[i].GetComponent<Animation>().Play("idle_majyu");
            // パーティクルを召喚
            Majyu_particle[i].transform.position = m_majyu_effect[i];
            Majyu_particle[i].transform.rotation = particlerotate;
            // 出現フラグを折っておく
            m_majyuappear[i] = m_majyuflag.IDLE;
            if (i < max - 1)
            {
                // 次の魔獣を表示
                Majyu[i + 1].GetComponent<Animation>().Play("majyu_appear");
                // 魔獣出現時のSEを再生
                AudioSource.PlayClipAtPoint(m_appearmajyu_se, transform.position);
                // 出現フラグを折っておく
                m_majyuappear[i + 1] = m_majyuflag.APPEAR;
            }
        }
    }
}
