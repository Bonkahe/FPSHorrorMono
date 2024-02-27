using Godot;
using Godot.Collections;
using System;
using System.Linq;

public enum LimbReference { LeftHand, RightHand, LeftFoot, RightFoot }
public partial class LimbPlacementController : Node
{
    [Export] public Array<AudioStream> FootstepSounds { get; set; } = new Array<AudioStream>();
    [Export] public Array<AudioStream> JumpSounds { get; set; } = new Array<AudioStream>();

    [Export] public Skeleton3D Skeleton { get; set; }
    [Export] public PhysicalBone3D ChestBone { get; set; }
    [Export] public SkeletonIK3D HeadIKSolver { get; set; }

    [Export] public Node3D ChestTargetPoint { get; set; }
    [Export] public Node3D ChestTargetContainer { get; set; }
    [Export] public Node3D HeadTargetContainer { get; set; }

    [Export] public float JumpVelocity { get; set; } = 8;
    [Export] public float LaunchVelocityMultiplier { get; set; } = 5;
    [Export] public float MaxLaunchVelocity { get; set; } = 40;

    [Export] public float StepBouncePower { get; set; } = 5;
    [Export] public float TorsoBounceVisualStrength { get; set; } = 0.2f;
    [Export] public float TorsoLerpSpeed { get; set; } = 12;
    [Export] public float TorsoRotationLerpSpeed { get; set; } = 8;
    [Export] public float HeadRotationLerpSpeed { get; set; } = 2;


    [Export] public EnemyAIController EnemyAIController { get; set; }
    [Export] public BasicEnemyNavigationAgent EnemyBody { get; set; }
    [Export] public RayCast3D LimbRaycast { get; set; }
    [Export] public float BodyLength { get; set; } = 1;
    [Export] public float ShoulderBodyWidth { get; set; } = 0.5f;
    [Export] public float BottomBodyWidth { get; set; } = 2;
    [Export] public float TargetOffsetDown { get; set; } = 2.5f;

    [Export] public Array<Limb> CurrentLimbs { get; set; } = new Array<Limb>();
    [Export] public float MinimumLimbStepDelay { get; set; } = 0.12f;
    [Export] public float VelocityAccountingMultiplier { get; set; } = 0.45f;

    [Export] public float FlightFlailSpeed { get; set; } = 15;
    [Export] public float FlightFlailSize { get; set; } = 3;

    public bool LaunchingRequested { get; private set; } = false;

    private float CurrentFlightTime;

    private float CurrentLimbStepDelayTimer;
    private int CurrentLimbIndex = 0;

    private Vector3 LastVelocity = Vector3.Forward;

    private Vector3 CurrentTorsoOffset = Vector3.Zero;
    private int ChestBoneID;

    private BasicEnemyNavigationAgent.GroundedState lastGroundedState;
    

    public override void _Ready()
    {
        lastGroundedState = EnemyBody.CurrentGroundedState;

        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        CurrentLimbStepDelayTimer = rng.RandfRange(0, MinimumLimbStepDelay);
        CurrentLimbIndex = rng.RandiRange(0, CurrentLimbs.Count - 1);

        ChestBoneID = ChestBone.GetBoneId();
        HeadIKSolver.Start();

        LimbRaycast.TargetPosition = new Vector3(0, 0, -TargetOffsetDown);

        foreach (Limb limb in CurrentLimbs)
        {
            limb.Controller = this;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        EnemyAIController.CheckTargetValid();

        if (lastGroundedState != EnemyBody.CurrentGroundedState)
        {
            CurrentFlightTime = 0;

            if (EnemyBody.CurrentGroundedState == BasicEnemyNavigationAgent.GroundedState.None)
            {
                RandomNumberGenerator rng = new RandomNumberGenerator();
                rng.Randomize();

                foreach (var limb in CurrentLimbs)
                {
                    limb.SetLimbGroundedState(false, rng.Randf() * 10.0f);
                }
            }
            else
            {
                foreach (var limb in CurrentLimbs)
                {
                    limb.SetLimbGroundedState(true, 0.0f);
                    limb.PlaceFootHard();
                }
            }

            lastGroundedState = EnemyBody.CurrentGroundedState;
        }


        if (lastGroundedState != BasicEnemyNavigationAgent.GroundedState.None)
        {
            if (!LaunchingRequested)
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
        }
        else
        {
            CurrentFlightTime += (float)delta * FlightFlailSpeed;
        }

        CurrentTorsoOffset = Vector3.Down * CurrentLimbs.Where(x => x.CurrentlyTraveling).Count() * TorsoBounceVisualStrength;
        UpdateBodyPositions((float)delta);
        UpdateHeadPosition((float)delta);
    }

    public async void OnLaunchRequested()
    {
        if (LaunchingRequested) return;
        LaunchingRequested = true;

        foreach (var limb in CurrentLimbs)
        {
            limb.PlaceFootHard();
        }

        await ToSignal(GetTree().CreateTimer(0.25f), "timeout");

        PathWaypoint waypoint = EnemyBody.GetNextWaypoint();
        Vector3 requestedVelocity = waypoint.WaypointPosition;

        requestedVelocity += Vector3.Up * EnemyBody.GlobalPosition.DistanceTo(requestedVelocity) * 0.5f;

        requestedVelocity = ((requestedVelocity - EnemyBody.GlobalPosition) * LaunchVelocityMultiplier) - EnemyBody.LinearVelocity;

        if (requestedVelocity.Length() > MaxLaunchVelocity)
        {
            requestedVelocity = requestedVelocity.Normalized() * MaxLaunchVelocity;
        }

        EnemyBody.ApplyImpulse(requestedVelocity);
        LaunchingRequested = false;

        if (FootstepSounds.Count > 0)
        {
            GetTree().CallGroup("AudioQues", "PlayAudioQue3D", FootstepSounds[new RandomNumberGenerator().RandiRange(0, FootstepSounds.Count - 1)], EnemyBody.GlobalPosition, 0);
        }
    }

    public async void OnAttackLaunchRequested(Node3D target)
    {
        if (LaunchingRequested) return;
        LaunchingRequested = true;

        foreach (var limb in CurrentLimbs)
        {
            limb.PlaceFootHard();
        }

        await ToSignal(GetTree().CreateTimer(0.25f), "timeout");

        Vector3 requestedVelocity = target.GlobalPosition;

        requestedVelocity += Vector3.Up * EnemyBody.GlobalPosition.DistanceTo(requestedVelocity) * 0.5f;

        requestedVelocity = ((requestedVelocity - EnemyBody.GlobalPosition) * LaunchVelocityMultiplier) - EnemyBody.LinearVelocity;

        if (requestedVelocity.Length() > MaxLaunchVelocity)
        {
            requestedVelocity = requestedVelocity.Normalized() * MaxLaunchVelocity;
        }

        EnemyBody.ApplyImpulse(requestedVelocity);
        LaunchingRequested = false;

        if (JumpSounds.Count > 0)
        {
            GetTree().CallGroup("AudioQues", "PlayAudioQue3D", JumpSounds[new RandomNumberGenerator().RandiRange(0, JumpSounds.Count - 1)], EnemyBody.GlobalPosition, 0);
        }
    }

    public void KickOffVelocity(Vector3 DesiredDirection, Vector3 targetPoint)
    {
        if (FootstepSounds.Count > 0)
        {
            GetTree().CallGroup("AudioQues", "PlayAudioQue3D", FootstepSounds[new RandomNumberGenerator().RandiRange(0, FootstepSounds.Count - 1)], targetPoint, -5);
        }

        float currentVelocity = JumpVelocity;

        if (EnemyBody.LinearVelocity.Dot(EnemyBody.DesiredVelocity) < 0)
        {
            currentVelocity *= EnemyBody.DesiredVelocity.Length() * VelocityAccountingMultiplier;
        }

        EnemyBody.ApplyImpulse((DesiredDirection * currentVelocity + (Vector3.Up * StepBouncePower)) - EnemyBody.LinearVelocity, EnemyBody.ToLocal(targetPoint));

    }


    public void UpdateBodyPositions(float delta)
    {
        Vector3 newLocation = ChestTargetContainer.GlobalPosition;
        newLocation.X = EnemyBody.GlobalPosition.X;
        newLocation.Z = EnemyBody.GlobalPosition.Z;
        newLocation.Y = Mathf.Lerp(newLocation.Y, (EnemyBody.GlobalPosition + (ChestTargetContainer.GlobalTransform.Basis * CurrentTorsoOffset)).Y, delta * TorsoLerpSpeed);
        ChestTargetContainer.GlobalPosition = newLocation;

        Vector3 UpVector = EnemyBody.GetGroundNormal();
        if (UpVector == Vector3.Up)
        {
            UpVector = Mathf.Abs(LastVelocity.Y) > 0.99f ? Vector3.Back : Vector3.Up;
        }

        Vector3 targetRotation = ChestTargetContainer.GlobalTransform.LookingAt(ChestTargetContainer.GlobalPosition + LastVelocity, UpVector).Basis.GetEuler();

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

    public void UpdateHeadPosition(float delta)
    {
        HeadTargetContainer.GlobalPosition = ChestTargetContainer.GlobalPosition;

        Vector3 targetLookAtPoint = EnemyAIController.CurrentTarget != null ? EnemyAIController.CurrentTarget.GlobalPosition : ChestTargetContainer.GlobalPosition + LastVelocity;

        Vector3 targetRotation = HeadTargetContainer.GlobalTransform.LookingAt(targetLookAtPoint,
            Mathf.Abs(ChestTargetContainer.GlobalTransform.Basis.Y.Dot(targetLookAtPoint - HeadTargetContainer.GlobalPosition)) > 0.99f
            ? Vector3.Up : ChestTargetContainer.GlobalTransform.Basis.Y).Basis.GetEuler();

        HeadTargetContainer.GlobalRotation = new Vector3()
        {
            X = Mathf.LerpAngle(HeadTargetContainer.GlobalRotation.X, targetRotation.X, delta * HeadRotationLerpSpeed),
            Y = Mathf.LerpAngle(HeadTargetContainer.GlobalRotation.Y, targetRotation.Y, delta * HeadRotationLerpSpeed),
            Z = Mathf.LerpAngle(HeadTargetContainer.GlobalRotation.Z, targetRotation.Z, delta * HeadRotationLerpSpeed),
        };

        HeadIKSolver.Interpolation = ((-ChestTargetContainer.GlobalTransform.Basis.Z).Dot(targetLookAtPoint - HeadTargetContainer.GlobalPosition) + 1) / 2;
    }

    public Vector3 GetTargetLimbPosition(LimbReference targetLimb, out bool hitSurface, out Vector3 hitNormal)
    {
        Vector3 UpVector = EnemyBody.GetGroundNormal();
        if (UpVector == Vector3.Up)
        {
            UpVector = ChestTargetContainer.GlobalTransform.Basis.Y;
        }

        Vector3 targetPosition = EnemyBody.GlobalPosition + UpVector;

        targetPosition += EnemyBody.LinearVelocity * VelocityAccountingMultiplier;

        if (EnemyBody.LinearVelocity.Length() > 0.5f)
        {
            LastVelocity = EnemyBody.LinearVelocity.Normalized();
        }

        targetPosition += LastVelocity * (BodyLength / 2) * (targetLimb == LimbReference.LeftFoot || targetLimb == LimbReference.RightFoot ? -1 : 1);

        Vector3 centerPoint = targetPosition;
        Vector3 sideAngle = LastVelocity.Cross(UpVector) * (targetLimb == LimbReference.LeftHand || targetLimb == LimbReference.LeftFoot ? -1 : 1);

        targetPosition = centerPoint + (sideAngle * (ShoulderBodyWidth / 2));

        LimbRaycast.GlobalPosition = targetPosition;

        targetPosition = centerPoint + (sideAngle * (BottomBodyWidth / 2)) + (-UpVector * (TargetOffsetDown + 1));

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
            hitNormal = UpVector;
        }

        return targetPosition;
    }

    /// <summary>
    /// Almost Identical to GetTargetLimbPosition, except that the position is generated via sin and cos to make a flailing motion.
    /// </summary>
    public Vector3 GetTargetLimbFlyingPosition(LimbReference targetLimb, float flightTimeOffset)
    {
        Vector3 UpVector = EnemyBody.GetGroundNormal();
        if (UpVector == Vector3.Up)
        {
            UpVector = ChestTargetContainer.GlobalTransform.Basis.Y;
        }

        Vector3 targetPosition = EnemyBody.GlobalPosition;

        targetPosition += EnemyBody.LinearVelocity * VelocityAccountingMultiplier * (targetLimb == LimbReference.LeftFoot || targetLimb == LimbReference.RightFoot ? 0.1f : 1);

        if (EnemyBody.LinearVelocity.Length() > 0.5f)
        {
            LastVelocity = EnemyBody.LinearVelocity.Normalized();
        }

        targetPosition += LastVelocity * BodyLength * (targetLimb == LimbReference.LeftFoot || targetLimb == LimbReference.RightFoot ? -1 : 1);

        Vector3 centerPoint = targetPosition;
        Vector3 sideAngle = LastVelocity.Cross(UpVector) * (targetLimb == LimbReference.LeftHand || targetLimb == LimbReference.LeftFoot ? -1 : 1);

        targetPosition = centerPoint + (sideAngle * (ShoulderBodyWidth / 2));

        LimbRaycast.GlobalPosition = targetPosition;

        targetPosition = centerPoint + (sideAngle * (BottomBodyWidth / 2)) - (UpVector * FlightFlailSize / 2.0f);

        Vector3 flailOffset = new Vector3(0, Mathf.Sin(CurrentFlightTime + flightTimeOffset), Mathf.Cos(CurrentFlightTime + flightTimeOffset)) * FlightFlailSize;

        targetPosition += ChestTargetContainer.GlobalTransform.Basis * flailOffset;

        LimbRaycast.LookAt(targetPosition, Mathf.Abs((targetPosition - LimbRaycast.GlobalPosition).Y) > 0.99f ? Vector3.Back : Vector3.Up);

        LimbRaycast.ForceRaycastUpdate();

        bool hitSurface = LimbRaycast.IsColliding();
        if (hitSurface)
        {
            targetPosition = LimbRaycast.GetCollisionPoint();
        }
        return targetPosition;
    }
}
