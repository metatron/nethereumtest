using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


/**
 * 
 * https://qiita.com/divideby_zero/items/b6bbcc35aef08e1d7d3e
 * 
 * 
 */
public class TaskTest : MonoBehaviour {
	public UnityEngine.UI.InputField inputField;

	void Start () {
		Debug.Log ("Start ====");

		SynchronizationContext context = SynchronizationContext.Current;

		//別スレッドでループ作成
		Task.Run(async() => { await Func (context);});

		Debug.Log ("End ====");
	}

	async Task Func(SynchronizationContext context) {
		SynchronizationContext.SetSynchronizationContext(context);
		for (int i = 0; i < 10; i++) {
			//inputFieldをここで使用するとエラー
			await Task.Delay(1000);
			//inputFieldはawait以降で使用。
			Debug.Log ("Count: " + i);
			inputField.text = "Count: " + i;
		}
	}

}
