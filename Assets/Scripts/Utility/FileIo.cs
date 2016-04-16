using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text; //Encoding


public class FileIo  {

	public static void Delete(string filename){
		string path = Application.dataPath + "/AppData/"+ filename;
		File.Delete(path);
		Debug.Log("delete file " + path);
	} 

	public static void Write(string filename, string text){
		StreamWriter sw;
		FileInfo fi;
		string path = Application.dataPath + "/AppData/"+ filename;
		fi = new FileInfo(path);
		sw = fi.CreateText();
		sw.WriteLine (text);
		sw.Flush();
		sw.Close ();
		Debug.Log (path);

	}

	public static string Load(string filename){
		string text = "";
		string path = Application.dataPath + "/AppData/"+ filename;
		FileInfo fi = new FileInfo(path);
		try {
			 // 一行毎読み込み
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)){
                text = sr.ReadToEnd();
            }
        } catch (Exception e){
            // 改行コード
            text += SetDefaultText();
        }
		return text;
	}

    // 改行コード処理
    static string SetDefaultText(){
        return "";
    }

    public static string getFileName(string baseName) {
        if ( isRunningOnAndroid () ) {
            return Application.persistentDataPath + "/" + baseName;
        }
        return baseName;
    }
    public static bool isRunningOnAndroid() {
        return (Application.platform == RuntimePlatform.Android);
    }

}
