using Godot;
using System;

public partial class DamageableObject : Node
{
    [Signal] public delegate void OnDamageEventHandler(Vector3 hitLocation, Vector3 force, Node3D AggressorBodyNode);
    [Export] public PackedScene ImpactEffect { get; set; }

    public virtual void HitObject(Vector3 hitLocation, Vector3 force, Node3D AggressorBodyNode)
    {
        EmitSignal(SignalName.OnDamage, hitLocation, force, AggressorBodyNode);
    }
}
