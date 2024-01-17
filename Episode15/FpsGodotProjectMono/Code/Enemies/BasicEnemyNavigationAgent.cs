using Godot;
using System;

public partial class BasicEnemyNavigationAgent : RigidBody3D
{
    [Export] public float MaximumVelocity { get; set; }
    [Export] public float VelocityChange { get; set; }
    [Export] public NavigationAgent3D NavigationAgent { get; set; }

    public Vector3 DesiredVelocity { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        if (NavigationAgent.IsTargetReached())
        {
            DesiredVelocity = Vector3.Zero;
            return;
        }

        DesiredVelocity = (NavigationAgent.GetNextPathPosition() - GlobalPosition).Normalized() * MaximumVelocity;
    }
}
