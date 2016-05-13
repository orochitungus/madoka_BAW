using UnityEngine;
using System.Collections;


// レーダーで背景となる黒い板を制御する
// オブジェクトBlackBoardにセットすること
// CPU時に描画しておくと、味方と高低差がついたときに味方のマーカーと重なって消えるので、CPU時は消しておく
public class BlackBoard : MonoBehaviour 
{
    // 描画するBlackBoard
    public GameObject m_Blackboard;   
    void Start () 
    {
        // 自分がCPUであるか否か検出する
	    // 親オブジェクトを取得
        GameObject parent = gameObject.transform.parent.gameObject;
        // 親オブジェクトのCharacterControl_Baseを取得
        var parentData = parent.GetComponent<CharacterControl_Base>();
        CharacterControl_Base.CHARACTERCODE cpu = parentData.IsPlayer;
        // CPUの場合、BlackBoardの描画を切る
        if (cpu != CharacterControl_Base.CHARACTERCODE.PLAYER)
        {
            m_Blackboard.transform.GetComponent<Renderer>().enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
