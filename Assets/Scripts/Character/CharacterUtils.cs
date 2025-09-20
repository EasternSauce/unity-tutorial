using System.Collections.Generic;
using UnityEngine;

public static class CharacterUtils
{
    public static List<Character> GetPlayerCharacters()
    {
        var allCharacters = Object.FindObjectsByType<Character>(FindObjectsSortMode.None);
        List<Character> players = new List<Character>();

        foreach (var character in allCharacters)
        {
            if (character.GetComponent<PlayerInputHandler>() != null)
                players.Add(character);
        }

        return players;
    }
}