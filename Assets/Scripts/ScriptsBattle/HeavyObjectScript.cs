using UnityEngine;
using System.Collections;

public class HeavyObjectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {	}
	
	// Update is called once per frame
	void Update () 
	{
		//Debug.Log(rigidbody.velocity.magnitude);
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		//Debug.Log(collision.gameObject.name);
		if(collision.gameObject.tag == "Player" && rigidbody.velocity.magnitude > 3.5f)
		{
			collision.gameObject.GetComponent<PlayerScript>().GetDamaged((collision.relativeVelocity.magnitude * 0.25f) * rigidbody.mass);
		}
		else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Enemy2" || collision.gameObject.tag == "Destructible")
		{
			if(rigidbody.mass	> collision.gameObject.rigidbody.mass && rigidbody.velocity.magnitude >2.0f)
			{
				float massDiff = rigidbody.mass - collision.gameObject.rigidbody.mass;
				collision.gameObject.GetComponent<DestructableObjectBaseScript>().DealDamage((int)((collision.relativeVelocity.magnitude * 0.3f) * massDiff));
			}
		}
    }
}
