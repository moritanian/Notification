using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text; //Encoding

public class FilePlugin : MonoBehaviour {

	static AndroidJavaObject    m_plugin = null;
	// Use this for initialization
	void Awake () {
		#if UNITY_ANDROID && !UNITY_EDITOR 
		if (m_plugin == null)
		m_plugin = new AndroidJavaClass("example.com.fileplugin.FileDirManager");
		#endif
	}

	public static string GetDownloadPath(string filename = ""){
		string downloadPath = "";
		#if UNITY_ANDROID && !UNITY_EDITOR 
		downloadPath = m_plugin.CallStatic<string>("getDownloadPath", filename);
		#else
		downloadPath = Environment.GetEnvironmentVariable ("USERPROFILE") + @"\"
			+"Downloads" + @"\" + filename;
		#endif

		return downloadPath;
	}	


	
	public static void WriteFile(string filePath, string text){
		#if UNITY_ANDROID && !UNITY_EDITOR 
		m_plugin.CallStatic("writeFile", filePath, text);
		#else
		StreamWriter sw;
		FileInfo fi;
		fi = new FileInfo(filePath);
		sw = fi.CreateText();
		sw.WriteLine (text);
		sw.Flush();
		sw.Close ();

		#endif

	}

	public static string ReadFileAsText(string filePath){
		string text = "";
		#if UNITY_ANDROID && !UNITY_EDITOR 
		text = m_plugin.CallStatic<string>("readFileAsText", filePath);
		#else
		FileInfo fi = new FileInfo(filePath);
		try {
			// 一行毎読み込み
			using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)){
				text = sr.ReadToEnd();
			}
		} catch (Exception e){
			// 改行コード
		}
		#endif

		return text;
	}

	public static bool ExistsFile(string filePath){
		bool exists;

		#if UNITY_ANDROID && !UNITY_EDITOR 
		exists = m_plugin.CallStatic<bool>("existsFile", filePath);
		#else
		FileInfo fi = new FileInfo(filePath);
		exists = fi.Exists;
		#endif

		return exists;
	}

	public static void ShowFileSelector(string mimeType){
	
		#if UNITY_ANDROID && !UNITY_EDITOR 
		m_plugin.CallStatic("showFileSelector", mimeType);
		#else
		Debug.LogWarning("showFileSelector only work in android");
		#endif
	
	}

}
