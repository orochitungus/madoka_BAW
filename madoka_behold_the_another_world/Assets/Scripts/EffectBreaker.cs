using UnityEngine;
using System.Collections;


// エフェクトを自壊するための関数（予め指定した時間に自壊させる）
public class EffectBreaker : MonoBehaviour
{   

    void Awake()
    {
        
    }

    public enum EffectType
    {
        ITEM,
        DAMAGE,
    };

    public EffectType m_Insp_Effecttype;
    
    void Start()
    {
        if (m_Insp_Effecttype == EffectType.ITEM)
        {
            StartCoroutine("BrokenEffect", MadokaDefine.ITEMEFFECTTIME);
        }
        else if (m_Insp_Effecttype == EffectType.DAMAGE)
        {
            StartCoroutine("BrokenEffect", MadokaDefine.DAMEGEEFFECTIME);
        }
    }

    private IEnumerator BrokenEffect(float delaytime)
    {
        // delaytime分処理を停止
        yield return new WaitForSeconds(delaytime);
        // 自壊させる
        Destroy(gameObject);
    }
}
