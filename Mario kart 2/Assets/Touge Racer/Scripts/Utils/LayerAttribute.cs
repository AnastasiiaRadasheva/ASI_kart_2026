using UnityEditor;
using UnityEngine;

namespace Sain.Utils
{
    public class LayerAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            if (property.propertyType == SerializedPropertyType.String)
            {
                int selected = -1;
                string propertyString = property.stringValue;
                for (int i = 0; i < 32; i++)
                {
                    string layerName = LayerMask.LayerToName(i);
                    if (layerName == propertyString)
                    {
                        selected = i;
                        break;
                    }
                }

                int newSelected = EditorGUI.LayerField(position, label, selected);

                if (newSelected >= 0)
                {
                    property.stringValue = LayerMask.LayerToName(newSelected);
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [Layer] with string.");
            }
        }
    }
#endif
}
