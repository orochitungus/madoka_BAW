using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

/// <summary>
/// NPCのAnimationを使う版。アセットストアから落としたものはAnimatorに対応していない場合があるので、こっちを使うこと
/// </summary>
public class NPCControlRegacy : MonoBehaviour 
{
	/// <summary>
	/// 通常時のAnimation
	/// </summary>
	public AnimationClip IdleAnimation;

	/// <summary>
	/// 会話時のAnimation
	/// </summary>
	public AnimationClip TalkAnimation;

	/// <summary>
	/// 本体を動かすAnimation
	/// </summary>
	public Animation AnimationUnit;

	/// <summary>
	/// 呼び出すシナリオの名前
	/// </summary>
	public string TalkName;

	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
	public AdvEngine engine;

	/// <summary>
	/// 宴の表示部分を呼び出す
	/// </summary>
	public AdvUguiManager2 Advuguimanager2;

	/// <summary>
	/// ストーリーが進行すると消す場合の最低Story
	/// </summary>
	public int BrokenLow;

	/// <summary>
	/// ストーリーが進行すると消す場合の最大Story
	/// </summary>
	public int BrokenHigh;

	// Use this for initialization
	void Start () 
	{
		// クリップを決定
		AnimationUnit.clip = IdleAnimation;
		AnimationUnit.Play();
		// 消す条件を満たしている場合消す
		if (savingparameter.story >= BrokenLow && savingparameter.story <= BrokenHigh)
		{
			Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// ショットキーの入力受け取り
		if (ControllerManager.Instance.Shot && Advuguimanager2 != null)
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
		AnimationUnit.clip = TalkAnimation;
		AnimationUnit.Play();
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
		AnimationUnit.clip = IdleAnimation;
		AnimationUnit.Play();
	}
}
