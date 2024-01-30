using Godot;
using System;

public partial class TriggerVolume : Area3D
{
    private void TriggerVolume_BodyEntered(Node3D _)
    {
        QueueFree();
    }
}
