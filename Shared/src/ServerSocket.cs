namespace Shared;


public struct ServerSocket
{
    DataState dataState;

    PlayerMessage playerMessage;

    public ServerSocket(DataState dataState, PlayerMessage playerMessage)
    {
        this.dataState = dataState;
        this.playerMessage = playerMessage;
    }

}