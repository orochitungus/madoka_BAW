using UnityEngine;
using System.Collections;

// アイテムのスペックを表すクラス
public class ItemSpec 
{
    //名前
    private string m_name;
    public string Name()
    {
        return m_name;
    }
    //機能
    public enum ItemFunction
    {
        REBIRETH_HP,        // HP回復
        REBIRTH_DEATH,      // 戦闘不能回復
        REBIRTH_SOUL,       // ソウルジェム汚染率回復
        REBIRTH_FULL,       // HPと戦闘不能を両方回復

        NONE,               // 効果なし
    }
    private ItemFunction m_itemFunction;
    public ItemFunction ItemFuciton()
    {
        return m_itemFunction;
    }
    //全体化の可否
    private bool m_isAll;
    public bool IsAll()
    {
        return m_isAll;
    }

    //回復の量
    private int m_rebirthEn;
    public int RebirthEn()
    {
        return m_rebirthEn;
    }

    //説明文の内容
    private string m_information;
    public string Information()
    {
        return m_information;
    }

    // 価格
    private int m_price;
    public int Price()
    {
        return m_price;
    }

    // コンストラクタ
    // 名前・機能・全体化の可否・回復量・説明文の内容
    public ItemSpec(string name, ItemFunction itemfunction, bool isall, int rebirthEn, string information,int price)
    {
        m_name = name;
        m_itemFunction = itemfunction;
        m_isAll = isall;
        m_rebirthEn = rebirthEn;
        m_information = information;
        m_price = price;
    }
}
