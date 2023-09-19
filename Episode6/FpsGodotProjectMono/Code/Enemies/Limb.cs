using Godot;
using System;
using System.Security.Cryptography;


public class BezierCurve
{
    public Vector3 TargetLocation { get; set; }
    public Vector3 TargetLocationControl { get; set; }
    public Vector3 OriginLocation { get; set; }
    public Vector3 OriginLocationControl { get; set; }
    public bool HitSurface { get; set; }

    public Vector3 Lerp(float t)
    {
        return DebugExtensions.GetBezierCurvePosition(TargetLocation, TargetLocationControl, OriginLocation, OriginLocationControl, t);
    }
}

[GlobalClass]
public partial class Limb : Node
{
    [Export] public LimbReference ThisLimb { get; set; }
    [Export] public float ControlPointOffset { get; set; }
    [Export] public float BlendSpeed { get; set; } = 3;
    [Export] public float MinimumMovementDistance { get; set; } = 0.5f;

    public Vector3 CurrentTargetLocation;
    public LimbPlacementController Controller;

    private float CurrentLerpValue;
    private bool CurrentlyTraveling;
    private BezierCurve CurrentCurve;

    public override void _Ready()
	{
        CurrentCurve = GetInitialCurve();
        CurrentTargetLocation = CurrentCurve.TargetLocation;
    }

    public override void _PhysicsProcess(double delta)
    {
        this.DrawPoint(CurrentTargetLocation, (float)delta, CurrentlyTraveling ? new Color(0, 1, 0) : new Color(1, 0, 0), 0.3f);
        if (CurrentlyTraveling)
        {
            CurrentLerpValue = Mathf.Clamp(CurrentLerpValue + (BlendSpeed * (float)delta), 0, 1);
            CurrentTargetLocation = CurrentCurve.Lerp(CurrentLerpValue);
            if (CurrentLerpValue == 1)
            {
                CurrentLerpValue = 0;
                CurrentlyTraveling = false;
            }
        }
    }

    public void InitializeStep()
    {
        BezierCurve newcurve = GetNewCurve();
        if (newcurve != CurrentCurve)
        {
            this.DebugBezierCurve(newcurve.TargetLocation, newcurve.TargetLocationControl, newcurve.OriginLocation, newcurve.OriginLocationControl, new Color(0, 1, 0), 0.3f);
            CurrentLerpValue = 0;
            CurrentlyTraveling = true;
            CurrentCurve = newcurve;
        }
    }

    private BezierCurve GetInitialCurve()
    {
        Vector3 targetPosition = Controller.GetTargetLimbPosition(ThisLimb, out bool hitSurface, out Vector3 hitNormal);

        return new BezierCurve()
        {
            OriginLocation = targetPosition,
            OriginLocationControl = targetPosition + hitNormal * ControlPointOffset,
            TargetLocation = targetPosition,
            TargetLocationControl = targetPosition + hitNormal * ControlPointOffset,
            HitSurface = hitSurface
        };
    }

    private BezierCurve GetNewCurve()
    {
        Vector3 targetPosition = Controller.GetTargetLimbPosition(ThisLimb, out bool hitSurface, out Vector3 hitNormal);

        if (targetPosition.DistanceTo(CurrentTargetLocation) > MinimumMovementDistance)
        {
            return new BezierCurve()
            {
                OriginLocation = CurrentTargetLocation,
                OriginLocationControl = CurrentTargetLocation + (CurrentCurve.TargetLocationControl - CurrentCurve.TargetLocation),
                TargetLocation = targetPosition,
                TargetLocationControl = targetPosition + hitNormal * ControlPointOffset,
                HitSurface = hitSurface
            };
        }
        else
        {
            return CurrentCurve;
        }
    }
}
