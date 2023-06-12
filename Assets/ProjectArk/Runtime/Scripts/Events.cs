using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardTimeStep : GameEvent
{
	public ForwardTimeStep(Character character)
	{
		this.character = character;
	}

	public Character character { get; private set; }
}

public class BackwardTimeStep : GameEvent
{
	public BackwardTimeStep(Character character)
	{
		this.character = character;
	}

	public Character character { get; private set; }
}

public class DeathEvent : GameEvent
{
	//public DeathEvent(Character character)
	//{
	//	this.character = character;
	//}

	//public Character character { get; private set; }
}

public class SpawnEvent : GameEvent
{
	//public SpawnEvent(Character character)
	//{
	//	this.character = character;
	//}

	//public Character character { get; private set; }
}

public class ElementClickedEvent : GameEvent
{
	public ElementClickedEvent(UIElement element)
	{
		this.element = element;
	}

	public UIElement element;
}

public class ElementBackClickedEvent : GameEvent
{
	public ElementBackClickedEvent(UIElement element)
	{
		this.element = element;
	}

	public UIElement element;
}

public class ElementHoveredEvent : GameEvent
{
	public ElementHoveredEvent(UIElement element)
	{
		this.element = element;
	}

	public UIElement element;
}

public class CellClickedEvent : GameEvent
{
	//public CellClickedEvent(HexCell newCell)
	//{
	//	this.newCell = newCell;
	//}

	//public HexCell newCell;
}

//public class DeselectedCellEvent : GameEvent { }

public class CellHoveredEvent : GameEvent
{
	//public CellHoveredEvent(HexCell newCell)
	//{
	//	this.newCell = newCell;
	//}

	//public HexCell newCell;
}

public class EmptyClickEvent : GameEvent { }

public class CharacterSelectedEvent : GameEvent
{
	//public CharacterSelectedEvent(Character character)
	//{
	//	this.character = character;
	//}

	//public Character character;
}

public class CharacterDeselectedEvent : GameEvent
{
	//public CharacterDeselectedEvent(Character character)
	//{
	//	this.character = character;
	//}

	//public Character character;
}

public class HoveredCharacterEvent : GameEvent
{
	//public HoveredCharacterEvent(Character character)
	//{
	//	this.character = character;
	//}

	//public Character character;
}

public class CharacterReadiedEvent : GameEvent
{
	//public CharacterReadiedEvent(Character character, bool readied)
	//{
	//	this.character = character;
	//	this.readied = readied;
	//}

	//public Character character;
	//public bool readied;
}

public class TeamPhaseEvent : GameEvent
{
	//public TeamPhaseEvent(TeamPhase phase)
	//{
	//	this.phase = phase;
	//}

	//public TeamPhase phase;
}

public class WanderersReadyEvent : GameEvent
{
	public WanderersReadyEvent(bool anyWanderersReady)
	{
		this.anyWanderersReady = anyWanderersReady;
	}

	public bool anyWanderersReady;
}

public class AbilitySelectEvent : GameEvent
{
	//public AbilitySelectEvent(Character character, Ability ability)
	//{
	//	this.character = character;
	//	this.ability = ability;
	//}

	//public Character character;
	//public Ability ability;
}

public class AbilityDeselectEvent : GameEvent
{
	//public AbilityDeselectEvent(Character character, Ability ability)
	//{
	//	this.ability = ability;
	//	this.character = character;
	//}

	//public Character character;
	//public Ability ability;
}