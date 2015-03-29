using UnityEngine;
using System.Collections;

public class Lollipop : MonoBehaviour {
	[SerializeField]
	private Transform[] waypoints;
	public Transform[] Waypoints {
		get { return waypoints; }
		set { waypoints = value; }
	}
	//private int waypointIndx = 0;

	//private Transform obj;

	//private void OnTriggerEnter2D(Collider2D col) {
	//    if (col.tag == "Player")
	//        obj = col.transform;
	//}

	//private void LateUpdate() {
	//    if (obj != null) {
	//        Vector3 vel = default(Vector3);
	//        obj.position = Vector3.SmoothDamp(obj.position, waypoints[waypointIndx].position, ref vel, 1f);

	//        if (Vector3.Distance(obj.position, waypoints[waypointIndx].position) < 1f)
	//            waypointIndx++;
	//    }
	//}
}
