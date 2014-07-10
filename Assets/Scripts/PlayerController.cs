using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public float speed;
	private int count;
	public GUIText countText;
	public GUIText winText;
	public Transform pickUpPrefab;

	void Start()
	{
		count = 0;
		SetCountText ();
		winText.text = "";
		Instantiate (pickUpPrefab, new Vector3 (-5f, 0.5f, 0), Quaternion.identity);
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
