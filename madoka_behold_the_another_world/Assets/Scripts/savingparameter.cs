using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

// セーブデータの中身件グローバル変数(staticをつけるとグローバルになる）
[Serializable()]        // セーブを有効化する
public static class savingparameter
{  

    // ストーリー関連
    public static int story;                       // シナリオ進行度。docファイル×100の値が話数を表し、そのあとの数字で区切る

    // 現在の主人公が誰か
    public static Character_Spec.CHARACTER_NAME nowHero;

    // キャラクターの配置位置
    public static Vector3 nowposition;
    // キャラクターの配置角度
    public static Vector3 nowrotation;

    // アイテムボックスの開閉フラグ
    public static bool[] itemboxopen = new bool[MadokaDefine.NUMOFITEMBOX];

    // 現在の所持金
    public static int nowmoney;

    // キャラクター関連
    // 現在のパーティー(護衛対象も一応パーティーに含める)
    private static int[] nowparty = new int[4];     
    // パーティーメンバー編成
    // index[in]       :何番目か
    // member[in]      :誰であるか
    public static void SetNowParty(int index, int member)
    {
        nowparty[index] = member;
    }
    // index[in]       :何番目か
    // rerurn          :誰であるか
    public static int GetNowParty(int index)
    {
        return nowparty[index];
    }
    
	/// <summary>
	/// 現在のパーティーは何人か
	/// </summary>
	/// <returns></returns>
	public static int GetNowPartyNum()
	{
		int partynum = 0;
		for(int i=0; i<3; i++)
		{
			if(GetNowParty(i) != 0)
			{
				partynum++;
			}
		}
		return partynum;
	}

    // 各キャラのレベル
    private static int[] nowlevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :何番目か
    // level[in]       :レベル
    public static void SetNowLevel(int index, int level)
    {
        nowlevel[index] = level;
    }
    // index[in]       :何番目か
    // rerurn          :現在のレベル
    public static int GetNowLevel(int index)
    {
        return nowlevel[index];
    }

    // 各キャラの現在の経験値
    private static int[] nowExp = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // 固定値へ変更
    // index[in]        :何番目か
    // value[in]        :変更する値
    public static void SetExp(int index,int value)
    {
        nowExp[index] = value;
    }
    // 固定値を加算
    // index[in]        :何番目か
    // value[in]        :加算する値
    public static void AddExp(int index, int value)
    {
        nowExp[index] += value;
        // 元々のレベル
        int levelOR = GetNowLevel(index);
        // 閾値を超えた場合はレベルアップ
        for (int i = levelOR; i < MadokaDefine.LEVELUPEXP.Length - 1; ++i)
        {
            if (nowExp[index] >= MadokaDefine.LEVELUPEXP[i])
            {
                SetNowLevel(index, i + 1);                
                AddSkillPoint(index, (i + 1 - levelOR) * MadokaDefine.SKILLPOINTINCREASE);
                LevelUpManagement.m_characterName = index;
                LevelUpManagement.m_nextlevel = i + 1;
            }
        }
    }


    // 経験値を取得
    // index[in]        :何番目か
    public static int GetNowExp(int index)
    {
        return nowExp[index];
    }
    // 次のレベルアップに必要な経験値を取得
    // index[in]        :何番目か
    public static int GetNextExp(int index)
    {
        // 現在の対象キャラのレベル
        int nowlevel = GetNowLevel(index);
        // 現在の対象キャラの経験値
        int nowexp = GetNowExp(index);
        // 対象キャラの次のレベルアップに必要な経験値
        return MadokaDefine.LEVELUPEXP[nowlevel + 2] - nowexp;
    }

    // 各キャラのHP    
    private static int[] nowHP = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :何番目か
    // HP[in]          :HPの値
    public static void SetNowHP(int index, int HP)
    {
        if (GetMaxHP(index) < HP)
        {
            nowHP[index] = GetMaxHP(index);
        }
        else
        {
            nowHP[index] = HP;
        }
    }
    // index[in]       :何番目か
    // rerurn          :現在のHP
    public static int GetNowHP(int index)
    {
        if(GetMaxHP(index) < nowHP[index])
        {
            return GetMaxHP(index);
        }
        return nowHP[index];
    }

    // 各キャラのHPを回復(蘇生含む）
    // index[in]        :何番目か
    // cureRatio[in]    :何割回復するか
    public static void CureHP(int index, int cureRatio)
    {
        SetNowHP(index, GetNowHP(index) + GetMaxHP(index) * cureRatio/100);        
    }


    // 各キャラの最大HP
    private static int[] nowMaxHP = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :何番目か
    // HP[in]          :HPの値
    public static void SetMaxHP(int index, int HP)
    {
        // HP初期値
        int NowHitpoint_OR = Character_Spec.HP_OR[index];
        // HP成長係数
        int NowHitpoint_Growth = Character_Spec.HP_Grouth[index];
        nowMaxHP[index] = NowHitpoint_Growth * (nowlevel[index]-1) + NowHitpoint_OR;
    }
    // index[in]       :何番目か
    // rerurn          :現在のHP
    public static int GetMaxHP(int index)
    {
        // HP初期値
        int NowHitpoint_OR = Character_Spec.HP_OR[index];
        // HP成長係数
        int NowHitpoint_Growth = Character_Spec.HP_Grouth[index];
        nowMaxHP[index] = NowHitpoint_Growth * (nowlevel[index] - 1) + NowHitpoint_OR;
        return nowMaxHP[index];
    }

    // 各キャラの覚醒ゲージ
    private static float[] nowArousal = new float[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :何番目か
    // HP[in]          :覚醒ゲージの値
    public static void SetNowArousal(int index, float Ar)
    {
        // 最大覚醒ゲージ量
        nowMaxArousal[index] = (float)(Character_Spec.Arousal_Growth[index] * (ArousalLevel[index] - 1) + Character_Spec.Arousal_OR[index]); 
        if (nowMaxArousal[index] < Ar)
        {
            nowArousal[index] = nowMaxArousal[index];
        }
        else
        {
            nowArousal[index] = Ar;
        }
        if (Ar < 0)
        {
            nowArousal[index] = 0.0f;
        }
    }
    // index[in]       :何番目か
    // rerurn          :現在の覚醒ゲージ
    public static float GetNowArousal(int index)
    {
        return nowArousal[index];
    }

    // index[in]       :何番目か
    // add[in]         :追加量
    // 覚醒ゲージの量を追加する
    public static void AddArousal(int index, float add)
    {
        nowArousal[index] += add;
        // 最大覚醒ゲージ量を算出
        nowMaxArousal[index] = (float)(Character_Spec.Arousal_Growth[index] * (ArousalLevel[index] - 1) + Character_Spec.Arousal_OR[index]); 
        if (nowArousal[index] > nowMaxArousal[index])
        {
            nowArousal[index] = nowMaxArousal[index];
        }
    }

    // 各キャラの最大覚醒ゲージ
    private static float[] nowMaxArousal = new float[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :何番目のキャラか
    public static void SetMaxArousal(int index)
    {
        // 覚醒ゲージ初期値
        int Arousal_OR = Character_Spec.Arousal_OR[index];
        // 覚醒ゲージ成長係数
        int ArousalGrowth = Character_Spec.Arousal_Growth[index];
        nowMaxArousal[index] = ArousalGrowth * (ArousalLevel[index] - 1) + Arousal_OR;
    }
    // index[in]       :何番目か
    // rerurn          :現在の最大覚醒ゲージ
    public static float GetMaxArousal(int index)
    {
        // 覚醒ゲージ初期値
        int Arousal_OR = Character_Spec.Arousal_OR[index];
        // 覚醒ゲージ成長係数
        int ArousalGrowth = Character_Spec.Arousal_Growth[index];
        return ArousalGrowth * (ArousalLevel[index] - 1) + Arousal_OR;
    }


    // 攻撃力レベル
    private static int[] StrLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :キャラクター
    // member[in]      :攻撃力の値
    public static void SetStrLevel(int index, int str)
    {
        StrLevel[index] = str;
    }
    // index[in]       :キャラクター
    // rerurn          :現在の攻撃力
    public static int GetStrLevel(int index)
    {
        return StrLevel[index];
    }

    // 防御力レベル
    private static int[] DefLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :キャラクター
    // member[in]      :防御力の値
    public static void SetDefLevel(int index, int def)
    {
        DefLevel[index] = def;
    }
    // index[in]       :キャラクター
    // rerurn          :現在の防御力
    public static int GetDefLevel(int index)
    {
        return DefLevel[index];
    }
    // 残弾数レベル
    private static int[] BulLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :キャラクター
    // bul[in]         :残弾数の値
    public static void SetBulLevel(int index, int bul)
    {
        BulLevel[index] = bul;
    }
    // index[in]       :キャラクター
    // rerurn          :現在の残弾数
    public static int GetBulLevel(int index)
    {
        return BulLevel[index];
    }
    // ブースト量レベル
    private static int[] BoostLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :キャラクター
    // boost[in]       :ブーストの値
    public static void SetBoostLevel(int index, int boost)
    {
        BoostLevel[index] = boost;
    }
    // index[in]       :キャラクター
    // rerurn          :現在のブースト量
    public static int GetBoostLevel(int index)
    {
        return BoostLevel[index];
    }

    // 覚醒ゲージレベル
    private static int[] ArousalLevel = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :キャラクター
    // arousal[in]     :覚醒ゲージの値
    public static void SetArousalLevel(int index, int arousal)
    {
        ArousalLevel[index] = arousal;
    }
    // index[in]       :キャラクター
    // rerurn          :現在の覚醒ゲージ量
    public static int GetArousalLevel(int index)
    {
        return ArousalLevel[index];
    }

    // ソウルジェム汚染率
    private static float[] GemContimination = new float[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    public static void SetGemContimination(int index, float cont)
    {
        if (cont < 0)
        {
            cont = 0;
        }
        else
        {
            GemContimination[index] = cont;
        }
    }
    public static float GetGemContimination(int index)
    {
        return GemContimination[index];
    }

    // ソウルジェム汚染率を回復
    // index[in]    :対象
    // cureRatio    :回復量(HPと異なり、そのままの数値を減算する）
    public static void CureContimination(int index, float cureRatio)
    {
        GemContimination[index] -= cureRatio;
        if (GemContimination[index] < 0)
        {
            GemContimination[index] = 0;
        }
    }

    // スキルポイント
    private static int[] SkillPoint = new int[(int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM];
    // index[in]       :キャラクター
    // skillpoint[in]  :スキルポイントの値
    public static void SetSkillPoint(int index, int skillpoint)
    {
        SkillPoint[index] = skillpoint;
    }
    // スキルポイントを加算する
    public static void AddSkillPoint(int index, int addpoint)
    {
        SkillPoint[index] += addpoint;
    }
    // index[in]       :キャラクター
    // rerurn          :現在のスキルポイント
    public static int GetSkillPoint(int index)
    {
        return SkillPoint[index];
    }

    // 各アイテムの保持数
    private static int[] m_numofItem = new int[Item.itemspec.Length];
    // index[in]       :アイテムの種類（itemspecの種類に準じる）
    // number[in]      :アイテムの数
    public static void SetItemNum(int index,int number)
    {
        m_numofItem[index] = number;
        // 所持限界数を超えることはない
        if (number > MadokaDefine.MAXITEM)
        {
            m_numofItem[index] = MadokaDefine.MAXITEM;
        }
        else if (number < 0)
        {
            m_numofItem[index] = 0;
        }
    }
    public static int GetItemNum(int index)
    {
        return m_numofItem[index];
    }


    // 現在装備中のアイテム(itemspecのインデックスと同じ。装備していないときは-1になる）
    private static int m_nowequipItem;
    public static void SetNowEquipItem(int item)
    {
        m_nowequipItem = item;
    }
    public static int GetNowEquipItem()
    {
        return m_nowequipItem;
    }

    /// <summary>
    /// 上記の名称表示版
    /// </summary>
    /// <returns></returns>
    public static string GetNowEquipItemString()
    {
        return Item.itemspec[m_nowequipItem].Name();
    }

    // アイテムを使用し、効果を出させる
    // itemkind         [in]:アイテムの種類
    // targetcharacter  [in]:アイテムの効果を出すキャラクター
    public static void ItemDone(int itemkind,int targetcharacter)
    {
        // 対象アイテムの数を減らす
        SetItemNum(itemkind, GetItemNum(itemkind) - 1);

        // HP回復系(HP0時回復無効）
        if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRETH_HP)
        {
            // 全体
            if (Item.itemspec[itemkind].IsAll())
            {
               
                for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
                {
                    // HP0時回復無効
                    if (GetNowHP(GetNowParty(i)) > 0)
                    {
                        CureHP(GetNowParty(i), Item.itemspec[itemkind].RebirthEn());
                    }
                }
            }
            // 単体
            else
            {
                CureHP(GetNowParty(targetcharacter), Item.itemspec[itemkind].RebirthEn());
            }
        }
        // 蘇生系(全体時HP0でないものは無効）
        else if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_DEATH)
        {
            // 全体
            if (Item.itemspec[itemkind].IsAll())
            {
               
                for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
                {
                    // 生存時回復無効
                    if (GetNowHP(GetNowParty(i)) == 0)
                    {
                        CureHP(GetNowParty(i), Item.itemspec[itemkind].RebirthEn());
                    }
                }
            }
            // 単体
            else
            {
                CureHP(GetNowParty(targetcharacter), Item.itemspec[itemkind].RebirthEn());
            }
        }
        // SG汚染率回復系
        else if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_SOUL)
        {
            // 全体
            if (Item.itemspec[itemkind].IsAll())
            {
                
                for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
                {
                    // 無汚染時回復無効
                    if (GetGemContimination(GetNowParty(i)) != 0)
                    {
                        CureContimination(GetNowParty(i), Item.itemspec[itemkind].RebirthEn());
                    }
                }
            }
            // 単体
            else
            {
                CureContimination(GetNowParty(targetcharacter), Item.itemspec[itemkind].RebirthEn());
            }
        }
        // HP＆戦闘不能を総合回復
        else if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_FULL)
        {
            // 全体
            if (Item.itemspec[itemkind].IsAll())
            {
                for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
                {
                    CureHP(GetNowParty(i), Item.itemspec[itemkind].RebirthEn());                    
                }
            }
            // 単体
            else
            {
                CureHP(GetNowParty(targetcharacter), Item.itemspec[itemkind].RebirthEn());
            }
        }
    }

    // アイテムの使用可否を問う（単体版）
    // itemkind         [in]:アイテムの種類
    // targetcharacter  [in]:アイテムの効果を出すキャラクター(パーティーの何番目か）
    // 使用可能:true,使用不可:false
    public static bool ItemUseCheck(int itemkind, int targetcharacter)
    {
        // HP回復系でHP＝MAXなら使用不可
        if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRETH_HP || Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_FULL)
        {
            if (GetNowHP(GetNowParty(targetcharacter)) == GetMaxHP(GetNowParty(targetcharacter)))
            {
                return false;
            }
        }
        // 蘇生系で生きているなら使用不可
        else if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_DEATH)
        {
            if (GetNowHP(GetNowParty(targetcharacter)) > 0)
            {
                return false;
            }
        }
        // SG汚染率回復系で汚染率0なら使用不可
        else if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_SOUL)
        {
            if (GetGemContimination(GetNowParty(targetcharacter)) == 0)
            {
                return false;
            }
        }        
        return true;
    }
    
    // アイテムの使用可否を問う（全体版）
    // itemkind         [in]:アイテムの種類
    // 使用可能:true,使用不可:false
    public static bool ItemUseCheckAll(int itemkind)
    {
        // HP回復系かHP回復＆蘇生系で全員HP＝MAXなら使用不可
        if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRETH_HP || Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_FULL)
        {
            int maxcount = 0;
            for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
            {
                if (GetNowHP(GetNowParty(i)) == GetMaxHP(GetNowParty(i)))
                {
                    maxcount++;
                }
            }
            if (maxcount == MadokaDefine.MAXPARTYMEMBER)
            {
                return false;
            }
        }
        // 蘇生系で全員生きているなら使用不可
        else if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_DEATH)
        {
            int maxcount = 0;
            for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
            {
                if (GetNowHP(GetNowParty(i)) > 0)
                {
                    maxcount++;
                }
            }
            if (maxcount == MadokaDefine.MAXPARTYMEMBER)
            {
                return false;
            }
        }
        // SG汚染率回復系で全員汚染率0なら使用不可
        else if (Item.itemspec[itemkind].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_SOUL)
        {
            int maxcount = 0;
            for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
            {
                if (GetGemContimination(GetNowParty(i)) == 0)
                {
                    maxcount++;
                }
            }
            if (maxcount == MadokaDefine.MAXPARTYMEMBER)
            {
                return false;
            }
        }
        return true;
    }

    // 現在の場所（Kanonと同じ形式、マップ.xlsxを参照）
    public static int nowField;
    // 前にいた場所（Kanonと同じ形式、マップ.xlsxを参照）
    public static int beforeField;
    // 初期化(staticだからコンストラクタは持てない)
    public static void savingparameter_Init()
    {
        // ストーリー関係
        story = 0;

		// パーティーメンバー(弓ほむらのみで、僚機なし）
		//nowparty[0] = (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B;
		// 魔獣試験
		nowparty[0] = (int)Character_Spec.CHARACTER_NAME.ENEMY_MAJYU;
		// スコノ試験
        //nowparty[0] = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;

        // 主人公
        nowHero = Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B;

        for (int i = 1; i < 4; i++)
        {
            nowparty[i] = 0;
        }
        // レベル初期化(一応全キャラ分やっておく)
        for (int i = 0; i < (int)(Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM); i++)
        {
            nowlevel[i] = 1;
            StrLevel[i] = 1;
            DefLevel[i] = 1;
            BulLevel[i] = 1;
            BoostLevel[i] = 1;
            ArousalLevel[i] = 1;
            GemContimination[i] = 0.0f;
            SkillPoint[i] = 0;
            // 経験値初期化
            nowExp[i] = 0;
            // HP初期化
            nowHP[i] = Character_Spec.HP_OR[i];
            // SG汚染率初期化
            GemContimination[i] = 0.0f;
            // 覚醒ゲージ初期化
            nowArousal[i] = 0.0f;
        }

        nowField = 1;
        beforeField = 0;      // テスト用に一時カット

        // 初期アイテム保持数を初期化（テスト用の暫定）
        for (int i = 0; i < Item.itemspec.Length; i++)
        {
            m_numofItem[i] = 10;
        }
        m_numofItem[0] = 5;
        m_nowequipItem = 0;

        // アイテムボックス開閉数を初期化
        for (int i = 0; i < MadokaDefine.NUMOFITEMBOX; ++i)
        {
            itemboxopen[i] = false;
        }
        // 所持金を初期化
        nowmoney = MadokaDefine.FIRSTMONEY;

		// アイテム入手フラグとレベルアップフラグを初期化
		LevelUpManagement.m_characterName = 0;
		FieldItemGetManagement.ItemKind = -2;
		FieldItemGetManagement.ItemNum = 0;

        
        //// 実験でリボほむらのみSKILLPOINTを10に
        //SkillPoint[(int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B] = 10;
        //// 実験でリボほむらのHPを半分に
        //nowHP[(int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B] = 50;
        //// 実験でリボほむらのSG汚染率を50％に
        //GemContimination[(int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B] = 50;
    }

    // データを保存する
    // obj      [in]:保存するオブジェクト
    // path     [in]:保存先のファイル名
    public static void SaveToBinaryFile(object obj, string path)
    {
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryFormatter bf = new BinaryFormatter();
        // シリアル化して書き込む
        bf.Serialize(fs, obj);
        fs.Close();
    }

    // セーブされたデータを復元する
    // path     [in]:保存先のファイル名
    // return       :復元されたオブジェクト
    public static object LoadFromBinaryFile(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryFormatter f = new BinaryFormatter();
        // 読み込んでデシリアライズする
        object obj = f.Deserialize(fs);
        fs.Close();

        return obj;

    }
}


