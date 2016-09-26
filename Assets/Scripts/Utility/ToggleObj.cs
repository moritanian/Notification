using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ToggleObj : Token {

	Toggle tgl;
	[SerializeField]
	int id;
	Text text;
	public int Id{
		get {return id;}
	}
	public bool IsOn{
		get {return tgl.isOn;}
		set {tgl.isOn = value;}
	}

	// Use this for initialization
	void Awake() {
		tgl = transform.GetComponent<Toggle>();
		text = FindChild<Text>("text");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string Label{
		get {return text.text;}
		set {text.text = value;}
	}
}
