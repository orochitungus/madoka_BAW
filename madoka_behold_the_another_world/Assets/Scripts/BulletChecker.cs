using UnityEngine;
using System.Collections;

// CPU操作時にガード判定を取らせるためのコリジョンを制御するためのクラス
public class BulletChecker : MonoBehaviour 
{
    // 親オブジェクト
    public GameObject m_Obj_OR;
    // 弾丸がコリジョン内に来ているか否か
    private bool m_bulletIn;
    // 1F前のm_bulletInの状態
    private bool m_prebulletIn;
    // 親オブジェクトのキャラクターのインデックス
    private int m_characterIndex;

	// Use this for initialization
	void Start () 
    {
        m_bulletIn = false;
        m_prebulletIn = false;
        var character = m_Obj_OR.GetComponent<CharacterControlBase>();
        m_characterIndex = (int)character.CharacterName;
	}
	
	// Update is called once per frame
	void Update () 
    {
        var characterAI = m_Obj_OR.GetComponent<AIControlBase>();
        if (m_bulletIn)
        {            
            characterAI.Cpumode = AIControlBase.CPUMODE.GUARD;
        }
        // 過去フレームでガード状態で、弾丸が消失するなどしてガードする必要がなくなった場合はNORMALにする
        if (m_prebulletIn && !m_bulletIn)
        {
            characterAI.Cpumode = AIControlBase.CPUMODE.NORMAL;            
        }
        m_prebulletIn = m_bulletIn;
	}

    // 弾丸がコリジョン内にいるか否か
    private void OnTriggerStay(Collider collision)
    {
        if (collision == null)
        {
            m_bulletIn = false;
        }
        // collisionが自分が放った以外の弾丸であるか判定する
        // Bullet系を判定
        var targetBullet = collision.gameObject.GetComponent<Bullet>();
        if (targetBullet != null)
        {
            // 自分の放った弾丸であるか否か判定する
            if (targetBullet.InjectionCharacterIndex != m_characterIndex)
            {
                m_bulletIn = true;
            }
        }
        // Laser系を判定
        var targetLaser = collision.gameObject.GetComponent<Laser>();
        if (targetLaser != null)
        {
            // 自分の放ったレーザーであるか否か判定する
            if (targetLaser.m_CharacterIndex != m_characterIndex)
            {
                m_bulletIn = true;
            }
        }

    }

}
