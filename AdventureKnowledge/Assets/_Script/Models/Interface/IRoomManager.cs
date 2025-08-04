using System;
using System.Collections.Generic;

public interface IRoomManager
{
    void StartOrReadyGame();
    void LeaveRoom();
    void CharacterSelected(string characterName);
}