using Godot;
using System;

/// Used to make sure it is addable via the add node menu in editor.
[GlobalClass]
public partial class EnemySpawnPoint : Marker3D
{
    [Export] public PackedScene EnemyPackedScene { get; private set; }
    [Export] public bool AutoSetTarget { get; private set; } = false;
    [Export] public Node3D TargetToSet { get; private set; }

    /// <summary>
	/// Executed via signal, primarily from a TriggerVolume Node, discards the Node3D (using "_") so it can be called via bodyentered trigger.
	/// </summary>
	public void SpawnInitiated(Node3D _)
    {
        EnemyAIController newEnemy = EnemyPackedScene.Instantiate() as EnemyAIController;
        GetParent().AddChild(newEnemy);
        newEnemy.GlobalPosition = GlobalPosition;
        newEnemy.GlobalRotation = GlobalRotation;

        newEnemy.PlaceOnMesh();
        if (AutoSetTarget)
        {
            newEnemy.SetNodeTarget(TargetToSet);
        }
    }
}
