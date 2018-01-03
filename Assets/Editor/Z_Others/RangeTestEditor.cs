using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(RangeTest))]
public class RangeTestEditor : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		RangeTest rangeAttribute = attribute as RangeTest;
		
		if(property.propertyType == SerializedPropertyType.Integer)
		{
			EditorGUI.IntSlider (position, property, rangeAttribute.minInt, rangeAttribute.maxInt, label);
		}
		
		else if(property.propertyType == SerializedPropertyType.Float)
		{
			EditorGUI.Slider (position, property, rangeAttribute.minFloat, rangeAttribute.maxFloat, label);
		}
	}
}
