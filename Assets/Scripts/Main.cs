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

	// スクロ＾ルcontent
	ScrollController todo_scl;
	Image todoadd_button;

	MyCalendar _mycalendar;

	SearchField _searchField;

	InputFieldDialog _textDialog;

	// 追加モードで表示される時間
	int def_hour = 8;
	int def_min = 0;

	// todoField リスト表示コルーチン
	IEnumerator showTodoFieldsCoroutine;

	// Use this for initialization
	// コンポーネント取得はStartの前に処理する
	void Awake(){
		Instance = this;
		_dpDown = MyCanvas.Find<Dropdown>("Dropdown");
		todo_scl = MyCanvas.Find<ScrollController>("TodoContent");
		todoadd_button = MyCanvas.Find<Image>("TodoAdd");
		_mycalendar = MyCanvas.Find<MyCalendar>("MyCalendar");
		_searchField = MyCanvas.Find<SearchField>("SearchField");
		_textDialog = transform.FindChild ("TextDialog").gameObject.GetComponent<InputFieldDialog>();
			
		// 祝日初期化
		Holiday.init();

		Todos = TodoData.LoadAll();

	}

	void Start () {	


		SetDispCal ();

		// Todo表示
		ApplySelectOpt();

		// event text 更新
		_mycalendar.SetEventText(Holiday.GetHoliday(DateTime.Now));

		// set Screen size 
		float sizeX = MyCanvas.GetCanvas ().GetComponent<RectTransform> ().localScale.x;
		float sizeY = MyCanvas.GetCanvas ().GetComponent<RectTransform> ().localScale.y;
		Vector2 sizeDelta = new Vector2( Screen.width / sizeX , Screen.height / sizeY) ;
		GetComponent<RectTransform> ().sizeDelta = sizeDelta;
		MyCanvas.Find<RectTransform> ("Body/BoardText").sizeDelta = sizeDelta;
	}

	void Update(){

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
	
		// 最上部に追加されるため、そこまでスクロール
		todo_scl.ScrollTop();
	}

	public void OnClickGoSetting(){
		Body.GoBoardSetting();
	}

	
	// 追加モードのカレンダーで日にち確定した際に呼ばれる
	public void TodoAdd(DateTime Dt, bool IsSet = true, bool IsMemo = false){
		SetTodoAddImg(true);
		if(!IsSet){
			// Dispモード指定
			SetDispCal(_mycalendar.MyDateTime);
			return ;
		}
		int id = TodoData.NewId(Todos);
		TodoData new_todo = new TodoData(id);
		new_todo.IsMemo = IsMemo; //
		
		Todos.Add(new_todo);// Todo 配列に追加

		// todoField を生成
		TodoField _todoField = TodoField.Add(new_todo);
		
		// TododField をスクロールビューの子要素にする
		todo_scl.SetContentFirst(_todoField.transform);
		
		_todoField.Create(Dt);	//新規に作成時の処理

		// unity に保存する手続き
		new_todo.SaveCreation();

		SelectOpt selectOpt = GetSelectOpt ();
		_dpDown.value = selectOpt.OptAfterAdd ();

		// Dispモード指定
		//Restart();
		SetDispCal(_mycalendar.MyDateTime);

		// 生成されたtodoField の notificationトグルにデータを合わせる
		new_todo.UpdateIsNotify (_todoField.IsNotify);
	}

	// todo 追加ボタンの表示、非表示
	public void SetTodoAddImg(bool IsEnabled){
		todoadd_button.enabled = IsEnabled;

	}

	void Show (SelectOpt selectOpt){	
		if(showTodoFieldsCoroutine != null)
			StopCoroutine (showTodoFieldsCoroutine);
		// 破棄する前に場所をcanvas 直下に移動
		TodoField.parent.ForEachExists(t => t.transform.SetParent(MyCanvas.GetCanvas().transform,false));
		// 生存しているフィールドをすべて破棄
		TodoField.parent.Vanish();
		List<TodoData> showList = new List<TodoData>(); 
		for(int i=0; i<Todos.Count; i++){
			if(!selectOpt.IsContain(Todos[i]))continue;
			if(!searchWord(Todos[i].Title))continue;// サーチワードある場合はそれにひっかからなかったものも除外
			showList.Add(Todos[i]);
		}	
		// ソート
		showList.Sort( (x, y) => selectOpt.Sort(x, y) );

		showTodoFieldsCoroutine = ShowTodoFieldsCoroutine (showList);
		StartCoroutine (showTodoFieldsCoroutine);
	}

	private IEnumerator ShowTodoFieldsCoroutine(List<TodoData> showList){
		foreach(TodoData todo_data in showList){
			TodoField _todoField = TodoField.Add(todo_data);
			todo_scl.SetContent(_todoField.transform);
			_todoField.SetText(todo_data.Title);
			_todoField.IsNotify = todo_data.IsNotify;
			_todoField.SetTimeText(todo_data.TodoTime);
			yield return null;
		}	
		showTodoFieldsCoroutine = null;
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
		_textDialog.Set (text, action);
	}

}
