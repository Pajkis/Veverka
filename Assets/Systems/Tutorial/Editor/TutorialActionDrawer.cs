using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom PropertyDrawer for TutorialAction that shows only relevant fields based on ActionType.
/// This keeps the inspector clean and focused by hiding unused fields.
/// </summary>
[CustomPropertyDrawer(typeof(TutorialAction))]
public class TutorialActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get the ActionType property to determine which fields to show
        SerializedProperty actionTypeProp = property.FindPropertyRelative("ActionType");
        TutorialActionType currentActionType = (TutorialActionType)actionTypeProp.enumValueIndex;

        // Current position for drawing fields
        Rect currentPosition = position;

        // Always draw the ActionType dropdown first
        currentPosition.height = EditorGUI.GetPropertyHeight(actionTypeProp, true);
        EditorGUI.PropertyField(currentPosition, actionTypeProp);
        currentPosition.y += currentPosition.height + EditorGUIUtility.standardVerticalSpacing;

        // Conditionally draw fields based on the selected ActionType
        switch (currentActionType)
        {
            case TutorialActionType.MovementSequence:
                DrawProperty(ref currentPosition, property.FindPropertyRelative("MovementSteps"));
                break;

            case TutorialActionType.MessageSequence:
                DrawProperty(ref currentPosition, property.FindPropertyRelative("MessageSteps"));
                break;

            case TutorialActionType.UndoSequence:
                DrawProperty(ref currentPosition, property.FindPropertyRelative("UndoCount"));
                break;

            case TutorialActionType.ImageSequence:
                DrawProperty(ref currentPosition, property.FindPropertyRelative("ImageAction"));
                break;
        }

        EditorGUI.EndProperty();
    }

    /// <summary>
    /// Helper method to draw a property and advance the position.
    /// </summary>
    private void DrawProperty(ref Rect position, SerializedProperty prop)
    {
        float height = EditorGUI.GetPropertyHeight(prop, true);
        position.height = height;
        EditorGUI.PropertyField(position, prop, true);
        position.y += height + EditorGUIUtility.standardVerticalSpacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get the ActionType to determine total height
        SerializedProperty actionTypeProp = property.FindPropertyRelative("ActionType");
        TutorialActionType currentActionType = (TutorialActionType)actionTypeProp.enumValueIndex;

        // Start with the height of the ActionType field
        float totalHeight = EditorGUI.GetPropertyHeight(actionTypeProp, true) + EditorGUIUtility.standardVerticalSpacing;

        // Add height for the relevant field based on ActionType
        SerializedProperty relevantProp = null;
        switch (currentActionType)
        {
            case TutorialActionType.MovementSequence:
                relevantProp = property.FindPropertyRelative("MovementSteps");
                break;

            case TutorialActionType.MessageSequence:
                relevantProp = property.FindPropertyRelative("MessageSteps");
                break;

            case TutorialActionType.UndoSequence:
                relevantProp = property.FindPropertyRelative("UndoCount");
                break;

            case TutorialActionType.ImageSequence:
                relevantProp = property.FindPropertyRelative("ImageAction");
                break;
        }

        if (relevantProp != null)
        {
            totalHeight += EditorGUI.GetPropertyHeight(relevantProp, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return totalHeight;
    }
}
