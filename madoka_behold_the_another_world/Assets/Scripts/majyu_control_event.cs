using UnityEngine;
using System.Collections;

// プロローグにおける魔獣を操作する
public class majyu_control_event : MonoBehaviour 
{
    // モード選択
    // 消滅（初期状態）、出現、通常、歩行（使うかな・・・？）
    public enum NowMode {ERASE, APPEAR, IDLE, WALK };
    public NowMode m_nowmode;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
