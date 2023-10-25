using Godot;
using System;
using System.Linq;

public partial class WeaponEffectsController : Node
{
    [ExportCategory("Shot Functionality")]
    [Export] public Node3D BarrelEnd { get; set; }
    [Export] public RayCast3D BarrelRayCast { get; set; }
    [Export] public PackedScene MuzzleFlash { get; set; }
    [Export] public PackedScene ImpactEffect { get; set; }

    [Export] public float ImpactForce { get; set; } = 20;

    [ExportCategory("Spread")]
    [Export] public FastNoiseLite SpreadNoise { get; set; }

    [Export] public float SpreadPanningSpeed { get; set; } = 10;
    [Export] public float SpreadAimingConeSize { get; set; } = 5;
    [Export] public float SpreadIdleConeSize { get; set; } = 20;

    [Export] public float SpreadBloomPerShot { get; set; } = 5;
    [Export] public float SpreadBloomDecay { get; set; } = 3;

    [Export(PropertyHint.Range, "-1,1")] public float RecoilHorizontalBias { get; set; } = -0.15f;
    [Export(PropertyHint.Range, "0,1")] public float RecoilRotationBias { get; set; } = 0.85f;

    [Export] public float RecoilSize { get; set; } = 15;
    [Export] public float RecoilPanningSpeed { get; set; } = 15;
    [Export] public float RecoilFade { get; set; } = 20;
    [Export] public float RecoilActualBlendSpeed { get; set; } = 10;


    [ExportCategory("Node References")]
    [Export] public SkeletonIK3D RightHandIKSolver { get; set; }
    [Export] public SkeletonIK3D LeftHandIKSolver { get; set; }

    [Export] public Node3D CameraNode { get; set; }

    [Export] public Node3D AimingIKContainer { get; set; }
    [Export] public Node3D RightHandIdleIKContainer { get; set; }
    [Export] public Node3D LeftHandIdleIKContainer { get; set; }

    [Export] public Node3D RightHandIdleIKTarget { get; set; }
    [Export] public Node3D RightHandAimingIKTarget { get; set; }

    [Export] public Node3D LeftHandIdleIKTarget { get; set; }
    [Export] public Node3D LeftHandAimingIKTarget { get; set; }

    public bool HasRoundAvailable { get; private set; } = false;
    public bool IsAiming { get; private set; } = false;
    private int currentRoundCount = 0;

    private float currentRecoilTarget;
    private float currentRecoilActual;

    private float currentRecoilTime;

    private float currentSpreadAdditive;
    private float currentSpreadTime;

    private Vector3 AimingTargetBasePosition;
    private Vector3 RightHandIdleBasePosition;
    private Vector3 LeftHandIdleBasePosition;

    public override void _Ready()
    {
        AimingTargetBasePosition = AimingIKContainer.Position;
        RightHandIdleBasePosition = RightHandIdleIKContainer.Position;
        LeftHandIdleBasePosition = LeftHandIdleIKContainer.Position;

        RightHandIKSolver.Start();
        LeftHandIKSolver.Start();

        Reload();
    }

    public override void _Process(double delta)
    {
        currentRecoilTime = Mathf.Wrap(currentRecoilTime + (float)(delta * RecoilPanningSpeed), 0, 1000000);
        currentSpreadTime = Mathf.Wrap(currentSpreadTime + (float)(delta * SpreadPanningSpeed), 0, 1000000);

        currentSpreadAdditive = Mathf.Max(currentSpreadAdditive - ((float)delta * SpreadBloomDecay), 0);
        currentRecoilTarget = Mathf.Max(currentRecoilTarget - ((float)delta * RecoilFade), 0);

        currentRecoilActual = Mathf.Lerp(currentRecoilActual, currentRecoilTarget, (float)delta * RecoilActualBlendSpeed);
        Vector3 recoil = (new Vector3(SpreadNoise.GetNoise1D(currentRecoilTime) * 0.5f + 1, SpreadNoise.GetNoise1D(currentRecoilTime + 1000) - RecoilHorizontalBias, 0) * Mathf.DegToRad(currentRecoilActual));

        if (IsAiming)
        {
            Vector3 spread = (new Vector3(SpreadNoise.GetNoise1D(currentSpreadTime), SpreadNoise.GetNoise1D(currentSpreadTime + 1000), 0) * Mathf.DegToRad(SpreadAimingConeSize + currentSpreadAdditive));
            AimingIKContainer.Rotation = recoil + spread;
            AimingIKContainer.GlobalPosition = CameraNode.ToGlobal(AimingTargetBasePosition).Lerp(CameraNode.GlobalPosition + (-AimingIKContainer.GlobalTransform.Basis.Z).Normalized() * AimingTargetBasePosition.Length(), RecoilRotationBias);
        }
        else
        {
            Vector3 spread = (new Vector3(SpreadNoise.GetNoise1D(currentSpreadTime), SpreadNoise.GetNoise1D(currentSpreadTime + 1000), 0) * Mathf.DegToRad(SpreadIdleConeSize + currentSpreadAdditive));
            RightHandIdleIKContainer.Rotation = recoil + spread;
            RightHandIdleIKContainer.GlobalPosition = CameraNode.ToGlobal(RightHandIdleBasePosition).Lerp(CameraNode.ToGlobal(new Vector3(RightHandIdleBasePosition.X, RightHandIdleBasePosition.Y, 0)) + (-RightHandIdleIKContainer.GlobalTransform.Basis.Z).Normalized() * RightHandIdleBasePosition.Length(), RecoilRotationBias);
        }
    }

    public void Reload()
    {
        currentRoundCount = 6;
        HasRoundAvailable = true;
    }

    public void FireRevolver()
    {
        currentRecoilTarget += RecoilSize;
        currentSpreadAdditive += SpreadBloomPerShot;

        Node3D newMuzzleFlash = MuzzleFlash.Instantiate() as Node3D;
        BarrelEnd.AddChild(newMuzzleFlash);
        newMuzzleFlash.GlobalPosition = BarrelEnd.GlobalPosition;
        newMuzzleFlash.GlobalRotation = BarrelEnd.GlobalRotation;

        currentRoundCount -= 1;
        HasRoundAvailable = currentRoundCount > 0;

        if (BarrelRayCast.IsColliding())
        {
            Vector3 hitLocation = BarrelRayCast.GetCollisionPoint();
            PackedScene damageableEffect = ImpactEffect;
            Node3D ImpactedTarget = BarrelRayCast.GetCollider() as Node3D;

            if (ImpactedTarget is RigidBody3D rigidbody)
            {
                rigidbody.ApplyImpulse(-BarrelRayCast.GlobalTransform.Basis.Z * ImpactForce, hitLocation - rigidbody.GlobalPosition);
            }
            else if (ImpactedTarget is PhysicalBone3D physicalBone)
            {
                physicalBone.ApplyImpulse(-BarrelRayCast.GlobalTransform.Basis.Z * ImpactForce, hitLocation - physicalBone.GlobalPosition);
            }

            if (ImpactedTarget.GetChildren().FirstOrDefault(x => x.Name == "Damageable") is DamageableObject damageable)
            {
                if (damageable.HitObject != null)
                {
                    damageableEffect = damageable.ImpactEffect;
                }
                damageable.HitObject(hitLocation, -BarrelRayCast.GlobalTransform.Basis.Z * ImpactForce);
            }

            Node3D newImpactEffect = damageableEffect.Instantiate() as Node3D;
            AddChild(newImpactEffect);
            newImpactEffect.GlobalPosition = hitLocation;

            Vector3 lookAtPoint = BarrelRayCast.GetCollisionNormal();
            if (lookAtPoint != Vector3.Zero)
            {
                newImpactEffect.LookAt(newImpactEffect.GlobalPosition + lookAtPoint, Mathf.Abs(lookAtPoint.Y) > 0.99 ? Vector3.Back : Vector3.Up);
            }
        }
    }

    public void SetAimMode()
    {
        LeftHandIKSolver.TargetNode = LeftHandAimingIKTarget.GetPath();
        RightHandIKSolver.TargetNode = RightHandAimingIKTarget.GetPath();
        IsAiming = true;
    }

    public void SetIdleMode()
    {
        LeftHandIKSolver.TargetNode = LeftHandIdleIKTarget.GetPath();
        RightHandIKSolver.TargetNode = RightHandIdleIKTarget.GetPath();
        IsAiming = false;
    }
}
