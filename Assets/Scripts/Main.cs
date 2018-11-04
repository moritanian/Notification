using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class Main : Token {

	public static Main Instance;

	List<TodoData> Todos;
	
	Dropdown _dpDown; 

	readonly string[] japanese_week = {"日", "月", "火", "水", "木", "金", "土"};

    TodoFieldScrollController todoFieldScroll;
	Image todoadd_button;

	MyCalendar _mycalendar;

	SearchField _searchField;

	InputFieldDialog _textDialog;

	// 追加モードで表示される時間
	int def_hour = 8;
	int def_min = 0;


	// Use this for initialization
	// コンポーネント取得はStartの前に処理する
	void Awake(){
		Instance = this;
		_dpDown = MyCanvas.Find<Dropdown>("Dropdown");
		todoFieldScroll = MyCanvas.Find<TodoFieldScrollController>("TodoContent");
		todoadd_button = MyCanvas.Find<Image>("TodoAdd");
		_mycalendar = MyCanvas.Find<MyCalendar>("MyCalendar");
		_searchField = MyCanvas.Find<SearchField>("SearchField");
		_textDialog = transform.FindChild ("TextDialog").gameObject.GetComponent<InputFieldDialog>();
			
		// 祝日初期化
		Holiday.init();

		Todos = TodoData.LoadAll();

	}

	void Start () {	
		//Screen.fullScreen = false;
		SetDispCal ();

		// Todo表示
		ApplySelectOpt();

		// event text 更新
		_mycalendar.SetEventText(Holiday.GetHoliday(DateTime.Now));


		// set Screen size 
		float sizeY = MyCanvas.GetCanvas ().GetComponent<RectTransform> ().localScale.y;

		RectTransform rect = _textDialog.transform.Find("Dialog").GetComponent<RectTransform>();

		PluginMsgHandler.getInst ().OnShowKeyboard = null;
		PluginMsgHandler.getInst ().OnShowKeyboard += (bool bKeyboardShow, int nKeyHeight) => {
			if (!_textDialog.gameObject.activeInHierarchy) {
				return;
			}

			Vector3 pos = rect.localPosition;
			rect.localPosition = new Vector3 (pos.x, -(Screen.height - nKeyHeight * 2 - rect.sizeDelta.y * sizeY) / sizeY / 2, pos.z);
		};

	}

	// カレンダーを通常表示モードで反映
	public void SetDispCal(){
		SetDispCal(DateTime.Now);
	}
	public void SetDispCal(DateTime pt){
		_mycalendar.DispCal(pt, (DateTime dt, bool IsSet, bool IsMemo) => ShowMyDay(dt));
	}

	// カレンダーを時間設定モードで反映
	public void SetGoCal(bool IsDispMemoButton = false){
		SetGoCal(DateTime.Now, IsDispMemoButton);
	}
	public void SetGoCal(DateTime pt, bool IsDispMemoButton = false){
		_mycalendar.GoCal(pt, (DateTime _celTime, bool IsSet, bool IsMemo) => TodoAdd(_celTime,IsSet, IsMemo), IsDispMemoButton);
	}
	
	public void OnclickTodoAdd(){
		
		SelectOpt opt = GetSelectOpt();

		TodoAdd(getDefaultTime(_mycalendar.MyDateTime),true, opt.IsMemo());
	
	}

	public void OnClickGoSetting(){
		Body.GoBoardSetting();
	}

	
	// 追加モードのカレンダーで日にち確定した際に呼ばれる
	public void TodoAdd(DateTime dt, bool IsSet = true, bool IsMemo = false){
		SetTodoAddImg(true);
		if(!IsSet){
			// Dispモード指定
			SetDispCal(_mycalendar.MyDateTime);
			return ;
		}
		int id = TodoData.NewId(Todos);
		TodoData newTodo = new TodoData(id);
		newTodo.IsMemo = IsMemo;
        newTodo.TodoTime = dt;

        // unity に保存する手続き
        newTodo.SaveCreation();

        Todos.Add(newTodo);// Todo 配列に追加


		SelectOpt selectOpt = GetSelectOpt ();
		_dpDown.value = selectOpt.OptAfterAdd ();

		// Dispモード指定
		//Restart();
		SetDispCal(_mycalendar.MyDateTime);

        TodoField todoField = todoFieldScroll.InsertItem(0, newTodo, /* isScroll  =*/ true);
		// 生成されたtodoField の notificationトグルにデータを合わせる
		//newTodo.UpdateIsNotify (_todoField.IsNotify);

		todoField.OnClickTitleEdit ();
	}

	// todo 追加ボタンの表示、非表示
	public void SetTodoAddImg(bool IsEnabled){
		todoadd_button.enabled = IsEnabled;

	}

	void Show (SelectOpt selectOpt){	

        List<TodoData> showList = new List<TodoData>(); 
		for(int i=0; i<Todos.Count; i++){
			if(!selectOpt.IsContain(Todos[i]))continue;
			if(!searchWord(Todos[i].Title))continue;// サーチワードある場合はそれにひっかからなかったものも除外
			showList.Add(Todos[i]);
		}	
		// ソート
		showList.Sort( (x, y) => selectOpt.Sort(x, y) );

        todoFieldScroll.SetItems(showList);
	}

	public void ShowMyDay(DateTime dt){


		SelectOpt selectOpt = new SelectOpt.PointedDay(dt);

		// コンボボックス変更
		_dpDown.value = SelectOpt.PointedDay.id;

		Show(selectOpt);
	}
	// 再読み込みして表示
	public void Reload(){
		// 読み込む前にセーブ対象をすべてセーブ
		Util.DoneSave();
		
		Todos = TodoData.LoadAll();
		TodoData.LogAll(Todos);
		// 全て表示
		Show( new SelectOpt.All() );
		_dpDown.value = SelectOpt.All.id;
		// カレンダーreload
		SetDispCal(_mycalendar.MyDateTime);
	}

	// Todo時間などを変更した際に反映してTodoField表示
	public void Restart(){
		ApplySelectOpt();
		SetDispCal(_mycalendar.MyDateTime);
	}

	// コンボボックスが変更された
	// todo一覧内容更新する
	public void OnChangeSelectOpt(){
		ApplySelectOpt();
	}

	public void ApplySelectOpt(){
		SelectOpt opt = GetSelectOpt();
		Show(opt);
	}

	public void ShowById(int id){
		SelectOpt selectOpt = new SelectOpt.PointedId(id);
		Show( selectOpt );
	}

	// idで指定されたdataのある日を表示
	public void ShowDayById(int id){
		TodoData todo = Todos.Find (t => t.Id == id);
		if (todo == null) {
			Debug.LogError ("ShowDayById: no todo data id = " + id);
			return;
		}	

		SetDispCal (todo.TodoTime);
		ShowMyDay (todo.TodoTime);

	}

	// 表示月の移動にともなって表示する内容を変える必要あるか
	public bool IsNeedUpdateShowInMonthChange(){
		return GetSelectOpt ().IsNeedUpdateShowInMonthChange ();
	}

	// Get SelectOpt instance for selected dropdown
	SelectOpt GetSelectOpt(){
		int id = _dpDown.value;
		return SelectOpt.GetInstanceById( id, _mycalendar);
	}
	
	public bool DeleteDataById(int id){
		return TodoData.DeleteDataById(Todos,id);
	}

	// サーチフィールド編集後に呼ばれる
	public void OnChangeSearchField(){
		ApplySelectOpt ();
	}
	
	// ワード検索 
	bool searchWord(string title){
		string search_text = _searchField.text;
		// 正規表現使いたい
		//string pattern = "/" +title + "\\?/";
		//return Regex.IsMatch(search_text, title);

		return title.Contains(search_text);
	}

	// 該当月のそれぞれの日の持つtodoの数をそれぞれ計算
	public List<int> CalcTodoNumbers(DateTime dt){
		int days_in_month = _days_in_month(dt);
		List<int> numbers = new List<int>();
		for(int i=0;i<days_in_month; i++){
			numbers.Add(0);
		}
		for(int i=0;i<Todos.Count;i++){
			if(Todos[i].IsMemo)continue;
			if(IsSameMonth(dt,Todos[i].TodoTime))numbers[Todos[i].TodoTime.Day - 1]++;
		}
		return numbers;
	}

	int _days_in_month(DateTime dt){
		return DateTime.DaysInMonth(dt.Year, dt.Month);
	}
	bool IsSameMonth(DateTime dt1, DateTime dt2){
		return (dt1.Year == dt2.Year && dt1.Month == dt2.Month);
	}

	// 新規作成の際の時間を設定したDateTimeを返す
	DateTime getDefaultTime(DateTime Dt){
		return new DateTime(Dt.Year, Dt.Month, Dt.Day, def_hour, def_min, 0);
	}

	public void SetSearchFieldVisibility(bool visible){
		_searchField.gameObject.SetActive (visible);
	}	

	public void SetTextDialog(string text, Action<string> action){
		_searchField.gameObject.SetActive (false);
		_textDialog.SetCancelAction ( () => {
			_searchField.gameObject.SetActive (true);
		});
		_textDialog.Set (text, (string newText) => {
			action.Invoke( newText );
			_searchField.gameObject.SetActive (true);
		});

	}

    public void UpdateTodoScroll()
    {
        todoFieldScroll.UpdateItems();
    }

}
