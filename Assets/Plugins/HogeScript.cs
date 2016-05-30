using UnityEngine;
using System.Collections;

public class HogeScript: MonoBehaviour {
	static AndroidJavaObject    m_plugin = null;
	static GameObject           m_instance;

	public void Awake () {
		Debug.Log("Android");
        // gameObject変数はstaticでないのでstatic関数から呼ぶことが出来ない.
        // そのためstaticの変数にあらかじめコピーしておく.
		m_instance = gameObject;
#if UNITY_ANDROID && !UNITY_EDITOR
        // プラグイン名をパッケージ名+クラス名で指定する。
		m_plugin = new AndroidJavaObject( "jp.relbox.Notification.Hoge" );
#endif
	}


    // NativeコードのFuncA 関数を呼び出す.
    // Native側のコードが引数を持たない関数なら、
    // m_plugin.Call("FuncA"); のように引数を省略すればOK。
	public static void CallFuncA(string str)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		Debug.Log("CallFuncA Android only");
		if (m_plugin != null){
			m_plugin.Call("FuncA", str);
		}
#endif
	}


    // NativeコードのFuncB 関数を呼び出す.
	public static string CallFuncB(string str)
	{
		string modoriValue = null;
#if UNITY_ANDROID && !UNITY_EDITOR
		if (m_plugin != null){
			modoriValue = m_plugin.Call<string>("FuncB", str);
		}
#endif
		return modoriValue;
	}


    // NativeコードのFuncC 関数を呼び出す.
	public static void CallFuncC(string str)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (m_plugin != null){
			Debug.Log("CallFuncC");
			// 2つめの引数はscene内のオブジェクト(コールバックされる関数がかかれているクラスがアタッチされている)
			m_plugin.Call("FuncC","Android", str);
		}
#endif
	}


    // ネイティブコードから呼ばれる関数
    // publicでかつ、非static関数でないと呼ばれない.
	public void onCallBack(string str)
	{
		Debug.Log("Call From Native. (" + str + ")");
	}

}