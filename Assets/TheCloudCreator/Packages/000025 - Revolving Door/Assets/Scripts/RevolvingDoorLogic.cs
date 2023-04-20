using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RevolvingDoorLogic : MonoBehaviour
{
	// ROTATING DOOR
	// Tells whether the center door is rotatin;
	[SerializeField]
	private bool rotating;
	// Used to store the object that is the center door
	private GameObject revolvingDoor;
	// How many degrees per second the center door rotates
	[Range(10, 30)]
	public float rotationSpeed;
	// Used to let every revolving door start at a different angle
	private float rotationOffset;
	// Brush sounds
	private GameObject[] brushSounds = new GameObject[3];
	
	// SLIDING DOORS
	// Tells wether the front doors are opened or closed (locked)
	[SerializeField]
	private bool open;
	//How fast do the doors open, lower = faster
	[Range(1f, 3f)]
	private float doorSpeed = 2.5f;
	// Which doors are turning clockwise and which open counterclockwise
	private GameObject[] doorsLeft = new GameObject[2];
	private GameObject[] doorsRight = new GameObject[2];
	// The degrees how far the doors are opened
	private float rotationAmountOpen = 30f;
	private float openedAngleLeft = 30f;
	private float openedAngleRight = -30f;
	private float closedAngle = 0f;
	private bool sliding = false;
	// How close do the doors need to be untill they "Snap" to the final position
	[Range(0f, 0.01f)]
	private float closingMargin = 0.0005f;
	[Range(0f, 0.258f)]
	private float openingMargin = 0.2580f;
	// Sliding door sounds front
	private GameObject slidingSoundOpenFront;
	private GameObject slidingSoundCloseFront;
	// Sliding door sounds back
	private GameObject slidingSoundOpenBack;
	private GameObject slidingSoundCloseBack;
	
	//STRUCTURE
	//Motor sound
	private GameObject motorSound;
	
	
	
	// Use this for initialization
	private void Start ()
	{
		// Search for the revolving door
		revolvingDoor = transform.Find("RevolvingDoor").gameObject;
		rotationOffset = 360*Random.value;
		revolvingDoor.transform.Rotate(Vector3.up, rotationOffset);
		// Search for the brushSound audio sources
		brushSounds[0] = transform.Find("RevolvingDoor/BrushSound1").gameObject;
		brushSounds[1] = transform.Find("RevolvingDoor/BrushSound2").gameObject;
		brushSounds[2] = transform.Find("RevolvingDoor/BrushSound3").gameObject;
		
		// Search for the left doors
		doorsLeft[0] = transform.Find("DoorBackR").gameObject;
		doorsLeft[1] = transform.Find("DoorFrontL").gameObject;
		
		// Search for the right doors
		doorsRight[0] = transform.Find("DoorBackL").gameObject;
		doorsRight[1] = transform.Find("DoorFrontR").gameObject;
		
		// Search for the front sliding doors audio sources
		slidingSoundOpenFront = transform.Find("Structure/SlidingSound OpenFront").gameObject;
		slidingSoundCloseFront = transform.Find("Structure/SlidingSound CloseFront").gameObject;
		
		// Search for the back sliding doors audio sources
		slidingSoundOpenBack = transform.Find("Structure/SlidingSound OpenBack").gameObject;
		slidingSoundCloseBack = transform.Find("Structure/SlidingSound CloseBack").gameObject;
		
		// Search for the motorSound audio source
		motorSound = transform.Find("Structure/MotorSound").gameObject;
		
		//Check whether the user wants the sliding door to start in a "Open" state
		if(open)
		{
			SetDoorsOpen();
		}else{
			SetDoorsClosed();
		}
		
		//Check whether the user wants the revolving door to start in a "Rotating" state
		if(rotating)
		{
			StartRotating();
		}else{
			StopRotating();
		}
	}	
	
	// Called when values change in the editor
	private void OnValidate()
	{
		// Needed to prevent an error when the doors are not set yet.
		try
		{
			// When the open boolean is changed the doors open and close accordingly
			if(open)
			{
				SetDoorsOpen();
			}else{
				SetDoorsClosed();
			}
		} catch {
			
		}
	}
	
	// Update is called once per frame
	private void Update ()
	{
		// Used to rotate the revolving door
		if (rotating)
		{
			revolvingDoor.transform.Rotate(Vector3.up, -rotationSpeed*Time.deltaTime); // -rotationSpeed*Time.deltaTime);
		}
	}
	
	//
	public void StartRotating()
	{
		// Stop the center door from rotating
		rotating = true;
		
		// Stop the motor sound
		motorSound.gameObject.SetActive(true);
		
		// Stop the brush sounds
		for(int i = 0; i < brushSounds.Length; i++)
		{
			brushSounds[i].SetActive(true);
		}
	}
	
	//
	public void StopRotating()
	{
		// Stop the center door from rotating
		rotating = false;
		
		// Stop the motor sound
		motorSound.gameObject.SetActive(false);
		
		// Stop the brush sounds
		for(int i = 0; i < brushSounds.Length; i++)
		{
			brushSounds[i].SetActive(false);
		}
	}
	
	// Open all doors gradually
	public void OpenDoors()
	{
		// Check whether the doors are already moving
		if(!sliding)
		{
			if(!open)
			{
				//Left doors slide "open"
				StartCoroutine(RotateDoorsSmoothly(doorsLeft[0],new Vector3(0f,openedAngleLeft,0f)));
				StartCoroutine(RotateDoorsSmoothly(doorsLeft[1],new Vector3(0f,openedAngleLeft,0f)));
				
				
				//Right doors slide "open"
				StartCoroutine(RotateDoorsSmoothly(doorsRight[0],new Vector3(0f ,openedAngleRight ,0f )));
				StartCoroutine(RotateDoorsSmoothly(doorsRight[1],new Vector3(0f ,openedAngleRight ,0f )));
				
				playSlidingSoundsOpen();
			}
		}else{
			//print("Door is still sliding");
		}
	}
	
	// Close all doors gradually
	public void CloseDoors()
	{
		// Check whether the doors are already moving
		if(!sliding)
		{
			if(open)
			{
				//Left doors slide "closed"
				StartCoroutine(RotateDoorsSmoothly(doorsLeft[0],new Vector3(0f ,closedAngle ,0f )));
				StartCoroutine(RotateDoorsSmoothly(doorsLeft[1],new Vector3(0f ,closedAngle ,0f )));
				
				//Right doors slide "closed"
				StartCoroutine(RotateDoorsSmoothly(doorsRight[0],new Vector3(0f ,closedAngle ,0f )));
				StartCoroutine(RotateDoorsSmoothly(doorsRight[1],new Vector3(0f ,closedAngle ,0f )));
				
				playSlidingSoundsClose();
			}
		}else{
			//print("Door is still sliding");
		}
	}
	
	//Single 
	private void playSlidingSoundsOpen()
	{
		slidingSoundOpenFront.GetComponent<AudioSource>().Play();
		slidingSoundOpenBack.GetComponent<AudioSource>().Play();
	}
	
	//Single 
	private void playSlidingSoundsClose()
	{
		slidingSoundCloseFront.GetComponent<AudioSource>().Play();
		slidingSoundCloseBack.GetComponent<AudioSource>().Play();
	}
	
	// Open all doors instantaneous
	public void SetDoorsOpen()
	{
		doorsRight[0].transform.rotation = Quaternion.AngleAxis(-rotationAmountOpen, Vector3.up);
		doorsRight[1].transform.rotation = Quaternion.AngleAxis(-rotationAmountOpen, Vector3.up);
		doorsLeft[0].transform.rotation = Quaternion.AngleAxis(rotationAmountOpen, Vector3.up);
		doorsLeft[1].transform.rotation = Quaternion.AngleAxis(rotationAmountOpen, Vector3.up);
		open = true;
	}
	
	// Close all doors instantaneous
	public void SetDoorsClosed()
	{
		doorsRight[0].transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
		doorsRight[1].transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
		doorsLeft[0].transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
		doorsLeft[1].transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
		open = false;
	}

	// Used to open and close the doors with an EaseInOut curve
	private IEnumerator RotateDoorsSmoothly(GameObject door, Vector3 targetRotation)
	{
		// Argument 1: which door is being moved
		// Argument 2: to what rotationl angle is it being set
		
		// The doors are sliding so no co-routine should be started.
		sliding = true;
		
		// The time on which the doors started opening
		float timeStamp = Time.time;
		
		// Used to open and close the doors
		// Right doors
		if(door == doorsRight[0] || door == doorsRight[1])
		{
			if(targetRotation.y == 0)
			{
				while(door.transform.rotation.y < -closingMargin)
				{
					door.transform.rotation = SmoothRotation(door.transform.rotation, targetRotation, timeStamp, doorSpeed);
					open = false;
					yield return null;
				}
			}else{
				while(door.transform.rotation.y > -openingMargin)
				{
					door.transform.rotation = SmoothRotation(door.transform.rotation,targetRotation,timeStamp,doorSpeed);
					open = true;
					yield return null;
				}
			}
		}		
		//Left doors
		if(door == doorsLeft[0] ||door == doorsLeft[1])
		{
			if(targetRotation.y == 0)
			{
				while(door.transform.rotation.y > closingMargin)
				{
					door.transform.rotation = SmoothRotation(door.transform.rotation,targetRotation,timeStamp,doorSpeed);
					open = false;
					yield return null;
				}
			}else{
				while(door.transform.rotation.y < openingMargin)
				{
					door.transform.rotation = SmoothRotation(door.transform.rotation,targetRotation,timeStamp,doorSpeed);
					open = true;
					yield return null;
				}
			}
		}
		
		// Snap the door to it's target rotations
		door.transform.rotation = Quaternion.Euler(targetRotation);
		
		// Doors have finished sliding
		sliding = false;
		yield return null;
	}
	
	// Used to give a smooth rotation to the doors
	private static Quaternion SmoothRotation(Quaternion doorRotation, Vector3 targetRotation, float timeStamp, float duration)
	{
		// Smoothly rotates towards target 
		float rotationValue = Easing.EaseInOut((Time.time) - timeStamp, 0f, 1f, duration);
		// Set the newly calculated rotation
		Quaternion rotationalPosition = Quaternion.Lerp(doorRotation, Quaternion.Euler(targetRotation), Time.deltaTime*rotationValue); 
		
		return rotationalPosition;
	}
}

// Used for the smoothness of the opening
class Easing : MonoBehaviour
{
    public static float EaseInOut(float t, float b, float c, float d)
    {
		if ((t/=d/2f) < 1f) return c/2f*t*t*t + b;
		return c/2f*((t-=2f)*t*t + 2f) + b;
	}
}