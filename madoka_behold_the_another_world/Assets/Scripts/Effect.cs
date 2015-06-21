using UnityEngine;
using System.Collections;

// 本体接続のエフェクトを持ったオブジェクトに取り付ける。エフェクトを消すためにGetCompornentを呼ぶための空クラス
// 本体につけるエフェクトは必ずこのクラスをアタッチすること
public class Effect : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    // 自分を壊してもらう
    public void Broken()
    {
        Destroy(this);
    }
}
