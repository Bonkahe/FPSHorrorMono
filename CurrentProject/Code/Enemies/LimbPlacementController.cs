using Godot;
using System;

public partial class LimbPlacementController : Node
{
    [Export] public RigidBody3D EnemyBody { get; set; }
    [Export] public RayCast3D LimbRaycast { get; set; }
    [Export] public float BodyLength { get; set; } = 1.5f;
    [Export] public float BodyWidth { get; set; } = 2f;
    [Export] public float TargetOffsetDown { get; set; } = 1.5f;
    [Export] public float UpdateRate { get; set; } = 0.1f;

    public enum LimbReference { LeftHand, RightHand, LeftFoot, RightFoot }

    private Vector3 LastVelocity = Vector3.Forward;

    private float CurrentUpdateTimer;

    public override void _Ready()
	{
        LimbRaycast.TargetPosition = new Vector3(0, 0, -TargetOffsetDown);
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentUpdateTimer += (float)delta;
        if (CurrentUpdateTimer >= UpdateRate)
        {
            CurrentUpdateTimer -= UpdateRate;
            foreach (LimbReference limbOption in Enum.GetValues(typeof(LimbReference)))
            {
                Vector3 targetPosition = GetTargetLimbPosition(limbOption, out bool impactedSurface);

                this.DrawPoint(targetPosition, UpdateRate, impactedSurface ? new Color(0, 1, 0) : new Color(1, 0, 0), 0.3f);
            }
        }
    }

    public Vector3 GetTargetLimbPosition(LimbReference targetLimb, out bool hitSurface)
    {
        Vector3 targetPosition = EnemyBody.GlobalPosition;

        if (EnemyBody.LinearVelocity.Length() > 0.5f)
        {
            LastVelocity = EnemyBody.LinearVelocity.Normalized();
        }

        targetPosition += LastVelocity * (BodyLength / 2) * (targetLimb == LimbReference.LeftFoot || targetLimb == LimbReference.RightFoot ? -1 : 1);

        LimbRaycast.GlobalPosition = targetPosition;

        targetPosition += LastVelocity.Cross(Vector3.Up) * (BodyWidth / 2) * (targetLimb == LimbReference.LeftHand || targetLimb == LimbReference.LeftFoot ? -1 : 1);
        
        targetPosition.Y -= TargetOffsetDown;

        LimbRaycast.LookAt(targetPosition, Mathf.Abs((targetPosition - LimbRaycast.GlobalPosition).Y) > 0.99f ? Vector3.Back : Vector3.Up);

        LimbRaycast.ForceRaycastUpdate();

        hitSurface = LimbRaycast.IsColliding();
        if (hitSurface)
        {
            targetPosition = LimbRaycast.GetCollisionPoint();
        }

        return targetPosition;
    }
}
