using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RD_UILogic : MonoBehaviour
{
	public GameObject revolvingDoor;
	private RevolvingDoorLogic doorLogic;
	private int currentCamera;
	
	public Camera[] selectableCameras;
	
	private void Start()
	{
		doorLogic = (RevolvingDoorLogic) revolvingDoor.GetComponent(typeof(RevolvingDoorLogic));
		
		//Turn off all cameras
		for(int i = 0; i < selectableCameras.Length; i++)
		{
			selectableCameras[i].enabled = false;
			selectableCameras[i].gameObject.SetActive(false);
		}
		//Turn on the first camera
		currentCamera = selectableCameras.Length-1;
		SelectNextCamera();
	}
	
	public void SelectNextCamera()
	{
		//Have the currentCamera int cycle between 0 and the amount of camera's in the array
		if(currentCamera == selectableCameras.Length-1){
			currentCamera = 0;
		}else{
			currentCamera++;
		}
		
		//
		for(int i = 0; i < selectableCameras.Length; i++)
		{
			if(currentCamera == i)
			{
				selectableCameras[i].gameObject.SetActive(true);
				selectableCameras[i].enabled = true;
			}else{
				selectableCameras[i].gameObject.SetActive(false);
				selectableCameras[i].enabled = false;
			}
		}
	}
	
	public void RotateDoorToggle(bool value)
	{
		if(value)
		{
			doorLogic.StartRotating();
		}else{
			doorLogic.StopRotating();
		}
	}
	
	public void RotationSpeedChanged(float value)
	{
		doorLogic.rotationSpeed = value;
	}
 
	public void OpenDoorsClicked()
	{
		doorLogic.OpenDoors();
	}
 
	public void CloseDoorsClicked()
	{
		doorLogic.CloseDoors();
	}
}
	