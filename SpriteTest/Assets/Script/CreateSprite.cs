﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSprite : MonoBehaviour 
{
	public Texture2D ORTexture;

	public SpriteRenderer Renderer;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    public void OnClickGoButton()
    {
        StartCoroutine(Initialize());
    }

	public IEnumerator Initialize()
	{
		// 総フレーム数
		int totalframenum = 8;

		// 各フレームRect
		Rect[] rect = new Rect[totalframenum];

		// 各フレームpivot
		Vector2[] pivot = new Vector2[totalframenum];

		// 各フレーム出現時間
		float[] appearTimePerFrame = new float[totalframenum];

		// 表示順番
		int[] appearOrder = new int[totalframenum];

		// 生成されるスプライト
		Sprite[] sprite = new Sprite[totalframenum];


		// 総出現回数
		int totalappearnum = 3;

		// 出現回数ごとの開始前待ち時間
		float[] beforeWaitApperTime = new float[totalappearnum];

		// 出現回数ごとの終了前待ち時間
		float[] afterWaitApperTime = new float[totalappearnum];
				


		// 数値代入
		rect[0] = new Rect(0, 0, 128, 64);
		rect[1] = new Rect(128, 0, 128, 64);
		rect[2] = new Rect(0, 64, 128, 64);
		rect[3] = new Rect(128, 64, 128, 64);
		rect[4] = new Rect(0, 128, 128, 64);
		rect[5] = new Rect(128, 128, 128, 64);
		rect[6] = new Rect(0, 192, 128, 64);
		rect[7] = new Rect(128, 192, 128, 64);

		pivot[0] = Vector2.zero;
		pivot[1] = Vector2.zero;
		pivot[2] = Vector2.zero;
		pivot[3] = Vector2.zero;
		pivot[4] = Vector2.zero;
		pivot[5] = Vector2.zero;
		pivot[6] = Vector2.zero;
		pivot[7] = Vector2.zero;

		appearTimePerFrame[0] = 0.25f;
		appearTimePerFrame[1] = 0.25f;
		appearTimePerFrame[2] = 0.25f;
		appearTimePerFrame[3] = 0.25f;
		appearTimePerFrame[4] = 0.25f;
		appearTimePerFrame[5] = 0.25f;
		appearTimePerFrame[6] = 0.25f;
		appearTimePerFrame[7] = 0.25f;

		appearOrder[0] = 0;
		appearOrder[1] = 1;
		appearOrder[2] = 2;
		appearOrder[3] = 3;
		appearOrder[4] = 4;
		appearOrder[5] = 5;
		appearOrder[6] = 6;
		appearOrder[7] = 7;

        // 開始前ウェイト
        beforeWaitApperTime[0] = 0.1f;
        beforeWaitApperTime[1] = 0.1f;
        beforeWaitApperTime[2] = 0.1f;

        // 終了後ウェイト
        afterWaitApperTime[0] = 0.1f;
        afterWaitApperTime[1] = 0.1f;
        afterWaitApperTime[2] = 0.1f;

		// スプライト生成
		for(int i=0; i<totalframenum; i++)
		{
			sprite[i] = Sprite.Create(ORTexture, rect[i], pivot[i]);
		}

		// 規定回数表示
		//for(int i=0; i<totalappearnum; i++)
		//{
  //          // 開始前の待ち状態
		//	yield return new WaitForSeconds(beforeWaitApperTime[i]);
  //          // フレーム分アニメーションを表示
  //          for (int j = 0; j < totalframenum; j++)
  //          {
  //              // どれを再生する？
  //              Renderer.sprite = sprite[appearOrder[j]];
  //              // 待ち時間を入れる
  //              yield return new WaitForSeconds(appearTimePerFrame[j]);
  //          }
  //          // 終了後の待ち状態
  //          yield return new WaitForSeconds(afterWaitApperTime[i]);
  //          // スプライト消去
  //          Renderer.sprite = null;
		//}

        for(int i=0; i<totalappearnum * totalframenum; i++)
        {
            // 開始前の待ち状態
            if (i% totalframenum == 0)
            {                
                yield return new WaitForSeconds(beforeWaitApperTime[i/ totalframenum]);                
            }
            // どれを再生する？
            Renderer.sprite = sprite[appearOrder[i%totalappearnum]];
            // 待ち時間を入れる
            yield return new WaitForSeconds(appearTimePerFrame[i % totalappearnum]);
            // 終了後の待ち時間
            if((i + 1)% totalframenum == 0)
            {
                yield return new WaitForSeconds(afterWaitApperTime[i / totalframenum]);
                // スプライト消去
                Renderer.sprite = null;
            }
        }
	}
}
