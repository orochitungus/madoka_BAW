using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControlBaseQuest : MonoBehaviour 
{
	/// <summary>
	/// 本体を追従するカメラ
	/// </summary>
	public Camera MainCamera;

	/// <summary>
	/// 地面設置確認用のレイの発射位置(コライダの中心とオブジェクトの中心)
	/// </summary>
	public Vector3 LayOriginOffs;

	/// <summary>
	/// レイの長さ
	/// </summary>
	public float Laylength;

	/// <summary>
	/// ground属性をもったレイヤー（この場合8）
	/// </summary>
	protected int LayMask;

	/// <summary>
	/// このオブジェクトのRigidbody
	/// </summary>
	protected Rigidbody RigidBody;

	/// <summary>
	/// 頭部オブジェクト
	/// </summary>
	public GameObject HeadObject;

	/// <summary>
	/// 胸部オブジェクト
	/// </summary>
	public GameObject BrestObject;

	/// <summary>
	/// ポーズコントローラー
	/// </summary>
	private GameObject Pausecontroller;

	/// <summary>
	/// ジャンプ時間
	/// </summary>
	public float JumpWaitTime;
	public float JumpTime;

	/// <summary>
	/// 着地硬直
	/// </summary>
	public float LandingWaitTime;
	public float LandingTime;

	// 移動用ステート
	/// <summary>
	/// 移動方向
	/// </summary>
	public Vector3 MoveDirection;

	/// <summary>
	/// 移動速度（歩行の場合）
	/// </summary>
	public float WalkSpeed;

	/// <summary>
	/// 移動速度（走行の場合）
	/// </summary>         
	public float RunSpeed;

	/// <summary>
	/// 移動速度（空中ダッシュの場合）
	/// </summary>
	public float AirDashSpeed;

	/// <summary>
	/// 移動速度（空中慣性移動の場合）
	/// </summary>
	public float AirMoveSpeed;

	/// <summary>
	/// 上昇速度
	/// </summary>
	public float RiseSpeed;

	/// <summary>
	/// 地上にいるか否か
	/// </summary>
	public bool IsGrounded;

	// 入力関係

	/// <summary>
	/// テンキー入力があったか否か
	/// </summary>
	protected bool HasVHInput;

	/// <summary>
	/// 射撃入力があったか否か
	/// </summary>
	protected bool HasShotInput;

	/// <summary>
	/// ジャンプ入力があったか否か
	/// </summary>
	protected bool HasJumpInput;

	/// <summary>
	/// ポーズ入力があったか否か
	/// </summary>
	protected bool HasMenuInput;

	/// <summary>
	/// コライダの地面からの高さ
	/// </summary>
	public float ColliderHeight;

	/// <summary>
	/// 使用する足音
	/// </summary>
	private StageSetting.FootType _Foottype;

	/// <summary>
	/// 移動可能か否か
	/// </summary>
	public bool Moveable;

	/// <summary>
	/// 接地判定関連
	/// </summary>
	protected int Hitcounter;
	bool Hitcounterdone;
	const int HitcounterBias = 20;

	/// <summary>
	/// 戦闘用インターフェース
	/// </summary>
	public BattleInterfaceController Battleinterfacecontroller;

	private void Awake()
	{
		// 戦闘用インターフェースを取得する
		Battleinterfacecontroller = GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>();
		if (Battleinterfacecontroller == null)
		{
			Debug.LogError("Caution!! BattleInterfaceCanvas is Nothing!!");
		}

		// インフォメーション表示内容
		Battleinterfacecontroller.InformationText.text = ParameterManager.Instance.EntityInformation.sheets[0].list[savingparameter.story].text;
	}

	// 接地判定を行う。足元に5本(中心と前後左右)レイを落とし、そのいずれかが接触していれば接地。全部外れていれば落下
	protected bool onGround()
	{
		// 座標
		Vector3[] layStartPos = new Vector3[5];
		// カプセルコライダ
		CapsuleCollider collider = GetComponent<CapsuleCollider>();
		// 中心
		layStartPos[0] = transform.position + LayOriginOffs;
		// 左
		layStartPos[1] = new Vector3(layStartPos[0].x + collider.radius, layStartPos[0].y, layStartPos[0].z);
		// 右
		layStartPos[2] = new Vector3(layStartPos[0].x - collider.radius, layStartPos[0].y, layStartPos[0].z);
		// 前
		layStartPos[3] = new Vector3(layStartPos[0].x, layStartPos[0].y, layStartPos[0].z + collider.radius);
		// 後
		layStartPos[4] = new Vector3(layStartPos[0].x, layStartPos[0].y, layStartPos[0].z - collider.radius);
		int hitcount = 0;
		for (int i = 0; i < layStartPos.Length; i++)
		{
			if (Physics.Raycast(layStartPos[i], -Vector3.up, Laylength + 1.5f, LayMask))
			{
				hitcount++;
			}
		}
		// 1つでもヒットしたら接地とする
		if (hitcount > 0)
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// 移動キーが押されているかどうかをチェックする
	/// </summary>
	/// <returns></returns>
	protected bool GetVHInput()
	{
		// PC時のみ。CPU時は何か別の関数を用意
		if (ControllerManager.Instance.Top || ControllerManager.Instance.Left || ControllerManager.Instance.Right || ControllerManager.Instance.Under || ControllerManager.Instance.LeftUpper
		|| ControllerManager.Instance.LeftUnder || ControllerManager.Instance.RightUnder || ControllerManager.Instance.RightUpper)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// 方向キー入力（上）が押されているかをチェックする（横が入っていたらアウト）
	/// </summary>
	/// <returns></returns>
	protected bool GetFrontInput()
	{
		
		if (ControllerManager.Instance.Top)
		{
			return true;
		}
		
		return false;
	}

	/// <summary>
	/// 方向キー入力（下）が押されているかをチェックする（横が入っていたらアウト）
	/// </summary>
	/// <returns></returns>
	protected bool GetBackInput()
	{
		if (ControllerManager.Instance.Under)
		{
			return true;
		}
		
		return false;
	}

	/// <summary>
	/// 方向キー入力（左）が押されているかをチェックする（縦が入っていたらアウト）
	/// </summary>
	/// <returns></returns>
	protected bool GetLeftInput()
	{
		if (ControllerManager.Instance.Left)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 方向キー入力（右）が押されているかをチェックする（縦が入っていたらアウト）
	/// </summary>
	/// <returns></returns>
	protected bool GetRightInput()
	{
		
		if (ControllerManager.Instance.Right)
		{
			return true;
		}
		
		return false;
	}


	/// <summary>
	/// ショット入力があったか否かをチェックする
	/// </summary>
	/// <returns></returns>
	protected bool GetShotInput()
	{
		
		if (ControllerManager.Instance.Shot)
		{
			return true;
		}
		
		return false;
	}

	/// <summary>
	/// メニュー入力があったか否か
	/// </summary>
	/// <returns></returns>
	protected bool GetMenuInput()
	{
		if (ControllerManager.Instance.Menu)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// ジャンプ入力があったか否かをチェックする 
	/// </summary>
	/// <returns></returns>
	protected bool GetJumpInput()
	{
		if (ControllerManager.Instance.Jump)
		{
			return true;
		}		
		return false;
	}

	/// <summary>
	/// ワールド座標でのカメラの基底ベクトルを計算し、それを基にキャラクターの回転を計算する
	/// </summary>
	protected void UpdateRotation()
	{
		var finalRot = transform.rotation;
		float horizontal = 0;        // 横入力を検出
		float vertical = 0;            // 縦入力を検出
									   // プレイヤー時
		
		if (ControllerManager.Instance.Top)
		{
			vertical = 1.0f;
		}
		else if (ControllerManager.Instance.Under)
		{
			vertical = -1.0f;
		}
		else if (ControllerManager.Instance.LeftUpper || ControllerManager.Instance.RightUpper)
		{
			vertical = 0.6f;
		}
		else if (ControllerManager.Instance.LeftUnder || ControllerManager.Instance.RightUnder)
		{
			vertical = -0.6f;
		}

		if (ControllerManager.Instance.Left)
		{
			horizontal = -1.0f;
		}
		else if (ControllerManager.Instance.Right)
		{
			horizontal = 1.0f;
		}
		else if (ControllerManager.Instance.LeftUpper || ControllerManager.Instance.LeftUnder)
		{
			horizontal = -0.6f;
		}
		else if (ControllerManager.Instance.RightUpper || ControllerManager.Instance.RightUnder)
		{
			horizontal = 0.6f;
		}
		

		var toWorldVector = MainCamera.transform.rotation;
		// ベクトルは平行移動の影響を受けないので逆行列は使わない
		// スケールの影響は受けるがここでは無視する。
		
		// 横入力時
		if (0.0f != horizontal)
		{
			// ワールド座標でのカメラの横ベクトルを計算
			var wRight = toWorldVector * Vector3.right;
			wRight.y = 0.0f;
			wRight.Normalize();
			if (0.0f > horizontal)
			{
				// ネガティブ側が押されているので反転
				wRight = -wRight;
			}
			finalRot = Quaternion.LookRotation(wRight);
		}

		// 縦入力時
		if (0.0f != vertical)
		{
			if (0.0f != horizontal)
			{
				// 横移動をすでに行っている場合 45°回転する
				var q = Quaternion.AngleAxis(-45 * vertical * horizontal, Vector3.up);
				finalRot = q * finalRot;
			}
			else
			{
				// ワールド座標でのカメラの視線ベクトル
				var wForward = toWorldVector * Vector3.forward;
				wForward.y = 0;
				wForward.Normalize();

				if (0.0f > vertical)
				{
					wForward = -wForward;
				}

				finalRot = Quaternion.LookRotation(wForward);
			}
		}
		//Debug.Log("Rotation:" + finalRot);

		transform.rotation = finalRot;

	}

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="animator"></param>
	protected virtual void FirstSetting(Animator animator)
	{
		// Rigidbodyを取得
		RigidBody = GetComponent<Rigidbody>();
				
		// 変数の初期設定はキャラごとに行う(アニメーションファイルの名前はここで入力。オーバーライドした場合継承元の内容も同時実行できたはず？）        

		// 設置関係を初期化
		var collider = GetComponent<CapsuleCollider>();
		if (null == collider)
		{
			Debug.LogError("カプセルコライダが見つからない");
			Application.Quit();
		}
		LayOriginOffs = new Vector3(0.0f, ColliderHeight, 0.0f);
		Laylength = collider.radius + collider.height;// / 2 + 1.5f;//0.2f;
													  //this.m_layOriginOffs = new Vector3(0.0f, m_Collider_Height, 0.0f);
													  //this.m_laylength = m_charactercontroller.radius + m_charactercontroller.height / 2 + 1.5f;
		LayMask = 1 << 8;       // layMaskは無視するレイヤーを指定する。8のgroundレイヤー以外を無視する


		// ジャンプ硬直
		JumpTime = -JumpWaitTime;

		// 着地硬直
		LandingTime = -LandingWaitTime;
		
		// 初期アニメIdleを再生する
		animator.SetTrigger("Idle");
		

		// 上体を初期化する
		BrestObject.transform.rotation = Quaternion.Euler(0, 0, 0);

		// 移動可能にする
		Moveable = true;

		// テンキー入力があったか否か
		HasVHInput = false;
		

		// m_DownRebirthTime/waitの初期化とカウントを行う
		// m_DamagedTime/waitの初期化とカウントを行う
		// m_DownTime/waitの初期化とカウントを行う

		
		// ポーズコントローラー取得
		Pausecontroller = GameObject.Find("PauseManager");

		// 足音取得(StageSettingで決めているので、その値を拾う
		GameObject stagesetting = GameObject.Find("StageSetting");
		if (stagesetting != null)
		{
			var ss = stagesetting.GetComponent<StageSetting>();
			int ft = ss.getFootType();
			if (ft == 0)
			{
				_Foottype = StageSetting.FootType.FootType_Normal;
			}
			else if (ft == 1)
			{
				_Foottype = StageSetting.FootType.FootType_Wood;
			}
			else if (ft == 2)
			{
				_Foottype = StageSetting.FootType.FootType_Jari;
			}
			else if (ft == 3)
			{
				_Foottype = StageSetting.FootType.FootType_Snow;
			}
			else if (ft == 4)
			{
				_Foottype = StageSetting.FootType.FootType_Carpet;
			}
		}

	}

	/// <summary>
	///  継承先のUpdate開幕で実行すること
	/// また、移動不能状態の場合、falseを返す
	/// </summary>
	/// <param name="animator">このオブジェクトのアニメーター</param>
	protected bool Update_Core(Animator animator, int idleid, int runid)
	{
		// 接地判定
		IsGrounded = onGround();

		if(!Moveable)
		{
			return false;
		}

		// 入力取得
		// 方向キー取得
		HasVHInput = GetVHInput();
		// ショット入力があったか否か
		HasShotInput = GetShotInput();
		// ジャンプ入力があったか否か
		HasJumpInput = GetJumpInput();
		// メニュー入力があったか否か
		HasMenuInput = GetMenuInput();

		// 位置を保持
		savingparameter.nowposition = this.transform.position;
		// 角度を保持
		savingparameter.nowrotation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

		// MoveDirection は アニメーションステート Walk で設定され、アニメーションステートが Idle の場合リセットされる。
		// 移動処理はステートの影響を受けないように UpdateAnimation のステートスイッチの前に処理する。
		// ステートごとに処理をする場合、それをメソッド化してそれぞれのステートに置く。
		// 走行速度を変更する、アニメーションステートが Run だった場合 RunSpeed を使う。
		var MoveSpeed = RunSpeed;

		if (RigidBody != null)
		{
			Vector3 velocity = MoveDirection * MoveSpeed;
			//走行中 / 吹き飛び中 / ダウン中
			if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == runid)
			{
				velocity.y = MadokaDefine.FALLSPEED;      // ある程度下方向へのベクトルをかけておかないと、スロープ中に落ちる
			}
			// アイドル時は下方向ベクトル止める
			else if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == idleid)
			{
				velocity = Vector3.zero;
			}
			RigidBody.velocity = velocity;
		}
		// 時間停止を解除したら動き出す
		return true;
	}
		
	/// <summary>
	/// 着地共通動作
	/// <param name="animator">本体のanimator</param>
	/// </summary>
	protected void LandingDone(Animator animator)
	{
		// ずれた本体角度を戻す(Yはそのまま）
		transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
		// 無効になっていたら重力を復活させる
		GetComponent<Rigidbody>().useGravity = true;
		animator.SetTrigger("Landing");
		// 着地したので硬直を設定する
		LandingTime = Time.time;
	}

	/// <summary>
	/// Idle時共通操作
	/// </summary>
	protected virtual void Animation_Idle(Animator animator)
	{
		if (IsGrounded)
		{
			// 方向キーで走行
			if (HasVHInput)
			{
				animator.SetTrigger("Run");
			}
		}
	}

	/// <summary>
	/// Run時共通動作
	/// </summary>
	/// <param name="animator"></param>
	protected virtual void Animation_Run(Animator animator)
	{
		RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		// ずれた本体角度を戻す(Yはそのまま）
		transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));

		// 接地中かどうか
		if (IsGrounded)
		{
			// 入力中はそちらへ進む
			if (HasVHInput)
			{
				FootSteps();
				UpdateRotation();
				MoveDirection = transform.rotation * Vector3.forward;
			}
			// 入力が外れるとアイドル状態へ
			else
			{
				animator.SetTrigger("Idle");
				MoveDirection = Vector3.zero;
			}
		}
	}

	/// <summary>
	/// Jump時共通動作 
	/// </summary>
	/// <param name="animator"></param>
	protected virtual void Animation_Jump(Animator animator)
	{
		transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		// ずれた本体角度を戻す(Yはそのまま）
		transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
		// ジャンプしたので硬直を設定する
		JumpTime = Time.time;
	}

	/// <summary>
	/// Fall時共通動作
	/// </summary>
	/// <param name="animator"></param>
	protected virtual void Animation_Fall(Animator animator)
	{
		// 方向キー入力で慣性移動
		if (HasVHInput)
		{
			UpdateRotation();
			MoveDirection = transform.rotation * Vector3.forward;
		}

		// 落下速度調整
		MoveDirection.y = MadokaDefine.FALLSPEED / 2;

		// 着地時に着陸へ移行
		if (IsGrounded)
		{
			MoveDirection = Vector3.zero;
			LandingDone(animator);
		}
	}

	/// <summary>
	/// 着地時に実行
	/// </summary>
	/// <param name="animator"></param>
	protected virtual void Animation_Landing(Animator animator)
	{
		transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		// ずれた本体角度を戻す(Yはそのまま）
		transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
		
		MoveDirection = transform.rotation * new Vector3(0, 0, 0);
		// モーション終了時にアイドルへ移行
		// 硬直時間が終わるとIdleへ戻る。オバヒ着地とかやりたいならBoost0でLandingTimeの値を変えるとか
		if (Time.time > LandingTime + LandingWaitTime)
		{
			animator.SetTrigger("Idle");
		}
	}

	// 地上走行中は足音を鳴らす
	private const float _WalkTime = 0.5f;
	private float _WalkTimer;

	private void FootSteps()
	{
		if (_WalkTimer > 0)
		{
			_WalkTimer -= Time.deltaTime;
		}
		if (_WalkTimer <= 0)
		{
			switch (_Foottype)
			{
				case StageSetting.FootType.FootType_Normal:
					AudioManager.Instance.PlaySE("ashioto_normal");
					break;
				case StageSetting.FootType.FootType_Jari:
					AudioManager.Instance.PlaySE("ashioto_jari");
					break;
				case StageSetting.FootType.FootType_Carpet:
					AudioManager.Instance.PlaySE("ashiot_carpet");
					break;
				case StageSetting.FootType.FootType_Snow:
					AudioManager.Instance.PlaySE("ashioto_snow");
					break;
				case StageSetting.FootType.FootType_Wood:
					AudioManager.Instance.PlaySE("ashioto_wood");
					break;
				default:
					break;

			}
			_WalkTimer = _WalkTime;
		}
	}



	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
