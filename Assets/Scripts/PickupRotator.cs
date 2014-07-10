using UnityEngine;
using System.Collections;

public class PickupRotator : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		this.transform.Rotate (new Vector3(15, 30, 45) * Time.deltaTime);
	}
}
