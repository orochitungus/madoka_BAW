using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UniRx.Triggers;

public class Player_Camera_Controller : MonoBehaviour 
{
    // フィールド変数を設定する
    public GameObject Player;   // カメラが映すオブジェクト
    public GameObject Enemy;    // ロックオンするターゲット
    public float Distance;      // 映すオブジェクトまでの距離(非ロックオン時）
    public float Distance_mine; // ロックオン時の自機とカメラとの距離
    public float SensitivityX;  // マウスの感度
    public float SensitivityY;
    public float RotX;          // それぞれの軸の回転角度
    public float RotY;
    public float RotZ;
    public float height;        // カメラ高度
    public float rockon_height; // ロックオン時カメラ高度
    public float gaze_offset;   // 注視点のオフセット量
    public float m_enemy_offset_height;    // 敵高度のオフセット量
    public bool m_DrawInterface;           // インターフェース描画の有無

    // ロックオンの有無
    public bool IsRockOn;
	/// <summary>
	/// 覚醒技の演出中であるか否か（覚醒技専用カメラワークになったとき、一時的にカーソルを消す）
	/// </summary>
	public bool IsArousalAttack;


    // ロックオン対象(何個あるかわからないので)
    public List<GameObject> RockOnTarget;
    public int          NowTarget;                // 現在ロックオンしている相手

    // 有効範囲か否か
    public bool IsRockonRange;

    // CPU
    // ルーチンを拾う
    AIControl.CPUMODE CPUmode;
    // 入力を拾う
    AIControl.KEY_OUTPUT CPUKeyInput;

    public MainCameraMode Maincameramode;

	/// <summary>
	/// ロックオンカーソル制御
	/// </summary>
	public RockOnCursorControl Rockoncursorcontrol;

	/// <summary>
	/// インターフェース
	/// </summary>
	public BattleInterfaceController Battleinterfacecontroller;

    void Awake()
    {
        Application.targetFrameRate = 60;
        Maincameramode = MainCameraMode.NORMAL;
    }

	// Use this for initialization
	void Start () 
    {
        // フィールド変数を初期化する
        this.UpdateAsObservable().Subscribe(_ =>
        {
            if (Maincameramode == MainCameraMode.HOMURABOWAROUSAL)
            {
                Distance = 40.0f;
            }
            else
            {
                Distance = 20.0f;
            }
        });
        Distance = 20.0f;
        SensitivityX = 50.0f;
        SensitivityY = 50.0f;
        RotX = 0.0f;
        RotY = 0.0f;
        height = 4.0f;
        gaze_offset = 5.5f;
        rockon_height = 0.0f;
        // ロックオンの有無
        IsRockOn = false;
		// 覚醒技演出中の有無
		IsArousalAttack = false;
        IsRockonRange = false;
        m_enemy_offset_height = 0.0f;
        Distance_mine = 18.0f;

		// インターフェースを拾う
		Battleinterfacecontroller = GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>();
		if(Battleinterfacecontroller == null)
		{
			Debug.LogError("BattleInterfaceCanvas is Nothing!!");
		}
		
        var target = Player.GetComponentInChildren<CharacterControlBase>();    // 戦闘用キャラの場合
        // 戦闘用キャラ
        if (target != null)
        {
            if (target.IsPlayer != CharacterControlBase.CHARACTERCODE.PLAYER)
            {
                m_DrawInterface = false;
            }
            else
            {
                // FPSを固定しておく
                Application.targetFrameRate = 60;
                // インターフェースを有効化しておく
                m_DrawInterface = true;
            }
        }
        // クエストパート用キャラ
        else
        {
            Application.targetFrameRate = 60;
            m_DrawInterface = false;
        }
		// 視点を初期化
		RotX = Mathf.Asin(height / Distance) * Mathf.Rad2Deg;

		// ロックオンカーソル制御用クラスにBattleInterfaceControllerを与える(PC時のみ)
		if (target.IsPlayer == CharacterControlBase.CHARACTERCODE.PLAYER)
		{
			Rockoncursorcontrol.parentRectTrans = Battleinterfacecontroller.GetComponent<RectTransform>();
		}

		// ロックオンカーソル制御(PC時のみ)
		this.UpdateAsObservable().Where(_ => target.IsPlayer == CharacterControlBase.CHARACTERCODE.PLAYER).Subscribe(_ => 
		{
			if (IsRockOn && !IsArousalAttack)
			{				
				// ロックオンカーソルの位置を計算
				Vector2 rockoncursorpos = Rockoncursorcontrol.RockonCursorPos;
				// BattleInterfaceControllerのロックオンカーソルの位置を決定する
				Battleinterfacecontroller.RockOnCursorGreen.transform.localPosition = rockoncursorpos;
				Battleinterfacecontroller.RockOnCursorRed.transform.localPosition = rockoncursorpos;
				Battleinterfacecontroller.RockOnCursorYellow.transform.localPosition = rockoncursorpos;
				// ダウン値を超えているorダウンしているなら黄色ロック
				if (Enemy.GetComponentInChildren<CharacterControlBase>().NowDownRatio >= Enemy.GetComponentInChildren<CharacterControlBase>().DownRatioBias 
					|| Enemy.GetComponentInChildren<CharacterControlBase>().DownTime > 0)
				{
					Battleinterfacecontroller.RockOnCursorGreen.gameObject.SetActive(false);
					Battleinterfacecontroller.RockOnCursorRed.gameObject.SetActive(false);
					Battleinterfacecontroller.RockOnCursorYellow.gameObject.SetActive(true);
				}
				// ロックオンして有効射程内にいるなら赤ロック
				else if ( Vector3.Distance(Player.transform.position, Enemy.transform.position) <= Player.GetComponentInChildren<CharacterControlBase>().RockonRange)
				{
					Battleinterfacecontroller.RockOnCursorGreen.gameObject.SetActive(false);
					Battleinterfacecontroller.RockOnCursorRed.gameObject.SetActive(true);
					Battleinterfacecontroller.RockOnCursorYellow.gameObject.SetActive(false);
				}
				// いずれでもないなら緑ロック
				else
				{
					Battleinterfacecontroller.RockOnCursorGreen.gameObject.SetActive(true);
					Battleinterfacecontroller.RockOnCursorRed.gameObject.SetActive(false);
					Battleinterfacecontroller.RockOnCursorYellow.gameObject.SetActive(false);
				}
				// 敵HP情報を表示
				Battleinterfacecontroller.EnemyHPGauge.SetActive(true);
				string enemyName = Character_Spec.Name[(int)Enemy.GetComponent<CharacterControlBase>().CharacterName];
				int nowHP = Enemy.GetComponent<CharacterControlBase>().NowHitpoint;
				int maxHP = Enemy.GetComponent<CharacterControlBase>().GetMaxHitpoint(Enemy.GetComponent<CharacterControlBase>().Level);
				bool istarget = Enemy.GetComponent<CharacterControlBase>().IsTarget;
				Battleinterfacecontroller.EnemyHPGauge.GetComponent<EnemyHPGauge>().HPGaugeUpudate(nowHP, maxHP, enemyName, istarget);
			}
			else
			{
				// ロックオンカーソルを消す
				Battleinterfacecontroller.RockOnCursorGreen.gameObject.SetActive(false);
				Battleinterfacecontroller.RockOnCursorRed.gameObject.SetActive(false);
				Battleinterfacecontroller.RockOnCursorYellow.gameObject.SetActive(false);
				Battleinterfacecontroller.EnemyHPGauge.SetActive(false);
			}
		});
	}
	
	// Update is called once per frame
	void Update () 
    {
        // ロックオン（非ロックオン時）
        // このカメラが追跡しているオブジェクトの情報を拾う   
        CharacterControlBase target = Player.GetComponentInChildren<CharacterControlBase>();
        var targetAI = Player.GetComponentInChildren<AIControl_Base>();

        // クエストパート時、使わないので抜ける
        if (target == null)
            return;

        // CPU時、CPUの情報を拾う
        if (target.IsPlayer != CharacterControlBase.CHARACTERCODE.PLAYER)
        {
            // ルーチンを拾う
            // TODO:CPU作るまで一旦カット
			// m_cpumode = targetAI.m_cpumode;
            // 入力を拾う
            //m_key = targetAI.m_keyoutput; 
        }

        // 解除入力が行われた
        if (target.IsRockon && target.HasSearchCancelInput)
        {
            UnlockDone(target);
        }
        // ロックオンボタンが押された（この判定はCharacterControl_Baseの派生側で拾う）
        else if (target.GetSearchInput() || this.CPUKeyInput == AIControl.KEY_OUTPUT.SEARCH)
        {
            // ロックオンしていなかった
            if (!target.IsRockon)
            {
                // 自分がPC側か敵側か取得する
                switch (target.IsPlayer)
                {
                    // PC側の場合、敵を検索する
                    // 自機
                    case CharacterControlBase.CHARACTERCODE.PLAYER:
                    // 僚機
                    case CharacterControlBase.CHARACTERCODE.PLAYER_ALLY:
                        if (!OnPushSerchButton(true,false))
                        {
                            return;
                        }
                        break;
                    // 敵側の場合、PC側のタグを検索する
                    case CharacterControlBase.CHARACTERCODE.ENEMY:
						// TODO:CPU作るまで一旦カット               
						//switch (m_cpumode)
						//{
						//    // 敵の哨戒モードの場合、起点か終点を検索する
						//    // 終点へ向けて移動中
						//    case AIControl.CPUMODE.OUTWARD_JOURNEY:
						//        // 終点をロックオンする                                                             
						//        if (target.EndingPoint == null)
						//        {
						//            Debug.Log("EndingPoint isn't setting");
						//        }
						//        Enemy = target.EndingPoint;
						//        IsRockOn = true;
						//        target.IsRockon = true;                                
						//        break;
						//    // 起点へ向けて移動中
						//    case AIControl.CPUMODE.RETURN_PATH:
						//        // 起点をロックオンする
						//        if (target.StartingPoint == null)
						//        {
						//            Debug.Log("StartingPoint isn't setting");
						//        }
						//        Enemy = target.StartingPoint;
						//        IsRockOn = true;
						//        target.IsRockon = true;                                
						//        break;                            
						//    default:
						//        // 外れたら哨戒に戻る
						//        if (!OnPushSerchButton(false,false))
						//        {
						//            this.m_cpumode = AIControl.CPUMODE.OUTWARD_JOURNEY;
						//            IsRockOn = false;
						//            target.IsRockon = false;
						//        }
						//        break;
						//}
						break;
                }
                
            }
            // ロックオンしていた
            else
            {
				// TODO:CPU作るまで一旦カット
				// 敵もしくは僚機の哨戒状態で目的地にたどり着いたときにこの状態になる
				//           if (target.IsPlayer != CharacterControlBase.CHARACTERCODE.PLAYER)
				//           {   
				//// ここに指示が来る前に、cpumodeは切り替わっている
				//               // 往路(終点をロックオン）
				//               if (m_cpumode == AIControl.CPUMODE.OUTWARD_JOURNEY)
				//               {
				//                   Enemy = target.EndingPoint;
				//               }
				//               // 復路（起点をロックオン）
				//               else if(m_cpumode == AIControl.CPUMODE.RETURN_PATH)
				//               {
				//                   Enemy = target.StartingPoint;
				//               }
				//           }

				// 別の相手にロックオン対象を切り替える(2体以上候補が居る場合）
				if (RockOnTarget.Count > 1)
                {
					RockOnSelecter(target);
                }
                // 候補が1体の場合
                else if (RockOnTarget.Count == 1)
                {
                    // 自分の場合はロックオンさせない
                    float distance_check = Vector3.Distance(RockOnTarget[0].transform.position, target.transform.position);
                    if (distance_check > 0)
                    {
						RockDone(target, 0);
                    }
                }
                else
                {
                    // だれもいなければ解除
                    UnlockDone(target);
                }
            }
        }

		// ロックオン対象が死んでいたら強制的にロックを解除する（CPUの哨戒モード時は除く）
		// TODO:CPU作るまで一旦カット
		//if (target.IsRockon && m_cpumode != AIControl.CPUMODE.OUTWARD_JOURNEY && m_cpumode != AIControl.CPUMODE.RETURN_PATH)
  //      {
  //          var rockontarget = Enemy.GetComponentInChildren<CharacterControlBase>();
  //          if (rockontarget != null && rockontarget.NowHitpoint < 1)
  //          {
  //              UnlockDone(target);
  //          }
  //      }

	}

    // サーチボタンが押されたときの処理
    // playerside[in]       :プレイヤー側（true)
    // cpu[in]              :CPUであるか否か
    // output               :ロックオンする相手がいた
	// exitdown				:ダウンしていたら強制的にfalseを返す
    public bool OnPushSerchButton(bool playerside,bool cpu,bool exitdown = false)
    {
        CharacterControlBase target = Player.GetComponentInChildren<CharacterControlBase>();
        // Playerの座標を取得する
        Vector3 Player_Position = target.transform.position;
        // 検索
        if (playerside)
        {
            InRangeTargetRockon("Enemy");
        }
        else
        {
            InRangeTargetRockon("Player");
        }

        // 距離を計算する
        // 有効範囲内に敵が誰もいなければ、ロックオンを解除する
        if (RockOnTarget == null)
        {
            target.IsRockon = false;
            this.IsRockOn = false;
            return false;
        }

        // 敵の座標を取得する
        // ソートして一番近い相手をロックオン
        int most_near = 0;
        float distance_OR = 0;

        // 全部のキャラクターと自機との距離を取り、一番小さい値をmost_nearとする
        // 基準値が見つかったか？
        bool criteriaExist = false;
        for (int i = 0; i < RockOnTarget.Count; i++)
        {
            // 0でない値を探し、それが基準値となる
            if (!criteriaExist)
            {
                distance_OR = Vector3.Distance(Player_Position, RockOnTarget[i].transform.position);
                if (distance_OR > 0)
                {
                    criteriaExist = true;
                    most_near = i;
                }
            }
            // 以降は基準値より小さいと、それをmost_nearとする.ただしデバッグモードで敵を使うと自分を拾うことがあるので、0の場合は除外
            else
            {
                if (Vector3.Distance(Player_Position, RockOnTarget[i].transform.position) < distance_OR)
                {
                    float distance_check = Vector3.Distance(Player_Position, RockOnTarget[i].transform.position);
                    if (distance_check > 0)
                    {
                        distance_OR = distance_check;
                        most_near = i;
                    }
                }
            }
        }

        // 自機しかロックオン対象が居なかった場合は強制抜け
        if (distance_OR == 0)
        {
            if (cpu) return false;        // CPUの場合は哨戒起点や終点のロックも外してしまうので、Unlock処理は行わせない
            UnlockDone(target);
            return false;
        }

		// ダウンしていたら強制的にfalse
		if(exitdown)
		{
			CharacterControlBase rockontarget = RockOnTarget[most_near].GetComponent<CharacterControlBase>();
			if(rockontarget.NowDownRatio >= rockontarget.DownRatioBias)
			{
				return false;
			}
		}

        // フラグをロックオン状態に切り替える
        IsRockOn = true;
        // 対象をmost_nearとする
        Enemy = RockOnTarget[most_near];
        // 対象のインデックスを保持する
        NowTarget = most_near;
        // PC側のロックオンフラグを立てる
        target.IsRockon = true;
        return true;
    }

	/// <summary>
	/// ロックオン対象が複数いるときの処理
	/// </summary>
	/// <param name="target">このカメラが追跡しているキャラクター</param>
	public void RockOnSelecter(CharacterControlBase target)
	{
		// 自分＋1の相手を選択する
		int nexttarget = NowTarget + 1;
		// 認識している相手の中で、ロックオンできる相手を探す
		while (true)
		{
			// 最大値を超えていれば0に
			if (nexttarget > RockOnTarget.Count - 1)
			{
				nexttarget = 0;
			}
			// 相手が存在すればロックオン
			if (RockOnTarget[nexttarget] != null)
			{
				// 自分の場合はロックオンさせない
				float distance_check = Vector3.Distance(RockOnTarget[nexttarget].transform.position, target.transform.position);
				if (distance_check > 0)
				{
					RockDone(target,nexttarget);
				}
				break;
			}
			// しなければもう１つインデックスを加算して仕切り直し
			else
			{
				nexttarget++;
				// もしnowtargetと同じ値なら残り1体なのでロックオン解除
				if (NowTarget == nexttarget)
				{
					IsRockOn = false;
					target.IsRockon = false;
					// 増援が来たことの判定はここでやる（初めてロックオンした時の処理をここでもう一度）
					return;
				}
			}
		}
	}

    // ロックオン範囲内にいる敵を検索し、ロックオンする
    // target   [in]:敵側をロックオンするかプレイヤー側をロックオンするか 
    public void InRangeTargetRockon(string targettype)
    {
        var target = Player.GetComponentInChildren<CharacterControlBase>();
        // とりあえず画面上にいる敵を全員検索する
        GameObject[] RockonCandidate = GameObject.FindGameObjectsWithTag(targettype);
        for (int i = 0; i < RockonCandidate.Length; ++i)
        {
            // 見つけたキャラとプレイヤーとの距離を測る
            float distance = Vector3.Distance(Player.transform.position, RockonCandidate[i].transform.position);
            // 距離がm_Rockon_RangeLimitの中に入っていたら、RockOnTargetに加える
            if (distance <= target.RockonRangeLimit)
            {
                RockOnTarget.Add(RockonCandidate[i]);
            }
        }
    }

	/// <summary>
	/// ロックオンしたときフラグを立てる
	/// </summary>
	/// <param name="target">カメラの追跡対象</param>
	/// <param name="nexttarget">ロックオン対象のインデックス</param>
	private void RockDone(CharacterControlBase target, int nexttarget)
	{
		// フラグをロックオン状態に切り替える
		IsRockOn = true;
		// 対象をmost_nearとする
		Enemy = RockOnTarget[nexttarget];
		// 対象のインデックスを保持する
		NowTarget = nexttarget;
		// PC側のロックオンフラグを立てる
		target.IsRockon = true;
	}

    /// <summary>
    ///  ロックオンを解除した時フラグを折る
    ///  target(入力）：カメラの追跡対象
    /// </summary>
    private void UnlockDone(CharacterControlBase target)
    {
        IsRockOn = false;
        target.IsRockon = false;
        Enemy = null;
        RockOnTarget.Clear();
		// カメラを戻す
		RotX = Mathf.Asin(this.height / this.Distance) * Mathf.Rad2Deg;
    }

    public void LateUpdate()
    {
        // 敵が死んだら強制ロック解除
        if (Enemy == null)
        {
            IsRockOn = false;
        }

        // ロックオンしていない場合
        if (!IsRockOn)
        {
            // 方向転換をした場合いきなり反対側に飛ぶという現象を解除するためにLateUpdate
            // 一旦カメラの位置が方向転換後の背中の後ろへ飛ぶ（カメラの位置をもとにして方向を計算しているため,方向転換時に一旦カメラがその後ろへ行く）
            // LateUpdateは処理が終わってから行われるので

            // キー入力により、Y軸中心に視点を回転
            //// デフォルトではマウス入力を取得する構造になっているため、Edit→ProjectSettings→InputでInputを作る          
            if (ControllerManager.Instance.AzimuthLeft)  // 左回転
            {
                this.RotY -= 50.0f * Time.deltaTime;
            }
            else if (ControllerManager.Instance.AzimuthRight)  // 右回転
            {
                this.RotY += 50.0f * Time.deltaTime;
            }
			// 縦回転
			//Debug.Log(Input.GetAxisRaw("Vertical3"));
			//if (Input.GetAxisRaw("Vertical3") > 0)
			//{
			//	// 下に傾いている
			//	this.RotX -= 50.0f * Time.deltaTime;
			//}
			//else if(Input.GetAxisRaw("Vertical3") < 0)
			//{
			//	// 上に傾いている
			//	this.RotX += 50.0f * Time.deltaTime;
			//}

            // 視点は本体より上にするので角度だけを定義する. θ=Sin-1(高さ/斜辺）で出せる(Mathf.Asinはradian出力な点に注意)
			//this.RotX = Mathf.Asin(this.height / this.Distance) * Mathf.Rad2Deg;
            // クォータニオンを計算(X,Y,Z軸の回転角度を指定してクォータニオン(3軸をまとめた回転軸みたいなもの)を作成する）
            var q = Quaternion.Euler(this.RotX, this.RotY, 0.0f);   // Z軸方向を考慮していない(=敵ロックをする場合は互いに飛び回るのでZの計算が必須になるので後で追加が要る）
            // 注視点を少し上にずらす
            Vector3 target_pos = this.Player.transform.position;
            target_pos.y = target_pos.y + this.gaze_offset;


            transform.position = target_pos - q * (Vector3.forward * Distance);
            // クォータニオンを角度へ
            transform.rotation = q;

        }
        // ロックオンしている場合（ガンダムやバーチャロン同様常時自機の背後にカメラが移動するようになる）
        else if(IsRockOn)
        {
            // 配置位置（自機）
            Vector3 nowpos = this.Player.transform.position;
            // 配置位置（敵機）
            Vector3 epos = this.Enemy.GetComponentInChildren<CharacterControlBase>().RockonCursorPosition.transform.position;

            // 敵のコライダの位置を拾う
            //CapsuleCollider ecapsule = this.Enemy.GetComponentInChildren<CapsuleCollider>();
           
            
            // ロックオン対象を取得する
            CharacterControlBase rockontarget = this.Enemy.GetComponentInChildren<CharacterControlBase>();
                     
            
            // 敵機と自機の位置関係を正規化する（＝敵機と自機の相対位置関係が分かる）
            Vector3 positional_relationship = Vector3.Normalize(nowpos - epos);


            // 配置位置を自機より少し上にして、正規化分×距離を足す
            // 視点を少し上にずらす
            nowpos.y = nowpos.y + this.gaze_offset + 2.5f;
            // カメラの位置を変更
            transform.position = new Vector3(nowpos.x + positional_relationship.x * Distance_mine, nowpos.y + positional_relationship.y * Distance_mine, nowpos.z + positional_relationship.z * Distance_mine);
            // カメラの方向を変更(LookRotationメソッドで、常に第1引数？側の方向を向く
            transform.rotation = Quaternion.LookRotation(epos - transform.position);
            
        }
        // 非ロックオン時のみ障害物を避ける（障害物の手前にカメラを持ってくる）
        // ロックオンしていない場合
        if (!IsRockOn)
        {
            RaycastHit hitInfo;
            CapsuleCollider pc = Player.GetComponent<CapsuleCollider>(); 
            Vector3 playerPosition = Player.transform.position;
            playerPosition.y += pc.height;        // transform.positionは足元をとるので、コライダの高さ分上方向に補正する
            if (Physics.Linecast(playerPosition, transform.position, out hitInfo, 1 << LayerMask.NameToLayer("ground")))
            {
                transform.position = hitInfo.point;
                // ただしカメラと本体が極端に近づいた場合は、カメラを押し上げて見下ろす
                if (Vector3.Distance(transform.position, playerPosition) < 8.0f)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 4.5f, transform.position.z);
                    playerPosition.y += 0.8f;
                    transform.LookAt(playerPosition);
                }
            }
        }
    }

      

   
}

/// <summary>
/// カメラの特殊状態を定義
/// </summary>
public enum MainCameraMode
{
    NORMAL,                 // 通常
    HOMURABOWAROUSAL,       // 弓ほむら覚醒突進
}

