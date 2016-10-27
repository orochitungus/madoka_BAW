using UnityEngine;
using System.Collections;

// 画面右下の残弾数表示を司る(対象のキャラクターにくっつけること)
public class DrawBulletNum : MonoBehaviour 
{
    public Character_Spec.CHARACTER_NAME        m_Player;           // この残弾数表示の対象となるキャラクター
    public GameObject   m_PlayerObject;                             // 対象オブジェクト
    public Texture2D    m_WeaponGraphic;                            // 表示されるグラフィック
    public CharacterSkill.SkillType m_Weapontype;                   // どの武器を使用するか
    public bool         m_isUse = true;                             // 使用可能か否か（本体から取得）    
    public bool         m_ChargeExist;                              // チャージがあるか否か(インスペクタから取得)                
    public int          m_BulletNum;                                // 残弾数（本体から取得）
    public int          m_MaxBulletNum;                             // 最大残弾数（本体から取得）
    public int          m_SetPos  = 0;                              // 何段目に配置されるか(一応MAX4とする）   
    public int          m_nowmode = 0;                              // モードチェンジ持ちのキャラの場合、どちらか
    public GUISkin      m_guiskin;                                  // GUISKIN
    public Vector3 position;
    public Vector2 btype;
    public Texture2D    m_ChargeGraphic;                            // チャージショットのテクスチャ       
    public ChargeMode   m_chargemode;

    // チャージショットがある場合、射撃格闘どちらか
    public enum ChargeMode
    {
        SHOT,
        WRESTLE
    };

    public void Start()
    {
        
    }
	

    // 描画
    public void OnGUI()
    {
        var target = m_PlayerObject.GetComponentInChildren<CharacterControlBase>();
        // CPU制御の時は描かない
        if (target.IsPlayer != CharacterControlBase.CHARACTERCODE.PLAYER)
        {
            return;
        }
        // GUIスキンを設定
        if (m_guiskin)
        {
            GUI.skin = m_guiskin;
        }
        // レベル
        int character_level = target.Level;
        // その武装をcharacter_specから探す
        for (int i = 0; i < (int)Character_Spec.cs[(int)m_Player].Length; i++)
        {
            if (Character_Spec.cs[(int)m_Player][i].m_Skilltype == m_Weapontype)
            {
                // 使用可能なら描画・不可能ならここで強制抜け
                if (character_level >= Character_Spec.cs[(int)m_Player][i].m_LearningLevel)
                {
                    Vector3 scale = new Vector3(Screen.width / MadokaDefine.SCREENWIDTH, Screen.height / MadokaDefine.SCREENHIGET, 1);
                    // GUIが解像度に合うように変換行列を設定(先にTRSの第1引数を0で埋めていくとリサイズにも対応可能ということらしい）
                    GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);

                    Vector3 position = new Vector3(896.0f, 70.0f * m_SetPos + 200.0f, 0.0f);
                    GUI.BeginGroup(new Rect(position.x, position.y, 128.0f, 128.0f));
                    // チャージ量描画
                    // チャージショットが描画されるときのみ描画
                    if (m_ChargeExist)
                    {
                        // 最大値を取得
                        int max = target.ChargeMax;
                        // 現在のチャージ量を取得
                        int nowcharge = 0;
                        if (m_chargemode == ChargeMode.SHOT)
                        {
                            nowcharge = target.GetShotCharge();
                        }
                        else
                        {
                            nowcharge = target.GetWrestleCharge();
                        }
                        // ゲージサイズを取得
                        float gaugesize = 0;
                        gaugesize = 72.0f * nowcharge / max;
                        GUI.DrawTexture(new Rect(0.0f, 112.0f, gaugesize, 10.0f), m_ChargeGraphic);
                    }
                    // 背景描画
                    GUI.DrawTexture(new Rect(0.0f, 0.0f, 128.0f, 128.0f), m_WeaponGraphic);
                    // 弾数描画
                    // 使用可能状態にあり、かつ残弾数が0を超えているか？
                    // あるなら緑字で描画
                    if (target.WeaponUseAble[i] && target.BulletNum[i] > 0)
                    {
                        if (target.BulletNum[i] >= 100)
                        {
                            GUI.Label(new Rect(0, 80.0f, 1500.0f, 100.0f), target.BulletNum[i].ToString(), "Bullet_Use");
                        }
                        else if (target.BulletNum[i] >= 10)
                        {
                            GUI.Label(new Rect(20, 80.0f, 1500.0f, 100.0f), target.BulletNum[i].ToString(), "Bullet_Use");
                        }
                        else
                        {
                            GUI.Label(new Rect(30, 80.0f, 1500.0f, 100.0f), target.BulletNum[i].ToString(), "Bullet_Use");
                        }
                    }
                    // ないなら赤字で描画
                    else
                    {
                        if (target.BulletNum[i] >= 100)
                        {
                            GUI.Label(new Rect(0, 80.0f, 1500.0f, 100.0f), target.BulletNum[i].ToString(), "Bullet_NotUse");
                        }
                        else if (target.BulletNum[i] >= 10)
                        {
                            GUI.Label(new Rect(20, 80.0f, 1500.0f, 100.0f), target.BulletNum[i].ToString(), "Bullet_NotUse");
                        }
                        else
                        {
                            GUI.Label(new Rect(30, 80.0f, 1500.0f, 100.0f), target.BulletNum[i].ToString(), "Bullet_NotUse");
                        }
                    }

                   
                    

                    // 武装タイプ描画
                    btype = new Vector2(55.0f,100.0f);
                    switch (Character_Spec.cs[(int)m_Player][i].m_Skilltype)
                    {
                        case CharacterSkill.SkillType.SHOT:
                        case CharacterSkill.SkillType.SHOT_M2:
                            GUI.Label(new Rect(btype.x, btype.y, 1500.0f, 100.0f),"Shot", "Bullet_Type");
                            break;
                        case CharacterSkill.SkillType.SUB_SHOT:
                        case CharacterSkill.SkillType.SUB_SHOT_M2:
                            GUI.Label(new Rect(btype.x, btype.y, 1500.0f, 100.0f), "Sub Shot", "Bullet_Type");
                            break;
                        case CharacterSkill.SkillType.EX_SHOT:
                        case CharacterSkill.SkillType.EX_SHOT_M2:
                            GUI.Label(new Rect(btype.x, btype.y, 1500.0f, 100.0f), "Ex Shot", "Bullet_Type");
                            break;
                        case CharacterSkill.SkillType.WRESTLE_1:
                        case CharacterSkill.SkillType.WRESTLE_1_M2:
                            GUI.Label(new Rect(btype.x, btype.y, 1500.0f, 100.0f), "Wrestle", "Bullet_Type");
                            break;
                        case CharacterSkill.SkillType.EX_WRESTLE_1:
                        case CharacterSkill.SkillType.EX_WRESTLE_1_M2:
                            GUI.Label(new Rect(btype.x, btype.y, 1500.0f, 100.0f), "Ex Wrestle", "Bullet_Type");
                            break;
                        default:
                            break;
                    }
                    GUI.EndGroup();
                    return;
                }
                else
                {
                    return;
                }
            }
        }
    }
}
