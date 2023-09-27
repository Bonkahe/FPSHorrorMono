@tool
extends Node

# FABRIK for Skeletons in Godot4 based on https://github.com/joaen/EasyIK/blob/master/EasyIK/Assets/Scripts/EasyIK.cs

# Usage:
# - add the script to a node
# call the activate function
# fabrik.activate(bone_ids, skeleton, left_hand_target, left_hand_pole)
# bone_ids is an Array with the bone ids of the chain
# skeleton is the skeleton
# left_hand_target is a Node3D
# left_hand_pole is another Node3D

@export var Bones: Array[PhysicalBone3D];
@export var Skeleton: Skeleton3D;
@export var Target: Node3D;
@export var Pole: Node3D;

var numberOfJoints := 2
var iterations := 10
var tolerance := 0.005

var jointTransforms := []
var jointPositions := []
var boneLength := []
var jointStartDirection := []
var startRotation := []

var bones := []
var bone_ids := []
var ikTarget:Transform3D

var distanceToTarget:float
var jointChainLength:float

var startPosition:Vector3

var poleTarget:Transform3D

var skeleton:Skeleton3D
var ikTargetNode:Node3D
var poleTargetNode:Node3D

@export var active := false


func _ready():
	jointTransforms.resize(numberOfJoints+1)
	jointPositions.resize(numberOfJoints+1)
	jointStartDirection.resize(numberOfJoints+1)
	startRotation.resize(numberOfJoints+1)
	if (Bones.size() > 0 and Skeleton != null and Target != null and Pole != null):
		var boneIDs : Array[int];
		for bone in Bones:
			print(bone.get_bone_id());
			boneIDs.append(bone.get_bone_id());
		activate(boneIDs, Skeleton, Target, Pole)


func activate(_bone_ids:Array, _skeleton:Skeleton3D, _ikTargetNode:Node3D, _poleTargetNode:Node3D):
	bone_ids = _bone_ids
	skeleton = _skeleton
	ikTargetNode = _ikTargetNode
	poleTargetNode = _poleTargetNode
	active = true
	
	for b in bone_ids:
		bones.append(skeleton.get_bone_global_pose(b))
		
	jointChainLength = 0
	for i in range(numberOfJoints):
		var bl = bones[i].origin.distance_to(bones[i+1].origin)
		boneLength.append(bl)
		jointChainLength += bl
		jointStartDirection[i] = bones[i+1].origin - bones[i].origin
		startRotation[i] = bones[i].basis.get_rotation_quaternion()
		#startRotation[i] = skeleton.get_bone_pose_rotation(bone_ids[i])


func _physics_process(_delta):
	if !active:
		return
	fabrik_step()


func fabrik_step():
	
	for b in bone_ids:
		bones.append(skeleton.get_bone_global_pose(b))
	
	for i in range(numberOfJoints+1):
		jointTransforms[i] = bones[i]
	
	ikTarget = ikTargetNode.global_transform

	poleTarget = poleTargetNode.global_transform
	
	SolveIK()


func SolveIK():
	for i in range(jointTransforms.size()):
		jointPositions[i] = jointTransforms[i].origin

	distanceToTarget = jointPositions[0].distance_to(ikTarget.origin)
	
	if distanceToTarget > jointChainLength:
		var direction = ikTarget.origin - jointPositions[0]

		for i in range(1, jointPositions.size()):
			jointPositions[i] = jointPositions[i - 1] + direction.normalized() * boneLength[i - 1]
	else:
		var distToTarget:float = jointPositions[jointPositions.size() - 1].distance_to(ikTarget.origin)
		var counter := 0
		
		while distToTarget > tolerance:
			startPosition = jointPositions[0]
			Backward()
			Forward()
			counter += 1
			if counter > iterations:
				break
	
		PoleConstraint()
	
	for i in range(jointPositions.size()-1):
		jointTransforms[i].origin = jointPositions[i]
		var direction = jointPositions[i + 1] - jointPositions[i]
		var targetRotation = Quaternion(jointStartDirection[i].normalized(), direction.normalized())
		jointTransforms[i].basis = Basis(targetRotation * startRotation[i])
		skeleton.set_bone_global_pose_override(bone_ids[i], jointTransforms[i], 1, true)


func Backward():
	# Iterate through every position in the list until we reach the start of the chain
	for i in range(jointPositions.size()-1, -1, -1):
		# The last bone position should have the same position as the ikTarget
		if i == jointPositions.size() - 1:
			jointPositions[i] = ikTarget.origin
		else:
			jointPositions[i] = jointPositions[i + 1] + (jointPositions[i] - jointPositions[i + 1]).normalized() * boneLength[i]

func Forward():
	# Iterate through every position in the list until we reach the end of the chain
	for i in range(jointPositions.size()):
		# The first bone position should have the same position as the startPosition
		if i == 0:
			jointPositions[i] = startPosition
		else:
			jointPositions[i] = jointPositions[i - 1] + (jointPositions[i] - jointPositions[i - 1]).normalized() * boneLength[i - 1]


func PoleConstraint():
	if poleTarget != null and numberOfJoints < 4:
		# Get the limb axis direction
		var limbAxis = (jointPositions[2] - jointPositions[0]).normalized()

		# Get the direction from the root joint to the pole target and mid joint
		var poleDirection:Vector3 = Vector3(poleTarget.origin - jointPositions[0]).normalized()
		var boneDirection:Vector3 = Vector3(jointPositions[1] - jointPositions[0]).normalized()
		
		# Ortho-normalize the vectors
		# Vector3.OrthoNormalize(ref limbAxis, ref poleDirection)
		# Vector3.OrthoNormalize(ref limbAxis, ref boneDirection)
		var on_pole = OrthoNormalize(limbAxis, poleDirection)
		limbAxis = on_pole[0]
		poleDirection = on_pole[1]
		var on_bone = OrthoNormalize(limbAxis, boneDirection)
		limbAxis = on_bone[0]
		boneDirection = on_bone[1]

		# Calculate the angle between the boneDirection vector and poleDirection vector
		# Quaternion angle = Quaternion.FromToRotation(boneDirection, poleDirection)
		var angle = Quaternion(boneDirection, poleDirection).normalized()

		# Rotate the middle bone using the angle
		jointPositions[1] = angle * (jointPositions[1] - jointPositions[0]) + jointPositions[0]


func bone_set_position(bone_index:int, bone_global_position:Vector3, lerp_amount:float = 1.0):
	var bone_transform = skeleton.get_bone_global_pose_no_override(bone_index)
	bone_transform.origin = bone_global_position
	skeleton.set_bone_global_pose_override(bone_index, bone_transform, lerp_amount, true)


func bone_look_at(bone_index:int, bone_global_position:Vector3, target_global_position:Vector3, lerp_amount:float = 1.0):
	var bone_transform = skeleton.get_bone_global_pose_no_override(bone_index)
		
	#var target_pos = to_local(target_global_position)
	#var target_pos = target_global_position
	#var bone_origin = bone_transform.origin
	var bone_origin = bone_global_position
	#bone_transform.basis = bone_transform.basis.looking_at( -(target_global_position - bone_transform.origin).normalized())
	bone_transform.basis = bone_transform.basis.looking_at( -(target_global_position - bone_global_position).normalized())
	# bone_transform.origin = bone_origin
	bone_transform.origin = bone_global_position
	skeleton.set_bone_global_pose_override(bone_index, bone_transform, lerp_amount, true)


# Generated with ChatGPT
func OrthoNormalize(normal:Vector3, tangent:Vector3):
	normal = normal.normalized()
	tangent = tangent - normal * tangent.dot(normal)
	tangent = tangent.normalized()
	return [normal, tangent]
