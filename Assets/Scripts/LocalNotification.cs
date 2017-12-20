using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

public class LocalNotification : MonoBehaviour {
	static AndroidJavaObject    m_plugin = null;
	static AndroidJavaClass unityPlayer = null;
	static AndroidJavaObject context = null;

	static GameObject           m_instance;
	private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);	
	private static int primary_key;

	public void Awake(){
		Debug.Log("localNotification_C#script");
        // gameObject変数はstaticでないのでstatic関数から呼ぶことが出来ない.
        // そのためstaticの変数にあらかじめコピーしておく.
		m_instance = gameObject;
		primary_key = 0;

#if UNITY_ANDROID && !UNITY_EDITOR
        // プラグイン名をパッケージ名+クラス名で指定する。
		m_plugin = new AndroidJavaObject( "example.com.alarmplugin.MyAlarmManager" );
		unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		context  = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

#endif
	}
	
	public static void LocalCallSet(int id, DateTime call_time, string name, string title, string label, string type = ""){
		// 前回分をreset
		LocalCallReset(id);

		// #TODO unique id で識別するようにする
		int seconds_from_now = (int)SecondsFromNow(call_time);
		Debug.Log("SetUpLocalSet id:" + id.ToString() + " label" + label + " " + seconds_from_now.ToString());
	#if UNITY_ANDROID && !UNITY_EDITOR
		m_plugin.CallStatic("addNotification", context, id, name , title, label, (int)seconds_from_now);
	#else
		Debug.LogWarning("This asset is for Android only. It will not work inside the Unity editor!");
	#endif

	}

	public static bool LocalCallReset(int id){
		Debug.Log("localCallReset " + id.ToString());
	#if UNITY_ANDROID && !UNITY_EDITOR
		//send notification here
		m_plugin.CallStatic("resetNotification", context, id);
	#else
		Debug.LogWarning("This asset is for Android only. It will not work inside the Unity editor!");
		return false;
	#endif
		return true;
	}

	public static long SecondsFromNow(DateTime dt){
		return FromDateTime(dt) - Now();
	}
	
	/*===========================================================================*/
	/**
	 * 現在時刻からUnixTimeを計算する.
	 *
	 * @return UnixTime.
	 */
	public static long Now()
	{
		return ( FromDateTime( DateTime.UtcNow ) );
	}

	/*===========================================================================*/
	/**
	 * UnixTimeからDateTimeに変換.
	 *
	 * @param [in] unixTime 変換したいUnixTime.
	 * @return 引数時間のDateTime.
	 */
	public static DateTime FromUnixTime( long unixTime )
	{
		return UNIX_EPOCH.AddSeconds( unixTime ).ToLocalTime();
	}

	/*===========================================================================*/
	public static long FromDateTime( DateTime dateTime )
	{
		double nowTicks = ( dateTime.ToUniversalTime() - UNIX_EPOCH ).TotalSeconds;
		return (long)nowTicks;
	}
}
