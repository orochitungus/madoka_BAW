using UnityEngine;
using System.Collections;

public class HockControl : MonoBehaviour 
{
    // 差し替え用のオブジェクト
    // 差し替え対象となるプレハブを指定する
    // どのプレハブにするのかはこのスクリプトをセットしたプレハブのInspecterで指定する
    public GameObject changeweapon;
    // 0：取り替え前、1：取り替え用意、2：取り替え完了
    public int changestandby;

	// Use this for initialization
	void Start () 
	{
        changestandby = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
        //if (changestandby == 1)
        //{
        //    // 現在のフックの位置に弓を置く
        //    var obj = (GameObject)Instantiate(changeweapon, transform.position, transform.rotation);
        //    // 親子関係を再設定する
        //    obj.transform.parent = transform.parent;
        //    // 自分を削除する
        //    Destroy(gameObject);
        //    changestandby = 2;
        //}
	}

    public void EquipWeapon()
    {
        // 現在のフックの位置にchangeweaponで指定したオブジェクトを置く
        var obj = (GameObject)Instantiate(changeweapon, transform.position, transform.rotation);
        // 親子関係を再設定する
        obj.transform.parent = transform.parent;
        // 自分を削除する
        Destroy(gameObject);
        changestandby = 2;
    }

 
}
