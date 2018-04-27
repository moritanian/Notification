using UnityEngine;
using System.Collections;

public class MyLocalNotification : LocalNotification {

	protected override void OnClickNotification(int id){

		Body.GoBoardMain ();
		Main.Instance.ShowDayById (id);
	}
}
