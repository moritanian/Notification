using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

// 祝日データ管理
public class Holiday {

	//メンバー
	int type; // 0: 固定, 1:日にちが変わる, 2: 振替休日
	/* 日にちが変動する場合、
		m月、第n週, w 曜
		*/
	int month;
	int day;
	string hd_name;
	int start_year = 1970; // default 1970 unix time start year
	int end_year; // default 0
	int week_row; //何週目
	int week; //曜日


	// static 
	static private List<Holiday> holidays;

	// 初めに外部からこれを実行しないといけない
	static public void init(){
		holidays = new List<Holiday>();
		// 固定休日登録
		Add(1 , 1, "元旦");
		Add(1, 11, "成人の日", 1948, 1999); // 2000から月曜固定
		Add(2, 11, "建国記念日");

		Add(4 , 29, "天皇誕生日", 1948, 1988);
		Add(4, 29, "みどりの日", 1989, 2006);
		Add(4, 29, "昭和の日", 2007);
		
		Add(5, 3 , "憲法記念日");
		Add(5, 4 , "みどりの日", 2007);
		Add(5, 5 , "こどもの日");
		Add(7, 20, "海の日",1996, 2002); //2003年から月曜固定 
		Add(8, 11, "山の日", 2016);
		Add(9, 15, "敬老の日", 1966, 2002); // 2003 年から月曜固定
		Add(10,10, "体育の日", 1966, 1999); // 2000年から月曜固定
		Add(11,3, "文化の日");
		Add(11,23,"勤労感謝の日");
		Add(12,23, "天皇誕生日", 1989);
		

		// 日にちが変わる休日
		Add2(1, 2, 1, "成人の日", 2000);
		Add2(7, 3, 1, "海の日" ,  2003);
		Add2(9, 3, 1, "敬老の日", 2003);
		Add2(10,2, 1, "体育の日", 2000);

		
		// 不定　成人の日 春分  秋分 敬老 体育
	}

	// 日にち指定で名前を返す
	// 平日or 日曜は""
	public static string GetHoliday(DateTime dt, bool simle_search = false){
		// type0 type1 の確認
		List<Holiday>my_holidays = holidays.FindAll(delegate(Holiday hd) 
		{
			// 固定日の場合
			if(hd.type == 0){
				if(dt.Month == hd.month && dt.Day == hd.day && hd.start_year <= dt.Year){
					if(hd.end_year == 0)return true;
					else if( dt.Year <= hd.end_year)return true;
				}
			// 曜日固定の場合
			}else if(hd.type == 1){
				if(dt.Month == hd.month && hd.start_year <= dt.Year && (int)dt.DayOfWeek == hd.week){
					// 第何週目一致するか
					int week_row = (dt.Day + 6)/7;
					if(week_row == hd.week_row)return true;
				}
			}
			return false;
		});

		if(my_holidays.Count > 0){
			return my_holidays[0].hd_name;
		} 
		 
		// type0 , type1 該当しないとき、type3 探索
		DateTime autumn_holiday = GetAutumnHoliday(dt.Year);
		DateTime spring_holiday = GetSpringHoliday(dt.Year);
		if(IsEqualDate(dt, autumn_holiday))return "秋分の日";
		if(IsEqualDate(dt, spring_holiday))return "春分の日";

		if(dt.DayOfWeek == DayOfWeek.Sunday)return "";

		/* 振替休日 
			1973年　日曜祝日の場合、月曜祝日
			2007年法改正　月曜が祝日の場合、繰り越しに
		*/
		if(1973 <= dt.Year && dt.Year <= 2006){
			if(dt.DayOfWeek == DayOfWeek.Monday){
				if(GetHoliday(dt.AddDays(-1)) != "")return "振替休日";
			}
		} else if(2007 <= dt.Year ) {
			if(GetHoliday(dt.AddDays(-(int)(dt.DayOfWeek))) != ""){
				// 定義通り 月、火と祝日が続いているか確認
				bool flg = true;
				for(int i=1; i<(int)(dt.DayOfWeek); i++){
					string holi = GetHoliday(dt.AddDays(-i), true);
					if(holi == "" || holi == "振替休日"){
						flg = false;
						break;
					}
				}
				if(flg){
					return "振替休日";
				}
			}

		}
		
		if(simle_search)return "";
		
		/* 国民の休日
		1988年施行
		 その日および翌日が国民の祝日である平日は休日
			\無限ループしないようsimple_serchで前後祝日か調べる
		*/
		if(1988 <= dt.Year && GetHoliday(dt.AddDays(1), true)!= "" &&   GetHoliday(dt.AddDays(-1), true) != ""){
			return "国民の休日";
		}
		//祝日でないとき""を返す
		return ""; 
	}

	static void Add(int month, int day, string name, int start_y = 1948, int end_y = 0){
		holidays.Add(new Holiday(month, day, 0, 0, name, start_y, end_y, 0));
	}

	static void Add2(int month, int row_week, int week, string name, int start_y = 1948, int end_y = 0){
		holidays.Add(new Holiday(month, 0, row_week, week, name, start_y, end_y, 1));
	}
	// コンストラクタ
	public Holiday(int _month, int _day, int _week_row, int _week, string name, int start_y = 1948, int end_y = 0, int _type = 0){
		month = _month;
		day = _day;
	 	week_row = _week_row;
		week = _week;
		hd_name = name;
		start_year = start_y;
		end_year = end_y;
		type = _type;
	} 

	// 春分の日
	public static DateTime GetSpringHoliday(int year){
		int month = 3;
		int day = 20;
		if(1900 <= year && year <= 2025){
			if(year % 4 < 2){
				day = 20;
			} else {
				day = 21;
			}
		}else if(year > 2026){
			if(year % 4 == 3){
				day = 21;
			} else {
				day = 20;
			}
		}
		return new DateTime(year, month, day);
	}
	// 秋分の日 1979 ~ 
	public static DateTime GetAutumnHoliday(int year){
		int month = 9;
		int day = 23;
		if(year == 1979){
			day = 24;
		}else if(1980 <= year && year <= 2011 ){
			day = 23;
		}else if(2012 <= year && year < 2044 ){
			if(year % 4 == 0){
				day = 22; 
			}else{
				day = 23;
			}
		}
		return new DateTime(year, month, day);
	}

	// 日付一致
	public static bool IsEqualDate(DateTime dt1, DateTime dt2){
		if(dt1.Month == dt2.Month && dt1.Day == dt2.Day)return true;
		return false;
	}



}
