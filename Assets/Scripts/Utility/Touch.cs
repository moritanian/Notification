using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Touch : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler {

	[SerializeField]
	UnityEvent onLongTouch;
	[SerializeField]
	UnityEvent onTouchDown;
	[SerializeField]
	UnityEvent onTouchUp;

	[SerializeField]
	UnityEvent onClick;

	float longTouchTime = 0.5f; 

	Coroutine longTouchObserver;
	bool longTouched;

	public void OnBeginDrag(PointerEventData e){
		Debug.Log ("drag");
	}

	public void OnEndDrag(PointerEventData e){
		Debug.Log ("enddrag");
	}


	public void OnPointerDown(PointerEventData e){
		longTouched = false;
		onTouchDown.Invoke ();
		longTouchObserver = StartCoroutine (ObserveLongTouch ());
	}

	public void OnPointerUp(PointerEventData e){
		StopCoroutine (longTouchObserver);
		onTouchUp.Invoke ();

		// TODO なぜかdragするとよばれる
		if (e.delta.sqrMagnitude > 0) {
			return;
		}
		if (!longTouched) {
			onClick.Invoke ();
		}
	}

	IEnumerator ObserveLongTouch(){
		yield return new WaitForSeconds (longTouchTime);
		onLongTouch.Invoke ();
		longTouched = true;
	}
}
