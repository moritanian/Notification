using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class ToggleDialog : Dialog {

	int id = 0;
	int toggleMax = 0;
	List<DialogAction> callbacks;
	List<string> texts;
	ToggleGroup tglGroup;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Set(List<string> titles, List<string> texts,  List<DialogAction> callbacks, int id = 0){
		// 長さ設定
		this.toggleMax =  titles.Count < callbacks.Count ? callbacks.Count : titles.Count;
		if(this.toggleMax <= id) id = 0;
		
		if(!this.gameObject.activeInHierarchy){
			Revive();
			tglGroup = FindChild<ToggleGroup>("ToggleGroup");
		}
		this.callbacks = callbacks;
		this.texts = texts;
		SetToggleDialog(id);
		int count = 0;
		foreach (Transform child in tglGroup.transform){
			ToggleObj tglObj = child.gameObject.GetComponent<ToggleObj>();
			if(tglObj != null)tglObj.Label = titles[count];
			if(tglObj.Id == id){
				tglObj.IsOn = true;
			}
			count++;
		}

		SetCancelAction (null);

	}

	// 
	void SetToggleDialog(int id){
		this.id = id;
		this._yesCallBack = callbacks[id];
		Text = texts[id];
	}

	public void OnToggle(bool isOn){
		// スペクタ上で初めのやつだけtrueに。初めの(オフになったトグルからも呼ばれる)
		// #TODO これonのやつのみ発火するようにしたい
		if(!isOn)return ;
		IEnumerable<Toggle> toggles = tglGroup.ActiveToggles();
		if(toggles.Count() > 0){
			Toggle tgl = toggles.First();
			Debug.Log(tgl.ToString());
			ToggleObj tglObj = tgl.gameObject.GetComponent<ToggleObj>();
			SetToggleDialog(tglObj.Id);
		}
	}

	public override void OnClickYes(){
		_yesCallBack();
		Vanish();
	}

}
