using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/* 
 * Swipe slide event invoke class
 * 
 * {
 * 		Swipe.SwipeStart (SwipeEvent)
 * 		Swipe.SwipeMove (SwipeEvent)
 * 		Swipe.SwipeEnd (SwipeEvent)
 * 		Swipe.SwipeCancel (SwipeEvent)
 *	}
 *	=>
 *	{
 *		SlideStart (SwipeSliderEvent)
 *		SlideMove  (SwipeSliderEvent)
 *		SlideNext (SwipeSliderEvent)
 *		SlideCancel (SwipeSliderEvent)
 *		SlideEnd (SwipeSliderEvent)
 *	}
 */ 
public class SwipeSlider : MonoBehaviour {
	[SerializeField]
	GameObject prefab;

	[SerializeField]
	SwipeSliderEvent slideStart;

	[SerializeField]
	SwipeSliderEvent slideMove;

	[SerializeField]
	SwipeSliderEvent slideNext;

	[SerializeField]
	SwipeSliderEvent slideCancel;

	[SerializeField]
	SwipeSliderEvent slideEnd;

	[SerializeField]
	AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);

	[SerializeField]
	float slideAnimationTime = 0.1f;

	[SerializeField]
	Vector2 targetDirection = Vector2.right;

	[SerializeField]
	float slideMinDistance = 60.0f;

	[SerializeField]
	Vector2 distanceVector;

	Vector2 previousItemInitPosition;
	Vector2 currentItemInitPosition;
	Vector2 nextItemInitPosition;
	float itemDistance;

	enum State {
		Idle,
		SwipeSliding,
		AutoSliding,
		ReturnSliding
	}

	State state = State.Idle;

	SwipeSliderEventObj eventObj;

	Transform previousItemTransform {
		get{return eventObj.previousItem.transform;}
	}

	Transform currentItemTransform {
		get{return eventObj.currentItem.transform;}
	}

	Transform nextItemTransform {
		get{return eventObj.nextItem.transform;}
	}

	void Awake(){

		currentItemInitPosition = Vector2.zero;
		previousItemInitPosition = currentItemInitPosition - distanceVector;
		nextItemInitPosition = currentItemInitPosition + distanceVector;

		eventObj = new SwipeSliderEventObj ();
		eventObj.currentItem = CreateItem( currentItemInitPosition );
		eventObj.previousItem = CreateItem( previousItemInitPosition );
		eventObj.nextItem = CreateItem( nextItemInitPosition );
	}

	// Use this for initialization
	void Start () {
		itemDistance = Vector2.Distance (currentItemInitPosition, nextItemInitPosition);
		InvokeSlideEnd ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject CreateItem(Vector2 position){
		Vector3 vec3 = new Vector3 (position.x, position.y, 0);
		GameObject item = (GameObject)Instantiate (prefab, Vector3.zero, Quaternion.identity);
		item.transform.parent = this.transform;
		item.transform.localPosition = vec3;
		item.transform.localScale = Vector3.one;
		return item;
	}

	// should register in swipe
	public void OnSwipeMove(SwipeEventObj obj){

		eventObj.directionDelta = Vector2.Dot (obj.deltaVector, targetDirection);
		eventObj.slideDirection = eventObj.directionDelta > 0 ? 1 : -1;

		if (state == State.Idle) {
			StartSlide ();

		} else if (state == State.AutoSliding) {
			// TODO cancel animation
			StartSlide ();

		}

		if (state == State.SwipeSliding) {
			SetItemsPosition ( 
				eventObj.directionDelta / itemDistance * eventObj.slideDirection
			);
			InvokeSlideMove ();
		}

	}

	// should register in swipe
	public void OnSwipeEnd(SwipeEventObj obj){
		if (eventObj.directionDelta * eventObj.slideDirection > slideMinDistance) {
			SlideAuto ();
			Debug.Log ("slideAuto");
			InvokeSlideNext ();
		} else {
			SlideReturn ();
			Debug.Log ("slideReturn");

			InvokeSlideNext ();
		}
	}


	// should register in swipe
	public void OnSwipeCancel(SwipeEventObj obj){
		Debug.Log("OnSwipeCancel");
		if (state == State.SwipeSliding) {
			SlideReturn ();
		} else if (state == State.AutoSliding) {
			SlideReturn ();

		}
	}

	// show slide animation without swipe or complete slide if slide animation has not completed by swipe.
	void SlideAuto(){
		state = State.AutoSliding;
		StartCoroutine( SlideAnimation() );
	}

	void SlideReturn(){
		state = State.ReturnSliding;
		StartCoroutine( SlideAnimation() );
	}
		
	private IEnumerator SlideAnimation(){

		// degree of progress
		float initProgress =  eventObj.directionDelta *  eventObj.slideDirection / itemDistance;
		// time to move remain distance
		float duration;
		if (state == State.AutoSliding) {
			duration = (1.0f - initProgress) * slideAnimationTime;
		} else {
			duration = initProgress * slideAnimationTime;
		}

		// start time
		float startTime = Time.time;

		float progress;

		// animate until finished
		if (state == State.AutoSliding) {
			while ((progress = initProgress + (1.0f - initProgress) * (Time.time - startTime) / duration) < 1.0f) {
				SetItemsPosition (animCurve.Evaluate (progress));
				yield return 0;        // 1フレーム後、再開
			}
		} else {
			while ((progress = initProgress - (initProgress) * (Time.time - startTime) / duration) > 0.0f) {
				SetItemsPosition (animCurve.Evaluate (progress));
				yield return 0;        // 1フレーム後、再開
			}
		}
		FinishSlide();
	}


	void StartSlide(){

		state = State.SwipeSliding;

		InvokeSlideStart ();

		Debug.Log ("OnStart");

	}

	void FinishSlide(){

		if (state == State.AutoSliding) {
			Debug.Log ("ENd");
			// swap item
			if (eventObj.slideDirection == 1) {
				GameObject temp = eventObj.nextItem;
				eventObj.nextItem = eventObj.currentItem;
				eventObj.currentItem = eventObj.previousItem;
				eventObj.previousItem = temp;
			} else if (eventObj.slideDirection == -1) {
				GameObject temp = eventObj.previousItem;
				eventObj.previousItem = eventObj.currentItem;
				eventObj.currentItem = eventObj.nextItem;
				eventObj.nextItem = temp;
			}
			InvokeSlideEnd ();


		} else {
			InvokeSlideCancel ();
			Debug.Log ("Cancel");
		}

		state = State.Idle;

		nextItemTransform.localPosition = nextItemInitPosition;
		currentItemTransform.localPosition = currentItemInitPosition;
		previousItemTransform.localPosition = previousItemInitPosition;

		currentItemTransform.localScale =  Vector3.one ;
		//nextItemTransform.localScale = Vector3.one;
		//previousItemTransform.localScale = Vector3.one;

		currentItemTransform.localRotation = Quaternion.AngleAxis(0,  Vector3.forward);
		nextItemTransform.localRotation = Quaternion.AngleAxis(0,  Vector3.forward);
		previousItemTransform.localRotation =  Quaternion.AngleAxis(0,  Vector3.forward);

	}

	public GameObject getCurrentItem(){
		return eventObj.currentItem;
	}

	private void SetItemsPosition(float ratio){
		Vector2 moveDelta =  ( eventObj.slideDirection * ratio * itemDistance) * targetDirection;

		previousItemTransform.localPosition = previousItemInitPosition + moveDelta;
		currentItemTransform.localPosition = currentItemInitPosition + moveDelta;
		nextItemTransform.localPosition = nextItemInitPosition  + moveDelta;

		previousItemTransform.localPosition +=  (1.0f - ratio) * 3.0f * Vector3.up;
		currentItemTransform.transform.localPosition += ratio * 3.0f * Vector3.up;
		nextItemTransform.localPosition +=  (1.0f - ratio) * 3.0f * Vector3.up;

		Vector3 angleAxis = new Vector3(0, 0.9f, -0.05f);
		previousItemTransform.localRotation = Quaternion.AngleAxis(-(1.0f - ratio) * 130.0f * (- eventObj.slideDirection), angleAxis);
		currentItemTransform.localRotation = Quaternion.AngleAxis(ratio * 130.0f * (- eventObj.slideDirection), angleAxis);
		nextItemTransform.localRotation = Quaternion.AngleAxis(-(1.0f - ratio) * 130.0f *(- eventObj.slideDirection),  angleAxis);
	
	}

	void InvokeSlideStart (){
		eventObj.type = "SlideStart";
		eventObj.isAutoSlide = false;
		eventObj.isReturnSlide = false;
		slideStart.Invoke (eventObj);
	}

	void InvokeSlideMove (){
		eventObj.type = "SlideMove";
		slideMove.Invoke (eventObj);
	}

	void InvokeSlideNext (){
		eventObj.type = "SlideNext";
		eventObj.isAutoSlide = state == State.AutoSliding;
		eventObj.isReturnSlide = state == State.ReturnSliding;
		slideNext.Invoke (eventObj);
	}

	void InvokeSlideCancel (){
		eventObj.type = "SlideCancel";
		slideCancel.Invoke (eventObj);
	}

	void InvokeSlideEnd (){
		eventObj.type = "SlideEnd";
		eventObj.isAutoSlide = false;
		eventObj.isReturnSlide = false;
		slideEnd.Invoke (eventObj);
	}
}
