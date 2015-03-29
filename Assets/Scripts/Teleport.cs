using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour {

	[SerializeField]
	private Transform to;

	private void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Player") {
			col.transform.position = to.position;
		}
	}
}
