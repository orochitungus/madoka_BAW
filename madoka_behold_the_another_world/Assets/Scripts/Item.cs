﻿using UnityEngine;
using System.Collections;

// アイテムを表すクラス
public static class Item
{
	// アイテム管理クラス
    public static ItemSpec[] itemspec = new ItemSpec[]
    {
        new ItemSpec("まじょのひげ薬",ItemSpec.ItemFunction.REBIRETH_HP,false,30,"キャラクターのHPを30％回復",100),
        new ItemSpec("まじょのねっこ薬",ItemSpec.ItemFunction.REBIRETH_HP,false,50,"キャラクターのHPを50％回復",150),
        new ItemSpec("まじょの蜜薬",ItemSpec.ItemFunction.REBIRETH_HP,false,100,"キャラクターのHPを100％回復",300),
        new ItemSpec("ふしぎなシロップ",ItemSpec.ItemFunction.REBIRETH_HP,true,50,"味方全体のHP50％回復",500),
        new ItemSpec("おじょうさまリキュール",ItemSpec.ItemFunction.REBIRETH_HP,true,100,"味方全体のHP100％回復",1000),
        new ItemSpec("まほうのなみだ",ItemSpec.ItemFunction.REBIRTH_DEATH,false,50,"味方全体の戦闘不能をHPを50％まで回復",500),
        new ItemSpec("まほうの結晶",ItemSpec.ItemFunction.REBIRTH_FULL,true,100,"味方全体の戦闘不能とHPを全回復",2000),
        new ItemSpec("グリーフシード・キューブ",ItemSpec.ItemFunction.REBIRTH_SOUL,true,20,"キャラ全体のソウルジェム汚染率を20％回復",3000),
        new ItemSpec("グリーフシード",ItemSpec.ItemFunction.REBIRTH_SOUL,true,100,"キャラ全体のソウルジェム汚染率を全回復",10000)
    };
}
