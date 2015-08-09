using UnityEngine;
using System.Collections;

/// <summary>
/// CPUによるキャラクター操作を行う（魔獣用）
/// </summary>
public class AIControl_Majyu : AIControl_Base
{
	// 飛び道具発射フラグ（魔獣は1発撃ったら終了でズンダさせない）
	private bool Shoted;
	// 飛び道具発射インターバル時間
	private float ShotIntervalTime = 5.0f;
	// 上昇限界高度
	private float RiseLimit = 10.0f;

	// Use this for initialization
	void Start () 
	{
		Initialize();
		// 上昇する時間
		m_risetime = 0.1f;
		// 近接レンジ
		m_fightRange = 5.0f;
		// 近接レンジ（前後特殊格闘を使って上下するか）
		m_fightRangeY = 2.5f;
		// 飛び道具発射フラグ
		Shoted = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateCore();
	}

	/// <summary>
	/// 接触したときの行動
	/// </summary>
	/// <param name="keyoutput"></param>
	/// <returns></returns>
	protected override bool engauge(ref AIControl_Base.KEY_OUTPUT keyoutput)
	{
		// 制御対象
		var target = ControlTarget.GetComponent<CharacterControl_Base>();
		// ロックオン距離内にいる
		if (RockonTarget != null && Vector3.Distance(target.transform.position, RockonTarget.transform.position) <= target.m_Rockon_Range)
		{
			// 頭上をとったか取られたかした場合、前後特殊格闘（DOGFIGHT_AIR）を行ってもらう
			// 自機のXZ座標
			Vector2 nowposXZ = new Vector2(target.transform.position.x, target.transform.position.z);
			// ロックオン対象のXZ座標
			Vector2 RockonXZ = new Vector2(RockonTarget.transform.position.x, RockonTarget.transform.position.z);
			// XZ距離算出
			float DistanceXZ = Vector2.Distance(nowposXZ, RockonXZ);
			// 高低差算出
			float DistanceY = target.transform.position.y - RockonTarget.transform.position.y;
			// 格闘レンジに入ったか
			if (DistanceXZ <= m_fightRange)
			{
				keyoutput = KEY_OUTPUT.NONE;
				// 高低差が一定以上か
				if (m_fightRangeY <= DistanceY)
				{
					// 自分が高ければ後特殊格闘(降下攻撃）
					if (target.transform.position.y >= RockonTarget.transform.position.y)
					{
						m_cpumode = CPUMODE.DOGFIGHT_DOWNER;
						return true;
					}
					// 自分が低ければ前特殊格闘（上昇攻撃）
					else
					{
						m_cpumode = CPUMODE.DOGFIGHT_UPPER;
						return true;
					}
				}
				// 高低差がないならば通常格闘へ移行
				else
				{
					m_cpumode = CPUMODE.DOGFIGHT_STANDBY;
					return true;
				}
			}
			// そうでなければ空中におり、敵との間に遮蔽物がなければ射撃攻撃（現行、地上にいると射撃のループをしてしまう）
			else if (!target.GetInGround())
			{
				// 前面に向かってレイキャストする
				RaycastHit hit;
				Vector3 RayStartPosition = new Vector3(target.transform.position.x, target.transform.position.y + 1.5f, target.transform.position.z);
				if (Physics.Raycast(RayStartPosition, transform.forward, out hit, 230.0f))
				{
					// 衝突した「何か」が障害物であったら測距する
					if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ground"))
					{
						// 障害物との距離が敵との距離より近ければ、falseを返す
						// 敵との距離
						float EnemyDistance = Vector3.Distance(target.transform.position, RockonTarget.transform.position);
						// 障害物との距離
						float hitDistance = Vector3.Distance(target.transform.position, hit.transform.position);
						if (hitDistance < EnemyDistance)
						{
							return false;
						}
					}
				}
				// 残弾数がなく、弾を撃ってから規定時間はなにもしない
				var targetState = ControlTarget.GetComponent<majyu_BattleControl>();
				if (targetState.m_BulletNum[0] > 0 && !Shoted)
				{
					m_cpumode = CPUMODE.FIREFIGHT;
					keyoutput = KEY_OUTPUT.SHOT;
					Shoted = true;
					StartCoroutine(ShotInterval());
					return true;
				}
				//else
				//{
				//	m_cpumode = CPUMODE.NORMAL;
				//	keyoutput = KEY_OUTPUT.NONE;
				//	return false;
				//}
			}
		}
		return false;
	}

	/// <summary>
	/// 通常状態になったときの処理
	/// </summary>
	/// <param name="tenkeyoutput"></param>
	/// <param name="keyoutput"></param>
	protected override void normal(ref AIControl_Base.TENKEY_OUTPUT tenkeyoutput, ref AIControl_Base.KEY_OUTPUT keyoutput)
	{
		base.normal(ref tenkeyoutput, ref keyoutput);
		// ロックオン対象情報を取得
		CharacterControl_Base rockonTarget = RockonTarget.GetComponent<CharacterControl_Base>();
		// 制御対象
		CharacterControl_Base target = ControlTarget.GetComponent<CharacterControl_Base>();

		// 相手がダウンしていた場合(回復中含む)
		if(rockonTarget != null && rockonTarget.m_nowDownRatio >= rockonTarget.m_DownRatio)
		{ 
			// ロックオン対象が二人以上いた場合、ロックを切り替える
			// ロックオン対象が誰もいなかった場合、哨戒に戻す
			keyoutput = KEY_OUTPUT.SEARCH;
			ReturnPatrol(target);
		}
	}

	private IEnumerator ShotInterval()
	{
		yield return new WaitForSeconds(ShotIntervalTime);
		Shoted = false;
	}

	protected override void noraml_rise1(ref KEY_OUTPUT keyoutput)
	{
		// 何らかの理由で哨戒起点か終点をロックしたまま攻撃体制に入った場合は元に戻す
		if (UnRockAndReturnPatrol())
			return;

		// 制御対象
		var target = ControlTarget.GetComponent<majyu_BattleControl>();
		RaycastHit hit;
		Vector3 RayStartPosition = new Vector3(ControlTarget.transform.position.x, ControlTarget.transform.position.y + 1.5f, ControlTarget.transform.position.z);
		// 地上から離れて一定時間たったか上昇限界高度を超えていると空中ダッシュ
		if ((!Physics.Raycast(RayStartPosition, -transform.up, out hit, RiseLimit)))
		{
			keyoutput = KEY_OUTPUT.NONE;
			m_cpumode = CPUMODE.NORMAL_RISE2;
			m_totalrisetime = Time.time;
		}
		//// 地上から離れずに一定時間いるとNORMALへ戻って仕切り直す
		//if (Time.time > m_totalrisetime + m_risetime && target.GetInGround())
		//{
		//	m_cpumode = CPUMODE.NORMAL;
		//}
		// 敵との距離が離れすぎるとロックオンを解除して哨戒に戻る
		// カメラ
		Player_Camera_Controller pcc = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
		float distance = Vector3.Distance(pcc.Player.transform.position, pcc.Enemy.transform.position);
		if ((int)m_latecpumode > (int)CPUMODE.RETURN_PATH && distance > target.m_Rockon_RangeLimit)
		{
			ReturnPatrol(target);
		}
	}
}
