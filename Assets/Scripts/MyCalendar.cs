using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

/*
DayOfWeek 列挙体
Sunday :0 ~ 6
*/

public class MyCalendar : Token {

	PanelSlider _panelSlider;
	int year;
	int month;

	// Use this for initialization
	void Start () {
		_panelSlider = GetComponent<PanelSlider>();
		year = DateTime.Now.Year;
		month = DateTime.Now.Month;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GoCal(){
		_panelSlider.SlideIn();

	}
	
	public void ExitCal(){
		_panelSlider.SlideOut();
	}

	public void GetMonthData(int year, int month){	
		int days_in_month = DateTime.DaysInMonth(year, month);
		DateTime first_day = new DateTime(year, month, 1);
		DayOfWeek week = first_day.DayOfWeek;
	}
}
