using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public float speed;
	private int count;
	public GUIText countText;
	public GUIText winText;
	public Transform pickUpPrefab;
	public Transform sphere;
	public Transform twoSidedCube;
	public Transform threeSidedShape;
	public Transform fourSidedCube;

	void Start()
	{
		count = 0;
		SetCountText ();
		winText.text = "";

		//Instantiate (pickUpPrefab, new Vector3 (-5f, 0.5f, 0), Quaternion.identity);
		Instantiate (sphere, new Vector3 (Random.Range(-10,10), 0.5f, Random.Range(-10,10)), Quaternion.identity);
		Instantiate (twoSidedCube, new Vector3 (Random.Range(-10,10), 0.5f, Random.Range(-10,10)), Quaternion.identity);
		Instantiate (threeSidedShape, new Vector3 (Random.Range(-10,10), 0.5f, Random.Range(-10,10)), Quaternion.identity);
		Instantiate (threeSidedShape, new Vector3 (Random.Range(-10,10), 0.5f, Random.Range(-10,10)), Quaternion.identity);
		Instantiate (fourSidedCube, new Vector3 (Random.Range(-10,10), 0.5f, Random.Range(-10,10)), Quaternion.identity);
		Instantiate (fourSidedCube, new Vector3 (Random.Range(-10,10), 0.5f, Random.Range(-10,10)), Quaternion.identity);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rigidbody.AddForce (movement * speed * Time.deltaTime);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive (false);
			count++;
			SetCountText();
		}
	}

	void SetCountText()
	{
		countText.text = "Count: " + count.ToString();
		if (count >= 15) {
			winText.text = "You Win!";
		}
	}
}
