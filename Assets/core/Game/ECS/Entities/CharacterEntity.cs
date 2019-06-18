using Kernel.core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEntity : ECSEntity
{
	public CharacterEntity():base()
	{
		AddComponent<AttributeComponent>()
		.AddComponent<ActionComponent>()
		.AddComponent<PhysicsComponent>()
		.AddComponent<TargetComponent>()
		.AddComponent<NavigationComponent>()
		.AddComponent<CharacterPresentComponent>()
		.AddComponent<BehaviorTreeComponent>()
		.AddComponent<NodeCanvasComponent>();
	}
}
