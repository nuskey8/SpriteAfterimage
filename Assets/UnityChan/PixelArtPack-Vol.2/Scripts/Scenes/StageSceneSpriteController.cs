using UnityEngine;

[RequireComponent(typeof(Animator),typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class StageSceneSpriteController : BaseSpriteController {
	protected override void UpdateInternalV() {
		
		PixelArtPackInputReader inputReader = GetInputReader();
		
		Vector2 move = inputReader.ReadMove();

		float axis = move.x;
		bool isDown = move.y < 0;
		
        Vector2 velocity = m_rigidbody2D.linearVelocity;
		if (inputReader.ReadJump()) {
			velocity.y = 5;
		}
		if (axis != 0){
			m_spriteRenderer.flipX = axis < 0;
            velocity.x = axis * 2;
        }
        m_rigidbody2D.linearVelocity = velocity;
        
        Vector2 startPos = new Vector2(transform.position.x, transform.position.y);
        float groundDistance = Physics2DUtility.Calculate2DDistanceToLayer(startPos, Vector2.down, 1, groundMask);
        
		// update animator parameters
		m_animator.SetBool (PixelArtPackConstants.IS_CROUCH_PARAM, isDown);
		m_animator.SetFloat (PixelArtPackConstants.GROUND_DISTANCE_PARAM, groundDistance);
		m_animator.SetFloat (PixelArtPackConstants.FALL_SPEED_PARAM, m_rigidbody2D.linearVelocity.y);
		m_animator.SetFloat (PixelArtPackConstants.SPEED_PARAM, Mathf.Abs (axis));
	}

    void OnTriggerEnter2D(Collider2D other) {
        m_animator.SetTrigger(PixelArtPackConstants.DAMAGE_PARAM);  
    }

    private void OnValidate() {
	    m_animator = GetComponent<Animator> ();
	    m_spriteRenderer = GetComponent<SpriteRenderer> ();
	    m_rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
//----------------------------------------------------------------------------------------------------------------------

	[SerializeField] LayerMask groundMask;

	[SerializeField, HideInInspector]Animator m_animator;
	[SerializeField, HideInInspector]SpriteRenderer m_spriteRenderer;
	[SerializeField, HideInInspector]Rigidbody2D m_rigidbody2D;
	
}
