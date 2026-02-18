using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomMaker))]
public class RoomMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomMaker maker = (RoomMaker)target;
        if (GUILayout.Button("Capture Room"))
        {
            maker.CaptureRoom();
        }

        if (GUILayout.Button("Load Room"))
        {
            maker.LoadRoom();
        }
    }
}
