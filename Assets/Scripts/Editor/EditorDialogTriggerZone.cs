using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogTriggerZone))]
public class DialogTriggerZoneEditor : Editor
{
    SerializedProperty _dialogChannel;

    SerializedProperty _texts;
    SerializedProperty _forceShow;
    SerializedProperty _onlyOnce;


    SerializedProperty _endCommandType;
    SerializedProperty _endIntArg;
    SerializedProperty _endBoolArg;
    SerializedProperty _endObj;
    SerializedProperty _endIntCommand;
    SerializedProperty _endBoolCommand;
    SerializedProperty _endObjCommand;
    SerializedProperty _startCommandType;
    SerializedProperty _startIntArg;
    SerializedProperty _startBoolArg;
    SerializedProperty _startObj;
    SerializedProperty _startIntCommand;
    SerializedProperty _startBoolCommand;
    SerializedProperty _startObjCommand;
    SerializedProperty _delay;

    void OnEnable()
    {
        _dialogChannel = serializedObject.FindProperty("_dialogChannel");
        _endCommandType = serializedObject.FindProperty("_endCommandType");
        _endIntArg = serializedObject.FindProperty("_endIntArg");
        _endBoolArg = serializedObject.FindProperty("_endBoolArg");
        _endObj = serializedObject.FindProperty("_endObj");
        _endIntCommand = serializedObject.FindProperty("_endIntCommand");
        _endBoolCommand = serializedObject.FindProperty("_endBoolCommand");
        _endObjCommand = serializedObject.FindProperty("_endObjCommand");
        _startCommandType = serializedObject.FindProperty("_startCommandType");
        _startIntArg = serializedObject.FindProperty("_startIntArg");
        _startBoolArg = serializedObject.FindProperty("_startBoolArg");
        _startObj = serializedObject.FindProperty("_startObj");
        _startIntCommand = serializedObject.FindProperty("_startIntCommand");
        _startBoolCommand = serializedObject.FindProperty("_startBoolCommand");
        _startObjCommand = serializedObject.FindProperty("_startObjCommand");
        _texts = serializedObject.FindProperty("_texts");
        _forceShow = serializedObject.FindProperty("_forceShow");
        _onlyOnce = serializedObject.FindProperty("_onlyOnce");
        _delay = serializedObject.FindProperty("_delay");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default fields
        // DrawDefaultInspector();
        EditorGUILayout.PropertyField(_dialogChannel);
        EditorGUILayout.PropertyField(_texts);
        EditorGUILayout.PropertyField(_forceShow);
        EditorGUILayout.PropertyField(_onlyOnce);
        EditorGUILayout.PropertyField(_delay);

        // Draw the fields for the selected start command type
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(_startCommandType);

        switch ((CommandType)_startCommandType.enumValueIndex)
        {
            case CommandType.Int:
                EditorGUILayout.PropertyField(_startIntArg);
                EditorGUILayout.PropertyField(_startIntCommand);
                break;
            case CommandType.Bool:
                EditorGUILayout.PropertyField(_startBoolArg);
                EditorGUILayout.PropertyField(_startBoolCommand);
                break;
            case CommandType.GameObject:
                EditorGUILayout.PropertyField(_startObj);
                EditorGUILayout.PropertyField(_startObjCommand);
                break;
        }

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(_endCommandType);

        // Draw the fields for the selected end command type
        switch ((CommandType)_endCommandType.enumValueIndex)
        {
            case CommandType.Int:
                EditorGUILayout.PropertyField(_endIntArg);
                EditorGUILayout.PropertyField(_endIntCommand);
                break;
            case CommandType.Bool:
                EditorGUILayout.PropertyField(_endBoolArg);
                EditorGUILayout.PropertyField(_endBoolCommand);
                break;
            case CommandType.GameObject:
                EditorGUILayout.PropertyField(_endObj);
                EditorGUILayout.PropertyField(_endObjCommand);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}