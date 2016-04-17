using UnityEngine;
using System.Collections;
using UnityEngine.Events;

// ボタン長押しイベント管理
public class LongPress : MonoBehaviour {

		[SerializeField]
	float RequiredTime = 0.5f;
		[SerializeField]
	float SelectTime = 0.4f;
	float time;

		[SerializeField]
	UnityEvent LpEvent;
		[SerializeField]
	UnityEvent SelectEvent;
		[SerializeField]
	UnityEvent UntouchEvent;

	enum PressStaus{
		None,
		Tounch,
		Selected,
		LongPressed,
		UnTounched
	}
	PressStaus _pressStatus;

	// Use this for initialization
	void Start () {
		time = 0.0f;
		_pressStatus = PressStaus.None;
	}
	
	// Update is called once per frame
	void Update () {
			if(_pressStatus == PressStaus.Tounch ){
				// 1フレーム間隔分加算
				time += Time.deltaTime;
				if(time > SelectTime ){
					_pressStatus = PressStaus.Selected;
					SelectEvent.Invoke();
				}
			}
			if(_pressStatus == PressStaus.Selected){
				time += Time.deltaTime;
				if(time > RequiredTime){
					_pressStatus = PressStaus.LongPressed;
					// イベント実行
					LpEvent.Invoke();
					time = 0.0f;
				}
			}
	}

	public void ClickDown(){
		if(_pressStatus == PressStaus.None){
			_pressStatus = PressStaus.Tounch;
		}
	}

	public void ClickUp(){
		time = 0;
		_pressStatus = PressStaus.None;
		UntouchEvent.Invoke();
	}

}
