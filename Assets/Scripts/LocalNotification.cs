using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

public class LocalNotification : MonoBehaviour {
	static AndroidJavaObject    m_plugin = null;
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
		m_plugin = new AndroidJavaObject( "com.moritanian.localpush.Notifier" );
		//m_plugin = new AndroidJavaObject( "jp.relbox.Notification.localNotification" );

#endif
	}
	
	public static void SetNotification(){
#if UNITY_ANDROID && !UNITY_EDITOR
		
		if (m_plugin != null){
			Debug.Log("SetNotification");
			m_plugin.Call("sendNotification", (long)(Now() + 5), primary_key, "test_notify", "test_title", "test_content"  );
			primary_key++;
		}
#endif
	}

	public static void ClearNotification(){

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
