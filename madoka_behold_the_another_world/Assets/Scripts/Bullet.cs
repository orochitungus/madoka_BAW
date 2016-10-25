using UnityEngine;
using System.Collections;

// 飛び道具系の基底クラス
public class Bullet : MonoBehaviour 
{
    /// <summary>
    /// 射出元のゲームオブジェクト
    /// </summary>
    public GameObject InjectionObject;

    /// <summary>
    /// 弾速
    /// </summary>
    public float ShotSpeed;

    /// <summary>
    /// 移動方向
    /// </summary>
    public Vector3 MoveDirection;

    /// <summary>
    /// 属性
    /// </summary>
    public enum BulletType
    {
        BEAM,			// ビーム（実弾貫通）
        BULLET,			// 実弾
        BAZOOKA,		// 着弾時拡散する実弾
        BOMB,			// 爆風を撒き散らすボム
        LASER,			// レーザー（相殺なし）
		FUNNEL_LASER,	// ファンネル（レーザーを撃ち出す）
		FUNNEL_SWORD,	// ファンネル（本体が特攻する）
    };
    public BulletType Bullettype;

	/// <summary>
	/// ファンネルである場合のステート
	/// </summary>
	public enum FunnelState
	{
		Injcection,		// 射出
		Shoot,			// 目標地点で発射
	}

	/// <summary>
	/// ファンネル時のステート
	/// </summary>
	public FunnelState Funnnelstate;

	/// <summary>
	/// ファンネル時の目標地点
	/// </summary>
	public Vector3 FunnelInjectionTargetPos;

    /// <summary>
    /// 親のスクリプト名
    /// </summary>
    public string ParentScriptName;

    /// <summary>
    /// 維持時間
    /// </summary>
    public float LifeTime;

    /// <summary>
    /// 攻撃力
    /// </summary>
    public int OffemsivePower;

    /// <summary>
    /// ダウン値
    /// </summary>
    public float DownRatio;

    /// <summary>
    /// 覚醒ゲージ増加量
    /// </summary>
    public float ArousalRatio;

    /// <summary>
    /// 着弾時のSE
    /// </summary>
    public AudioClip HitSE;

    /// <summary>
    /// 射出元のゲームオブジェクトの名前
    /// </summary>
    public string InjectionObjectName;

    /// <summary>
    /// 射出元のキャラクターのインデックス
    /// </summary>
    public int InjectionCharacterIndex;

    /// <summary>
    /// 目標のゲームオブジェクト
    /// </summary>
    public GameObject TargetObject;

    /// <summary>
    /// 接触したゲームオブジェクト
    /// </summary>
    private GameObject HitTarget;

    /// <summary>
    /// ポーズ処理(時間のそれと同じ）
    /// </summary>
    public CharacterControlBase.TimeStopMode Timestopmode;

    /// <summary>
    /// バズーカタイプの時の衝撃波件エフェクト
    /// </summary>
    public GameObject BazookaShockWave;

    /// <summary>
    /// 誘導カウント
    /// </summary>
    public int InductionCounter;

    /// <summary>
    /// 誘導が続くフレーム数の閾値
    /// </summary>
    public int InductionBias;

    /// <summary>
    /// ヒットタイプ(ダメージの種類.弓ほむらのみインスペクターで設定する。他のキャラは各弾丸クラスでCharacter_Specの値を引っ張る形で設定する）
    /// </summary>
    protected CharacterSkill.HitType Hittype;

    /// <summary>
    /// 親オブジェクトから切り離されて独立飛行しているか否か
    /// </summary>
    public bool IsIndependence;

    /// <summary>
    /// 射出後の角度変更を行ったか否か
    /// </summary>
    public bool RotationControl = false;

    /// <summary>
    /// 弾丸の出現時間
    /// </summary>
    protected float TimeNow = 0.0f;

    /// <summary>
    /// BOMBの場合、アニメーション再生が始まっているか
    /// </summary>
    protected bool StartedBombAnimation;

    /// <summary>
    /// BOMBのアニメーションの名前
    /// </summary>
    public string BombAnimationName;

    /// <summary>
    /// 着弾時のヒットエフェクト
    /// </summary>
    public GameObject HitEffect;

	/// <summary>
	/// 時間遅延が発動した時の弾速
	/// </summary>
	protected float DelayShotspeed;

	/// <summary>
	/// ファンネルの攻撃判定
	/// </summary>
	public GameObject FunnelBeam;

 
    // Updateでの共通処理（継承用）
    protected void UpdateCore()
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
            // 親のステートがSHOTになったら、親子関係を切り離す
            if (InjectionObject != null && InjectionObject.GetComponent<CharacterControlBase>().Shotmode == CharacterControlBase.ShotMode.SHOT)
            {
                // その時の座標を保持する
                GetComponent<Rigidbody>().position = InjectionObject.GetComponent<CharacterControlBase>().MainShotRoot.GetComponent<Rigidbody>().position;
                // 進行方向ベクトルを保持する
                MoveDirection = InjectionObject.GetComponent<CharacterControlBase>().BulletMoveDirection;
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
		// 規定フレーム間誘導する(ファンネル射出中）
		if ((Bullettype == BulletType.FUNNEL_LASER || Bullettype == BulletType.FUNNEL_SWORD) && Funnnelstate == FunnelState.Injcection)
		{
			InductionFunnel(shotspeed);		
		}
		// 規定フレーム間誘導する
		else
		{ 
			InductionBullet(shotspeed);
		}
        // レイキャストで接触したオブジェクト
        RaycastHit hit;

        // 進行方向に対してレイキャストする
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 300.0f))
        {
            var hittarget = hit.collider.gameObject.GetComponent<AIControl_Base>();
            var hittarget2 = hit.collider.gameObject.GetComponent<CharacterControlBase>();
            // ガード姿勢を取らせる(自分の弾は除く)
            if (hittarget != null && InjectionCharacterIndex != (int)hittarget2.CharacterName)
            {
                // 相手がNOMOVEの時にはガードは取らせない
                if (hittarget.m_cpumode != AIControl_Base.CPUMODE.NOMOVE)
                {
                    hittarget.m_cpumode = AIControl_Base.CPUMODE.GUARD;
                }
            }
        }
             

        BrokenMySelf();
        TimeNow++;
    }

    // 規定フレーム間誘導する（弾丸）
    protected void InductionBullet(float shotspeed)
    {
        // カメラから親と対象を拾う
        var target = InjectionObject.transform.GetComponentInChildren<Player_Camera_Controller>();
        // 親オブジェクトを拾う
        InjectionObject = target.Player;
        // ターゲットオブジェクトを拾う
        TargetObject = target.Enemy;
        // 移動＆回転補正
        if (Timestopmode != CharacterControlBase.TimeStopMode.TIME_STOP || Timestopmode != CharacterControlBase.TimeStopMode.PAUSE || Timestopmode != CharacterControlBase.TimeStopMode.AROUSAL)
        {
            // 独立状態になってから行う（射出前に回すとPCに突き刺さったりと変な形になるので）
            if (IsIndependence)
            {
                // 親のステートを拾う
                var parent = target.Player.GetComponent<CharacterControlBase>();
                // ロックオン範囲にいて、かつカウンターが閾値を超えていない場合は誘導する
                if (InductionCounter < InductionBias && TargetObject != null)
                {
                    if (Vector3.Distance(InjectionObject.transform.position, TargetObject.transform.position) <= parent.RockonRange)
                    {
                        // 敵のカプセルコライダの高さ分オフセット（そうしないと下向きのベクトルになる）
                        // キャラクターコントローラーを拾う
                        CapsuleCollider offset_OR = TargetObject.GetComponentInChildren<CapsuleCollider>();
                        if (offset_OR == null)
                        {
                            return;
                        }
                        // 高さを取得する
                        float offset = offset_OR.height;
                        // 対象の高さを変更する
                        Vector3 Targetpos = TargetObject.transform.position;
                        Targetpos.y += offset;
                        // 変換したベクトルを正規化
                        MoveDirection = Vector3.Normalize(Targetpos - GetComponent<Rigidbody>().transform.position);
                        // オブジェクトを相手の方向へ向ける
                        Quaternion looklot = Quaternion.LookRotation(TargetObject.transform.position - transform.position);
                        Vector3 looklotE = looklot.eulerAngles;
                        looklot = Quaternion.Euler(looklotE.x, looklotE.y, looklotE.z);                        
                        transform.rotation = looklot;
                    }
                }
                GetComponent<Rigidbody>().position = GetComponent<Rigidbody>().position + MoveDirection * shotspeed * Time.deltaTime;
            }
        }
        // 親が死んだ場合は自壊
        if (InjectionObject == null)
        {
            Destroy(gameObject);
        }

        // カウンターを加算
        if (IsIndependence)
        {
            InductionCounter++;
        }

        // 誘導しない場合は撃った後は方向は固定
        if (InductionCounter >= InductionBias)
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

	/// <summary>
	/// 規定位置まで移動する（ファンネル）
	/// </summary>
	/// <param name="movespeed"></param>
	protected void InductionFunnel(float movespeed)
	{
		// カメラから親と対象を拾う
		var target = InjectionObject.transform.GetComponentInChildren<Player_Camera_Controller>();
		// 親オブジェクトを拾う
		InjectionObject = target.Player;
		// ターゲットオブジェクトを拾う
		TargetObject = target.Enemy;
		// 移動＆回転補正
		if (Timestopmode != CharacterControlBase.TimeStopMode.TIME_STOP || Timestopmode != CharacterControlBase.TimeStopMode.PAUSE || Timestopmode != CharacterControlBase.TimeStopMode.AROUSAL)
		{
			// 独立状態になってから行う（射出前に回すとPCに突き刺さったりと変な形になるので）
			if (IsIndependence)
			{
				// 目標地点に近接していたら展開する
				if (Vector3.Distance(FunnelInjectionTargetPos, transform.position) < 1.0f)
				{
					Funnnelstate = FunnelState.Shoot;
					FunnelBeam.SetActive(true);			// 攻撃判定をアクティブにする
				}
				else
				{
					// 移動先へのベクトルを作る
					MoveDirection = Vector3.Normalize(FunnelInjectionTargetPos - GetComponent<Rigidbody>().transform.position);
					// オブジェクトを移動先に向ける
					Quaternion looklot = Quaternion.LookRotation(FunnelInjectionTargetPos - transform.position);
					Vector3 looklotE = looklot.eulerAngles;
					looklot = Quaternion.Euler(looklotE.x, looklotE.y, looklotE.z);
					transform.rotation = looklot;
					GetComponent<Rigidbody>().position = GetComponent<Rigidbody>().position + MoveDirection * movespeed * Time.deltaTime;
				}
			}
		}
		// 親が死んだ場合は自壊
		if (InjectionObject == null)
		{
			Destroy(gameObject);
		}

		// カウンターを加算
		if (IsIndependence)
		{
			InductionCounter++;
		}

		// 誘導しない場合は撃った後は方向は固定
		if (InductionCounter >= InductionBias)
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
	} 

    // 指定時間後自分を破棄する
    protected void BrokenMySelf()
    {
        // とりあえず時間が通常のときだけ自壊させる（何かしらと接触すると壊す予定ではあるが・・・時間停止中どうするべきか・・・？）
        if (Timestopmode == CharacterControlBase.TimeStopMode.NORMAL || Timestopmode == CharacterControlBase.TimeStopMode.TIME_DELAY)
        {
            if (LifeTime*60 < TimeNow)
            {
                Destroy(gameObject);
            }
        }
    }

    // 接触時相手にダメージを与えて自壊する（射出したキャラを除く）着弾時爆風とかやりたいならこの関数をオーバーライド
    public void OnCollisionEnter(Collision collision)
    {
        string player;
        string enemy;

        // 親オブジェクトを拾う
        // 自機がPLAYERかPLAYER_ALLYの場合
        if (InjectionObject.GetComponent<CharacterControlBase>().IsPlayer != CharacterControlBase.CHARACTERCODE.ENEMY)
        {
            player = "Player";
            enemy = "Enemy";
        }
        // 自機がEnemyの場合
        else
        {
            player = "Enemy";
            enemy = "Player";
        }
        // 接触対象を取得
        var target = collision.gameObject.GetComponent<CharacterControlBase>();
        HitTarget = collision.gameObject;

		// ビーム属性同士の弾丸が干渉したときは自壊させない
		var hittargetBullet = collision.gameObject.GetComponent<Bullet>();
		if(hittargetBullet != null)
		{
			if (hittargetBullet.Bullettype == BulletType.BEAM && Bullettype == BulletType.BEAM)
			{
				return;
			}
		}

        // ゼロ距離射撃したときは親を受け取る
        if (InjectionObject == null)
        {
            // カメラから親と対象を拾う
            var parent = InjectionObject.transform.GetComponentInChildren<Player_Camera_Controller>();
            // 親オブジェクトを拾う
            InjectionObject = parent.Player;
        }

        // 接触対象のルーチンをGUARDからNORMALへ戻してルーチンをNORMALに戻す
        var targetAI = collision.gameObject.transform.root.GetComponentInChildren<AIControl_Base>();
        if (targetAI != null && targetAI.m_cpumode == AIControl_Base.CPUMODE.GUARD)
        {
            targetAI.m_cpumode = AIControl_Base.CPUMODE.GUARDEND;
            Destroy(gameObject);
            return;
        }        

        // BEAM・BULLETの場合
        if (Bullettype == BulletType.BEAM || Bullettype == BulletType.BULLET)
        {
            // 着弾音を鳴らす
            AudioSource.PlayClipAtPoint(HitSE, transform.position);
            // 着弾した位置にヒットエフェクトを置く            
            GameObject hiteffect = (GameObject)Instantiate(HitEffect, transform.position, transform.rotation);
			
            // targetがCharacterControl_Baseクラスでなければ「自壊させて」強制抜け
            if (target == null)
            {
                // オブジェクトを自壊させる
                //Destroy(gameObject);
                return;
            }
            // ダウン中かダウン値MAXならダメージを与えない
            // そうでないならダメージとダウン値加算を確定
            // ヒット時にダメージの種類をCharacterControl_Baseに与える
            // ただしアーマー時ならダウン値とダメージだけ加算する
            // Blow属性の攻撃を与えた場合も吹き飛びへ移行
            // のけぞりならDamageInit
            // 吹き飛びならBlowInit
            // 吹き飛びの場合、相手に方向ベクトルを与える

            // ダウン中かダウン値MAXかリバーサルならダメージを与えない
            if (target.Invincible)
            {
                // オブジェクトを自壊させる
                Destroy(gameObject);
                return;
            }
            // そうでないならダメージとダウン値加算と覚醒ゲージ増加を確定
            else
            {
                HitDamage(player, enemy, collision);
            }
            // ヒット時にダメージの種類をCharacterControlBaseに与える
            // ダウン値を超えていたら吹き飛びへ移行
            // Blow属性の攻撃を与えた場合も吹き飛びへ移行
            if (target.NowDownRatio >= target.DownRatioBias || this.Hittype == CharacterSkill.HitType.BLOW)
            {   // 吹き飛びの場合、相手に方向ベクトルを与える            
                // Y軸方向は少し上向き
                target.MoveDirection.y += 10;
                target.BlowDirection = this.MoveDirection;
                // 吹き飛びの場合、攻撃を当てた相手を浮かす（MadokaDefine.LAUNCHOFFSET)            
                target.GetComponent<Rigidbody>().position = target.GetComponent<Rigidbody>().position + new Vector3(0, MadokaDefine.LAUNCHOFFSET, 0);
                target.GetComponent<Rigidbody>().AddForce(this.MoveDirection.x * MadokaDefine.LAUNCHOFFSET, this.MoveDirection.y * MadokaDefine.LAUNCHOFFSET, this.MoveDirection.z * MadokaDefine.LAUNCHOFFSET);
                target.DamageInit(target.AnimatorUnit, 41, true, 43, 44);
            }
            // それ以外はのけぞりへ
            else
            {
                // ただしアーマー時ならダウン値とダメージだけ加算する(Damageにしない）
                if (!target.IsArmor)
                {
                    target.DamageInit(target.AnimatorUnit, 41, false, 43, 44); 
                }
                // アーマーなら以降の処理が関係ないのでオブジェクトを自壊させてサヨウナラ                
            }
            // オブジェクトを自壊させる
            Destroy(gameObject); 
        }
        // BAZOOKAの場合
        else if (Bullettype == BulletType.BAZOOKA)
        {
            // 爆発エフェクトを生成する(Bazooka_ShockWaveプレハブ作成)
            // このオブジェクトの子としてBazooka_ShockWaveを作成する
            var bazooka_shockwave = (GameObject)Instantiate(BazookaShockWave, transform.position, transform.rotation);
            bazooka_shockwave.transform.parent = this.transform;
            // ダメージとダウン値を設定
            var shockwave_state = bazooka_shockwave.GetComponentInChildren<Bazooka_ShockWave>();
            // ダメージ
            // 敵に触れた場合
            if (collision.gameObject.tag == enemy)
            {
                // 覚醒補正
                DamageCorrection();
                shockwave_state.SetDamage(OffemsivePower);
                shockwave_state.SetCharacter(InjectionCharacterIndex);
            }
            // 味方に触れた場合
            else if (collision.gameObject.tag == player)
            {
                // 覚醒補正
                DamageCorrection();
                shockwave_state.SetDamage((int)(OffemsivePower/MadokaDefine.FRENDLYFIRE_RATIO));
                shockwave_state.SetCharacter(InjectionCharacterIndex);
            }
            // ダウン値
            shockwave_state.SetDownratio(DownRatio);
            // 覚醒ゲージ増加（覚醒時除く）
            if (InjectionObject.GetComponent<CharacterControl_Base>().IsArousal == false)
            {
                // 攻撃を当てた側が味方側の場合
                if (InjectionObject.GetComponent<CharacterControl_Base>().IsPlayer != CharacterControl_Base.CHARACTERCODE.ENEMY)
                {
                    savingparameter.AddArousal(InjectionCharacterIndex, ArousalRatio);
                }
                // 攻撃を当てた側が敵側の場合
                else
                {
                    InjectionObject.GetComponent<CharacterControl_Base>().AddArousal(ArousalRatio);
                }
            }
            // 攻撃を当てた相手の覚醒ゲージを増加
            shockwave_state.SetArousal(ArousalRatio);
            // オブジェクトの自壊に巻き込まれないように切り離す
            shockwave_state.transform.parent = null;

            // オブジェクトを自壊させる
            Destroy(gameObject); 
        }
        // BOMBの場合
        else if (Bullettype == BulletType.BOMB)
        {
            // アニメが起動していなかった場合、アニメを起動する
            if (!StartedBombAnimation)
            {
                this.GetComponent<Animation>().Play(BombAnimationName);
                StartedBombAnimation = true;
            }
            // 敵にあたったわけでないなら強制抜け
            if (target == null)
            {
                return;
            }
            // ダウン中かダウン値MAXならダメージを与えない
            if (target.Invincible)
            {
                return;
            }
            else
            {
                HitDamage(player, enemy, collision);
            }
            // ヒット時にダメージの種類をCharacterControl_Baseに与える
            // ダウン値を超えていたら吹き飛びへ移行
            // Blow属性の攻撃を与えた場合も吹き飛びへ移行
            if (target.NowDownRatio >= target.DownRatioBias || this.Hittype == CharacterSkill.HitType.BLOW)
            {   // 吹き飛びの場合、相手に方向ベクトルを与える            
                // Y軸方向は少し上向き
                target.MoveDirection.y += 10;
                target.BlowDirection = this.MoveDirection;
                // 吹き飛びの場合、攻撃を当てた相手を浮かす（MadokaDefine.LAUNCHOFFSET)            
                target.GetComponent<Rigidbody>().position = target.GetComponent<Rigidbody>().position + new Vector3(0, MadokaDefine.LAUNCHOFFSET, 0);
                target.GetComponent<Rigidbody>().AddForce(this.MoveDirection.x * MadokaDefine.LAUNCHOFFSET, this.MoveDirection.y * MadokaDefine.LAUNCHOFFSET, this.MoveDirection.z * MadokaDefine.LAUNCHOFFSET);
                target.DamageInit(target.AnimatorUnit, 41, true, 43, 44);
            }
            // それ以外はのけぞりへ
            else
            {
                // ただしアーマー時ならダウン値とダメージだけ加算する(Damageにしない）
                if (!target.IsArmor)
                {
                    target.DamageInit(target.AnimatorUnit, 41, false, 43, 44);
                }              
            }
        }
		// FUNNEL
		else if(Bullettype == BulletType.FUNNEL_LASER)
		{
			Funnnelstate = FunnelState.Shoot;
			FunnelBeam.SetActive(true);         // 攻撃判定をアクティブにする
		}           
    }

    // BEAM/BOMB属性のときにダメージを有効化する
    // 第1引数：敵か味方か
    // 第2引数：敵か味方か
    // 第3引数：コリジョン
    private void HitDamage(string player, string enemy, Collision collision)
    {
        // 敵に触れた場合
        if (collision.gameObject.tag == enemy)
        {
            // 覚醒補正
            DamageCorrection();
            // ダメージ
            collision.gameObject.GetComponent<CharacterControlBase>().DamageHP(InjectionCharacterIndex, OffemsivePower);
        }
        // 味方に触れた場合
        else if (collision.gameObject.tag == player)
        {
            // 覚醒補正
            DamageCorrection();
            // ダメージ
            collision.gameObject.GetComponent<CharacterControlBase>().DamageHP(InjectionCharacterIndex, (int)((float)OffemsivePower / MadokaDefine.FRENDLYFIRE_RATIO));
        }
        // ダウン値加算
        collision.gameObject.SendMessage("DownRateInc", DownRatio);
        // 覚醒ゲージ加算（覚醒時除く）
        if (InjectionObject.GetComponent<CharacterControlBase>().IsArousal == false)
        {
            // 攻撃を当てた側が味方側の場合
            if (InjectionObject.GetComponent<CharacterControlBase>().IsPlayer != CharacterControlBase.CHARACTERCODE.ENEMY)
            {
                float nextArousal = ArousalRatio + savingparameter.GetNowArousal(InjectionCharacterIndex);
                savingparameter.AddArousal(InjectionCharacterIndex, nextArousal);
            }
            // 攻撃を当てた側が敵側の場合
            else
            {
                InjectionObject.GetComponent<CharacterControlBase>().AddArousal(ArousalRatio);
            }

        }
        // 敵に覚醒ゲージを加算
        collision.gameObject.SendMessage("DamageArousal", ArousalRatio);
    }
    

    // 覚醒時のダメージ補正を行う
    private void DamageCorrection()
    {
        // 攻撃側が覚醒中の場合
        if (InjectionObject != null)
        {
            bool injection = InjectionObject.GetComponent<CharacterControlBase>().IsArousal;
            if (injection)
            {
                OffemsivePower = (int)(OffemsivePower * MadokaDefine.AROUSAL_OFFENCE_UPPER);
            }
        }
        // 防御側が覚醒中の場合
        if (HitTarget != null)
        {
            bool target = HitTarget.GetComponent<CharacterControlBase>().IsArousal;
            if (target)
            {
                OffemsivePower = (int)(OffemsivePower * MadokaDefine.AROUSAL_DEFFENSIVE_UPPER);
            }
        }
    }

    // ゲームオブジェクトを自壊させる（アニメーションファイルから呼ばれる）
    public void DestroyMyself()
    {
        Destroy(gameObject);
    }
}
