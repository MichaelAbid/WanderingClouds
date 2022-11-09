using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

namespace WanderingCloud
{
	public class PlayerV3 : Pawn
	{

	#region Variables
	[Foldout("_Reference"), SerializeField] Transform chara;
	[Foldout("_Reference"), SerializeField] CinemachineFreeLook cinemachine;
	[Foldout("_Reference"), SerializeField] Rigidbody body;
	[Foldout("_Reference"), SerializeField] CapsuleCollider capsuleCollider;

	// Camera Movement
	[MinMaxSlider(30, 90)]
	[Foldout("Camera"), SerializeField] Vector2 fov = new Vector2(45, 60);
	[Foldout("Camera")] protected Vector2 camCurMovement;
	[Foldout("Camera"), SerializeField] Vector3 zoomOffset;
	[Foldout("Camera"), SerializeField] AnimationCurve sensibilityEvolve = AnimationCurve.Linear(0,0,1,1);
	[Foldout("Camera"), SerializeField] float camSensibility = 1f;
	[Foldout("Camera")] private float aimTime;
	[Foldout("Camera")] public bool isAiming;

	// Pawn Movement
	[ReadOnly, SerializeField] 
	[Foldout("Movement")] protected Vector3 inputMovement;
	[Foldout("Movement")] public float speed = 2;

	[Foldout("Slide")] public float slopeValue = -15f;
	[Foldout("Slide")] public float curAngle;
	[Foldout("Slide")] public Vector3 slopeVector;

	[Foldout("Jump")] public float jumpForce = 5;
	[Foldout("Jump"), Range(0,1)] public float flotiness;	
	#endregion

	#region UnityMethods
	private void OnEnable() {}
	private void OnDisable() {}

	protected virtual void Update()
	{
		CalcGrounded();
		//cinemachine.m_YAxis.Value += camCurMovement.y;
		//cinemachine.m_XAxis.Value += camCurMovement.x;
	}
	protected virtual void FixedUpdate()
	{
		if (allowMovement) MovementUpdate();
	}
	#endregion
	
	#region input
	public override void CameraMovementInput(Vector2 input)
	{
		camCurMovement = input;
	}
	public override void MovementInput(Vector2 input)
	{ 
		inputMovement = chara.transform.forward * input.y + chara.transform.right * input.x;
	} 

	public override void RightTriggerInput(){}
	public override void SouthButtonInput() => Jump();
	public override void LeftTriggerInput()
	{
		isAiming = true;
		Camera.transform.DOLocalMove(zoomOffset, 0.2f);
		Camera.DOFieldOfView(fov.y, 0.2f);
	}
	public override void LeftTriggerInputReleased()
	{
		isAiming = false;
		Camera.transform.DOLocalMove(Vector3.zero, 0.2f);
		Camera.DOFieldOfView(fov.x, 0.2f);
	}

	#endregion

	public void Jump()
	{
		if (!isGrounded) return;
        
		body.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
	}
    
	public override void CalcGrounded()
	{
		var height = capsuleCollider.height;
		var feetPos = transform.position + Vector3.down * ((height -0.05f) / 2);
        
		RaycastHit underHit;
		Ray underRay =         inputMovement.magnitude < Mathf.Epsilon ?
			new Ray(feetPos, Vector3.down) : new Ray(feetPos, Vector3.down);
		isGrounded = Physics.Raycast(underRay,out underHit,0.5f);
		Debug.DrawRay(underRay.origin,underRay.direction * 0.5f, isGrounded ? Color.green : Color.red );

		if (!isGrounded)
		{
			curAngle = float.NaN;
			return;
		}

		float predictDist = capsuleCollider.radius + 0.05f;
		RaycastHit forwardHit;
		Ray forwardRay = new Ray(feetPos+ Vector3.up * height + chara.forward * predictDist, Vector3.down);
		isOnEdge = !Physics.Raycast(forwardRay, out forwardHit, height*2);
		Debug.DrawRay(forwardRay.origin,forwardRay.direction * (capsuleCollider.height * 2), isOnEdge ? Color.green : Color.red );

		if (isOnEdge)
		{
			curAngle = float.NaN;
			return;
		}
        
		//Calcul Slope
		slopeVector = forwardHit.point - underHit.point; 
		Debug.DrawLine(forwardHit.point,underHit.point, Color.yellow);

		curAngle = (Mathf.Atan2(slopeVector.y, predictDist) * Mathf.Rad2Deg);
        
	}
	public void MovementUpdate()
	{
		CalcGrounded();

		if (inputMovement.magnitude < Mathf.Epsilon) return;
		var aimRot = Quaternion.LookRotation(inputMovement, Vector3.up);
		chara.rotation = Quaternion.Slerp(chara.rotation, aimRot, 4 *Time.deltaTime);

		if (isGrounded)
		{
			body.AddForce(slopeVector.normalized * (inputMovement.magnitude * (speed * Time.deltaTime)), ForceMode.VelocityChange);
            
			if (!isOnEdge)
			{
				body.velocity = slopeVector.normalized * body.velocity.magnitude;
			}
		}
		else
		{
			body.AddForce(inputMovement * (speed * Time.deltaTime), ForceMode.VelocityChange);
			body.AddForce(Vector3.up * (flotiness * Time.deltaTime), ForceMode.VelocityChange);
		}
        

		if (curAngle < slopeValue)
		{
			body.AddForce(slopeVector.normalized * Mathf.InverseLerp(-20, -60, curAngle), ForceMode.VelocityChange);
		}
        
	}
	}
}
