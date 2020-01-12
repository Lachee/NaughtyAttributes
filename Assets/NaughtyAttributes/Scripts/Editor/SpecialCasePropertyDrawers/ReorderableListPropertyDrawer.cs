﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace NaughtyAttributes.Editor
{
	public class ReorderableListPropertyDrawer : NaughtyPropertyDrawer, ISpecialCasePropertyDrawer
	{
		private Dictionary<string, ReorderableList> _reorderableListsByPropertyName = new Dictionary<string, ReorderableList>();

		string GetPropertyKeyName(SerializedProperty property)
		{
			return property.serializedObject.targetObject.GetInstanceID() + "/" + property.name;
		}

		public void OnGUI_Custom(SerializedProperty property)
		{
			if (property.isArray)
			{
				var key = GetPropertyKeyName(property);

				if (!_reorderableListsByPropertyName.ContainsKey(key))
				{
					ReorderableList reorderableList = new ReorderableList(property.serializedObject, property, true, true, true, true)
					{
						drawHeaderCallback = (Rect rect) =>
						{
							EditorGUI.LabelField(rect, string.Format("{0}: {1}", property.displayName, property.arraySize), EditorStyles.label);
						},

						drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
						{
							var element = property.GetArrayElementAtIndex(index);
							rect.y += 1.0f;
							rect.x += 10.0f;
							rect.width -= 10.0f;

							EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 0.0f), element, true);
						},

						elementHeightCallback = (int index) =>
						{
							return EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index)) + 4.0f;
						}
					};

					_reorderableListsByPropertyName[key] = reorderableList;
				}

				_reorderableListsByPropertyName[key].DoLayoutList();
			}
			else
			{
				string message = typeof(ReorderableListAttribute).Name + " can be used only on arrays or lists";
				EditorGUILayout.HelpBox(message, MessageType.Warning);
				EditorGUILayout.PropertyField(property, true);
			}
		}

		public void ClearCache()
		{
			_reorderableListsByPropertyName.Clear();
		}
	}
}