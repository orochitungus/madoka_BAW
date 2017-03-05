using UnityEngine;
using System.Collections;

public class HomuraBowSubShotL : Bullet
{

	// Use this for initialization
	void Start () 
	{
		// 維持時間を初期化（後で調整し、赤ロック範囲内を飛行中は生き残らせること）
		LifeTime = 6.0f;
		MoveDirection = Vector3.zero;
		// 独立フラグを初期化
		IsIndependence = false;
		// 誘導カウンターを初期化
		InductionCounter = 0;
		// 誘導時間を初期化
		InductionBias = 30;
		InjectionObjectName = "HomuraBowBattleUse";
		// 親オブジェクトを取得(rootでツリーの一番上から検索をかける）        
		InjectionObject = transform.root.GetComponentInChildren<HomuraBowControl>().gameObject;
		// 接触対象に自分は除く（散弾や爆風は発生時に別のオブジェクトを呼ぶ）
		Physics.IgnoreCollision(transform.GetComponent<Collider>(), InjectionObject.transform.GetComponent<Collider>());
		// 撃ったキャラが誰であるか保持
		InjectionCharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float shotspeed = 0.0f;
		// 飛行開始
		// キリカの時間遅延を受けているとき、1/4に
		if (Timestopmode == CharacterControlBase.TimeStopMode.TIME_DELAY)
		{
			shotspeed = DelayShotspeed;
		}
		// ほむらの時間停止を受けているときなど、0に
		else if (Timestopmode == CharacterControlBase.TimeStopMode.TIME_STOP || Timestopmode == CharacterControlBase.TimeStopMode.PAUSE || Timestopmode == CharacterControlBase.TimeStopMode.AROUSAL)
		{
			return;
		}
		// 通常
		else
		{
			shotspeed = ShotSpeed;
		}

		// カメラから親と対象を拾う
		// (ほむの下にカメラがいるので、GetComponentChildlenで拾える）
		//var target = m_Obj_OR.GetComponentInChildren<Player_Camera_Controller>();
		// 敵の場合、弾丸があっても親が死んでいる可能性もあるので、その場合は強制抜け
		if (InjectionObject == null)
		{
			BrokenMySelf();
			return;
		}
		var target = InjectionObject.transform.GetComponentInChildren<Player_Camera_Controller>();
		// 親オブジェクトを拾う
		InjectionObject = target.Player;
		// ターゲットオブジェクトを拾う
		this.TargetObject = target.Enemy;
		// 親オブジェクトから切り離されたときかつ非独立状態なら、親が保持している方向ベクトルを拾う
		// 非独立状態の判定をしないと、射出後も親に合わせて動いてしまう
		// 親オブジェクトを拾う
		if (transform.parent != null && !IsIndependence)
		{
			// 親のステートがSHOTになったら、親子関係を切り離す(先にセンターがSHOTになるのでSHOTDONEにしておく）
			if (InjectionObject != null && InjectionObject.GetComponent<CharacterControlBase>().Shotmode == CharacterControlBase.ShotMode.SHOTDONE)
			{
				// その時の座標を保持する
				GetComponent<Rigidbody>().position = InjectionObject.GetComponent<HomuraBowControl>().SubShotRootL.GetComponent<Rigidbody>().position;
				// 進行方向ベクトルを保持する
				MoveDirection = InjectionObject.GetComponent<HomuraBowControl>().BulletMoveDirectionL;
				// 攻撃力を保持する
				OffemsivePower = InjectionObject.GetComponent<CharacterControlBase>().OffensivePowerOfBullet;
				// ダウン値を保持する
				DownRatio = InjectionObject.GetComponent<CharacterControlBase>().DownratioPowerOfBullet;
				// 覚醒ゲージ増加量を保持する
				ArousalRatio = InjectionObject.GetComponent<CharacterControlBase>().ArousalRatioOfBullet;

				// 親のステートを発射完了へ切り替える
				InjectionObject.GetComponent<CharacterControlBase>().Shotmode = CharacterControlBase.ShotMode.SHOTDONE;
				// 切り離し
				transform.parent = null;
				// 独立フラグを立てる
				IsIndependence = true;
				// IsKinematicを折る
				transform.GetComponent<Rigidbody>().isKinematic = false;
				{
					{
						Vector3 Correction_Rot = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
						transform.rotation = Quaternion.Euler(Correction_Rot);
					}
					// 誘導がないなら進行方向は固定
					if (TargetObject == null)
					{
						InductionCounter = this.InductionBias + 1; // 下でカウンターが閾値を超えたら固定されるので
					}
				}
			}
		}
		// 規定フレーム間誘導する
		InductionBullet(shotspeed);

		// レイキャストで接触したオブジェクト
		RaycastHit hit;

		// 進行方向に対してレイキャストする
		if (Physics.Raycast(transform.position, Vector3.forward, out hit, 300.0f))
		{
			var hittarget = hit.collider.gameObject.GetComponent<AIControlBase>();
			var hittarget2 = hit.collider.gameObject.GetComponent<CharacterControlBase>();
			// ガード姿勢を取らせる(自分の弾は除く)
			if (hittarget != null && InjectionCharacterIndex != (int)hittarget2.CharacterName)
			{
				// 相手がNOMOVEの時にはガードは取らせない
				if (hittarget.Cpumode != AIControlBase.CPUMODE.NOMOVE)
				{
					hittarget.Cpumode = AIControlBase.CPUMODE.GUARD;
				}
			}
		}


		BrokenMySelf();
		TimeNow++;
	}
}
