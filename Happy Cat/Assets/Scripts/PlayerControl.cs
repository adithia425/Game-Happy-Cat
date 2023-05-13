using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	[SerializeField] private PlayerData data;

	[Header("Object")]
	public Transform feetPos;
	public Vector2 feetRange;

	private Rigidbody2D RB;


	[Header("Variable")]
	[SerializeReference] private float moveSpeed = 4;
	[SerializeField] private float acceleration = 6;
	[SerializeField] private float decceleration = 5;
	[SerializeField] private float velPower = 1.3f;
	[SerializeReference] private float frictionAmount = 0.2f;

	private float lastGroundTime;
	private float lastJumpTime;
	private float moveDirect;

	[Header("Jump")]
	[SerializeReference] private float jumpForce = 12;
	[Range(0, 1)] [SerializeField] private float jumpCutMultiplier = 0.5f;
	[SerializeField] private float jumpCoyoteTime = 0.1f;
	[SerializeField] private float jumpBufferTime = 0.1f;

	private bool isGround;
	private bool isJumping;

	//private bool isFacingRight;

	[Header("Layers & Tags")]
	public LayerMask layerGround;

	private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		SetGravityScale(data.gravityScale);
		//isFacingRight = true;
	}

	private void Update()
	{
		moveDirect = Input.GetAxisRaw("Horizontal");

		//Timer
		lastGroundTime -= Time.deltaTime;
		lastJumpTime -= Time.deltaTime;

		isGround = Physics2D.OverlapBox(feetPos.position, feetRange, 1, layerGround);

		if(isGround)
        {
			lastGroundTime = jumpCoyoteTime;
        }
		

		if(Input.GetKeyDown(KeyCode.Space))
        {
			OnJump();
		}

		if (CanJump() && lastJumpTime > 0)
		{
			Jump();
		}

		if (Input.GetKeyUp(KeyCode.Space))
		{
			OnJumpUp();
		}

		JumpGravity();
	}

    private void FixedUpdate()
    {
		Movement();
		Friction();
	}

	private void Movement()
    {
		float targetSpeed = moveDirect * moveSpeed;
		float speedDif = targetSpeed - RB.velocity.x;

		float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

		float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

		movement = Mathf.Lerp(RB.velocity.x, movement, moveSpeed);

		RB.AddForce(movement * Vector2.right);
	}
	private void Friction()
    {
		if (moveDirect < 0.01f)
        {
			float amount = Mathf.Min(Mathf.Abs(RB.velocity.x), Mathf.Abs(frictionAmount));
			amount *= Mathf.Sign(RB.velocity.x);
			RB.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);

		}
    }
	private void Jump()
	{
		lastGroundTime = 0;
		lastJumpTime = 0;
		isJumping = true;
		RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
	}

	#region Button Control
	public void MoveLeft()
	{
		moveDirect = -1;
	}
	public void MoveRight()
	{
		moveDirect = 1;
	}
	public void MoveStop()
	{
		moveDirect = 0;
	}
	#endregion

	private bool CanJump()
    {
		return lastGroundTime > 0 && !isJumping;
	}
	//User Pencet Tombol Loncat
	private void OnJump()
	{
		lastJumpTime = jumpBufferTime;
	}

	//User Lepas Tombol Loncat
	public void OnJumpUp()
    {
		if(RB.velocity.y > 0 && isJumping)
        {
			JumpCut();

		}

		lastJumpTime = 0;
		isJumping = false;
	}

	private void JumpCut()
    {
		RB.AddForce(Vector2.down * RB.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
	}

	private void JumpGravity()
    {
		if(RB.velocity.y < 0)
        {
			SetGravityScale(data.gravityScale * data.fallGravityMultiplier);
        }
        else
        {
			SetGravityScale(data.gravityScale);
        }
    }

	private void SetGravityScale(float scale)
    {
		RB.gravityScale = scale;

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(feetPos.position, feetRange);
	}

}
