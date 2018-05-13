using UnityEngine;
using System.Collections;


// 画面上に出現するインフォメーションの描画および敵HPの描画を司る
public class DrawInformation : MonoBehaviour 
{

	// public
    // インフォメーションのテクスチャー
    public Texture2D m_Information_Tex;       // 描画用背景
    // GUISkin
    public GUISkin m_guiskin;
    // 敵HPのテクスチャ
    public Texture2D m_EnemyHP_Tex;
    // 星のテクスチャ
    public Texture2D m_EnemyHP_Star;
    // このスクリプトで描画する対象オブジェクト
    public GameObject m_Player; // 本体
    public GameObject m_Camera; // カメラ
    // レベルアップ時のSE
    public AudioClip m_LevelupSE;
	

    // private
    // 1本のゲージあたりのHPの値（とりあえず1000）
    private int m_maxHPValue = 1000;

     // 描画
    public void OnGUI()
    {
        CharacterControlBase target = m_Player.GetComponentInChildren<CharacterControlBase>();
        // CPU制御の時は描かない
        if (target != null && target.IsPlayer != CharacterControlBase.CHARACTERCODE.PLAYER)
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
        Vector3 position = new Vector3(14.0f, 2.0f, 0.0f);

        // グループ設定
        // HPをm_Playerから引き出して描画
        // SG汚染率をm_Playerから引き出して描画
        // 対象の名前をm_Playerから引き出してCharacter_Specの配列から取得して描画
        // 対象のレベルをm_Playerから引き出して描画
        // グループ設定
        GUI.BeginGroup(new Rect(position.x, position.y, MadokaDefine.SCREENWIDTH, 256.0f));
       
        
        // インフォメーション
        // 上面に描画
        GUI.DrawTexture(new Rect(0.0f, 0.0f, MadokaDefine.SCREENWIDTH, 30.0f), m_Information_Tex);
        // informationの文字列
        GUI.Label(new Rect(10.0f, 5.0f, 1500.0f, 100.0f), "Information", "Information");
        GUI.Label(new Rect(200.0f, 5.0f, 1500.0f, 100.0f), DrawInformationWords(), "Ename");       
        GUI.EndGroup();
        // 敵HP・名前
        // インフォメーションの下に描画(僚機HPはこの下に変更）
        // 基本的にBoostなどと同じ方式で変更
        // その下に敵の名前
        // 非ロックオン時は書かない
        if (target != null && target.GetIsRockon())
        {
            DrawEnemyHPGauge(m_EnemyHP_Tex, new Vector2(210.0f, 35.0f));
        }
    }

    // TimeWait起動判定
    private bool m_timewaitodone;

    // インフォメーションバー文字列決定
    public string DrawInformationWords()
    {
        string drawwords = "";
        // キャラがレベルアップした場合、そのキャラのレベルアップを20秒間表示する
        if (LevelUpManagement.m_characterName != 0)
        {
            if (!m_timewaitodone)
            {
                m_timewaitodone = true;
                // レベルアップ音を鳴らす
                AudioSource.PlayClipAtPoint(m_LevelupSE, transform.position);
                StartCoroutine(TimeWait(20.0f));
            }
            drawwords = ParameterManager.Instance.CharacterbasicSpec.sheets[0].list[LevelUpManagement.m_characterName].NAME_JP + "は、Level" + LevelUpManagement.m_nextlevel.ToString() + "にレベルアップ！";
        }
		// アイテムを入手した場合、そのアイテムを20秒間表示する
		else if(FieldItemGetManagement.ItemKind > -2)
		{
			if (!m_timewaitodone)
			{
				m_timewaitodone = true;
				StartCoroutine(TimeWait(20.0f));
			}
			// 金以外
			if(FieldItemGetManagement.ItemKind > -1)
			{
				drawwords = Item.itemspec[FieldItemGetManagement.ItemKind].Name() + "を" + FieldItemGetManagement.ItemNum + "個入手！";
			}
			// 金
			else
			{
				drawwords = FieldItemGetManagement.ItemNum + "円入手！";
			}
		}
        // それ以外の時はsavingparameter.storyに応じて内容を変更する
        else
        {
			drawwords = ParameterManager.Instance.EntityInformation.sheets[0].list[savingparameter.story].text;
        }
        if ((m_timewaitodone && LevelUpManagement.m_characterName == 0) || (m_timewaitodone && FieldItemGetManagement.ItemKind > -2))
        {
            StopCoroutine("TimeWait");
            m_timewaitodone = false;
        }


        return drawwords;
    }

    // レベルアップ表示などの時間カウント
    public IEnumerator TimeWait(float time)
    {
        yield return new WaitForSeconds(time);
        LevelUpManagement.m_characterName = 0;
        LevelUpManagement.m_nextlevel = 0;
		FieldItemGetManagement.ItemKind = -2;
		FieldItemGetManagement.ItemNum = 0;
    }


    // HPゲージ描画
    // EnemyHPGauge         [in]    :敵のHPゲージ
    // pos                  [in]    :スクリーン上の配置位置
    public void DrawEnemyHPGauge(Texture2D EnemyHPGauge, Vector2 pos)
    {        
        // ・カメラのステートを取得
	    var Camera = m_Camera.GetComponentInChildren<Player_Camera_Controller>();
    	// 敵のステートを取得
        var Enemy = Camera.Enemy.GetComponentInChildren<CharacterControlBase>();
	    // 敵のHPを取得
        int Enemy_HP = Enemy.GetNowHitpoint();
        
        // 下に余り分のHPを星として表示する
        // HPゲージの長さは650。HPの量：ゲージの長さ=m_maxHPValue:650として計算する
        // 星の数を計算
        int starNum = (int)(Enemy_HP / m_maxHPValue);
        // MAX量を超えていたら、端数を出力する
        if (Enemy_HP > m_maxHPValue)
        {
            Enemy_HP = Enemy_HP % m_maxHPValue;
        }

        // 現在のHPがHPのMAX量の何割かを計算
        float HPRatio = (float)Enemy_HP / (float)m_maxHPValue;            

        //float Bratio = this.m_Boost / GetMaxBoost(this.m_BoostLevel);
        GUI.BeginGroup(new Rect(pos.x, pos.y, 650.0f * HPRatio, 70.0f));
        // ここ以降はBeginGroup原点（上記のpos）からの座標になる点に注意
        // ゲージ本体
        GUI.DrawTexture(new Rect(0, 0, 1000, 12), EnemyHPGauge);         
        
        GUI.EndGroup();
        // 下に余り分のHPを星として表示する
        // 星
        GUI.BeginGroup(new Rect(pos.x,pos.y + 30,650.0f,70.0f));
        if (starNum > 0)
        {
            for (int i = 0; i < starNum; i++)
            {
                GUI.DrawTexture(new Rect(i * 30, 0, 24, 24), m_EnemyHP_Star);
            }
        }
        GUI.EndGroup();
        // 名前
        string name = ParameterManager.Instance.CharacterbasicSpec.sheets[0].list[(int)Enemy.CharacterName].NAME_JP;
        GUI.Label(new Rect(pos.x, pos.y + 15.0f, 1500.0f, 100.0f), name, "Ename"); 
    }
}
