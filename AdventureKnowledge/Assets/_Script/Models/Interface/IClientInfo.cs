using UnityEngine;

public interface IClientInfo
{
    void StartOrReadyGame();
    void SelectCharacter(string characterName);
    GameObject GetPlayerInfo();
    void SendMessage(string message, string Sender);
}