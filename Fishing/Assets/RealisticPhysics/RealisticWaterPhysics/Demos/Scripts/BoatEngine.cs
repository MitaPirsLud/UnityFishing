using UnityEngine;
using System.Collections;

public class BoatEngine : MonoBehaviour
{
	public bool ForcedON = false;

	public bool Checkunderwater = true;

	public float normalThrust;
	public float BoostThrust;

	private Rigidbody rb;
	private bool enableTrustForwards = false;
	private bool enableTrustBackwards = false;

	private bool enableTrustRight = false;
	private bool enableTrustLeft = false;

	private float currentY;
	private float currentWaterLevel;

	private float thrust;

	private float calculatedMass;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		calculatedMass = this.transform.parent.GetComponent<Rigidbody>().mass + this.GetComponent<Rigidbody>().mass;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey(KeyCode.V))
		{
			thrust = BoostThrust;
		}
		else
		{
			thrust = normalThrust;
		}

		if (Input.GetKey(KeyCode.W))
		{
			enableTrustForwards = true;
		}
		else
		{
			enableTrustForwards = false;
		}

		if (Input.GetKey(KeyCode.S))
		{
			enableTrustBackwards = true;
		}
		else
		{
			enableTrustBackwards = false;
		}

		if (Input.GetKey(KeyCode.A))
		{
			enableTrustLeft = true;
		}
		else
		{
			enableTrustLeft = false;
		}


		if (Input.GetKey(KeyCode.D))
		{
			enableTrustRight = true;
		}
		else
		{
			enableTrustRight = false;
		}
	}
	void FixedUpdate ()
	{
		currentWaterLevel = RealisticWaterPhysics.currentWaterLevel;
		currentY = this.gameObject.transform.position.y;

		if(Checkunderwater)
		{
			//Check if engine is underwater
			if(currentY < currentWaterLevel)
			{
				appyForce();
			}

		}
		else
		{
			appyForce();
		}
	}

	void appyForce()
	{
		if (enableTrustForwards || ForcedON)
		{
			rb.AddForce(transform.forward * thrust * calculatedMass);
		}
		if (enableTrustBackwards)
		{
			rb.AddForce(-transform.forward * thrust * calculatedMass);
		}

		if(enableTrustLeft)
		{
			rb.AddRelativeForce(-Vector3.left * thrust * (calculatedMass / 4));
		}
		if(enableTrustRight)
		{
			rb.AddRelativeForce(Vector3.left * thrust *  (calculatedMass / 4));
		}
	}
}
