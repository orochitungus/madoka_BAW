using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;

// アクションステージで使うキャラの基底クラス
public class CharacterControlBase : MonoBehaviour
{
    /// <summary>
    /// 本体を追従するカメラ
    /// </summary>
    public Camera MainCamera;

    /// <summary>
    /// 覚醒演出用カメラ
    /// </summary>
    public Camera ArousalCamera;

    /// <summary>
    /// 地面設置確認用のレイの発射位置(コライダの中心とオブジェクトの中心)
    /// </summary>
    public Vector3 LayOriginOffs;

    /// <summary>
    /// レイの長さ
    /// </summary>
    private float _Laylength;

    /// <summary>
    /// ground属性をもったレイヤー（この場合8）
    /// </summary>
    protected int LayMask;

    /// <summary>
    /// このオブジェクトのRigidbody
    /// </summary>
    protected Rigidbody RigidbodyObject;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
