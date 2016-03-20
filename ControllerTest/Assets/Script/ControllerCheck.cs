using UnityEngine;
using System.Collections;

public class ControllerCheck : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//左スティック（今までと同じ）
		if (Input.GetAxisRaw("Vertical") < 0)
		{
			//上に傾いている
			Debug.Log("L TOP");
		}
		else if (0 < Input.GetAxisRaw("Vertical"))
		{
			//下に傾いている
			Debug.Log("L UNDER");
		}
		else
		{
			//上下方向には傾いていない
		}
		if (Input.GetAxisRaw("Horizontal") < 0)
		{
			//左に傾いている
			Debug.Log("L LEFT");
		}
		else if (0 < Input.GetAxisRaw("Horizontal"))
		{
			//右に傾いている
			Debug.Log("L RIGHT");
		}
		else
		{
			//左右方向には傾いていない
		}

		//右スティック（追加）
		if (Input.GetAxisRaw("Vertical2") < 0)
		{
			//上に傾いている
			Debug.Log("R TOP");
		}
		else if (0 < Input.GetAxisRaw("Vertical2"))
		{
			//下に傾いている
			Debug.Log("R UNDER");
		}
		else
		{
			//上下方向には傾いていない
		}
		if (Input.GetAxisRaw("Horizontal2") < 0)
		{
			//左に傾いている
			Debug.Log("R LEFT");
		}
		else if (0 < Input.GetAxisRaw("Horizontal2"))
		{
			//右に傾いている
			Debug.Log("R RIGHT");
		}
		else
		{
			//左右方向には傾いていない

		}
	}
}
