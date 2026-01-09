using UnityEngine;
using System;

[Serializable]
public struct DialoguePiece
{
    public string name;
    [TextArea] public string dialogue;

}