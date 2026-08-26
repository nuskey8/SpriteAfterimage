using UnityEngine.Assertions;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class PixelArtPackInputReader : MonoBehaviour {
	private void Awake() {
#if ENABLE_INPUT_SYSTEM
		InputActionMap currentMap = m_playerInput.currentActionMap;
		m_moveAction = currentMap.FindAction("Move", throwIfNotFound: false);
		m_jumpAction = currentMap.FindAction("Jump", throwIfNotFound: false);


		for (int i = 0; i < PixelArtPackConstants.NUM_ATTACKS; i++) {
			m_attackActions[i] = currentMap.FindAction("Attack" + (i), throwIfNotFound: false);
		}
#endif
	}

	private void OnValidate() {
#if ENABLE_INPUT_SYSTEM
		m_playerInput = GetComponent<PlayerInput>();
#endif
	}
	
	
//----------------------------------------------------------------------------------------------------------------------	

	public Vector2 ReadMove()
	{
#if ENABLE_INPUT_SYSTEM
		return m_moveAction.ReadValue<Vector2>();
#else		
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		return new Vector2(x, y);
#endif
	}
	
	public bool ReadJump()
	{
#if ENABLE_INPUT_SYSTEM
		return m_jumpAction.WasPressedThisFrame();
#else		
		return Input.GetButtonDown ("Jump");
#endif
	}

	public bool ReadAttack(int i)
	{
		Assert.IsTrue(i < PixelArtPackConstants.NUM_ATTACKS);
#if ENABLE_INPUT_SYSTEM
		return m_attackActions[i].WasPressedThisFrame();
#else		
		switch (i) {
			case 0: return Input.GetKeyDown(KeyCode.Z);
			case 1: return Input.GetKeyDown(KeyCode.X);
			case 2: return Input.GetKeyDown(KeyCode.C);
			default: return false;
		}
#endif
		
	}
//----------------------------------------------------------------------------------------------------------------------


#if ENABLE_INPUT_SYSTEM
	private InputAction m_moveAction;
	private InputAction m_jumpAction;
	private readonly InputAction[] m_attackActions = new InputAction[PixelArtPackConstants.NUM_ATTACKS];

	[SerializeField] private PlayerInput m_playerInput;
#endif
	
}

