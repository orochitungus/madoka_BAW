using UnityEngine;
using System.Collections;
using System;
using UniRx;
using UniRx.Triggers;
using Utage;

public class NPCContol : MonoBehaviour 
{
	/// <summary>
	/// 会話時のアニメーションのトリガーの名前
	/// </summary>
	public string TalkAnimationName;

	/// <summary>
	/// 通常時のアニメーションのトリガーの名前
	/// </summary>
	public string NormalAnimationName;

	/// <summary>
	/// 呼び出すシナリオの名前
	/// </summary>
	public string TalkName;

	/// <summary>
	/// 本体を動かすAnimator
	/// </summary>
	public Animator AnimatorUnit;

	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
	public AdvEngine engine;

	/// <summary>
	/// 宴の表示部分を呼び出す
	/// </summary>
	public AdvUguiManager2 Advuguimanager2;


	// Use this for initialization
	void Start () 
	{
		AnimatorUnit.Play(NormalAnimationName);
	}
	
	// Update is called once per frame
	void Update () 
	{
		// ショットキーの入力受け取り
		if (ControllerManager.Instance.Shot)
		{
			Advuguimanager2.OnInput();
		}
	}

	protected void OnCollisionEnter(Collision collision)
	{
		// 本体をプレイヤーキャラの方向に向ける
		// プレイヤーキャラの座標
		Vector3 playerPos = collision.transform.position;
		// 自分の位置
		Vector3 mypos = this.transform.position;
		// 方向を算出
		Quaternion looklot = Quaternion.LookRotation(playerPos - mypos);
		// 方向を変える
		this.transform.rotation = looklot;
		// アニメーションをさせる
		AnimatorUnit.Play(TalkAnimationName);
		// プレイヤーキャラクターの移動を封じる
		collision.transform.GetComponent<CharacterControlBaseQuest>().Moveable = false;
		// プレイヤーキャラクターのアニメを止める
		Animator animator = collision.transform.GetComponent<Animator>();
		animator.SetTrigger("Idle");
		// 会話シーンを呼び出す
		StartCoroutine(CoTalk(collision));
	}

	private IEnumerator CoTalk(Collision collision)
	{
		// 「宴」のシナリオを呼び出す
		Engine.JumpScenario(TalkName);

		// 「宴」のシナリオ終了待ち
		while (!Engine.IsEndScenario)
		{
			yield return 0;
		}
		// キャラクターの移動を復活させる
		collision.transform.GetComponent<CharacterControlBaseQuest>().Moveable = true;
		// ポーズを戻す
		AnimatorUnit.Play(NormalAnimationName);
	}
}
