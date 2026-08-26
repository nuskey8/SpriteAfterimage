using UnityEngine;

[RequireComponent(typeof(Animator),typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class BattleActionsSceneSpriteController : BaseSpriteController {
	protected override void UpdateInternalV() {

		PixelArtPackInputReader inputReader = GetInputReader();
		
		Vector2 move = inputReader.ReadMove();

		float axis = move.x;
		bool isDown = move.y < 0;
		
		if (inputReader.ReadJump()) {
			m_rigidbody2D.linearVelocity = new Vector2 (m_rigidbody2D.linearVelocity.x, 5);
		}

		
		Vector2 startPos = new Vector2(transform.position.x, transform.position.y);
		float groundDistance = Physics2DUtility.Calculate2DDistanceToLayer(startPos, Vector2.down, 1, groundMask);
		
		// update animator parameters
		m_animator.SetBool (PixelArtPackConstants.IS_CROUCH_PARAM, isDown);
		m_animator.SetFloat (PixelArtPackConstants.GROUND_DISTANCE_PARAM, groundDistance);
		m_animator.SetFloat (PixelArtPackConstants.FALL_SPEED_PARAM, m_rigidbody2D.linearVelocity.y);
		m_animator.SetFloat (PixelArtPackConstants.SPEED_PARAM, Mathf.Abs (axis));
		
		if( inputReader.ReadAttack(0)) {  m_animator.SetTrigger(PixelArtPackConstants.ATTACK1_PARAM); }
		if( inputReader.ReadAttack(1)) {  m_animator.SetTrigger(PixelArtPackConstants.ATTACK2_PARAM); }
		if( inputReader.ReadAttack(2)) {  m_animator.SetTrigger(PixelArtPackConstants.ATTACK3_PARAM); }

		// flip sprite
		if (axis != 0)
			m_spriteRenderer.flipX = axis < 0;
	}

	private void OnValidate() {
		m_animator = GetComponent<Animator> ();
		m_spriteRenderer = GetComponent<SpriteRenderer> ();
		m_rigidbody2D = GetComponent<Rigidbody2D>();
	}

//----------------------------------------------------------------------------------------------------------------------
	[SerializeField] LayerMask groundMask;

	[SerializeField, HideInInspector] Animator m_animator;
	[SerializeField, HideInInspector] SpriteRenderer m_spriteRenderer;
	[SerializeField, HideInInspector] Rigidbody2D m_rigidbody2D;
	
}
