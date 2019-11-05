using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Experiment_Ctrl))]
public class Experiment_Ctrl_Editor : Editor
{
    KeyCode key;
    SerializedProperty SMI_Prefab;
    private void OnEnable()
    {
        SMI_Prefab = serializedObject.FindProperty("SMI_Prefab");
    }


    public override void OnInspectorGUI()
    {
        Experiment_Ctrl ctrl = (Experiment_Ctrl)target;

        serializedObject.Update();

        ctrl.SMI_Prefab = (GameObject)EditorGUILayout.ObjectField("SMI_Prefab", ctrl.SMI_Prefab, typeof(GameObject), true);
        ctrl.sample_rate = EditorGUILayout.FloatField("sample rate", ctrl.sample_rate);
        ctrl.stimulus_interval = EditorGUILayout.FloatField("stimulus interval", ctrl.stimulus_interval);
        ctrl.user_name = EditorGUILayout.TextField("user name", ctrl.user_name);

        SerializedProperty usrTags = serializedObject.FindProperty("usrTags");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(usrTags,true);
        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();

        serializedObject.ApplyModifiedProperties();
    }
}