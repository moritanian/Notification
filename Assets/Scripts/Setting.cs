using UnityEngine;
using System.Collections;

public class Setting : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClickGoMain(){
		Body.GoBoardMain();
	}
	public void OnClickDataDelete(){
		Debug.Log("OnClickDataDelete");
		Dialog dialog = MyCanvas.FindChild<Dialog>("DialogBack");
		dialog.Revive();
		dialog._yesCallBack = new YesCallBack(deleteData);
		
	}
	void  deleteData(){
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Debug.Log("DeleteAllData!!");
		Main _main = MyCanvas.Find<Main>("BoardMain");
		_main.Reload();
	}

	// Todo 一覧を再読み込みして表示
	public void OnClickReload(){
		Main _main = MyCanvas.Find<Main>("BoardMain");
		_main.Reload();
	}
}
