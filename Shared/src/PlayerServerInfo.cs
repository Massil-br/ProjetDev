
using SFML.System;
namespace Shared;

public struct PlayerServerInfo
{
    public Vector2f pos;
    public Animation anim;
    public bool facing;
    public float vertical;
    public Vector2f movement;
    public PlayerServerInfo(Vector2f pos, Animation anim, bool facing, float vertical, Vector2f movement)
    {
        this.pos = pos;
        this.anim = anim;
        this.facing = facing;
        this.vertical = vertical;
        this.movement = movement;
    }
    public PlayerServerInfo(Player player)
    {
        this.pos = player.GetPosition();
        this.anim = player.GetAnimationState();
        this.facing = player.IsFacingRight();
        this.vertical = player.GetVerticalSpeed();
        this.movement = player.GetPlayerMovement();
    }
}

