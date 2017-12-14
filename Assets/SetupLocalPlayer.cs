using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SetupLocalPlayer : NetworkBehaviour {

	Animator animator;

	[SyncVar (hook = "OnChangeAnimation")]
	public string animState = "idle";

	void OnChangeAnimation (string aS)
    {
		if(isLocalPlayer) return;
		UpdateAnimationState(aS);
    }


	[Command]
	public void CmdChangeAnimState(string aS)
	{
		UpdateAnimationState(aS);
	}

	void UpdateAnimationState(string aS)
	{
		if(animState == aS) return;
		animState = aS;
		if(animState == "idle")
			animator.SetBool("Idling",true);
		else if (animState == "run")
			animator.SetBool("Idling",false);
	}

	private void OnGUI() {
		if(isLocalPlayer)
		{
			if(Event.current.Equals(Event.KeyboardEvent("0")) ||
			   Event.current.Equals(Event.KeyboardEvent("1")) ||
			   Event.current.Equals(Event.KeyboardEvent("2")) ||
			   Event.current.Equals(Event.KeyboardEvent("3")))
			   {
				   int charid = int.Parse(Event.current.keyCode.ToString().Substring(5)) + 1;
				   CmdUpdatePlayerCharacter(charid);
			   }
		}	
	}

	[Command] public void CmdUpdatePlayerCharacter(int cid) 
	{
		NetworkManager.singleton.GetComponent<CustomNetworkManager>().SwitchPlayer(this,cid);
	}

	// Use this for initialization
	void Start () 
	{
		animator = GetComponentInChildren<Animator>();
        animator.SetBool("Idling", true);

		if(isLocalPlayer)
		{
			GetComponent<PlayerController>().enabled = true;
			CameraFollow360.player = this.gameObject.transform;
		}
		else
		{
			GetComponent<PlayerController>().enabled = false;
		}
	}
}
