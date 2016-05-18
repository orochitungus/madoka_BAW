using UnityEngine;
using System.Collections;
using System;

// HP及びソウルジェム汚染率を描画
public class DrawHPGauge : MonoBehaviour 
{
    public GameObject m_Player;     // 描画対象となるキャラ
    public Texture2D m_Gauge;       // 描画用背景
    public GUISkin m_guiskin;                                  // GUISKIN
    public Vector2 HP_Position;     // HP表示位置
    public Vector2 SG_InfoPosition; // SG汚染率（タイトル）表示位置
    public Vector2 SG_Position;     // SG汚染率表示位置
    public Vector2 NamePosition;    // 名前表示位置
    public Vector2 LevelPosition;   // レベル表示位置
    // 描画
    public void OnGUI()
    {
        var target = m_Player.GetComponentInChildren<CharacterControl_Base>();
        // CPU制御の時は描かない
        if (target.IsPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
        {
            return;
        }
        // GUIスキンを設定
        if (m_guiskin)
        {
            GUI.skin = m_guiskin;
        }
        Vector3 scale = new Vector3(Screen.width / MadokaDefine.SCREENWIDTH, Screen.height / MadokaDefine.SCREENHIGET, 1);
        // GUIが解像度に合うように変換行列を設定(先にTRSの第1引数を0で埋めていくとリサイズにも対応可能ということらしい）
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
        Vector3 position = new Vector3(1.0f, 250.0f, 0.0f);
        // グループ設定
        // HPをm_Playerから引き出して描画
        // SG汚染率をm_Playerから引き出して描画
        // 対象の名前をm_Playerから引き出してCharacter_Specの配列から取得して描画
        // 対象のレベルをm_Playerから引き出して描画
        // グループ設定
        GUI.BeginGroup(new Rect(position.x, position.y, 256.0f, 256.0f));
        // 背景描画
        GUI.DrawTexture(new Rect(0.0f, 0.0f, 256.0f, 256.0f), m_Gauge);
        // HPをm_Playerから引き出して描画
        // 配置位置
        HP_Position.x = 40;
        HP_Position.y = 195;
        // HP
        int nowHP = target.m_DrawHitpoint;
        // 1000の位
        int nowHP_1000 = (int)(nowHP / 1000);
        // 100の位
        int nowHP_100 = (int)((nowHP - nowHP_1000*1000)/100);
        // 10の位
        int nowHP_10 = (int)((nowHP - nowHP_1000*1000 - nowHP_100*100) / 10);
        // 1の位
        int nowHP_1 = (int)(nowHP - nowHP_1000*1000 - nowHP_100*100 - nowHP_10*10);
        // HPは0で合わせるので、桁数に応じてオフセット
        if (nowHP >= 1000)
        {
            int HP_Position1000 = 40;
            GUI.Label(new Rect(HP_Position1000, HP_Position.y, 1500.0f, 100.0f), nowHP_1000.ToString(), "HP");
        }
        if (nowHP >= 100)
        {
            int HP_Position100 = 70;
            GUI.Label(new Rect(HP_Position100, HP_Position.y, 1500.0f, 100.0f), nowHP_100.ToString(), "HP");
        }
        if (nowHP > 10)
        {
            int HP_Position10 = 100;
            GUI.Label(new Rect(HP_Position10, HP_Position.y, 1500.0f, 100.0f), nowHP_10.ToString(), "HP");
        }
        if(nowHP > 0)
        {
            HP_Position.x = 130;
            GUI.Label(new Rect(HP_Position.x, HP_Position.y, 1500.0f, 100.0f), nowHP_1.ToString(), "HP");
        }
        // 0を割ったら0で固定
        if(nowHP <= 0)
        {
            HP_Position.x = 130;
            GUI.Label(new Rect(HP_Position.x, HP_Position.y, 1500.0f, 100.0f), "0", "HP");
        }
        
        // SG汚染率をm_Playerから引き出して描画
        // タイトル
        SG_InfoPosition.x = 16;
        SG_InfoPosition.y = 110;
        GUI.Label(new Rect(SG_InfoPosition.x, SG_InfoPosition.y, 1500.0f, 100.0f), "Contamination rate", "SG_Info");        
        // 小数点以下2桁まで描画するので、とりあえず100倍
        int SG = (int)(target.m_GemContamination * 100);
        int[] SG_Value = new int[5];
        // 各桁の値を取得
        for (int i = 4; i >= 0; i--)
        {
            // 10000(100の位）
            if (i == 4)
            {
                SG_Value[4] = (int)(SG / 10000);
            }
            else
            {
                int more=SG;
                // 自分より大きな桁の値を減算する
                for(int j=4; j>i; j--)
                {
                    more = more - (int)(SG_Value[j] * Math.Pow(10.0,(double)j)); 
                }
                SG_Value[i] = (int)(more / (Math.Pow(10.0, (double)i)));
            }
        }
        SG_Position.x = 100.0f;
        SG_Position.y = 125.0f;
        for (int i = 0; i < 5; i++)
        {
            if (i > 2)
            {   // 上で100倍しているので帳尻を合わせないとならない
                if (SG/100 >= Math.Pow(10.0, (i - 2)))
                {
                    GUI.Label(new Rect(SG_Position.x - 14.0f * i, SG_Position.y, 1500.0f, 100.0f), SG_Value[i].ToString(), "SG_dot");
                }
            }
            else
            {
                GUI.Label(new Rect(SG_Position.x - 14.0f * i, SG_Position.y, 1500.0f, 100.0f), SG_Value[i].ToString(), "SG_dot");
            }
        }
        GUI.Label(new Rect(SG_Position.x - 28.0f + 10, SG_Position.y, 1500.0f, 100.0f), ".", "SG_dot");
        GUI.Label(new Rect(SG_Position.x + 14.0f, SG_Position.y, 1500.0f, 100.0f), "%", "SG_dot");

        // 対象の名前をm_Playerから引き出してCharacter_Specの配列から取得して描画
        string playername = Character_Spec.Name[(int)target.m_character_name];
        NamePosition.x = 10;
        NamePosition.y = 152;
        GUI.Label(new Rect(NamePosition.x, NamePosition.y, 1500.0f, 100.0f), playername, "Name");

        // 対象のレベルをm_Playerから引き出して描画
        int level = target.Level;
        // 10の位
        int level_10 = (int)(level / 10);
        // 1の位
        int level_1 = level%10;
        LevelPosition = new Vector2(10, 237);
        GUI.Label(new Rect(LevelPosition.x, LevelPosition.y, 1500.0f, 100.0f), "Level", "Level");
        if (level >= 10)
        {
            GUI.Label(new Rect(LevelPosition.x + 14*4, LevelPosition.y, 1500.0f, 100.0f), level_10.ToString(), "Level");
        }
        GUI.Label(new Rect(LevelPosition.x + 14 * 5, LevelPosition.y, 1500.0f, 100.0f), level_1.ToString(), "Level");

        GUI.EndGroup();
    }
}
