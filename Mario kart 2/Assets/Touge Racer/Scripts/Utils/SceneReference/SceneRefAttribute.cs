using UnityEditor;
using UnityEngine;

namespace Sain.Utils
{
	/// <summary>
	/// Uses a CustomPropertyDrawer to draw a string field as a SceneAsset object field.
	/// </summary>
	/// <example><code>
	/// [SceneRef] string mainMenu;
	/// </code></example>
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public sealed class SceneRefAttribute : PropertyAttribute { }

#if UNITY_EDITOR
	/// <summary>
	/// Shows a string variable field as a SceneAsset object reference field.
	/// Fully supports standard built-in Editor features such as Undo, through the use of SerializedProperties.
	/// </summary>
	[CustomPropertyDrawer(typeof(SceneRefAttribute))]
	public class SceneRefDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect _rect, SerializedProperty _prop, GUIContent _label)
		{
			if (_prop.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.HelpBox(_rect, "Error: SceneRefAttribute only works on strings", MessageType.Error);
				return;
			}
			var selectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_prop.stringValue);

			_label = EditorGUI.BeginProperty(_rect, _label, _prop);
			EditorGUI.BeginChangeCheck();

			var newScene = EditorGUI.ObjectField(_rect, _label, selectedScene, typeof(SceneAsset), false);

			if (EditorGUI.EndChangeCheck())
			{
				_prop.stringValue = AssetDatabase.GetAssetPath(newScene);
			}
			EditorGUI.EndProperty();
		}
	}
#endif
}
