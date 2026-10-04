using ParkMinDev.UPM.MediaPipePlugin.Objects.Runners;
using ParkMinDev.UPM.MediaPipePlugin.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace ParkMinDev.UPM.MediaPipePlugin.Editor
{
	[CustomPropertyDrawer(typeof(SkeletonRunner), true)]
	[CustomPropertyDrawer(typeof(VideoSkeletonRunner.Setting))]
	internal sealed class SkeletonRunnerDrawer : PropertyDrawer
	{
		// - Statics -
		const string ProjectSettingsPath = "Project/ParkMinPackages/MediaPipe Plugin";
		const string UnregisteredModelMessage = "The selected MediaPipe model is not registered in Project Settings.";

		// - Public Methods -
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			Rect currentPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			property.isExpanded = EditorGUI.Foldout(currentPosition, property.isExpanded, label, true);

			if (!property.isExpanded) {
				EditorGUI.EndProperty();
				return;
			}

			currentPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			if (GUI.Button(EditorGUI.IndentedRect(currentPosition), "Open MediaPipe Project Settings"))
				SettingsService.OpenProjectSettings(ProjectSettingsPath);

			currentPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			if (HasUnregisteredModel(property)) {
				currentPosition.height = GetHelpBoxHeight(position.width);
				EditorGUI.HelpBox(EditorGUI.IndentedRect(currentPosition), UnregisteredModelMessage, MessageType.Error);
				currentPosition.y += currentPosition.height + EditorGUIUtility.standardVerticalSpacing;
			}

			DrawChildren(currentPosition, property);

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			if (!property.isExpanded)
				return EditorGUIUtility.singleLineHeight;

			float height = EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 2f;

			if (HasUnregisteredModel(property))
				height += GetHelpBoxHeight(EditorGUIUtility.currentViewWidth) + EditorGUIUtility.standardVerticalSpacing;

			height += GetChildrenHeight(property);

			return height;
		}

		// - Internals -
		static bool HasUnregisteredModel(SerializedProperty property) {
			SerializedProperty modelProperty = property.FindPropertyRelative("_model");
			MediaPipeModelAsset model = modelProperty?.objectReferenceValue as MediaPipeModelAsset;
			return model != null && !MediaPipeProjectSettings.instance.Contains(model);
		}

		static float GetHelpBoxHeight(float width) {
			return EditorStyles.helpBox.CalcHeight(new GUIContent(UnregisteredModelMessage), Mathf.Max(1f, width - 32f));
		}

		static void DrawChildren(Rect position, SerializedProperty property) {
			SerializedProperty childProperty = property.Copy();
			SerializedProperty endProperty = childProperty.GetEndProperty();
			bool hasChild = childProperty.NextVisible(true);

			EditorGUI.indentLevel++;

			while (hasChild && !SerializedProperty.EqualContents(childProperty, endProperty)) {
				position.height = EditorGUI.GetPropertyHeight(childProperty, true);
				EditorGUI.PropertyField(position, childProperty, true);
				position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
				hasChild = childProperty.NextVisible(false);
			}

			EditorGUI.indentLevel--;
		}

		static float GetChildrenHeight(SerializedProperty property) {
			SerializedProperty childProperty = property.Copy();
			SerializedProperty endProperty = childProperty.GetEndProperty();
			bool hasChild = childProperty.NextVisible(true);
			float height = 0f;

			while (hasChild && !SerializedProperty.EqualContents(childProperty, endProperty)) {
				height += EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;
				hasChild = childProperty.NextVisible(false);
			}

			return height;
		}
	}
}
