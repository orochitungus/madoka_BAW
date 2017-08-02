using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SconosciutoCPU : AIControlBase
{

    // 上昇限界高度
    private float RiseLimit = 75.0f;
       

    // Use this for initialization
    void Start ()
    {
        Initialize();
        // 上昇する時間
        Risetime = 0.1f;
        // 近接レンジ
        FightRange = 2.0f;
        // 近接レンジ（前後特殊格闘を使って上下するか）
        FightRangeY = 2.5f;
        
    }

    // Update is called once per frame
    void Update ()
    {
        if (IsPlayer != CharacterControlBase.CHARACTERCODE.PLAYER)
        {
            UpdateCore();
        }
    }

    /// <summary>
    /// 接触したときの行動
    /// </summary>
    /// <param name="keyoutput"></param>
    /// <returns></returns>
    protected override bool Engauge(ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        // ロックオン距離内にいる
        // 
        if (RockonTarget != null && Vector3.Distance(target.transform.position, RockonTarget.transform.position) <= target.RockonRange)
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
            if (DistanceXZ <= FightRange)
            {
                keyoutput = KEY_OUTPUT.NONE;
                // 地上にいるなら通常格闘開始
                if (target.IsGrounded)
                {
                    Cpumode = CPUMODE.DOGFIGHT_STANDBY;
                    return true;
                }
                // 高低差が一定以上なら特殊格闘で移動攻撃
                else if (FightRangeY <= DistanceY)
                {
                    // 自分が高ければ後特殊格闘(降下攻撃）
                    if (target.transform.position.y >= RockonTarget.transform.position.y)
                    {
                        Cpumode = CPUMODE.DOGFIGHT_DOWNER;
                        return true;
                    }
                    // 自分が低ければ前特殊格闘（上昇攻撃）
                    else
                    {
                        Cpumode = CPUMODE.DOGFIGHT_UPPER;
                        return true;
                    }
                }
            }
            // そうでなければ空中におり、敵との間に遮蔽物がなければ射撃攻撃（現行、地上にいると射撃のループをしてしまう）
            else if (!target.IsGrounded)
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
                var targetState = ControlTarget.GetComponent<SconosciutoControl>();

				

				// どの射撃をするか決める
				float attacktype = Random.value;// 攻撃手段(0-0.2:覚醒技、0.2-0.6(通常射撃）、0.6-0.8(サブ射撃）、0.8-1.0（特殊射撃）、ゲージなし(BD格闘)
				
				// 覚醒技
				if (0.0f <= attacktype && attacktype < 0.2f && target.IsArousal)
				{
					Cpumode = CPUMODE.AROUSAL;
					keyoutput = KEY_OUTPUT.AROUSAL;
					return true;
				}
				// 通常射撃
				else if (0.2f <= attacktype && attacktype < 0.6f && targetState.BulletNum[0] > 0)
				{
					Cpumode = CPUMODE.FIREFIGHT;
					keyoutput = KEY_OUTPUT.SHOT;
					return true;
				}
				// サブ射撃
				else if (0.6f <= attacktype && attacktype < 0.75f && targetState.BulletNum[1] > 0)
				{
					Cpumode = CPUMODE.FIREFIGHT;
					keyoutput = KEY_OUTPUT.SUBSHOT;
					return true;
				}
				// 特殊射撃
				else if (0.75f <= attacktype && attacktype <= 1.0f && targetState.BulletNum[2] > 0)
				{
					Cpumode = CPUMODE.FIREFIGHT;
					keyoutput = KEY_OUTPUT.EXSHOT;
					return true;
				}
				// BD格闘
				else
				{
					Cpumode = CPUMODE.DOGFIGHT_EX;
					keyoutput = KEY_OUTPUT.WRESTLE;
					return true;
				}
            }
        }
        return false;
    }

	protected override void Firefight(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
	{
		// 制御対象
		var target = ControlTarget.GetComponent<MajyuControl>();
		keyoutput = KEY_OUTPUT.NONE;
		// 地上にいた場合（→再度飛行）
		if (target.IsGrounded)
		{
			keyoutput = KEY_OUTPUT.JUMP;
			Cpumode = CPUMODE.NORMAL_RISE1;
			tenkeyoutput = TENKEY_OUTPUT.TOP;
			Totalrisetime = Time.time;
		}
		// 飛び越えられる壁に接触していた場合（→NORMAL_RISE3へ）
		else if (target.Gethitjumpover())
		{
			// 一旦ジャンプボタンとテンキーを放す
			tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
			keyoutput = KEY_OUTPUT.NONE;
			Cpumode = CPUMODE.NORMAL_FALL;
		}
		// 飛び越えられない壁に接触していた場合（→
		// 空中にいた場合（→再度ダッシュしてビームずんだ）
		else if (target.Gethitunjumpover())
		{
			// レーダー左とロックオン対象の距離を求める
			float distL = Vector3.Distance(SearcherL.transform.position, RockonTarget.transform.position);
			// レーダー右とロックオン対象の距離を求める
			float distR = Vector3.Distance(SearcherR.transform.position, RockonTarget.transform.position);
			// 左の方が大きければ左上方向へ飛行開始
			if (distL >= distR)
			{
				tenkeyoutput = TENKEY_OUTPUT.TOPLEFT;
			}
			// 右の方が大きければ右上方向へ飛行開始
			else
			{
				tenkeyoutput = TENKEY_OUTPUT.TOPRIGHT;
			}
		}
		// 射撃のフォロースルーに入ったら再度空中ダッシュさせる
		else if (target.AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == target.FollowThrowShotID || target.AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == target.FollowThrowAirShotID)
		{
			tenkeyoutput = TENKEY_OUTPUT.TOP;
			keyoutput = KEY_OUTPUT.BOOSTDASH;
			Cpumode = CPUMODE.BOOSTDASH;
		}
		// それ以外の時は何もしない
		else
		{
			tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
			keyoutput = KEY_OUTPUT.NONE;
		}
	}

	/// <summary>
	/// 通常状態になったときの処理
	/// </summary>
	/// <param name="tenkeyoutput"></param>
	/// <param name="keyoutput"></param>
	protected override void Normal(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
	{
		base.Normal(ref tenkeyoutput, ref keyoutput);
		// ロックオン対象情報を取得
		CharacterControlBase rockonTarget = RockonTarget.GetComponent<CharacterControlBase>();
		// 制御対象
		CharacterControlBase target = ControlTarget.GetComponent<CharacterControlBase>();

		// 相手がダウンしていた場合(回復中含む)
		if (rockonTarget != null && rockonTarget.NowDownRatio >= rockonTarget.DownRatioBias)
		{
			// ロックオン対象が二人以上いた場合、ロックを切り替える
			// ロックオン対象が誰もいなかった場合、哨戒に戻す
			keyoutput = KEY_OUTPUT.SEARCH;
			ReturnPatrol(target);
		}
	}

	protected override void Normal_rise1(ref KEY_OUTPUT keyoutput)
	{
		// 何らかの理由で哨戒起点か終点をロックしたまま攻撃体制に入った場合は元に戻す
		if (UnRockAndReturnPatrol())
			return;

		// 制御対象
		var target = ControlTarget.GetComponent<MajyuControl>();

		// 地上から離れて一定時間後空中ダッシュさせる
		if (Time.time > Totalrisetime + Risetime && !target.IsGrounded)
		{
			keyoutput = KEY_OUTPUT.BOOSTDASH;
			Cpumode = CPUMODE.BOOSTDASH;
			Totalrisetime = Time.time;
		}
		else
		{
			keyoutput = KEY_OUTPUT.JUMPING;
		}
		// 地上から離れずに一定時間いるとNORMALへ戻って仕切り直す
		if (Time.time > Totalrisetime + Risetime && target.IsGrounded)
		{
			Cpumode = CPUMODE.NORMAL;
		}
		// 敵との距離が離れすぎるとロックオンを解除して哨戒に戻る
		// カメラ
		Player_Camera_Controller pcc = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
		float distance = Vector3.Distance(pcc.Player.transform.position, pcc.Enemy.transform.position);
		if ((int)Latecpumode > (int)CPUMODE.RETURN_PATH && distance > target.RockonRangeLimit)
		{
			ReturnPatrol(target);
		}
	}
}
}
