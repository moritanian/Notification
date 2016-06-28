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

	public InputField HourField;
	public InputField MinField;

	int hour = 0;
	int min = 0;

	// Use this for initialization
	void Start () {
		//初期値取得
		OnChangeTimeField();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTime(DateTime Dt){
		SetTime(Dt.Hour, Dt.Minute);
	} 

	public void SetTime(int _hour, int _min){
		hour =  hour;
		min = _min;
		HourField.text = formTimeFormat(_hour);
		MinField.text = formTimeFormat(_min);
	}

	public void OnChangeTimeField(){
		int _hour = _getHour();
		int _min = _getMin();
		// 時間が不正
		if(_hour == -1 || _hour < 10){
			HourField.text = formTimeFormat(hour);
		}
		// 分が不正
		if(_min == -1 || _min < 10){
			MinField.text = formTimeFormat(min);
		}
	}

	public int GetHour(){
		return hour;
	}
	public int GetMin(){
		return min;
	}

	int _getHour(){
		int _hour = int.Parse(HourField.text);
		if(0<= _hour && _hour < 24){
			hour = _hour;
			return _hour;
		}
		return -1;
	}

	int _getMin(){
		int _min = int.Parse(MinField.text);
		if(0<= _min && _min < 60){
			min = _min;
			return _min;
		}
		return -1;
	}

	string formTimeFormat(int time){
		string time_str = time.ToString();
		if(time < 10){
			time_str = "0" + time_str;
		}
		return time_str;
	}
}
