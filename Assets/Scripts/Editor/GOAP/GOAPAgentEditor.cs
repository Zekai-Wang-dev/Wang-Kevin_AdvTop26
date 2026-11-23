using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GOAPAgent))]
public class GOAPAgentEditor : UnityEditor.Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add Action"))
        {

            GenericMenu menu = new GenericMenu();

            foreach (var actionType in TypeCache.GetTypesDerivedFrom<GOAPAction>())
            {
                menu.AddItem(new GUIContent(actionType.Name), false, () => AddAction(actionType));
            }

            menu.ShowAsContext();

        }

    }

    public void AddAction(System.Type actionType)
    {
        
        serializedObject.Update();

        var actionsProperty = serializedObject.FindProperty("actions");

        int currentIndex = actionsProperty.arraySize;

        actionsProperty.arraySize++;

        GOAPAction newAction = (GOAPAction)System.Activator.CreateInstance(actionType);

        actionsProperty.GetArrayElementAtIndex(currentIndex).managedReferenceValue = newAction;

        serializedObject.ApplyModifiedProperties();

    }

}
