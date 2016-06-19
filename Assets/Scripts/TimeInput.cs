using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/*
 時間ドロップダウン入力管理
 不正な値が入った時は値をもとに戻す
*/
public class TimeInput : MonoBehaviour {

	// ドロップダウン
	public Dropdown DpHour1;
	public Dropdown DpHour2;
	public Dropdown DpMin1;
	public Dropdown DpMin2;

	// ドロップダウン変化メソッド呼び出し時の識別用
	enum DropId{
		Hour1,
		Hour2,
		Min1,
		Min2
	}

	int hour = 0;
	int min = 0;

	// Use this for initialization
	void Start () {
		//初期値取得
		OnChangeDpDown(0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnChangeDpDown(int DpId){
		int _hour = _getHour();
		int _min = _getMin();
		// 時間が不正
		if(_hour == -1){
			DpHour1.value = hour / 10;
			DpHour2.value = hour % 10;
			return ;
		}
		// 分が不正
		if(_min == -1){
			DpMin1.value = min / 10;
			DpMin2.value = min % 10;
			return ;
		}
		// 正確だった場合は値更新
		hour = _hour;
		min = _min;
	}

	public int GetHour(){
		return hour;
	}
	public int GetMin(){
		return min;
	}

	int _getHour(){
		int _hour = DpHour1.value*10 + DpHour2.value;
		if(0<= _hour && _hour < 24)return _hour;
		return -1;
	}

	int _getMin(){
		int _min = DpMin1.value*10 + DpMin2.value;
		if(0<= _min && _min <60)return _min;
		return -1;
	}
}
