using UnityEngine;
using System.Collections;

public class AndroidSamp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void DebugAction(){
		Debug.Log("AndroidSamp");
		HogeScript.CallFuncA("callA");
		Debug.Log(HogeScript.CallFuncB("callB"));
		HogeScript.CallFuncC("callC");
	}
	// Update is called once per frame
	void Update () {
	
	}
}
