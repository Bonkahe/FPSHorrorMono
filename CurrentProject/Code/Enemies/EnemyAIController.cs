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
    [Export] public NavigationAgent3D NavigationAgent { get; private set; }
    [Export] public Node3D CurrentTarget { get; private set; }

    /// Not exposed as it is only for in gameplay purposes, used as the origin of the  
    public Vector3 WanderingPosition { get; set; } = Vector3.Zero;

    private bool TargetOverwritten = false;
    private Vector3 TargetOverride = Vector3.Zero;

    private Vector3 LastTargetPosition = Vector3.Zero;


    /// <summary>
    /// Handles snapping to navmesh
    /// </summary>
    public async void PlaceOnMesh()
    {
        WanderingPosition = GlobalPosition;
        NavigationAgent.TargetPosition = GlobalPosition;

        /// Allows the repathing to occur cleanly before teleporting.
        await Task.Delay(1);
        GlobalPosition = NavigationAgent.GetFinalPosition();
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

    public override void _Process(double delta)
    {
        /// Handle pathing override (Later will be used for in combat pathing, and cinematic overrides)
		if (TargetOverwritten)
        {
            if (NavigationAgent.TargetPosition != TargetOverride)
            {
                NavigationAgent.TargetPosition = TargetOverride;
            }
            else if (NavigationAgent.IsNavigationFinished())
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
                NavigationAgent.TargetPosition = LastTargetPosition;
            }

            return;
        }

        /// If neither are available use basic wandering code.
        if (NavigationAgent.IsNavigationFinished())
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

        NavigationAgent.TargetPosition = WanderingPosition + wanderDir;
    }
}