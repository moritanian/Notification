using UnityEngine;
using System.Collections;
using System;

public class AndroidSamp : MonoBehaviour {
	private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	int state = 0;
	int count = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	public void DebugAction(){
		if(state == 0){
			Debug.Log("AndroidSamp");
			HogeScript.CallFuncA("callA");
			Debug.Log(HogeScript.CallFuncB("callB"));
			HogeScript.CallFuncC("callC");
			state = 1;
		}else{
			state = 0;
			LocalNotification.SetNotification();
		}
	}

	public void SetNotify(int id = 0){
		//Debug.Log("SetNotify");
		//LocalNotification.SetNotification();
		if(id != 0)count = id;
		Debug.Log("TestNotify" + count);
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject ajc = new AndroidJavaObject("com.moritanian.localpush.Notifier");
           // AndroidJavaObject ajc = new AndroidJavaObject("com.zeljkosassets.notifications.Notifier");
         
         	ajc.CallStatic("sendNotification", count, "Test Name " + count.ToString(), "Test Title "+ count.ToString(), "Test Label "+ count.ToString(), 5);
#else
            Debug.LogWarning("This asset is for Android only. It will not work inside the Unity editor!");
#endif
	}
	public void DebugSet(int id  = 0){
		LocalCallSet(id, DateTime.Now.AddSeconds(5), "Test Name" + id.ToString(), "Test Title" + id.ToString(), "Test Label" + id.ToString());
	}

	public void DebugReset(int id = 0){
		LocalCallReset(id);
	}
	// Update is called once per frame
	void Update () {
	
	}

	public void SampleCall(){
#if UNITY_ANDROID && !UNITY_EDITOR
            //send notification here
			count++;

			AndroidJavaObject ajc = new AndroidJavaObject("com.moritanian.localpush.Notifier");
           // AndroidJavaObject ajc = new AndroidJavaObject("com.zeljkosassets.notifications.Notifier");
            ajc.CallStatic("sendNotification", "Test Name " + count.ToString(), "Test Title "+ count.ToString(), "Test Label "+ count.ToString(), 5);
#else
            Debug.LogWarning("This asset is for Android only. It will not work inside the Unity editor!");
#endif

	  
	}

	public static void LocalCallSet(int id, DateTime call_time, string name, string title, string label, string type = ""){
		// 前回分をreset
		LocalCallReset(id);
		
		// #TODO unique id で識別するようにする
		int seconds_from_now = (int)SecondsFromNow(call_time);
		Debug.Log("SetUpLocalSet id:" + id.ToString() + " label" + label + " " + seconds_from_now.ToString());
#if UNITY_ANDROID && !UNITY_EDITOR
		//send notification here
		AndroidJavaObject ajc = new AndroidJavaObject("com.moritanian.localpush.Notifier");
       // AndroidJavaObject ajc = new AndroidJavaObject("com.zeljkosassets.notifications.Notifier");
        ajc.CallStatic("sendNotification", id, name , title, label, (int)seconds_from_now);
#else
        Debug.LogWarning("This asset is for Android only. It will not work inside the Unity editor!");
#endif

	}

	public static bool LocalCallReset(int id){
		Debug.Log("localCallReset " + id.ToString());
#if UNITY_ANDROID && !UNITY_EDITOR
		//send notification here
        //AndroidJavaObject ajc = new AndroidJavaObject("com.zeljkosassets.notifications.Notifier");
        AndroidJavaObject ajc = new AndroidJavaObject("com.moritanian.localpush.Notifier");
        
        ajc.CallStatic("resetNotification", id);
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
