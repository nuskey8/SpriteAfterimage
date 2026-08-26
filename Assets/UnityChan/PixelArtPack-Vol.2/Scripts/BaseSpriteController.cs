using UnityEngine;

public abstract class BaseSpriteController : MonoBehaviour {

    void Update() {
        if (null == m_inputReader) {
            Debug.LogWarning($"Input Reader is not assigned to {gameObject.name}");
            return;
        }
        
        UpdateInternalV();
    }

    protected abstract void UpdateInternalV();
    
    protected PixelArtPackInputReader GetInputReader() => m_inputReader;

//----------------------------------------------------------------------------------------------------------------------

    [SerializeField] private PixelArtPackInputReader m_inputReader;
}