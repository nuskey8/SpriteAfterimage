using UnityEngine;

public static class PixelArtPackConstants {
    public static readonly int SPEED_PARAM = Animator.StringToHash("Speed");
    public static readonly int FALL_SPEED_PARAM = Animator.StringToHash ("FallSpeed");
    public static readonly int GROUND_DISTANCE_PARAM = Animator.StringToHash ("GroundDistance");
    public static readonly int IS_CROUCH_PARAM = Animator.StringToHash ("IsCrouch");
    public static readonly int DAMAGE_PARAM = Animator.StringToHash ("Damage");
    
    //"Attack" variables in InputSystem start from 0 
    public static readonly int ATTACK1_PARAM = Animator.StringToHash ("Attack1");
    public static readonly int ATTACK2_PARAM = Animator.StringToHash ("Attack2");
    public static readonly int ATTACK3_PARAM = Animator.StringToHash ("Attack3");

    public static readonly int EMOTION_POSITIVE_PARAM = Animator.StringToHash("Emotion.Positive");
    public static readonly int EMOTION_NEGATIVE_PARAM = Animator.StringToHash("Emotion.Negative");
    public static readonly int EMOTION_WARMING_UP_PARAM = Animator.StringToHash("Emotion.Warmingup");
    

    public const int NUM_ATTACKS = 3;

    public const int NO_HIT_DISTANCE = 99;
}
