using UnityEngine;
using System.Collections;

// ステージ間移動を行う
// このスクリプトがインポートされたオブジェクトに触れるとシーン間移動を行う

public class StageMovement : MonoBehaviour
{
	// 移動元コード（StageCode.csを参照）
    public int m_fromcode;
    // 移動先コード（StageCode.csのインデックスを参照）
    public int m_forcode;
    // 移動先のシーン名
    public string m_forscene;

    // イベント移動であるか否か
    public bool m_IsEvent;
    // イベント時、最小発動条件
    public int m_minXstory;
    // イベント時、最大発動条件
    public int m_maxXstroy;

    // 自己破壊用のオブジェクト
    [SerializeField] StageMovement stagemovement;

    // 接触した場合、シーンを差し替える(CharacterControllerで動いている物体は、OnControllerColliderHitでないと反応しない）
    private void OnCollisionEnter(Collision collision)
    {
        // 接触対象を取得
        var target = collision.gameObject.GetComponent<CharacterControl_Base>();
        var target_quest = collision.gameObject.GetComponent<CharacterControl_Base_Quest>();
        // プレイヤーだった場合指定したシーンへ移動
        if(target != null)
        {
            if(target.m_isPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER)
            {
                CheckStory();
            }
        }
        else if (target_quest != null)
        {
            CheckStory();
        }
    }

    // ストーリー発動条件を満たしているか調べる
    void CheckStory()
    {
        if (!m_IsEvent)
        {
            savingparameter.beforeField = m_fromcode;
            savingparameter.nowField = m_forcode;
            Application.LoadLevel(m_forscene);
        }
        else
        {
            if (savingparameter.story >= m_minXstory && savingparameter.story <= m_maxXstroy)
            {
                Application.LoadLevel(m_forscene);
            }
            // ストーリーの発動条件を満たしていない場合、OnTriggerを無効化する
            else
            {
                Destroy(stagemovement.gameObject);
            }
        }
    }
}
