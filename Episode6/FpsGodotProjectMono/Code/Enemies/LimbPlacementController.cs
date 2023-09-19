using Godot;
using Godot.Collections;
using System;

public enum LimbReference { LeftHand, RightHand, LeftFoot, RightFoot }
public partial class LimbPlacementController : Node
{
    [Export] public RigidBody3D EnemyBody { get; set; }
    [Export] public RayCast3D LimbRaycast { get; set; }
    [Export] public float BodyLength { get; set; } = 1.5f;
    [Export] public float BodyWidth { get; set; } = 2f;
    [Export] public float TargetOffsetDown { get; set; } = 1.5f;

    [Export] public Array<Limb> CurrentLimbs { get; set; } = new Array<Limb>();
    [Export] public float MinimumLimbStepDelay { get; set; } = 0.15f;
    [Export] public float VelocityAccountingMultiplier { get; set; } = 1;

    private float CurrentLimbStepDelayTimer;
    private int CurrentLimbIndex = 0;

    private Vector3 LastVelocity = Vector3.Forward;

    public override void _Ready()
	{
        LimbRaycast.TargetPosition = new Vector3(0, 0, -TargetOffsetDown);

        foreach (Limb limb in CurrentLimbs)
        {
            limb.Controller = this;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentLimbStepDelayTimer += (float)delta;
        if (CurrentLimbStepDelayTimer >= MinimumLimbStepDelay)
        {
            CurrentLimbStepDelayTimer -= MinimumLimbStepDelay;
            CurrentLimbs[CurrentLimbIndex].InitializeStep();
            CurrentLimbIndex++;
            if (CurrentLimbIndex == CurrentLimbs.Count)
            {
                CurrentLimbIndex = 0;
            }
        }
    }

    public Vector3 GetTargetLimbPosition(LimbReference targetLimb, out bool hitSurface, out Vector3 hitNormal)
    {
        Vector3 targetPosition = EnemyBody.GlobalPosition;

        targetPosition += EnemyBody.LinearVelocity * VelocityAccountingMultiplier;

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
            hitNormal = LimbRaycast.GetCollisionNormal();
        }
        else
        {
            hitNormal = Vector3.Up;
        }

        return targetPosition;
    }
}
