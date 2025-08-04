public interface IRoomUI
{
    void TurnReadyButtonOnOFF(bool isOn);
    void UpdateStartButton(bool isAllReady);
    void UpdateReadyButton(bool amReady);
    void Onclick_LeaveRoom();
    void Onclick_StartOrReadyGame();
    void Onclick_OpenPanelCharacterSelection();
    void Onclick_ClosePanelCharacterSelection();
    void Onclick_SelectCharacter(string chacracterName);
    void SetStartButtonFirstTime();
    void SetReadyButtonFirstTime();
    MyColor32 GetMyColor32();
}