using Godot;
using System;

public partial class IKTester : SkeletonIK3D
{
	[Export] public Skeleton3D Skeleton { get; set; }
	[Export] public PhysicalBone3D HandNode { get; set; }
	[Export] public Node3D TargetNode { get; set; }

	int boneID;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		boneID = HandNode.GetBoneId();
        Start();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//this.DrawPoint(TargetNode.GlobalPosition, (float)delta, new Color(1, 1, 1));
		//Transform3D boneTransform = Skeleton.GetBoneGlobalPose(boneID);

		//boneTransform.Origin = TargetNode.GlobalPosition;
		//Skeleton.SetBoneGlobalPoseOverride(boneID, boneTransform, 1, true);
    }
}
