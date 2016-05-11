using UnityEngine;
using System.Collections;

// 飛び道具系の基底クラス
public class Bullet : MonoBehaviour 
{
    // 射出元取得
    public GameObject m_Obj_OR;

    // 弾速
    public float m_Speed;

    public Vector3 m_MoveDirection;   // 移動方向

    // 属性
    public enum BulletType
    {
        BEAM,       // ビーム（実弾貫通）
        BULLET,     // 実弾
        BAZOOKA,    // 着弾時拡散する実弾
        BOMB,       // 爆風を撒き散らすボム
        LASER,      // レーザー（相殺なし）
    };
    public BulletType m_Bullettype;

    // 親のスクリプト名
    public string m_ParentScript;
    
    // 維持時間
    public float m_LifeTime;

    // 攻撃力
    public int m_OffemsivePower;

    // ダウン値
    public float m_DownRatio;

    // 覚醒ゲージ増加量
    public float m_ArousalRatio;

    // 着弾時のSE
    public AudioClip m_Insp_HitSE;

    // 射出元のゲームオブジェクト
    public GameObject m_InjectionObject;

    // 射出元のゲームオブジェクトの名前
    public string m_InjectionObjectName;

    // 射出元のキャラクターのインデックス
    public int m_CharacterIndex;

    // 目標のゲームオブジェクト
    public GameObject m_TargetObject;

    // 接触したゲームオブジェクト
    private GameObject m_HitTarget;

    // ポーズ処理(時間のそれと同じ）
    public CharacterControl_Base.TimeStopMode m_Timestopmode;

    // バズーカタイプの時の衝撃波件エフェクト
    public GameObject m_Insp_Bazooka_ShockWave;

    // 誘導カウント
    public int m_InductionCounter;

    // 誘導が続くフレーム数の閾値
    public int m_InductionBias;

    // ヒットタイプ(ダメージの種類.弓ほむらのみインスペクターで設定する。他のキャラは各弾丸クラスでCharacter_Specの値を引っ張る形で設定する）
    protected CharacterSkill.HitType m_Hittype;

    // 親オブジェクトから切り離されて独立飛行しているか否か
    public bool m_IsIndependence;
    
    // 射出後の角度変更を行ったか否か
    public bool m_RotationControl = false;

    // 弾丸の出現時間
    protected float m_TimeNow = 0.0f;

    // BOMBの場合、アニメーション再生が始まっているか
    protected bool m_startedAnimation;

    // BOMBのアニメーションの名前
    public string m_BombAnimation_Name;
 
    // Updateでの共通処理（継承用）
    protected void UpdateCore()
    {
        
        // 飛行開始
        // キリカの時間遅延を受けているとき、1/4に
        if (m_Timestopmode == CharacterControl_Base.TimeStopMode.TIME_DELAY)
        {

        }
        // ほむらの時間停止を受けているときなど、0に
        else if (m_Timestopmode == CharacterControl_Base.TimeStopMode.TIME_STOP || m_Timestopmode == CharacterControl_Base.TimeStopMode.PAUSE || m_Timestopmode == CharacterControl_Base.TimeStopMode.AROUSAL)
        {
            // アニメを止めておく
            //Time.timeScale = 0;
            return;
        }

        // カメラから親と対象を拾う
        // (ほむの下にカメラがいるので、GetComponentChildlenで拾える）
        //var target = m_Obj_OR.GetComponentInChildren<Player_Camera_Controller>();
        // 敵の場合、弾丸があっても親が死んでいる可能性もあるので、その場合は強制抜け
        if (m_Obj_OR == null)
        {
            BrokenMySelf();
            return;
        }
        var target = m_Obj_OR.transform.GetComponentInChildren<Player_Camera_Controller>();
        // 親オブジェクトを拾う
        this.m_InjectionObject = target.Player;
        // ターゲットオブジェクトを拾う
        this.m_TargetObject = target.Enemy;
        // 親オブジェクトから切り離されたときかつ非独立状態なら、親が保持している方向ベクトルを拾う
        // 非独立状態の判定をしないと、射出後も親に合わせて動いてしまう
        // 親オブジェクトを拾う
        if (this.transform.parent != null && !m_IsIndependence)
        {
            // 親のステートがSHOTになったら、親子関係を切り離す
            if (m_Obj_OR != null && m_Obj_OR.GetComponent<CharacterControl_Base>().GetShotmode() == CharacterControl_Base.ShotMode.SHOT)
            {
                // その時の座標を保持する
                this.GetComponent<Rigidbody>().position = m_Obj_OR.GetComponent<CharacterControl_Base>().MainShotRoot.GetComponent<Rigidbody>().position;
                // 進行方向ベクトルを保持する
                this.m_MoveDirection = m_Obj_OR.GetComponent<CharacterControl_Base>().BulletMoveDirection;
                // 攻撃力を保持する
                this.m_OffemsivePower = m_Obj_OR.GetComponent<CharacterControl_Base>().OffensivePowerOfBullet;
                // ダウン値を保持する
                this.m_DownRatio = m_Obj_OR.GetComponent<CharacterControl_Base>().DownratioPowerOfBullet;
                // 覚醒ゲージ増加量を保持する
                this.m_ArousalRatio = m_Obj_OR.GetComponent<CharacterControl_Base>().ArousalRatioOfBullet;

                // 親のステートを発射完了へ切り替える
                m_Obj_OR.GetComponent<CharacterControl_Base>().SetShotmode(CharacterControl_Base.ShotMode.
                    SHOTDONE);
                // 切り離し
                this.transform.parent = null;
                // 独立フラグを立てる
                this.m_IsIndependence = true;
                // IsKinematicを折る
                this.transform.GetComponent<Rigidbody>().isKinematic = false;

                                
                {                    
                    {
                        Vector3 Correction_Rot = new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y + 12.0f , this.transform.rotation.eulerAngles.z);
                        this.transform.rotation = Quaternion.Euler(Correction_Rot);
                    }
                    // 誘導がないなら進行方向は固定
                    if (this.m_TargetObject == null)
                    {
                        this.m_InductionCounter = this.m_InductionBias + 1; // 下でカウンターが閾値を超えたら固定されるので
                    }
                }
            }
        }
        // 規定フレーム間誘導する
        InductionBullet();

        // レイキャストで接触したオブジェクト
        RaycastHit hit;

        // 進行方向に対してレイキャストする
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 300.0f))
        {
            var hittarget = hit.collider.gameObject.GetComponent<AIControl_Base>();
            var hittarget2 = hit.collider.gameObject.GetComponent<CharacterControl_Base>();
            // ガード姿勢を取らせる(自分の弾は除く)
            if (hittarget != null && m_CharacterIndex != (int)hittarget2.m_character_name)
            {
                // 相手がNOMOVEの時にはガードは取らせない
                if (hittarget.m_cpumode != AIControl_Base.CPUMODE.NOMOVE)
                {
                    hittarget.m_cpumode = AIControl_Base.CPUMODE.GUARD;
                }
            }
        }
             

        BrokenMySelf();
        m_TimeNow++;
    }

    // 規定フレーム間誘導する
    protected void InductionBullet()
    {
        // カメラから親と対象を拾う
        var target = m_Obj_OR.transform.GetComponentInChildren<Player_Camera_Controller>();
        // 親オブジェクトを拾う
        this.m_InjectionObject = target.Player;
        // ターゲットオブジェクトを拾う
        this.m_TargetObject = target.Enemy;
        // 移動＆回転補正
        if (m_Timestopmode != CharacterControl_Base.TimeStopMode.TIME_STOP || m_Timestopmode != CharacterControl_Base.TimeStopMode.PAUSE || m_Timestopmode != CharacterControl_Base.TimeStopMode.AROUSAL)
        {
            // 独立状態になってから行う（射出前に回すとPCに突き刺さったりと変な形になるので）
            if (this.m_IsIndependence)
            {
                // 親のステートを拾う
                var parent = target.Player.GetComponent<CharacterControl_Base>();
                // ロックオン範囲にいて、かつカウンターが閾値を超えていない場合は誘導する
                if (this.m_InductionCounter < this.m_InductionBias && this.m_TargetObject != null)
                {
                    if (Vector3.Distance(this.m_InjectionObject.transform.position, this.m_TargetObject.transform.position) <= parent.m_Rockon_Range)
                    {
                        // 敵のカプセルコライダの高さ分オフセット（そうしないと下向きのベクトルになる）
                        // キャラクターコントローラーを拾う
                        CapsuleCollider offset_OR = this.m_TargetObject.GetComponentInChildren<CapsuleCollider>(); //var offset_OR = this.m_TargetObject.GetComponentInChildren<CharacterController>();
                        if (offset_OR == null)
                        {
                            return;
                        }
                        // 高さを取得する
                        float offset = offset_OR.height;
                        // 対象の高さを変更する
                        Vector3 Targetpos = this.m_TargetObject.transform.position;
                        Targetpos.y += offset;
                        // 変換したベクトルを正規化
                        this.m_MoveDirection = Vector3.Normalize(Targetpos - this.GetComponent<Rigidbody>().transform.position);
                        // オブジェクトを相手の方向へ向ける
                        Quaternion looklot = Quaternion.LookRotation(m_TargetObject.transform.position - this.transform.position);
                        Vector3 looklotE = looklot.eulerAngles;
                        // ほむらの時のみ90度補正
                        if (parent.m_character_name == Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B)
                        {
                            looklot = Quaternion.Euler(looklotE.x, looklotE.y + 90.0f, looklotE.z);
                        }
                        else
                        {
                            looklot = Quaternion.Euler(looklotE.x, looklotE.y, looklotE.z);
                        }
                        this.transform.rotation = looklot;
                    }
                }
                GetComponent<Rigidbody>().position = GetComponent<Rigidbody>().position + this.m_MoveDirection * m_Speed * Time.deltaTime;
            }
        }
        // 親が死んだ場合は自壊
        if (m_InjectionObject == null)
        {
            Destroy(gameObject);
        }

        // カウンターを加算
        if (this.m_IsIndependence)
        {
            this.m_InductionCounter++;
        }

        // 誘導しない場合は撃った後は方向は固定
        if (this.m_InductionCounter >= this.m_InductionBias)
        {
            this.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    // 指定時間後自分を破棄する
    protected void BrokenMySelf()
    {
        // とりあえず時間が通常のときだけ自壊させる（何かしらと接触すると壊す予定ではあるが・・・時間停止中どうするべきか・・・？）
        if (this.m_Timestopmode == CharacterControl_Base.TimeStopMode.NORMAL || this.m_Timestopmode == CharacterControl_Base.TimeStopMode.TIME_DELAY)
        {
            if (m_LifeTime*60 < m_TimeNow)
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
        if (m_Obj_OR.GetComponent<CharacterControl_Base>().m_isPlayer != CharacterControl_Base.CHARACTERCODE.ENEMY)
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
        var target = collision.gameObject.GetComponent<CharacterControl_Base>();
        m_HitTarget = collision.gameObject;

		// ビーム属性同士の弾丸が干渉したときは自壊させない
		var hittargetBullet = collision.gameObject.GetComponent<Bullet>();
		if(hittargetBullet != null)
		{
			if (hittargetBullet.m_Bullettype == BulletType.BEAM && m_Bullettype == BulletType.BEAM)
			{
				return;
			}
		}

        // ゼロ距離射撃したときは親を受け取る
        if (m_InjectionObject == null)
        {
            // カメラから親と対象を拾う
            var parent = m_Obj_OR.transform.GetComponentInChildren<Player_Camera_Controller>();
            // 親オブジェクトを拾う
            this.m_InjectionObject = parent.Player;
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
        if (m_Bullettype == BulletType.BEAM || m_Bullettype == BulletType.BULLET)
        {
            // 着弾音を鳴らす
            AudioSource.PlayClipAtPoint(m_Insp_HitSE, transform.position);
            // 着弾した位置にヒットエフェクトを置く
            UnityEngine.Object HitEffect = null;
            HitEffect = Resources.Load("DamageEffect");
            Instantiate(HitEffect, transform.position, transform.rotation);

            // targetがCharacterControl_Baseクラスでなければ「自壊させて」強制抜け
            if (target == null)
            {
                // オブジェクトを自壊させる
                Destroy(gameObject);
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

            // ダウン中かダウン値MAXならダメージを与えない
            if (target.m_AnimState[0] == CharacterControl_Base.AnimationState.Down || (target.m_DownRatio <= target.m_nowDownRatio))
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
            // ヒット時にダメージの種類をCharacterControl_Baseに与える
            // ダウン値を超えていたら吹き飛びへ移行
            // Blow属性の攻撃を与えた場合も吹き飛びへ移行
            if (target.m_nowDownRatio >= target.m_DownRatio || this.m_Hittype == CharacterSkill.HitType.BLOW)
            {   // 吹き飛びの場合、相手に方向ベクトルを与える            
                // Y軸方向は少し上向き
                target.m_MoveDirection.y += 10;
                target.m_BlowDirection = this.m_MoveDirection;
                // 吹き飛びの場合、攻撃を当てた相手を浮かす（MadokaDefine.LAUNCHOFFSET)            
                target.GetComponent<Rigidbody>().position = target.GetComponent<Rigidbody>().position + new Vector3(0, MadokaDefine.LAUNCHOFFSET, 0);
                target.GetComponent<Rigidbody>().AddForce(this.m_MoveDirection.x * MadokaDefine.LAUNCHOFFSET, this.m_MoveDirection.y * MadokaDefine.LAUNCHOFFSET, this.m_MoveDirection.z * MadokaDefine.LAUNCHOFFSET);
                target.m_AnimState[0] = CharacterControl_Base.AnimationState.BlowInit;
            }
            // それ以外はのけぞりへ
            else
            {
                // ただしアーマー時ならダウン値とダメージだけ加算する(Damageにしない）
                if (!target.m_IsArmor)
                {
                    target.m_AnimState[0] = CharacterControl_Base.AnimationState.DamageInit;
                }
                // アーマーなら以降の処理が関係ないのでオブジェクトを自壊させてサヨウナラ                
            }
            // オブジェクトを自壊させる
            Destroy(gameObject); 
        }
        // BAZOOKAの場合
        else if (m_Bullettype == BulletType.BAZOOKA)
        {
            // 爆発エフェクトを生成する(Bazooka_ShockWaveプレハブ作成)
            // このオブジェクトの子としてBazooka_ShockWaveを作成する
            var bazooka_shockwave = (GameObject)Instantiate(m_Insp_Bazooka_ShockWave, transform.position, transform.rotation);
            bazooka_shockwave.transform.parent = this.transform;
            // ダメージとダウン値を設定
            var shockwave_state = bazooka_shockwave.GetComponentInChildren<Bazooka_ShockWave>();
            // ダメージ
            // 敵に触れた場合
            if (collision.gameObject.tag == enemy)
            {
                // 覚醒補正
                DamageCorrection();
                shockwave_state.SetDamage(m_OffemsivePower);
                shockwave_state.SetCharacter(m_CharacterIndex);
            }
            // 味方に触れた場合
            else if (collision.gameObject.tag == player)
            {
                // 覚醒補正
                DamageCorrection();
                shockwave_state.SetDamage((int)(m_OffemsivePower/MadokaDefine.FRENDLYFIRE_RATIO));
                shockwave_state.SetCharacter(m_CharacterIndex);
            }
            // ダウン値
            shockwave_state.SetDownratio(m_DownRatio);
            // 覚醒ゲージ増加（覚醒時除く）
            if (m_Obj_OR.GetComponent<CharacterControl_Base>().m_isArousal == false)
            {
                // 攻撃を当てた側が味方側の場合
                if (m_InjectionObject.GetComponent<CharacterControl_Base>().m_isPlayer != CharacterControl_Base.CHARACTERCODE.ENEMY)
                {
                    savingparameter.AddArousal(m_CharacterIndex, m_ArousalRatio);
                }
                // 攻撃を当てた側が敵側の場合
                else
                {
                    m_InjectionObject.GetComponent<CharacterControl_Base>().AddArousal(m_ArousalRatio);
                }
            }
            // 攻撃を当てた相手の覚醒ゲージを増加
            shockwave_state.SetArousal(m_ArousalRatio);
            // オブジェクトの自壊に巻き込まれないように切り離す
            shockwave_state.transform.parent = null;

            // オブジェクトを自壊させる
            Destroy(gameObject); 
        }
        // BOMBの場合
        else if (m_Bullettype == BulletType.BOMB)
        {
            // アニメが起動していなかった場合、アニメを起動する
            if (!m_startedAnimation)
            {
                this.GetComponent<Animation>().Play(m_BombAnimation_Name);
                m_startedAnimation = true;
            }
            // 敵にあたったわけでないなら強制抜け
            if (target == null)
            {
                return;
            }
            // ダウン中かダウン値MAXならダメージを与えない
            if (target.m_AnimState[0] == CharacterControl_Base.AnimationState.Down || (target.m_DownRatio <= target.m_nowDownRatio))
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
            if (target.m_nowDownRatio >= target.m_DownRatio || this.m_Hittype == CharacterSkill.HitType.BLOW)
            {   // 吹き飛びの場合、相手に方向ベクトルを与える            
                // Y軸方向は少し上向き
                target.m_MoveDirection.y += 10;
                target.m_BlowDirection = this.m_MoveDirection;
                // 吹き飛びの場合、攻撃を当てた相手を浮かす（MadokaDefine.LAUNCHOFFSET)            
                target.GetComponent<Rigidbody>().position = target.GetComponent<Rigidbody>().position + new Vector3(0, MadokaDefine.LAUNCHOFFSET, 0);
                target.GetComponent<Rigidbody>().AddForce(this.m_MoveDirection.x * MadokaDefine.LAUNCHOFFSET, this.m_MoveDirection.y * MadokaDefine.LAUNCHOFFSET, this.m_MoveDirection.z * MadokaDefine.LAUNCHOFFSET);
                //rigidbody.position = rigidbody.position + ;
                target.m_AnimState[0] = CharacterControl_Base.AnimationState.BlowInit;
            }
            // それ以外はのけぞりへ
            else
            {
                // ただしアーマー時ならダウン値とダメージだけ加算する(Damageにしない）
                if (!target.m_IsArmor)
                {
                    target.m_AnimState[0] = CharacterControl_Base.AnimationState.DamageInit;
                }              
            }
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
            collision.gameObject.GetComponent<CharacterControl_Base>().DamageHP(m_CharacterIndex, m_OffemsivePower);
        }
        // 味方に触れた場合
        else if (collision.gameObject.tag == player)
        {
            // 覚醒補正
            DamageCorrection();
            // ダメージ
            collision.gameObject.GetComponent<CharacterControl_Base>().DamageHP(m_CharacterIndex, (int)((float)m_OffemsivePower / MadokaDefine.FRENDLYFIRE_RATIO));
        }
        // ダウン値加算
        collision.gameObject.SendMessage("DownRateInc", m_DownRatio);
        // 覚醒ゲージ加算（覚醒時除く）
        if (m_Obj_OR.GetComponent<CharacterControl_Base>().m_isArousal == false)
        {
            // 攻撃を当てた側が味方側の場合
            if (m_InjectionObject.GetComponent<CharacterControl_Base>().m_isPlayer != CharacterControl_Base.CHARACTERCODE.ENEMY)
            {
                float nextArousal = m_ArousalRatio + savingparameter.GetNowArousal(m_CharacterIndex);
                savingparameter.AddArousal(m_CharacterIndex, nextArousal);
            }
            // 攻撃を当てた側が敵側の場合
            else
            {
                m_InjectionObject.GetComponent<CharacterControl_Base>().AddArousal(m_ArousalRatio);
            }

        }
        // 敵に覚醒ゲージを加算
        collision.gameObject.SendMessage("DamageArousal", m_ArousalRatio);
    }
    

    // 覚醒時のダメージ補正を行う
    private void DamageCorrection()
    {
        // 攻撃側が覚醒中の場合
        if (m_InjectionObject != null)
        {
            bool injection = m_InjectionObject.GetComponent<CharacterControl_Base>().m_isArousal;
            if (injection)
            {
                m_OffemsivePower = (int)(m_OffemsivePower * MadokaDefine.AROUSAL_OFFENCE_UPPER);
            }
        }
        // 防御側が覚醒中の場合
        if (m_HitTarget != null)
        {
            bool target = m_HitTarget.GetComponent<CharacterControl_Base>().m_isArousal;
            if (target)
            {
                m_OffemsivePower = (int)(m_OffemsivePower * MadokaDefine.AROUSAL_DEFFENSIVE_UPPER);
            }
        }
    }

    // ゲームオブジェクトを自壊させる（アニメーションファイルから呼ばれる）
    public void DestroyMyself()
    {
        Destroy(gameObject);
    }
}
