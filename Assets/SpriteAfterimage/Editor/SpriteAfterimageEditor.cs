using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(SpriteAfterimage)), CanEditMultipleObjects]
public sealed class SpriteAfterimageEditor : Editor
{
    SerializedProperty source;
    SerializedProperty emissionEnabled;
    SerializedProperty emitInterval;
    SerializedProperty lifetime;
    SerializedProperty color;
    SerializedProperty colorMode;
    SerializedProperty fade;
    SerializedProperty useUnscaledTime;
    SerializedProperty shader;
    SerializedProperty sortingLayerID;
    SerializedProperty orderInLayer;

    void OnEnable()
    {
        source = serializedObject.FindProperty("source");
        emissionEnabled = serializedObject.FindProperty("emissionEnabled");
        emitInterval = serializedObject.FindProperty("emitInterval");
        lifetime = serializedObject.FindProperty("lifetime");
        color = serializedObject.FindProperty("color");
        colorMode = serializedObject.FindProperty("colorMode");
        fade = serializedObject.FindProperty("fade");
        useUnscaledTime = serializedObject.FindProperty("useUnscaledTime");
        shader = serializedObject.FindProperty("shader");
        sortingLayerID = serializedObject.FindProperty("sortingLayerID");
        orderInLayer = serializedObject.FindProperty("orderInLayer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(source);
        if (!source.hasMultipleDifferentValues && source.objectReferenceValue == null)
            EditorGUILayout.HelpBox("Source SpriteRenderer is not assigned.", MessageType.Warning);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(emissionEnabled);
        EditorGUILayout.PropertyField(emitInterval);
        EditorGUILayout.PropertyField(lifetime);
        EditorGUILayout.PropertyField(color);
        EditorGUILayout.PropertyField(colorMode);
        EditorGUILayout.PropertyField(fade);
        EditorGUILayout.PropertyField(useUnscaledTime);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rendering", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(shader);
        if (!shader.hasMultipleDifferentValues && shader.objectReferenceValue == null)
            EditorGUILayout.HelpBox(
                "Shader is not assigned. Afterimages will not be rendered.",
                MessageType.Warning
            );

        var detectedSortingGroup = GetDetectedSortingGroup();
        DrawSortingLayerField(sortingLayerID);
        EditorGUILayout.PropertyField(orderInLayer, new GUIContent("Order in Layer"));

        if (detectedSortingGroup != null)
        {
            EditorGUILayout.HelpBox(
                $"Sorting Group was detected in '{detectedSortingGroup.name}'. SpriteAfterimage does not support sorting of render order via SortingGroup; SortingGroup settings are ignored.",
                MessageType.Warning
            );
        }

        serializedObject.ApplyModifiedProperties();
    }

    SortingGroup GetDetectedSortingGroup()
    {
        if (source.hasMultipleDifferentValues)
            return null;

        var sourceRenderer = source.objectReferenceValue as SpriteRenderer;
        return sourceRenderer != null ? sourceRenderer.GetComponentInParent<SortingGroup>() : null;
    }

    static void DrawSortingLayerField(SerializedProperty property)
    {
        var layers = SortingLayer.layers;
        var names = new string[layers.Length];
        var selectedIndex = 0;

        for (int i = 0; i < layers.Length; i++)
        {
            names[i] = layers[i].name;
            if (layers[i].id == property.intValue)
                selectedIndex = i;
        }

        EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUILayout.Popup("Sorting Layer", selectedIndex, names);
        if (EditorGUI.EndChangeCheck())
            property.intValue = layers[selectedIndex].id;
        EditorGUI.showMixedValue = false;
    }
}
