using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text; //Encoding


public class FileIo  {

	public static void Delete(string filename){
		if(isRunningOnAndroid()){
			_deleteAndroid(filename);
		}else{
			_deleteWin(filename);
		}

	} 
	static void _deleteWin(string filename){
		string path = Application.dataPath + "/AppData/"+ filename;
		File.Delete(path);
		Debug.Log("delete file " + path);
	} 
	static void _deleteAndroid(string filename){
		string path = getFileName(filename);
		File.Delete(path);
		Debug.Log("delete file " + path);
	} 

	public static void Write(string filename, string text){
		if(isRunningOnAndroid()){
			_writeAndroid(filename,text);
		}else{
			_writeWin(filename, text);
		}
	}

	static void _writeWin(string filename, string text){
		string path = Application.dataPath + "/AppData/"+ filename;
		_write (path, text);
	}
	static void _writeAndroid(string filename, string text){
		
		string path = getFileName(filename);
		/*
		if (!Directory.Exists(Application.persistentDataPath)){
        	Directory.CreateDirectory(path);
    	}
    	*/
		_write (path, text);

	}

	static void _write(string fullpath, string text){
		StreamWriter sw;
		FileInfo fi;
		fi = new FileInfo(fullpath);
		sw = fi.CreateText();
		sw.Write (text);
		sw.Flush();
		sw.Close ();
		Debug.Log (fullpath);
	}	

	public static string Load(string filename){
		if(isRunningOnAndroid()){
			return _loadAndroid(filename);
		}
		return _loadWin(filename );
	}

	static string _loadWin(string filename){
		string path = Application.dataPath + "/AppData/"+ filename;
		return _load(path);
	}

	static string _loadAndroid(string filename){
		string path = getFileName(filename);
		return _load (path);
	}

	static string _load(string fullpath){
		string text = "";
		FileInfo fi = new FileInfo(fullpath);
		try {
			// 一行毎読み込み
			using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)){
				text = sr.ReadToEnd();
			}
		} catch (Exception e){
			// 改行コード
			text += SetDefaultText();
		}
		Debug.Log("Load!! " + fullpath);
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
