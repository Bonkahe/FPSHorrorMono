using Godot;
using Godot.Collections;
using System;
using System.Linq;

public enum LimbReference { LeftHand, RightHand, LeftFoot, RightFoot }
public partial class LimbPlacementController : Node
{
    [Export] public Skeleton3D Skeleton { get; set; }
    [Export] public PhysicalBone3D ChestBone { get; set; }

    [Export] public Node3D ChestTargetPoint { get; set; }
    [Export] public Node3D ChestTargetContainer { get; set; }

    [Export] public float JumpVelocity { get; set; } = 10;
    [Export] public float StepBouncePower { get; set; } = 0.5f;
    [Export] public float TorsoBounceVisualStrength { get; set; } = 0.1f;
    [Export] public float TorsoLerpSpeed { get; set; } = 3;
    [Export] public float TorsoRotationLerpSpeed { get; set; } = 3;


    [Export] public BasicEnemyNavigationAgent EnemyBody { get; set; }
    [Export] public RayCast3D LimbRaycast { get; set; }
    [Export] public float BodyLength { get; set; } = 1.5f;
    [Export] public float ShoulderBodyWidth { get; set; } = 0.5f;
    [Export] public float BottomBodyWidth { get; set; } = 1.5f;
    [Export] public float TargetOffsetDown { get; set; } = 1.5f;

    [Export] public Array<Limb> CurrentLimbs { get; set; } = new Array<Limb>();
    [Export] public float MinimumLimbStepDelay { get; set; } = 0.15f;
    [Export] public float VelocityAccountingMultiplier { get; set; } = 1;

    private float CurrentLimbStepDelayTimer;
    private int CurrentLimbIndex = 0;

    private Vector3 LastVelocity = Vector3.Forward;

    private Vector3 CurrentTorsoOffset = Vector3.Zero;
    private int ChestBoneID;

    public override void _Ready()
    {
        ChestBoneID = ChestBone.GetBoneId();

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

        CurrentTorsoOffset = Vector3.Down * CurrentLimbs.Where(x => x.CurrentlyTraveling).Count() * TorsoBounceVisualStrength;
        UpdateBodyPositions((float)delta);
    }

    public void KickOffVelocity(Vector3 DesiredDirection, Vector3 targetPoint)
    {
        float currentVelocity = JumpVelocity;

        if (EnemyBody.LinearVelocity.Dot(EnemyBody.DesiredVelocity) < 0)
        {
            currentVelocity *= EnemyBody.DesiredVelocity.Length() * VelocityAccountingMultiplier;
        }

        EnemyBody.ApplyImpulse(DesiredDirection * currentVelocity + (ChestTargetContainer.Transform.Basis.Y * StepBouncePower), EnemyBody.ToLocal(targetPoint));
    }


    public void UpdateBodyPositions(float delta)
    {
        Vector3 newLocation = ChestTargetContainer.GlobalPosition;
        newLocation.X = EnemyBody.GlobalPosition.X;
        newLocation.Z = EnemyBody.GlobalPosition.Z;
        newLocation.Y = Mathf.Lerp(newLocation.Y, (EnemyBody.GlobalPosition + (ChestTargetContainer.Transform.Basis * CurrentTorsoOffset)).Y, delta * TorsoLerpSpeed);
        ChestTargetContainer.GlobalPosition = newLocation;

        Vector3 targetRotation = ChestTargetContainer.GlobalTransform.LookingAt(ChestTargetContainer.GlobalPosition + LastVelocity, Mathf.Abs(LastVelocity.Y) > 0.99f ? Vector3.Back : Vector3.Up).Basis.GetEuler();

        ChestTargetContainer.GlobalRotation = new Vector3()
        {
            X = Mathf.LerpAngle(ChestTargetContainer.GlobalRotation.X, targetRotation.X, delta * TorsoRotationLerpSpeed),
            Y = Mathf.LerpAngle(ChestTargetContainer.GlobalRotation.Y, targetRotation.Y, delta * TorsoRotationLerpSpeed),
            Z = Mathf.LerpAngle(ChestTargetContainer.GlobalRotation.Z, targetRotation.Z, delta * TorsoRotationLerpSpeed),
        };
        Skeleton.GlobalPosition = ChestTargetContainer.GlobalPosition;
        Skeleton.SetBonePosePosition(ChestBoneID, Skeleton.ToLocal(ChestTargetPoint.GlobalPosition));
        Skeleton.SetBonePoseRotation(ChestBoneID, ChestTargetPoint.GlobalTransform.Basis.GetRotationQuaternion());
    }

    public Vector3 GetTargetLimbPosition(LimbReference targetLimb, out bool hitSurface, out Vector3 hitNormal)
    {
        Vector3 targetPosition = EnemyBody.GlobalPosition + ChestTargetContainer.Transform.Basis.Y;

        targetPosition += EnemyBody.LinearVelocity * VelocityAccountingMultiplier;

        if (EnemyBody.LinearVelocity.Length() > 0.5f)
        {
            LastVelocity = EnemyBody.LinearVelocity.Normalized();
        }

        targetPosition += LastVelocity * (BodyLength / 2) * (targetLimb == LimbReference.LeftFoot || targetLimb == LimbReference.RightFoot ? -1 : 1);

        Vector3 centerPoint = targetPosition;
        Vector3 sideAngle = LastVelocity.Cross(ChestTargetContainer.Transform.Basis.Y) * (targetLimb == LimbReference.LeftHand || targetLimb == LimbReference.LeftFoot ? -1 : 1);

        targetPosition = centerPoint + (sideAngle * (ShoulderBodyWidth / 2));

        LimbRaycast.GlobalPosition = targetPosition;

        targetPosition = centerPoint + (sideAngle * (BottomBodyWidth / 2)) + (-ChestTargetContainer.Transform.Basis.Y * (TargetOffsetDown + 1));

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
