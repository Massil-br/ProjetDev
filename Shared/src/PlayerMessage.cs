

namespace Shared;


public struct PlayerMessage
{
    public string Message { get; set; }
    public PlayerMessage(PlayerServerInfo player, int playerId)
    {
        Message = $"{playerId}:{player.pos.X}:{player.pos.Y}:" +
                        $"{player.anim}:{player.facing}:" +
                        $"{player.vertical}:{player.movement.X}:{player.movement.Y}";
    }

   

}