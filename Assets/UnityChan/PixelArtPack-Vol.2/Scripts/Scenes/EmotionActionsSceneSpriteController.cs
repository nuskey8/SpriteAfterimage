using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EmotionActionsSceneSpriteController : BaseSpriteController {
	protected override void UpdateInternalV() {
		
		PixelArtPackInputReader inputReader = GetInputReader();
		
		Vector2 move = inputReader.ReadMove();
		
		m_animator.SetFloat(PixelArtPackConstants.SPEED_PARAM, move.x );

		if (inputReader.ReadAttack(0)) { m_animator.Play(PixelArtPackConstants.EMOTION_POSITIVE_PARAM); }
		if (inputReader.ReadAttack(1)) { m_animator.Play(PixelArtPackConstants.EMOTION_NEGATIVE_PARAM); }
		if (inputReader.ReadAttack(2)) { m_animator.Play(PixelArtPackConstants.EMOTION_WARMING_UP_PARAM); }
	}

	private void OnValidate() {
		m_animator = GetComponent<Animator>();
	}

//----------------------------------------------------------------------------------------------------------------------

	[SerializeField] Animator m_animator;
}
