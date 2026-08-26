using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// The SpriteAfterImage component renders a sprite afterimage effect using the RenderSprite API.
/// </summary>
[DisallowMultipleComponent]
public sealed class SpriteAfterimage : MonoBehaviour
{
    public enum ColorMode
    {
        Tint,
        Solid,
    }

    // RenderSpriteInstanced's default object/world matrix layout supports up to 511 instances.
    const int MaxSnapshots = 511;

    [SerializeField]
    SpriteRenderer source;

    [Header("Settings")]
    [SerializeField, Min(0.001f)]
    float emitInterval = 0.05f;

    [SerializeField, Min(0.001f)]
    float lifetime = 0.3f;

    [SerializeField]
    Color color = new(1f, 1f, 1f, 0.7f);

    [SerializeField]
    ColorMode colorMode = ColorMode.Tint;

    [SerializeField]
    AnimationCurve fade = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    [SerializeField]
    bool useUnscaledTime;

    [Header("Rendering")]
    [SerializeField]
    Shader shader;

    [Tooltip(
        "Added to the source SpriteRenderer's sorting order. Use a negative value to draw behind it."
    )]
    [SerializeField]
    int sortingOrderOffset = -1;

    Snapshot[] snapshots;
    int nextSnapshot;
    float emitTimer;
    Material instancedMaterial;

    readonly Dictionary<Sprite, List<InstanceData>> batches = new();

    [StructLayout(LayoutKind.Sequential)]
    struct InstanceData
    {
        public Matrix4x4 objectToWorld;
        public Color spriteColor;
        public uint renderingLayerMask;
    }

    struct Snapshot
    {
        public Sprite sprite;
        public Matrix4x4 objectToWorld;
        public float remaining;
    }

    void Reset()
    {
        source = GetComponent<SpriteRenderer>();
        shader = Shader.Find("SpriteAfterimage/Unlit");
    }

    void Awake()
    {
        RebuildBuffer();
    }

    void OnEnable()
    {
        emitTimer = 0f;
    }

    void OnValidate()
    {
        emitInterval = Mathf.Max(0.001f, emitInterval);
        lifetime = Mathf.Max(0.001f, lifetime);

        if (Application.isPlaying)
            RebuildBuffer();
    }

    void LateUpdate()
    {
        if (source == null)
            return;

        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        UpdateSnapshots(deltaTime);
        EmitSnapshots(deltaTime);
        DrawSnapshots();
    }

    void OnDestroy()
    {
        DestroyMaterial();
    }

    /// <summary>
    /// Immediately records the current state of the configured SpriteRenderer.
    /// </summary>
    public void Emit()
    {
        if (source == null || !source.enabled || source.sprite == null)
            return;

        EnsureBuffer();

        Vector3 flip = new(source.flipX ? -1f : 1f, source.flipY ? -1f : 1f, 1f);
        snapshots[nextSnapshot] = new Snapshot
        {
            sprite = source.sprite,
            objectToWorld = source.localToWorldMatrix * Matrix4x4.Scale(flip),
            remaining = lifetime,
        };

        nextSnapshot = (nextSnapshot + 1) % snapshots.Length;
    }

    /// <summary>
    /// Removes all currently visible afterimages.
    /// </summary>
    public void Clear()
    {
        if (snapshots == null)
            return;

        for (int i = 0; i < snapshots.Length; i++)
            snapshots[i].remaining = 0f;
    }

    void UpdateSnapshots(float deltaTime)
    {
        EnsureBuffer();

        for (int i = 0; i < snapshots.Length; i++)
        {
            if (snapshots[i].remaining > 0f)
                snapshots[i].remaining -= deltaTime;
        }
    }

    void EmitSnapshots(float deltaTime)
    {
        emitTimer -= deltaTime;
        if (emitTimer > 0f)
            return;

        // Avoid an unbounded catch-up loop after a hitch. One current snapshot is sufficient.
        emitTimer = emitInterval;
        Emit();
    }

    void DrawSnapshots()
    {
        if (!SystemInfo.supportsInstancing || !EnsureMaterial())
            return;

        foreach (var batch in batches.Values)
        {
            batch.Clear();
        }

        for (int i = 0; i < snapshots.Length; i++)
        {
            ref var snapshot = ref snapshots[i];
            if (snapshot.remaining <= 0f || snapshot.sprite == null)
                continue;

            var normalizedAge = 1f - Mathf.Clamp01(snapshot.remaining / lifetime);
            var opacity = Mathf.Max(0f, fade.Evaluate(normalizedAge));
            // RGB comes from the material. Instance color carries only per-snapshot opacity.
            var finalColor = Color.white;
            finalColor.a = opacity;

            if (!batches.TryGetValue(snapshot.sprite, out List<InstanceData> batch))
            {
                batch = new List<InstanceData>(snapshots.Length);
                batches.Add(snapshot.sprite, batch);
            }

            batch.Add(
                new InstanceData
                {
                    objectToWorld = snapshot.objectToWorld,
                    spriteColor = finalColor,
                    renderingLayerMask = source.renderingLayerMask,
                }
            );
        }

        var renderParams = new RenderParams(instancedMaterial)
        {
            layer = source.gameObject.layer,
            renderingLayerMask = source.renderingLayerMask,
            sortingLayerID = source.sortingLayerID,
            sortingOrder = source.sortingOrder + sortingOrderOffset,
            shadowCastingMode = source.shadowCastingMode,
            receiveShadows = source.receiveShadows,
            lightProbeUsage = source.lightProbeUsage,
        };

        foreach (var pair in batches)
        {
            if (pair.Value.Count == 0)
                continue;

            var spriteParams = new SpriteParams(pair.Key, Color.white, source.maskInteraction);
            Graphics.RenderSpriteInstanced(in renderParams, in spriteParams, 0, pair.Value);
        }
    }

    bool EnsureMaterial()
    {
        var shader = this.shader != null ? this.shader : Shader.Find("SpriteAfterImage/Unlit");
        if (shader == null)
            return false;

        if (instancedMaterial == null || instancedMaterial.shader != shader)
        {
            DestroyMaterial();

            instancedMaterial = new Material(shader)
            {
                name = "Sprite AfterImage (Runtime)",
                enableInstancing = true,
                hideFlags = HideFlags.HideAndDontSave,
            };
        }

        instancedMaterial.SetColor("_Color", color);
        instancedMaterial.SetFloat("_SolidFill", colorMode == ColorMode.Solid ? 1f : 0f);
        return true;
    }

    void EnsureBuffer()
    {
        int required = GetRequiredSnapshotCount();
        if (snapshots == null || snapshots.Length != required)
            RebuildBuffer();
    }

    void RebuildBuffer()
    {
        snapshots = new Snapshot[GetRequiredSnapshotCount()];
        nextSnapshot = 0;
        batches.Clear();
    }

    int GetRequiredSnapshotCount()
    {
        return Mathf.Clamp(Mathf.CeilToInt(lifetime / emitInterval) + 1, 1, MaxSnapshots);
    }

    void DestroyMaterial()
    {
        if (instancedMaterial == null)
            return;

        if (Application.isPlaying)
            Destroy(instancedMaterial);
        else
            DestroyImmediate(instancedMaterial);

        instancedMaterial = null;
    }
}
