﻿using UnityEngine;
using System.Collections;

public class CubeRotator : MonoBehaviour {
	public int number;
	// Update is called once per frame
	void Update () {
		this.transform.Rotate (new Vector3(0, 15, 0) * Time.deltaTime);
	}
}
