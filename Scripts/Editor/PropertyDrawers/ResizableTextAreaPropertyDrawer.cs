using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System;
using System.Linq;

namespace NaughtyAttributes.Editor
{
    [CustomPropertyDrawer(typeof(ResizableTextAreaAttribute))]
    public class ResizableTextAreaPropertyDrawer : PropertyDrawerBase
    {
        private static readonly string[] delimiters = new[] { "\r\n", "\n\r", "\r", "\n", Environment.NewLine };
        private Func<string, float> calculateMaximumTextAreaHeight;
        private float? minimum;
        private float? maximum;

        public ResizableTextAreaPropertyDrawer()
        {
            calculateMaximumTextAreaHeight = InitializeMaximumHeightCalculator;
        }

        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                float labelHeight = EditorGUIUtility.singleLineHeight;
                float textAreaHeight = Math.Min(
                    Math.Max(GetTextAreaHeight(property.stringValue), GetMinimumTextAreaHeight()),
                    GetMaximumTextAreaHeight(property.stringValue)
                );
                return labelHeight + textAreaHeight;
            }
            else
            {
                return GetPropertyHeight(property) + GetHelpBoxHeight();
            }
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType == SerializedPropertyType.String)
            {
                Rect labelRect = new Rect()
                {
                    x = rect.x,
                    y = rect.y,
                    width = rect.width,
                    height = EditorGUIUtility.singleLineHeight
                };

                EditorGUI.LabelField(labelRect, label.text);

                EditorGUI.BeginChangeCheck();

                Rect textAreaRect = new Rect()
                {
                    x = labelRect.x,
                    y = labelRect.y + EditorGUIUtility.singleLineHeight,
                    width = labelRect.width,
                    height = Math.Min(
                        Math.Max(GetTextAreaHeight(property.stringValue), GetMinimumTextAreaHeight()),
                        GetMaximumTextAreaHeight(property.stringValue)
                    )
                };

                string textAreaValue = EditorGUI.TextArea(textAreaRect, property.stringValue);

                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = textAreaValue;
                }
            }
            else
            {
                string message = typeof(ResizableTextAreaAttribute).Name + " can only be used on string fields";
                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        private int GetNumberOfLines(string text)
        {
            return delimiters
				.AsParallel()
				.Select(delimiter => CountOccurrences(text, delimiter))
				.Sum() + 1;
        }

        private int CountOccurrences(string text, string delimiter)
        {
			var count = 0;
			var position = text.IndexOf(delimiter, 0);
			while (position >= 0) {
				++count;
				position = text.IndexOf(delimiter, position + 1);
			}
			return count;
        }

        private float GetMinimumTextAreaHeight()
        {
            minimum = minimum.HasValue
                ? minimum
                : (EditorGUIUtility.singleLineHeight - 3.0f) * ((ResizableTextAreaAttribute)attribute).MinimumLines + 3.0f;

            return minimum.Value;
        }

        private float InitializeMaximumHeightCalculator(string text)
        {
            var targetAttribute = (ResizableTextAreaAttribute)attribute;
            calculateMaximumTextAreaHeight = (targetAttribute.MaximumLines > 0)
                ? (Func<string, float>)GetMaximumTextAreaHeight
                : (Func<string, float>)GetTextAreaHeight;
            return calculateMaximumTextAreaHeight(text);
        }

        private float GetMaximumTextAreaHeight(string _)
        {
            maximum = maximum.HasValue
                ? maximum
                : (EditorGUIUtility.singleLineHeight - 3.0f) * ((ResizableTextAreaAttribute)attribute).MaximumLines + 3.0f;

            return maximum.Value;
        }

        private float GetTextAreaHeight(string text)
        {
            float height = (EditorGUIUtility.singleLineHeight - 3.0f) * GetNumberOfLines(text) + 3.0f;
            return height;
        }
    }
}
