using UnityEngine;
using System.Collections;

public class BackGound : MonoBehaviour {

	int size  = 0;
	int crt = 0;
	public Sprite[] BG;
	SpriteRenderer SpR = null;

	// Use this for initialization
	void Start () {
		string id_str = Util.LoadData(Setting.GetDataKey(Setting.DataKeys.BGid));
		if(id_str != "")crt = int.Parse(id_str);
		if(BG != null){
			size = BG.Length;
			SpR = GetComponent<SpriteRenderer>();
			if(crt<0 || crt>= size)crt = 0;
			SpR.sprite = BG[crt];
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public int BGChange(int next = -1 ){

		if(0<= next && next < size)crt = next;
		else {
			crt ++;
			if (crt == size)crt = 0;
		}
		SpR.sprite = BG[crt];
		return crt;
	}
}
