using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Swipe : Token {


	Vector2 startPosition;
	Vector2 endPosition;
	[SerializeField]
	UnityEvent SwipeEventRight;
	[SerializeField]
	UnityEvent SwipeEventLeft;
	[SerializeField]
	UnityEvent SwipeEventUp;
	[SerializeField]
	UnityEvent SwipeEventDown;

	[SerializeField] // swipe 時に処理するイベント
	UnityEvent StartEvent;

	bool isExcutedStartEvent; // startEvent処理したか

	[SerializeField] // swipe 終了時に行うイベント
	UnityEvent EndEvent;


	[SerializeField]
	SwipeEvent SyncSwipeEvent;
	[SerializeField]
	ImageObj TouchSpace;

	float margin_distance = 20f; // 
	float margin_delta = 0.4f; // これ以上スワイプすれば。長押しイベントがキャンセル

	enum State {
		Idle,
		Down
	}
	State state;

	Vector2 ObjPos1;
	Vector2 ObjPos2;

	Vector2 CrtPos;
	// Use this for initialization
	void Start () {
		//Application.OpenURL("https://github.com/moritanian/Notification");
		state = State.Idle;
		Vector2 sd = GetComponent<RectTransform>().sizeDelta;

		// 自身の左上と右下を取得
		ObjPos1.x = local_X - sd.x/2.0f;
		ObjPos1.y = local_Y - sd.y/2.0f;
		ObjPos2.x = local_X + sd.x/2.0f;
		ObjPos2.y = local_Y + sd.y/2.0f;
		//LogObjPos();
	}

	public void LogObjPos(){
		Debug.Log(name + "center: " +  local_X + ":" + local_Y);
		Debug.Log("Objpos1 = " + ObjPos1.x + ": " + ObjPos1.y);
		Debug.Log("Objpos2 = " + ObjPos2.x + ": " + ObjPos2.y);
	}
	
	// Update is called once per frame
	void Update () {
		// async swipe event 処理
		GetPos();

		// sysnc swipe event 処理
		SyncSwipe();
	}

	void GetPos(){
		if (Input.GetMouseButtonDown (0) && state == State.Idle) {
			startPosition = GetMousePos();
			if(!IsIn(startPosition))return;
			
			state = State.Down;
			isExcutedStartEvent = false;
			CrtPos = startPosition;

		}

		if (Input.GetMouseButtonUp (0) && state == State.Down) {
			EndEvent.Invoke();
			state = State.Idle;
			endPosition = GetMousePos();
			if(!IsIn(endPosition))return ;

			float distanceX = endPosition.x - startPosition.x;
			float distanceY = endPosition.y - startPosition.y;
			if(margin_distance < distanceX){
				SwipeEventRight.Invoke();
			}else if( -margin_distance > distanceX){
				SwipeEventLeft.Invoke();
			}
			if(margin_distance < distanceY){
				SwipeEventUp.Invoke();
			}else if( -margin_distance > distanceY){
				SwipeEventDown.Invoke();
			}
			
		}
	}
	// マウスのcanvas内での位置
	Vector2 GetMousePos(){
		Vector2 pos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
		return MyCanvas.GetCanvasPosFromWorld(pos);
	}

	void SyncSwipe(){
		if(state == State.Down){
			Vector2 newPos = GetMousePos();
			Vector2 pos_delta = newPos - CrtPos;
			CrtPos = newPos;
			if((pos_delta.x*pos_delta.x + pos_delta.y * pos_delta.y) > margin_delta*margin_delta){
				// 長押しイベントキャンセル
				LongPress.PressInit();
				if(!isExcutedStartEvent){
					StartEvent.Invoke();
					isExcutedStartEvent = true;
				}

				SyncSwipeEvent.Invoke(pos_delta * 2.5f);
			}
			
		}
	}

	bool IsIn(Vector2 pos){
		return (ObjPos1.x < pos.x && pos.x < ObjPos2.x && ObjPos1.y < pos.y && pos.y < ObjPos2.y );
	}

}
