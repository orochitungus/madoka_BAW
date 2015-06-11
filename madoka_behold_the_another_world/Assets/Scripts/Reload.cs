using UnityEngine;
using System.Collections;

public class Reload 
{
	// 一発ずつ回復
    // nowbullet    [ref]:該当する射撃の弾丸数
    // nowtime      [in] :現在時刻
    // maxbul       [in] :弾丸の最大数
    // reloadtime   [in] :リロードまでの時間
    // shotendtime  [ref]:撃ち終わった時間
    public void OneByOne(ref int nowbullet, float nowtime, int maxbul, float reloadtime, ref float shotendtime)
    {
        // 途中で成長により増えた場合を考慮して、Max-1で撃ち終わりが0になったら今の時間をTimeに設定する（MAX状態でも成長分の回復が始まる）
        if (shotendtime == 0.0f && nowbullet < maxbul)
        {
            shotendtime = Time.time;
        }

        // 残弾数がmaxになっていないならリロード処理
        if (nowbullet < maxbul)
        {
            // 現在時間＞撃ち終わった時間＋リロードまでの時間を満たすと
            //→弾数1発加算（MAX時はなにもしない）
            if (nowtime > shotendtime + reloadtime)
            {
                nowbullet++;
                //→残弾数Max-1回目以降は「現在の時間」が「撃ち終わった時間」になり、次の弾丸の回復を始める
                //if (nowbullet < maxbul - 1)
                {
                    shotendtime = nowtime;
                }
            }
        }
        // 残弾数がmaxになったら、shotendtimeを0に戻す
        else
        {
            shotendtime = 0.0f;
        }
    }

    // 撃ち切ってから回復
    // nowbullet    [ref]:該当する射撃の弾丸数
    // nowtime      [in] :現在時刻
    // maxbul       [in] :弾丸の最大数
    // reloadtime   [in] :リロードまでの時間
    // shotendtime  [ref]:撃ち終わった時間
    // return            :リロード完了の有無
    public void AllTogether(ref int nowbullet, float nowtime, int maxbul, float reloadtime, ref float shotendtime)
    {
        // 現在時間＞撃ち終わった時間＋リロードまでの時間
        //→0発であることを条件に最大まで回復
        if (nowbullet == 0)
        {
            if (nowtime > shotendtime + reloadtime)
            {
                nowbullet = maxbul;
            }
        }
        // 全回復した場合は、shotendtimeを0に戻す
        else if (nowbullet == maxbul)
        {
            shotendtime = 0.0f;
        }
    }
    
}
