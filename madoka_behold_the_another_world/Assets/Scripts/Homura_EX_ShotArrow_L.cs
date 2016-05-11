using UnityEngine;
using System.Collections;

// 特殊射撃時の本体左側の矢にインポートするコンポーネント
public class Homura_EX_ShotArrow_L : Bullet
{
	// Use this for initialization
	void Start () 
    {
        // 維持時間を初期化（後で調整し、赤ロック範囲内を飛行中は生き残らせること）
        this.m_LifeTime = 4.0f;
        this.m_MoveDirection = Vector3.zero;
        // 独立フラグを初期化
        this.m_IsIndependence = false;
        // 誘導カウンターを初期化
        this.m_InductionCounter = 0;
        // 誘導時間を初期化
        this.m_InductionBias = 30;
        // 親スクリプトを初期化
        this.m_ParentScript = "Homura_Final_BattleControl";
        m_InjectionObjectName = "homura_ribon_magica_battle_use";
        // 親オブジェクトを取得(rootでツリーの一番上から検索をかける）  
        m_Obj_OR = transform.root.GetComponentInChildren<Homura_Final_BattleControl>().gameObject;
        // 接触対象に自分は除く（散弾や爆風は発生時に別のオブジェクトを呼ぶ）
        Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), m_Obj_OR.transform.GetComponent<Collider>());
        // 撃ったキャラが誰であるか保持
        m_CharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B;
        // 被弾時の挙動を設定
        m_Hittype = Character_Spec.cs[m_CharacterIndex][3].m_Hittype;
	}
	
	// Update is called once per frame
	void Update () 
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
            // 親のステートがSHOTDONEになったら、親子関係を切り離す(真ん中の1発でSHOTDONEに切り替わるので、こっちで拾う）
            // 下記のHomura_Final_BattleControlをCharacterControl_Baseにすることで他のキャラでも応用可能
            if (m_Obj_OR != null && m_Obj_OR.GetComponent<CharacterControl_Base>().GetShotmode() == CharacterControl_Base.ShotMode.SHOTDONE)
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
                // 切り離し
                this.transform.parent = null;
                // 独立フラグを立てる
                this.m_IsIndependence = true;
                // IsKinematicを折る
                this.transform.GetComponent<Rigidbody>().isKinematic = false;

                Vector3 Correction_Rot = new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y + 12.0f, this.transform.rotation.eulerAngles.z);
                this.transform.rotation = Quaternion.Euler(Correction_Rot);
                    
                // 誘導がないなら進行方向は固定
                if (this.m_TargetObject == null)
                {
                    this.m_InductionCounter = this.m_InductionBias + 1; // 下でカウンターが閾値を超えたら固定されるので
                }
                
            }
        }
        // 規定フレーム間誘導する
        InductionBullet();


        // 時間停止を解除したら動き出す
        //Time.timeScale = 1;        

        BrokenMySelf();
        m_TimeNow++; 
	}
}
