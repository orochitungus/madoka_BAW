using UnityEngine;
using System.Collections;

public partial class CharacterControl_Base : MonoBehaviour 
{
    // インターフェース(CPUの時は切っといてもOK)
    public GUISkin m_guiskin;

    // インターフェース描画の有無
    public bool m_DrawInterface;

    // 描画関連
    public void OnGUI()
    {
        // CPU制御か覚醒した瞬間の時は書かない（多分無駄メモリ）
        if (m_isPlayer != CHARACTERCODE.PLAYER || !m_DrawInterface)
        {
            return;
        }
        
        // GUIスキンを設定
        if (m_guiskin)
        {
            GUI.skin = m_guiskin;
        }
        else
        {
            //Debug.Log("No GUI skin has been set!");
        }
        //GUIが解像度に合うように変換行列を設定
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / MadokaDefine.SCREENWIDTH, Screen.height / MadokaDefine.SCREENHIGET, 1));

        GUI.Label(new Rect(530.0f, 512.0f, 1500.0f, 100.0f), "Boost", "Boost");
        // 覚醒ゲージ描画
        GUI.Label(new Rect(374.0f, 512.0f, 1500.0f, 100.0f), "Magic", "Arousal");
        // ブーストゲージ
        DrawBoostGauge(m_BoostTex, new Vector2(524.0f, 537.0f));
        // 覚醒ゲージ
        DrawArousalGauge(m_ArousalTex, new Vector2(500.0f, 537.0f));
        // 装備アイテム
        DrawEquipItem(m_ItemEquip, new Vector2(ITEMEQUIP_X, ITEMEQUIP_Y));

        // ゲームオーバー表示
        if (m_NowHitpoint < 1)
        {
            // Game
            if (m_gameoverstringposition_Game_X < 50.0f)
            {
                m_gameoverstringposition_Game_X += 50;
            }
            else
            {
                m_gameoverstringposition_Game_X = 50.0f;
            }
            DrawGameOver(m_Game, new Vector2(m_gameoverstringposition_Game_X, 150.0f));
            // Over
            if (m_gameoverstringposition_Over_X > 500.0f)
            {
                m_gameoverstringposition_Over_X -= 25;
            }
            else
            {
                m_gameoverstringposition_Over_X = 500.0f;
            }
            DrawGameOver(m_Over, new Vector2(m_gameoverstringposition_Over_X, 150.0f));
            // Its~
            if (m_gameoverstringposition_Game_X == 50.0f && m_gameoverstringposition_Over_X == 500.0f)
            {
                DrawGameOver(m_its, new Vector2(30.0f, 220.0f));
            }
        }
    }
    // ゲームオーバー表示の文字列位置
    // Game
    private float m_gameoverstringposition_Game_X;
    // Over
    private float m_gameoverstringposition_Over_X;

    // 文字列表示位置を初期化
    private void GameOverString_Init()
    {
        m_gameoverstringposition_Game_X = -1274.0f;
        m_gameoverstringposition_Over_X = 1274.0f;
    }

    // ゲームオーバー表示の文字列
    public Texture2D m_Game;
    public Texture2D m_Over;
    public Texture2D m_its;

    private const float ITEMEQUIP_X = 306.0f;
    private const float ITEMEQUIP_Y = 480.0f;

    // ブーストゲージ描画
    // 第1引数：テクスチャ
    // 第2引数：配置位置
    public void DrawBoostGauge(Texture2D BoostGauge, Vector2 pos)
    {
        // ブースト量
        float Bratio = this.m_Boost / GetMaxBoost(this.m_BoostLevel);
        GUI.BeginGroup(new Rect(pos.x, pos.y, Bratio * GetMaxBoost(this.m_BoostLevel), 16.0f));
        // ここ以降はBeginGroup原点（上記のpos）からの座標になる点に注意
        // ゲージ本体
        GUI.DrawTexture(new Rect(0, 0, BoostGauge.width, 16), BoostGauge);
        GUI.EndGroup();
        // ゲージ外枠
        GUI.BeginGroup(new Rect(pos.x, pos.y, GetMaxBoost(this.m_BoostLevel), 16.0f));
        // ゲージ左端
        GUI.DrawTexture(new Rect(0, 0, 2, 16), m_WindowParts);
        // ゲージ上端
        GUI.DrawTexture(new Rect(0, 0, GetMaxBoost(this.m_BoostLevel), 2), m_WindowParts);
        // ゲージ下端
        GUI.DrawTexture(new Rect(0, 14, GetMaxBoost(this.m_BoostLevel), 2), m_WindowParts);
        // ゲージ右端
        GUI.DrawTexture(new Rect(GetMaxBoost(this.m_BoostLevel) - 2, 0, 2, 16), m_WindowParts);
        GUI.EndGroup();
    }

    // 覚醒ゲージ描画
    // 第1引数：テクスチャ
    // 第2引数：配置位置（右上基準）
    public void DrawArousalGauge(Texture2D ArousalGauge, Vector2 pos)
    {
        // ゲージ量
        float Aratio = this.m_Arousal / GetMaxArousal(this.m_ArousalLevel);
        // ゲージ量絶対値
        float Arousal_abs = Aratio * GetMaxArousal(this.m_ArousalLevel);
        // 左端が右端の座標‐現在のゲージ量となる
        GUI.BeginGroup(new Rect(pos.x - Arousal_abs, pos.y, GetMaxArousal(this.m_ArousalLevel) * Aratio, 16.0f));
        // ここ以降はBeginGroup原点（上記のpos）からの座標になる点に注意
        // ゲージ本体(最大値-現在の幅分左にオフセット)        
        GUI.DrawTexture(new Rect(-Arousal_abs, 0, ArousalGauge.width, 16), ArousalGauge);

        GUI.EndGroup();

        // 外枠左端座標
        float leftpos = pos.x - GetMaxArousal(this.m_ArousalLevel);
        // ゲージ外枠
        GUI.BeginGroup(new Rect(leftpos, pos.y, GetMaxArousal(this.m_ArousalLevel), 16.0f));
        // ゲージ左端１（Lv1のときのゲージ量）
        GUI.DrawTexture(new Rect(0, 0, 2, 16), m_WindowParts);
        // ゲージ左端２（右端のゲージ量）

        // ゲージ上端
        GUI.DrawTexture(new Rect(0, 0, GetMaxArousal(this.m_ArousalLevel), 2), m_WindowParts);
        // ゲージ下端
        GUI.DrawTexture(new Rect(0, 14, GetMaxArousal(this.m_ArousalLevel), 2), m_WindowParts);
        // ゲージ右端
        GUI.DrawTexture(new Rect(GetMaxArousal(this.m_ArousalLevel) - 2, 0, 2, 16), m_WindowParts);
        GUI.EndGroup();
        // OKの文字列（Lv1のMax以上になると表示される）
        if (m_Arousal >= GetMaxArousal(1) && savingparameter.GetGemContimination((int)m_character_name) < 100.0f)
        {
            float xposition = (pos.x - Arousal_abs) + 64;
            //GUI.DrawTexture(new Rect(xposition, pos.y, 128, 128), m_ArousalOK);
            GUI.Label(new Rect(xposition, pos.y - 5, 1500.0f, 100.0f), "OK!!", "Arousal");
        }
    }

    // 装備アイテム描画
    public void DrawEquipItem(Texture2D ItemEquip, Vector2 pos)
    {
        GUI.BeginGroup(new Rect(pos.x, pos.y, 512, 512), ItemEquip);
        // 文字列[ITEM]
        GUI.Label(new Rect(ITEM_X, ITEM_Y, 255, 255), "ITEM", "Information");
        // 文字列（アイテム名）
        // 装備品
        int index = savingparameter.GetNowEquipItem();
        GUI.Label(new Rect(EQUIP_ITEM_X, EQUIP_ITEM_Y, 255, 255), Item.itemspec[index].Name(), "ItemName");
        // 文字列（所持アイテム数）
        // アイテムの数
        int num = savingparameter.GetItemNum(index);
        GUI.Label(new Rect(ITEMNUM_X, ITEMNUM_Y, 255, 255), num.ToString("d2"), "Information");
        
        GUI.EndGroup();
    }
	
    // 文字列Game Over Its~
    public void DrawGameOver(Texture2D Texture, Vector2 pos)
    {
        GUI.BeginGroup(new Rect(pos.x, pos.y, 1024, 1024), Texture);
        GUI.EndGroup();
    }
}
