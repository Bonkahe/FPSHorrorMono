using Godot;
using System;
using System.Linq;

public partial class WeaponEffectsController : Node
{
    [Export] public Node3D BarrelEnd { get; set; }
    [Export] public RayCast3D BarrelRayCast { get; set; }
    [Export] public PackedScene MuzzleFlash { get; set; }
    [Export] public PackedScene ImpactEffect { get; set; }

    [Export] public float ImpactForce { get; set; } = 20;

    public bool HasRoundAvailable { get; private set; } = false;
    private int currentRoundCount = 0;

    public override void _Ready()
	{
        Reload();
    }

	public void Reload()
    {
        currentRoundCount = 6;
        HasRoundAvailable = true;
    }

    public void FireRevolver()
    {
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
}
