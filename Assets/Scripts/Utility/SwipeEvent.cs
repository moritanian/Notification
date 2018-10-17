using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

[Serializable]
public class SwipeEvent : UnityEvent <SwipeEventObj> {}


public struct SwipeEventObj {
	public GameObject target;
	public string type;
	public Vector2 startPosition;
	public Vector2 currentPosition;
	public Vector2 deltaVector {
		get { return currentPosition - startPosition;}
	}
}
