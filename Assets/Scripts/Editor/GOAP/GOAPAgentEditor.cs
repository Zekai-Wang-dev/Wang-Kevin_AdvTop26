using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GOAPAgent))]
public class GOAPAgentEditor : UnityEditor.Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawDefaultInspector();

        if (GUILayout.Button("Add Action"))
        {

            Debug.Log("Adding new action to GOAPAgent.");

        }

    }
    
}
