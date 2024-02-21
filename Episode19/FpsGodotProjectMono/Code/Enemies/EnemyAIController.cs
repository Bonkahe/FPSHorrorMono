using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyAIController : Node3D
{
    [ExportCategory("Wandering Settings")]
    /// If true, allows the center of the wander radius to update whenever moving by targetted pathing override.
    [Export] public bool AllowMovementOfWanderCenter { get; private set; } = false;
    [Export] public float WanderRadius { get; private set; } = 20.0f;


    [ExportCategory("Target Tracking Settings")]
    /// Distance moved by the target requiring pathing update
    [Export] public float TargetMovementBias { get; private set; } = 1.0f;
    [Export] public BasicEnemyNavigationAgent NavigationAgent { get; private set; }
    [Export] public Node3D CurrentTarget { get; private set; }

    [ExportCategory("Attack Settings")]
    [Export] public LimbPlacementController LimbController { get; private set; }
    [Export] public float MinimumAttackDelay { get; private set; } = 1.5f;
    [Export] public float MaximumAttackDelay { get; private set; } = 4.0f;
    [Export] public float AttackInitiateDistance { get; private set; } = 6.0f;
    [Export] public float AttackDamageDistance { get; private set; } = 2.0f;
    [Export] public float AttackForce { get; private set; } = 30.0f;

    [ExportCategory("Health Settings")]
    [Export] public float MaxHealth { get; private set; } = 2.0f;
    [Export] public float DamagePerShot { get; private set; } = 1.0f;

    /// Used to impart current velocity to the ragdoll
    [Export] public Skeleton3D BaseSkeleton { get; private set; }
    /// Must have a skeleton3D somewhere in the hirearchy
    [Export] public PackedScene RagdollScene { get; private set; }

    public float CurrentHealth { get; private set; }

    /// Not exposed as it is only for in gameplay purposes, used as the origin of the  
    public Vector3 WanderingPosition { get; set; } = Vector3.Zero;

    private bool TargetOverwritten = false;
    private Vector3 TargetOverride = Vector3.Zero;

    private Vector3 LastTargetPosition = Vector3.Zero;

    private float AttackCooldown;
    private bool IsAttacking;


    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }

    private void ResetAttackCooldown()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        AttackCooldown = rng.RandfRange(MinimumAttackDelay, MaximumAttackDelay);
        IsAttacking = false;
    }

    /// <summary>
    /// Used via signal from damageable object, to subtract health and handle death events.
    /// </summary>
    public void OnDamaged(Vector3 hitLocation, Vector3 force, Node3D AggressorBodyNode)
    {
        CurrentTarget = AggressorBodyNode;
        CurrentHealth -= DamagePerShot;
        if (CurrentHealth <= 0)
        {
            OnDeath(hitLocation, force);
        }
    }

    /// <summary>
    /// Adds a ragdoll to the current location, poses all it's bones to the correct poses
    /// imparts the current velocity to the body, and then finds the nearest bone to the
    /// last shot location, and imparts the impact velocity to that loation.
    /// </summary>
    private void OnDeath(Vector3 hitLocation, Vector3 force)
    {
        Node3D newRagdoll = RagdollScene.Instantiate() as Node3D;

        GetParent().AddChild(newRagdoll);
        Skeleton3D foundSkeleton = RetrieveSkeleton(newRagdoll);

        if (foundSkeleton == null)
        {
            GD.PrintErr("Could not find skeleton 3D in ragdoll scene.");
            return;
        }

        newRagdoll.GlobalPosition = BaseSkeleton.GlobalPosition;
        newRagdoll.GlobalRotation = BaseSkeleton.GlobalRotation;

        foundSkeleton.GlobalPosition = BaseSkeleton.GlobalPosition;
        foundSkeleton.GlobalRotation = BaseSkeleton.GlobalRotation;

        for (int i = 0; i < foundSkeleton.GetBoneCount(); i++)
        {
            foundSkeleton.SetBonePosePosition(i, BaseSkeleton.GetBonePosePosition(i));
            foundSkeleton.SetBonePoseRotation(i, BaseSkeleton.GetBonePoseRotation(i));
        }

        foundSkeleton.PhysicalBonesStartSimulation();

        PhysicalBone3D closestBone = null;
        float closestDistance = Mathf.Inf;

        foreach (var child in foundSkeleton.GetChildren())
        {
            if (child is PhysicalBone3D physicalBone)
            {
                physicalBone.ApplyImpulse(NavigationAgent.LinearVelocity);
                float thisDistance = hitLocation.DistanceTo(physicalBone.GlobalPosition);
                if (thisDistance < closestDistance)
                {
                    closestDistance = thisDistance;
                    closestBone = physicalBone;
                }
            }
        }

        if (closestBone != null)
        {
            closestBone.ApplyImpulse(force, hitLocation);
        }

        QueueFree();
    }


    /// <summary>
    /// Recursively goes through the node and all it's children searching for the first skeleton3D it finds.
    /// </summary>
    private Skeleton3D RetrieveSkeleton(Node curnode)
    {
        if (curnode is Skeleton3D foundSkeleton)
        {
            return foundSkeleton;
        }
        else
        {
            foreach (var child in curnode.GetChildren())
            {
                foundSkeleton = RetrieveSkeleton(child);
                if (foundSkeleton != null)
                {
                    return foundSkeleton;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Handles snapping to navmesh
    /// </summary>
    public void PlaceOnMesh()
    {
        WanderingPosition = GlobalPosition;
        NavigationAgent.SetNewTarget(GlobalPosition);

        GlobalPosition = NavigationAgent.TargetPosition;
    }

    /// <summary>
    /// Sets the node target to chase, this is overwritten by the pathing target, unless the override targetted pathing is true.
    /// </summary>
    public void SetNodeTarget(Node3D newTarget, bool overrideTargettedPathing = false)
    {
        if (TargetOverwritten && overrideTargettedPathing)
        {
            TargetOverwritten = false;
        }

        CurrentTarget = newTarget;
    }

    /// <summary>
    /// Sets the primary target to move too, this is overriding current target, if the overrideAggro option is set to true.
    /// </summary>
    public void SetPathingTarget(Vector3 targetPosition, bool overrideAggro = false)
    {
        if (CurrentTarget != null && !overrideAggro)
        {

            return;
        }

        TargetOverride = targetPosition;
        TargetOverwritten = true;
    }

    /// <summary>
    /// Takes a node and checks if it has a damageable node and apply damage and force to itself.
    /// </summary>
    /// <returns>Whether the object was in fact damaged.</returns>
    private bool AttackTarget(Node3D parentNode)
    {
        Node damageable = parentNode.GetNode("Damageable");
        if (damageable != null && damageable is DamageableObject damageableObject)
        {
            damageableObject.HitObject(NavigationAgent.GlobalPosition, (parentNode.GlobalPosition - NavigationAgent.GlobalPosition).Normalized() * AttackForce, this);

            Vector3 bounceVector = (NavigationAgent.GlobalPosition - parentNode.GlobalPosition).Normalized() * AttackForce;
            SetPathingTarget(NavigationAgent.GlobalPosition + bounceVector);
            NavigationAgent.ApplyImpulse(bounceVector);

            return true;
        }

        return false;
    }

    public void CheckTargetValid()
    {
        if (CurrentTarget != null && !IsInstanceValid(CurrentTarget))
        {
            CurrentTarget = null;
        }
    }

    public override void _Process(double delta)
    {
        CheckTargetValid();

        if (AttackCooldown > 0)
        {
            AttackCooldown = Mathf.Max(AttackCooldown - (float)delta, 0);
        }

        if (IsAttacking)
        {
            if (CurrentTarget == null)
            {
                IsAttacking = false;
                return;
            }

            if (CurrentTarget.GlobalPosition.DistanceTo(NavigationAgent.GlobalPosition) <= AttackDamageDistance)
            {
                ResetAttackCooldown();
                AttackTarget(CurrentTarget);
            }

            //We have spent too long trying to attack
            if (AttackCooldown == 0)
            {
                IsAttacking = false;
            }
            else
            {
                return;
            }
        }

        /// Handle pathing override (Later will be used for in combat pathing, and cinematic overrides)
        if (TargetOverwritten)
        {
            if (NavigationAgent.TargetPosition != TargetOverride)
            {
                NavigationAgent.SetNewTarget(TargetOverride);
            }
            else if (NavigationAgent.IsTargetReached())
            {
                if (AllowMovementOfWanderCenter)
                {
                    WanderingPosition = TargetOverride;
                }
                TargetOverwritten = false;
            }

            return;
        }
        /// If that is not present use the current target node if it is present, simply chasing.
        else if (CurrentTarget != null)
        {
            if (LastTargetPosition.DistanceTo(CurrentTarget.GlobalPosition) > TargetMovementBias)
            {
                LastTargetPosition = CurrentTarget.GlobalPosition;
                NavigationAgent.SetNewTarget(LastTargetPosition);
            }

            if (AttackCooldown == 0 && CurrentTarget.GlobalPosition.DistanceTo(NavigationAgent.GlobalPosition) < AttackInitiateDistance && !LimbController.LaunchingRequested)
            {
                LimbController.OnAttackLaunchRequested(CurrentTarget);
                NavigationAgent.SetNewTarget(CurrentTarget.GlobalPosition);
                IsAttacking = true;
                AttackCooldown = (CurrentTarget.GlobalPosition.DistanceTo(NavigationAgent.GlobalPosition) / (NavigationAgent.MaximumVelocity * LimbController.LaunchVelocityMultiplier)) * 2.0f;
            }

            return;
        }

        /// If neither are available use basic wandering code.
        if (NavigationAgent.IsTargetReached())
        {
            SetWanderPosition();
        }
    }


    /// <summary>
    /// Select direction randomly, then normalize and set the scale to a float between 0 and WanderRadius.
    /// </summary>
    private void SetWanderPosition()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();

        Vector3 wanderDir = new Vector3(rng.RandfRange(-1.0f, 1.0f), 0.0f, rng.RandfRange(-1.0f, 1.0f)).Normalized() * rng.Randf() * WanderRadius;

        NavigationAgent.SetNewTarget(WanderingPosition + wanderDir);
    }
}