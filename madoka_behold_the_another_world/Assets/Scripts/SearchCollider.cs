using UnityEngine;
using System.Collections;

// 索敵コライダを司る
// 敵が索敵範囲に入ったかを判定する
public class SearchCollider : MonoBehaviour 
{
    // 接触状態を表す
    private bool m_isEncounter;
   

    // 自機がプレイヤー側か敵側か（開始時にAIControlからもらう）
    private CharacterControl_Base.CHARACTERCODE m_isPlayer;

    // 上記変数のアクセサ
    public void SetPC(CharacterControl_Base.CHARACTERCODE player)
    {
        m_isPlayer = player;
    }

	// Use this for initialization
	void Start () 
    {
        m_isEncounter = false;
      
	}
	
	// Update is called once per frame
	void Update () 
    {
       
	}


    // 接触のアクセサ
    public bool GetEncounter()
    {
        return m_isEncounter;
    }

 
    // コライダの中心にいるキャラ
    public GameObject m_colliderMaster;
    // 接触したキャラ
    public GameObject m_hitCharacter;

    // 接触を判定する(CharacterControllerで動いている物体は、OnControllerColliderHitでないと反応しない）
    private void OnTriggerStay(Collider collision)
    //private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        // これだと「コライダの中心にいる」キャラも取ってしまう→勝手にm_isEncounterが折られる
        // 接触対象取得(キャラクターである必要があるため）
        var hit = collision.gameObject.GetComponent<CharacterControl_Base>();
        // コライダの中心にいるキャラを拾った場合は無意味なので強制抜け
        var mastercheck = m_colliderMaster.gameObject.GetComponent<CharacterControl_Base>();
        m_hitCharacter = null;
        if (hit == mastercheck)
        {
            return;
        }

        if (hit != null)
        {
            // 自機がプレイヤー側
            if (m_isPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER_ALLY || m_isPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER)
            {
                // 敵であればエンカウント
                if (hit.IsPlayer == CharacterControl_Base.CHARACTERCODE.ENEMY)
                {
                    m_isEncounter = true;
                    m_hitCharacter = collision.gameObject;
                    return;
                }
                // 味方であれば強制抜け（フラグを弄らせない）
                else if (hit.IsPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER_ALLY || m_isPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER)
                {
                    m_isEncounter = false;
                    return;
                }
            }
            // 自機が敵側
            else
            {
                // PCであればエンカウント
                if (hit.IsPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER || hit.IsPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER_ALLY)
                {
                    m_isEncounter = true;
                    m_hitCharacter = collision.gameObject;
                    return;
                }
                // 敵であれば強制抜け（フラグを弄らせない）
                else if (hit.IsPlayer == CharacterControl_Base.CHARACTERCODE.ENEMY)
                {
                    m_isEncounter = false;
                    return;
                }
            }           
        }
        m_isEncounter = false;
    } 
}
