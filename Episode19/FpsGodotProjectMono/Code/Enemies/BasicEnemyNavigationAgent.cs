using Godot;
using System;
using System.Collections.Generic;

public class PathWaypoint
{
    public Vector3 WaypointPosition;
    public Vector3 WaypointNormal;
    public bool WaypointNotFloor;
    public float EstimatedWaypointDuration;
}

public partial class BasicEnemyNavigationAgent : RigidBody3D
{
    [Signal] public delegate void LaunchRequestEventHandler();

    [ExportGroup("Path Navigation")]
    [Export] public float MaximumVelocity { get; set; } = 2;
    [Export] public float VelocityChange { get; set; } = 2;

    [Export] public float MinimumLaunchDelay { get; set; } = 0.25f;

    [ExportGroup("Path Generation")]
    [Export] public bool DebugActive { get; set; } = false;
    [Export] public float GroundedCheckDistance { get; set; } = 1.25f;
    [Export] public float StoppingDistance { get; set; } = 3.0f;
    [Export(PropertyHint.Range, "0,1")] public float WallRoofDistanceChangeAllowance { get; set; } = 0.25f;
    [Export] public float UpPathCheckDistance { get; set; } = 15.0f;
    [Export(PropertyHint.Layers3DPhysics)] public uint WorldCollision { get; set; }
    [Export(PropertyHint.Range, "0,1")] public float ChanceOfUpPath { get; set; } = 0.1f;


    public Vector3 DesiredVelocity { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    public PathWaypoint LastRetrievedPathWaypoint { get; private set; }

    public enum GroundedState { None, Floor, WallRoof }

    public GroundedState CurrentGroundedState { get; private set; } = GroundedState.None;
    public Vector3 CurrentGroundNormal { get; private set; } = Vector3.Up;


    private float InitialWallRoofDistance = 0.0f;

    private float LaunchAttemptInterval;
    private float WaypointFailureTimer;

    private float InternalGravityScale;
    private RandomNumberGenerator rng = new RandomNumberGenerator();

    private List<PathWaypoint> CurrentPath = new List<PathWaypoint>();

    private Rid NavMap;

    public override void _Ready()
    {
        NavMap = GetWorld3D().NavigationMap;
        InternalGravityScale = GravityScale;

        rng.Randomize();
        LaunchAttemptInterval = rng.RandfRange(0, 0.2f);
        LastRetrievedPathWaypoint = GetNextWaypoint();
    }

    public override void _PhysicsProcess(double delta)
    {
        WaypointReachedCheck();
        CurrentGroundedState = GetGrounded();

        if (IsTargetReached())
        {
            if (GlobalPosition.DistanceTo(TargetPosition) > StoppingDistance)
            {
                DesiredVelocity = (TargetPosition - GlobalPosition).Normalized() * MaximumVelocity;
            }
            else
            {
                DesiredVelocity = Vector3.Zero;
            }

            HandleGravity(LastRetrievedPathWaypoint.WaypointNotFloor);
            return;
        }

        WaypointFailureTimer -= (float)delta;
        if (WaypointFailureTimer <= 0)
        {
            SetNewTarget(TargetPosition);
        }

        PathWaypoint currentWaypoint = GetNextWaypoint();


        if (CurrentGroundedState == GroundedState.None)
        {
            LaunchAttemptInterval = MinimumLaunchDelay;
        }
        else
        {
            if (LaunchAttemptInterval > 0)
            {
                LaunchAttemptInterval -= (float)delta;
            }
        }
        HandleGravity(currentWaypoint.WaypointNotFloor);

        DesiredVelocity = (currentWaypoint.WaypointPosition - GlobalPosition).Normalized() * MaximumVelocity;
        if (DebugActive)
        {
            DebugExtensions.BuildDebugLine(this, GlobalPosition, currentWaypoint.WaypointPosition, (float)delta * 2.0f, new Color(0, 1, 0));
        }
    }

    public void HandleGravity(bool waypointOnFloor)
    {
        if (waypointOnFloor)
        {
            if (CurrentGroundedState == GroundedState.WallRoof)
            {
                LinearVelocity -= CurrentGroundNormal * 0.5f;
                GravityScale = 0;
            }
            else
            {
                if (CurrentGroundedState == GroundedState.Floor)
                {
                    if (LaunchAttemptInterval <= 0)
                    {
                        LaunchAttemptInterval = MinimumLaunchDelay;
                        EmitSignal(SignalName.LaunchRequest);
                    }
                }

                GravityScale = InternalGravityScale;
            }
        }
        else
        {
            GravityScale = InternalGravityScale;
        }
    }

    public GroundedState GetGrounded()
    {
        var sphereQuery = new PhysicsShapeQueryParameters3D();
        sphereQuery.Shape = new SphereShape3D() { Radius = GroundedCheckDistance };
        sphereQuery.Transform = GlobalTransform;
        sphereQuery.CollisionMask = WorldCollision;


        var spaceState = GetWorld3D().DirectSpaceState;

        CurrentGroundNormal = Vector3.Up;
        var result = spaceState.IntersectShape(sphereQuery);
        //If we're not near anything, we're in the air.
        if (result.Count > 0)
        {
            Vector3 currentNormal = GetCurrentNormal();
            //Quit out early if we're supposed to be on the ground.
            if (currentNormal == Vector3.Up)
            {
                return GroundedState.Floor;
            }

            //Check the direction of the current "wallroof"
            var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, GlobalPosition - (currentNormal * GroundedCheckDistance), WorldCollision);
            var raycastresult = spaceState.IntersectRay(query);

            //If that is nearby, we're on the wall/roof
            if (raycastresult.Count > 0)
            {
                CurrentGroundNormal = (Vector3)raycastresult["normal"];
                return GroundedState.WallRoof;
            }
            else
            {
                return GroundedState.Floor;
            }
        }
        else
        {

            return GroundedState.None;
        }
    }

    public Vector3 GetGroundNormal()
    {
        return CurrentGroundNormal;
    }
    public Vector3 GetCurrentNormal()
    {
        if (LastRetrievedPathWaypoint == null)
        {
            LastRetrievedPathWaypoint = GetNextWaypoint();
        }
        return LastRetrievedPathWaypoint.WaypointNormal;
    }

    public PathWaypoint GetFinalWaypoint()
    {
        if (CurrentPath.Count > 0)
        {
            return CurrentPath[CurrentPath.Count - 1];
        }
        else
        {
            return GetNextWaypoint();
        }
    }

    public PathWaypoint GetNextWaypoint()
    {
        if (CurrentPath.Count > 0)
        {
            LastRetrievedPathWaypoint = CurrentPath[0];
            WaypointFailureTimer = LastRetrievedPathWaypoint.EstimatedWaypointDuration;
            return LastRetrievedPathWaypoint;
        }
        else
        {
            if (LastRetrievedPathWaypoint == null)
            {
                LastRetrievedPathWaypoint = new PathWaypoint() { WaypointNormal = Vector3.Up, WaypointPosition = GlobalPosition, WaypointNotFloor = false };
            }

            return LastRetrievedPathWaypoint;
        }
    }

    public bool IsTargetReached()
    {
        return CurrentPath.Count == 0;
    }

    private void WaypointReachedCheck()
    {
        if (CurrentPath.Count > 0)
        {
            Vector3 currentDelta = CurrentPath[0].WaypointPosition - GlobalPosition;
            bool closertoNext = false;
            if (CurrentPath.Count > 1)
            {
                closertoNext = CurrentPath[0].WaypointPosition.DistanceTo(GlobalPosition) > CurrentPath[1].WaypointPosition.DistanceTo(GlobalPosition);
            }

            if (currentDelta.Length() <= StoppingDistance || closertoNext)
            {
                bool launch = CurrentPath.Count > 1 && CurrentPath[0].WaypointNotFloor != CurrentPath[1].WaypointNotFloor;

                CurrentPath.RemoveAt(0);
                if (launch)
                {
                    LaunchAttemptInterval = MinimumLaunchDelay;
                    EmitSignal(SignalName.LaunchRequest);
                }
            }
        }
    }

    public void SetNewTarget(Vector3 newTarget)
    {
        CurrentPath.Clear();
        rng.Randomize();

        Vector3[] newPath = NavigationServer3D.MapGetPath(NavMap, GlobalPosition, newTarget, false);

        Vector3 CurrentUpVector = Vector3.Up;
        Vector3 LastWaypoint = GlobalPosition;
        bool LastOnCeiling = false;
        float currentPathChance = ChanceOfUpPath / (float)newPath.Length;


        if (LastRetrievedPathWaypoint != null)
        {
            LastOnCeiling = LastRetrievedPathWaypoint.WaypointNotFloor;
        }

        var spaceState = GetWorld3D().DirectSpaceState;

        foreach (var waypoint in newPath)
        {

            float estimatedDuration = LastWaypoint.DistanceTo(waypoint) / MaximumVelocity * 2.0f;

            if (LastOnCeiling && rng.Randf() < currentPathChance)
            {
                if (DebugActive)
                {
                    DebugExtensions.BuildDebugLine(this, LastWaypoint, waypoint, 2.0f, new Color(0, 0, 1));
                }
                LastOnCeiling = false;
            }

            LastWaypoint = waypoint;

            if (!LastOnCeiling)
            {
                if (rng.Randf() < currentPathChance)
                {
                    CurrentUpVector = new Vector3(rng.RandfRange(-1, 1), rng.RandfRange(0, 1), rng.RandfRange(-1, 1));
                }
                else
                {
                    CurrentPath.Add(new PathWaypoint() { WaypointNormal = Vector3.Up, WaypointPosition = waypoint, WaypointNotFloor = false, EstimatedWaypointDuration = estimatedDuration });

                    continue;
                }
            }

            
            var query = PhysicsRayQueryParameters3D.Create(waypoint, waypoint + (CurrentUpVector.Normalized() * UpPathCheckDistance), WorldCollision);
            var result = spaceState.IntersectRay(query);
            if (result.Count > 0)
            {
                PathWaypoint newWaypoint = new PathWaypoint() { WaypointNormal = (Vector3)result["normal"], WaypointPosition = (Vector3)result["position"], WaypointNotFloor = true, EstimatedWaypointDuration = estimatedDuration };
                float currentDistance = newWaypoint.WaypointPosition.DistanceTo(waypoint);
                float currentChangeDelta = Mathf.Abs((currentDistance - InitialWallRoofDistance) / InitialWallRoofDistance);


                if (LastOnCeiling && currentChangeDelta > WallRoofDistanceChangeAllowance)
                {
                    LastOnCeiling = false;
                }
                else
                {
                    if (!LastOnCeiling)
                    {
                        InitialWallRoofDistance = currentDistance;
                        LastOnCeiling = true;
                    }

                    CurrentPath.Add(newWaypoint);
                    continue;
                }
            }

            CurrentPath.Add(new PathWaypoint() { WaypointNormal = Vector3.Up, WaypointPosition = waypoint, WaypointNotFloor = false, EstimatedWaypointDuration = estimatedDuration });

        }

        if (CurrentPath.Count > 0)
        {
            TargetPosition = CurrentPath[CurrentPath.Count - 1].WaypointPosition;
        }
        else
        {
            TargetPosition = GlobalPosition;
        }

        if (DebugActive)
        {
            Vector3 lastPosition = GlobalPosition;
            foreach (var waypoint in CurrentPath)
            {
                DebugExtensions.BuildDebugLine(this, lastPosition, waypoint.WaypointPosition, 5.0f, new Color(1, 0, 0));
                lastPosition = waypoint.WaypointPosition;
            }
        }
    }
}
