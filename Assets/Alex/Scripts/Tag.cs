using UnityEngine;

public static class Tag
{
    public const string Player = "Player";
    public const string Enemy = "Enemy";
    public const string Interactable = "Interactable";
    public const string Destructible = "Destructible";
    public const string Ground = "Ground";


    public enum Tags
    {
        Player,
        Enemy,
        Interactable,
        Destructible,
        Ground,

    }

    public static string EnumTagToString(Tags tag)
    {
        switch (tag)
        {
            case Tags.Player:
                return Player;
            case Tags.Enemy:
                return Enemy;
            case Tags.Interactable:
                return Interactable;
            case Tags.Destructible:
                return Destructible;
            case Tags.Ground:
                return Ground;
            default:
                Debug.LogWarning("No Tag Found.");
                return null;
        }
    }
}
