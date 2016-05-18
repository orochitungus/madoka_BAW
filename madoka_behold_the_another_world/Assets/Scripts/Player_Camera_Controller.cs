using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    public Vector3      m_rockoncursorpos;          // ロックオンカーソルの配置位置

    public Camera       m_perspectiveCamera;

    // ロックオン対象(何個あるかわからないので)
    public List<GameObject> RockOnTarget;
    public int          m_nowTarget;                // 現在ロックオンしている相手

    // ロックオンカーソル用テクスチャ
    // 射程距離内（赤）
    public Texture2D m_RockonCursor_Red;

    // 射程距離外（緑）
    public Texture2D m_RockonCurosor_Green;

    // 攻撃不可（黄色）
    public Texture2D m_RockonCursor_Yellow;


    public GUISkin   m_guiskin;

    // 有効範囲か否か
    public bool m_isRockonRange;

    // CPU
    // ルーチンを拾う
    AIControl.CPUMODE m_cpumode;
    // 入力を拾う
    AIControl.KEY_OUTPUT m_key; 

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

	// Use this for initialization
	void Start () 
    { 
        // フィールド変数を初期化する
        this.Distance = 20.0f;
        this.SensitivityX = 50.0f;
        this.SensitivityY = 50.0f;
        this.RotX = 0.0f;
        this.RotY = 0.0f;
        this.height = 4.0f;
        this.gaze_offset = 5.5f;
        this.rockon_height = 0.0f;
        // ロックオンの有無
        IsRockOn = false;
		// 覚醒技演出中の有無
		IsArousalAttack = false;
        this.m_isRockonRange = false;
        this.m_enemy_offset_height = 0.0f;
        this.Distance_mine = 18.0f;

        //Application.targetFrameRate = 60;
        var target = Player.GetComponentInChildren<CharacterControl_Base>();    // 戦闘用キャラの場合
        var Camera = this.GetComponent<AudioListener>();
        // 戦闘用キャラ
        if (target != null)
        {
            if (target.IsPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
            {
                // CPU時、AudioListenerを解除           
                Camera.enabled = false;
                m_DrawInterface = false;
            }
            else
            {
                // プレイヤー時、AudioListenerを有効化
                Camera.enabled = true;
                // FPSを固定しておく
                Application.targetFrameRate = 60;
                // インターフェースを有効化しておく
                m_DrawInterface = true;
            }
        }
        // クエストパート用キャラ
        else
        {
            Camera.enabled = true;
            Application.targetFrameRate = 60;
            m_DrawInterface = false;
        }
		// 視点を初期化
		this.RotX = Mathf.Asin(this.height / this.Distance) * Mathf.Rad2Deg;
	}
	
	// Update is called once per frame
	void Update () 
    {
        // ロックオン（非ロックオン時）
        // このカメラが追跡しているオブジェクトの情報を拾う   
        CharacterControl_Base target = Player.GetComponentInChildren<CharacterControl_Base>();
        var targetAI = Player.GetComponentInChildren<AIControl_Base>();

        // クエストパート時、使わないので抜ける
        if (target == null)
            return;

        // CPU時、CPUの情報を拾う
        if (target.IsPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
        {
            // ルーチンを拾う
            this.m_cpumode = targetAI.m_cpumode;
            // 入力を拾う
            this.m_key = targetAI.m_keyoutput; 
        }

        // 解除入力が行われた
        if (target.IsRockon && target.HasSearchCancelInput)
        {
            UnlockDone(target);
        }
        // ロックオンボタンが押された（この判定はCharacterControl_Baseの派生側で拾う）
        else if (target.GetSearchInput() || this.m_key == AIControl.KEY_OUTPUT.SEARCH)
        {
            // ロックオンしていなかった
            if (!target.IsRockon)
            {
                // 自分がPC側か敵側か取得する
                switch (target.IsPlayer)
                {
                    // PC側の場合、敵を検索する
                    // 自機
                    case CharacterControl_Base.CHARACTERCODE.PLAYER:
                    // 僚機
                    case CharacterControl_Base.CHARACTERCODE.PLAYER_ALLY:
                        if (!OnPushSerchButton(true,false))
                        {
                            return;
                        }
                        break;
                    // 敵側の場合、PC側のタグを検索する
                    case CharacterControl_Base.CHARACTERCODE.ENEMY:                   
                        switch (this.m_cpumode)
                        {
                            // 敵の哨戒モードの場合、起点か終点を検索する
                            // 終点へ向けて移動中
                            case AIControl.CPUMODE.OUTWARD_JOURNEY:
                                // 終点をロックオンする                                                             
                                if (target.EndingPoint == null)
                                {
                                    Debug.Log("EndingPoint isn't setting");
                                }
                                Enemy = target.EndingPoint;
                                IsRockOn = true;
                                target.IsRockon = true;                                
                                break;
                            // 起点へ向けて移動中
                            case AIControl.CPUMODE.RETURN_PATH:
                                // 起点をロックオンする
                                if (target.StartingPoint == null)
                                {
                                    Debug.Log("StartingPoint isn't setting");
                                }
                                Enemy = target.StartingPoint;
                                IsRockOn = true;
                                target.IsRockon = true;                                
                                break;                            
                            default:
                                // 外れたら哨戒に戻る
                                if (!OnPushSerchButton(false,false))
                                {
                                    this.m_cpumode = AIControl.CPUMODE.OUTWARD_JOURNEY;
                                    IsRockOn = false;
                                    target.IsRockon = false;
                                }
                                break;
                        }
                        break;
                }
                
            }
            // ロックオンしていた
            else
            {
                // 敵もしくは僚機の哨戒状態で目的地にたどり着いたときにこの状態になる
                if (target.IsPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
                {   // ここに指示が来る前に、cpumodeは切り替わっている
                    // 往路(終点をロックオン）
                    if (this.m_cpumode == AIControl.CPUMODE.OUTWARD_JOURNEY)
                    {
                        this.Enemy = target.EndingPoint;
                    }
                    // 復路（起点をロックオン）
                    else if(this.m_cpumode == AIControl.CPUMODE.RETURN_PATH)
                    {
                        this.Enemy = target.StartingPoint;
                    }
                }

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
        if (target.IsRockon && m_cpumode != AIControl.CPUMODE.OUTWARD_JOURNEY && m_cpumode != AIControl.CPUMODE.RETURN_PATH)
        {
            var rockontarget = Enemy.GetComponentInChildren<CharacterControl_Base>();
            if (rockontarget != null && rockontarget.NowHitpoint < 1)
            {
                UnlockDone(target);
            }
        }

	}

    // サーチボタンが押されたときの処理
    // playerside[in]       :プレイヤー側（true)
    // cpu[in]              :CPUであるか否か
    // output               :ロックオンする相手がいた
	// exitdown				:ダウンしていたら強制的にfalseを返す
    public bool OnPushSerchButton(bool playerside,bool cpu,bool exitdown = false)
    {
        CharacterControl_Base target = Player.GetComponentInChildren<CharacterControl_Base>();
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
			CharacterControl_Base rockontarget = RockOnTarget[most_near].GetComponent<CharacterControl_Base>();
			if(rockontarget.NowDownRatio >= rockontarget.DownRatio)
			{
				return false;
			}
		}

        // フラグをロックオン状態に切り替える
        IsRockOn = true;
        // 対象をmost_nearとする
        Enemy = RockOnTarget[most_near];
        // 対象のインデックスを保持する
        m_nowTarget = most_near;
        // PC側のロックオンフラグを立てる
        target.IsRockon = true;
        return true;
    }

	/// <summary>
	/// ロックオン対象が複数いるときの処理
	/// </summary>
	/// <param name="target">このカメラが追跡しているキャラクター</param>
	public void RockOnSelecter(CharacterControl_Base target)
	{
		// 自分＋1の相手を選択する
		int nexttarget = m_nowTarget + 1;
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
				if (m_nowTarget == nexttarget)
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
        var target = Player.GetComponentInChildren<CharacterControl_Base>();
        // とりあえず画面上にいる敵を全員検索する
        GameObject[] RockonCandidate = GameObject.FindGameObjectsWithTag(targettype);
        for (int i = 0; i < RockonCandidate.Length; ++i)
        {
            // 見つけたキャラとプレイヤーとの距離を測る
            float distance = Vector3.Distance(Player.transform.position, RockonCandidate[i].transform.position);
            // 距離がm_Rockon_RangeLimitの中に入っていたら、RockOnTargetに加える
            if (distance <= target.m_Rockon_RangeLimit)
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
	private void RockDone(CharacterControl_Base target, int nexttarget)
	{
		// フラグをロックオン状態に切り替える
		IsRockOn = true;
		// 対象をmost_nearとする
		Enemy = RockOnTarget[nexttarget];
		// 対象のインデックスを保持する
		m_nowTarget = nexttarget;
		// PC側のロックオンフラグを立てる
		target.IsRockon = true;
	}

    /// <summary>
    ///  ロックオンを解除した時フラグを折る
    ///  target(入力）：カメラの追跡対象
    /// </summary>
    private void UnlockDone(CharacterControl_Base target)
    {
        IsRockOn = false;
        target.IsRockon = false;
        this.Enemy = null;
        RockOnTarget.Clear();
		// カメラを戻す
		this.RotX = Mathf.Asin(this.height / this.Distance) * Mathf.Rad2Deg;
    }

    public void LateUpdate()
    {
        // 敵が死んだら強制ロック解除
        if (this.Enemy == null)
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
            if (Input.GetButton("Camera_L"))  // 左回転
            {
                this.RotY -= 50.0f * Time.deltaTime;
            }
            else if (Input.GetButton("Camera_R"))  // 右回転
            {
                this.RotY += 50.0f * Time.deltaTime;
            }
            // 右スティックでカメラを回転させる
			// 横回転
            if (Input.GetAxisRaw("Horizontal3") < 0)
            {
                //左に傾いている
                this.RotY -= 50.0f * Time.deltaTime;
            }
            else if (0 < Input.GetAxisRaw("Horizontal3"))
            {
                //右に傾いている
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


            transform.position = target_pos - q * (Vector3.forward * this.Distance);
            // クォータニオンを角度へ
            transform.rotation = q;

        }
        // ロックオンしている場合（ガンダムやバーチャロン同様常時自機の背後にカメラが移動するようになる）
        else if(IsRockOn)
        {
            // 配置位置（自機）
            Vector3 nowpos = this.Player.transform.position;
            // 配置位置（敵機）
            Vector3 epos = this.Enemy.transform.position;

            // 敵のコライダの位置を拾う
            //CapsuleCollider ecapsule = this.Enemy.GetComponentInChildren<CapsuleCollider>();
           
            
            // ロックオン対象を取得する
            CharacterControl_Base rockontarget = this.Enemy.GetComponentInChildren<CharacterControl_Base>();
            //if (rockontarget == null)
            //{                
            //    return;
            //}
           
            
            // 敵機と自機の位置関係を正規化する（＝敵機と自機の相対位置関係が分かる）
            Vector3 positional_relationship = Vector3.Normalize(nowpos - epos);


            // 配置位置を自機より少し上にして、正規化分×距離を足す
            // 視点を少し上にずらす
            nowpos.y = nowpos.y + this.gaze_offset + 2.5f;
            // カメラの位置を変更
            transform.position = new Vector3(nowpos.x + positional_relationship.x * Distance_mine, nowpos.y + positional_relationship.y * Distance_mine, nowpos.z + positional_relationship.z * Distance_mine);
            // カメラの方向を変更(LookRotationメソッドで、常に第1引数？側の方向を向く
            transform.rotation = Quaternion.LookRotation(epos - transform.position);
            // ロックオン対象の画面上での位置を取得する            
            // スクリーン上での座標を取得する
            
            if (rockontarget != null)
            {
                // Enemyは敵全体なので、rootを取ってみる
                GameObject enemyroot = rockontarget.gameObject.transform.FindChild("Mesh").gameObject;
                m_rockoncursorpos = m_perspectiveCamera.WorldToScreenPoint(enemyroot.transform.position);
            }
            // CPUの哨戒モード時のターゲットの時は考えない.
            else
            {
                m_rockoncursorpos = m_perspectiveCamera.WorldToScreenPoint(epos);
            }
        }
        // 非ロックオン時のみ障害物を避ける（障害物の手前にカメラを持ってくる）
        // ロックオンしていない場合
        if (!IsRockOn)
        {
            RaycastHit hitInfo;
            CapsuleCollider pc = Player.GetComponent<CapsuleCollider>(); //CharacterController pc = Player.GetComponent<CharacterController>();
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

    
    

    // 描画用関数
    // ロックオンカーソルを描画
    public void OnGUI()
    {
        // このカメラが追跡しているオブジェクトの情報を拾う   
        var target = Player.GetComponentInChildren<CharacterControl_Base>();                // 戦闘用.
        var target_quest = Player.GetComponentInChildren<CharacterControl_Base_Quest>();    // クエスト用.
        // このカメラが追跡している対象がCPUのときは書かない（多分無駄メモリ）
        if (target != null)
        {
            if (target.IsPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER || !m_DrawInterface)
            {
                return;
            }
        }
        if (target_quest != null)
        {
            if (target_quest.m_isPlayer != CharacterControl_Base_Quest.CHARACTERCODE.PLAYER || !m_DrawInterface)
            {
                return;
            }
        }

        // GUIスキンを設定
        if (m_guiskin)
        {
            GUI.skin = m_guiskin;
        }
        else
        {
            Debug.Log("No GUI skin has been set!");
        }
       
        // ロックオン状態になったら敵位置にカーソルを描画する（覚醒技演出中を除く）
        if (IsRockOn && !IsArousalAttack)
        {
            // 配置位置（自機）
            Vector3 nowpos = this.Player.transform.position;
            // 配置位置（敵機）
            Vector3 epos = this.Enemy.transform.position;
            
            Vector3 scale = new Vector3(Screen.width / MadokaDefine.SCREENWIDTH, Screen.height / MadokaDefine.SCREENHIGET, 1);

            // GUIが解像度に合うように変換行列を設定（計算で描画位置を出す場合Scaleを定義しておかないと解像度によって表示位置がずれる点に注意）
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
            
            // 大型キャラの場合はカーソルのY軸補正位置を関数化する（そうしないと上下にずれる）。なお、ここでの大型キャラとは人間以上のサイズを持つキャラのこと。
            // 計算はロックオン距離が100程度なので、とりあえずそれに合わせる
            // カーソルX軸位置オフセット値
            float cursorXoffset = -510.971f * (Screen.width/MadokaDefine.SCREENWIDTH) + 446.2571f;
            // カーソルY軸位置オフセット値
            float cursorYoffset = -269.714f * (Screen.height/MadokaDefine.SCREENHIGET) + 172.5714f;

            Vector3 position = new Vector3(m_rockoncursorpos.x + cursorXoffset, m_rockoncursorpos.y + cursorYoffset, m_rockoncursorpos.z);
            GUI.BeginGroup(new Rect(position.x, position.y, 256.0f, 256.0f));

            
            // ダウン値を超えている（ダウンか強制ダウン吹き飛び中）か、ダウン値を超えていてもダウンしているなら黄色ロック
            if (this.Enemy.GetComponentInChildren<CharacterControl_Base>().NowDownRatio >= this.Enemy.GetComponentInChildren<CharacterControl_Base>().DownRatio
                || Enemy.GetComponentInChildren<CharacterControl_Base>().m_AnimState[0] == CharacterControl_Base.AnimationState.Down)
            {
                GUI.DrawTexture(new Rect(0.0f, 0.0f, 128.0f, 128.0f), m_RockonCursor_Yellow);
            }
            // 有効範囲にいたら赤ロック
            else if (Vector3.Distance(nowpos, epos) <= this.Player.GetComponentInChildren<CharacterControl_Base>().m_Rockon_Range)
            {
                GUI.DrawTexture(new Rect(0.0f, 0.0f, 128.0f, 128.0f), m_RockonCursor_Red);
            }
            // そうでなければ緑ロック
            else
            {
                GUI.DrawTexture(new Rect(0.0f, 0.0f, 128.0f, 128.0f), m_RockonCurosor_Green);
            }
            GUI.EndGroup();
        }

    }
}
