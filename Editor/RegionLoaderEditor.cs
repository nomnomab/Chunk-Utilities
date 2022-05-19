using UnityEngine;
using UnityEditor;
using ChunkUtilities.Runtime;

namespace ChunkUtilities.Editor {
  [CustomEditor(typeof(RegionLoader))]
  public class RegionLoaderEditor : UnityEditor.Editor {
    private GUIContent _cubeTexture;
    private GUIContent _sphereTexture;

    // labels
    private readonly GUIContent _sphereRadiusTooltip = new GUIContent("Radius", "The size of the sphere.");
    private readonly GUIContent _cubeSizeTooltip = new GUIContent("Size", "The amount of chunks per axis.");
    private readonly GUIContent _isCenteredTooltip = new GUIContent("Centered", "Are the chunks centered onto the origin point?");
    private readonly GUIContent _isPerAxisTooltip = new GUIContent("Size Per Axis", "Will the size reflect on both sides of the axis?");
    private readonly GUIContent _showDebugTooltip = new GUIContent("Show Debug", "Show the debugging gizmos when the object is selected.");
    private readonly GUIContent _debugChunkSizeTooltip = new GUIContent("Debug Chunk Size", "The size of the gizmo chunks. Purely for visuals, not represented of any runtime data.");

    private void OnEnable() {
      bool isDarkTheme = EditorGUIUtility.isProSkin;
      string prefix = isDarkTheme ? "d_" : string.Empty;

      _cubeTexture = EditorGUIUtility.IconContent($"{prefix}PreMatCube@2x");
      _sphereTexture = EditorGUIUtility.IconContent($"{prefix}PreMatSphere@2x");
    }

    public override void OnInspectorGUI() {
      // shape selector
      SerializedProperty shapeProperty = serializedObject.FindProperty("_regionShape");
      int currentShape = shapeProperty.intValue;
      int selectedShape = GUILayout.Toolbar(currentShape, new GUIContent[] { _sphereTexture, _cubeTexture });

      if (currentShape != selectedShape) {
        shapeProperty.intValue = selectedShape;
        serializedObject.ApplyModifiedProperties();
      }

      EditorGUI.BeginChangeCheck();

      if (selectedShape == 0) {
        DrawSphereGUI();
      } else {
        DrawCubeGUI();
      }

      // shared data
      BeginPanel();
      SerializedProperty isCenteredProperty = serializedObject.FindProperty("_isCentered");
      SerializedProperty showDebugProperty = serializedObject.FindProperty("_showDebug");
      SerializedProperty debugChunkSizeProperty = serializedObject.FindProperty("_debugChunkSize");

      EditorGUILayout.PropertyField(isCenteredProperty, _isCenteredTooltip);
      EditorGUILayout.PropertyField(showDebugProperty, _showDebugTooltip);

      if (showDebugProperty.boolValue) {
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(debugChunkSizeProperty, _debugChunkSizeTooltip);
        EditorGUI.indentLevel--;
      }

      EndPanel();

      if (EditorGUI.EndChangeCheck()) {
        serializedObject.ApplyModifiedProperties();
      }
    }

    private void BeginPanel() {
      EditorGUILayout.BeginVertical("Wizard Box");
    }

    private void EndPanel() {
      EditorGUILayout.EndVertical();
    }

    private void DrawSphereGUI() {
      SerializedProperty sphereRadiusProperty = serializedObject.FindProperty("_sphereRadius");

      BeginPanel();
      EditorGUILayout.PropertyField(sphereRadiusProperty, _sphereRadiusTooltip);
      EndPanel();
    }

    private void DrawCubeGUI() {
      SerializedProperty cubeSizeProperty = serializedObject.FindProperty("_cubeSize");
      SerializedProperty isSizePerAxisProperty = serializedObject.FindProperty("_isSizePerAxis");

      BeginPanel();
      EditorGUILayout.PropertyField(cubeSizeProperty, _cubeSizeTooltip);
      EditorGUILayout.PropertyField(isSizePerAxisProperty, _isPerAxisTooltip);
      EndPanel();
    }
  }
}