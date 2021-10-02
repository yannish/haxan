using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TeamPhase
{
    PLAYER,
    ENEMY
}

[Serializable]
public class Turn
{
    [ReadOnly]
    public Character owner;
    public Stack<CharacterCommand> commands;

    //public Turn(Character owner)
    //{
    //    this.owner = owner;
    //    //this.commands = commands;
    //}
}

/*
 *... why put turnflow control right in here w/ mainflow...
 * 
 * when you've got a command chain in from whichever character (wanderer or enemy)
 * that's it's own flow, so makes sense to inject here?
 */

public class MainFlowController : FlowController
{
    //[ReadOnly]
    public TeamPhase phase = TeamPhase.PLAYER;
    public void SetPhase(TeamPhase newPhase)
    {
        switch(newPhase)
        {
            case TeamPhase.PLAYER:
                PopulatePlayerTurns();
                break;

            case TeamPhase.ENEMY:
                PopulateEnemyTurns();
                break;
        }

        phase = newPhase;
    }


	//[ReadOnly] 
    public Character currCharacter;


	//... the commands waiting to be processed:
	public Stack<CharacterCommand> currCommandStack;
    //... the command currently being ticked along:
    public CharacterCommand activeCommand;
	public Stack<CharacterCommand> commandHistory = new Stack<CharacterCommand>();


    public List<Turn> playerTurns;
    void PopulatePlayerTurns()
    {
        playerTurns.Clear();
        foreach(Wanderer wanderer in Globals.ActiveWanderers.Items)
        {
            if(!wanderer.isStunned)
            {
                playerTurns.Add(new Turn() { owner = wanderer });
            }
        }
    }

    public List<Turn> enemyTurns;
    void PopulateEnemyTurns()
    {
        enemyTurns.Clear();
        foreach(Enemy enemy in Globals.ActiveEnemies.Items)
        {
            if(!enemy.isStunned)
            {
                enemyTurns.Add(new Turn() { owner = enemy });
            }
        }
    }


    private void LateUpdate()
	{
        switch (phase)
        {
            case TeamPhase.PLAYER:
                if (
                    currCommandStack.IsNullOrEmpty()
                    && commandHistory.IsNullOrEmpty()
                    )
                {
                    if (subFlow != null && subFlow is WandererFlowController)
                    {
                        var currCharacterFlow = subFlow as WandererFlowController;

                        if (currCharacterFlow.TryGetCommandStack(ref currCommandStack))
                        {
                            activeCommand = currCommandStack.Pop();
                            activeCommand.Execute();

                            Debug.Log("new command chain from : " + currCharacterFlow.name);
                            Debug.Log("... chain length : " + currCommandStack.Count);
                        }
                    }
                }

                break;
        }


        //... process an active command, clear it if it's done:
        if(activeCommand != null)
        {
            if (activeCommand.Tick())
            {
                commandHistory.Push(activeCommand);
                if (!currCommandStack.IsNullOrEmpty())
                {
                    activeCommand = currCommandStack.Pop();
                    activeCommand.Execute();
                }
                else
                {
                    Debog.logGameflow("All commands processed.");
                    currCommandStack = null;
                    activeCommand = null;
                }
            }

            return;
        }
        
        //if(activeCommand == null && currCommandStack.IsNullOrEmpty())
        //{
        //}

		//if(activeCommand == null)
		//{
		//	if (Input.GetKeyDown(KeyCode.N))
		//		ProcessCommand();

		//	if (Input.GetKeyDown(KeyCode.R))
		//		RewindCommand();

		//	if (Input.GetKeyDown(KeyCode.C))
		//		BankCommand();
		//}
		//else
		//{
		//	if (activeCommand.Tick())
		//		activeCommand = null;
		//}
	}


	void BankCommand()
	{
		currCommandStack = null;
		commandHistory = null;
	}


    void ProcessCommand()
	{
        if(currCommandStack.IsNullOrEmpty())
        {
            Debug.LogWarning("No more commands to process!");
            return;
        }

        activeCommand = currCommandStack.Pop();
		activeCommand.Execute();
		commandHistory.Push(activeCommand);

        Events.instance.Raise(new ForwardTimeStep(activeCommand.character));

        //...shouldn't need to validate here, should be building a legit chain to begin with
        //if (currCommandChain.Peek().IsValid())
        //{
        //}
    }


    void RewindCommand()
	{
		if (!commandHistory.IsNullOrEmpty())
		{
			var poppedCommand = commandHistory.Pop();
			poppedCommand.Undo();
			currCommandStack.Push(poppedCommand);

			Events.instance.Raise(new BackwardTimeStep(poppedCommand.character));
		}
		else
		{
			Debug.LogWarning("No command history to undo!");
		}
	}

	public override void Enter()
	{
		Debug.Log("Entered maincontroller flow");
	}

	public override void Exit()
	{
		Debug.Log("Exited maincontroller flow");
		base.Exit();
	}

	public override bool HandleHover(ElementHoveredEvent e)
	{
		if (subFlow != null && subFlow.HandleHover(e))
			return true;

		if (peekedFlow != null)
        {
            peekedFlow.HoverUnpeek();
            peekedFlow = null;
        }

		//... 
		if (currCommandStack != null)
			return false;

		if (e.element == null || e.element.flowController == null)
			return false;

		peekedFlow = e.element.flowController;
		peekedFlow.HoverPeek();

		return false;
	}

	public override FlowState HandleBackInput(ElementBackClickedEvent e, FlowController parentController = null)
	{
		if(e.element.flowController == subFlow)
		{
			TransitionTo(null);
			peekedFlow = e.element.flowController;
			peekedFlow.HoverPeek();
		}

		return FlowState.RUNNING;
	}

	public override FlowState HandleInput(ElementClickedEvent e, FlowController owner)
	{
		Debog.logGameflow("Handling input on maincontroller");

		if (currCommandStack != null)
		{
			Debog.logGameflow("... curr command stack still being processed");
			return FlowState.RUNNING;
		}

		if (e.element is TurnButton)
        {
            Debog.logInput("TURN BUTTON PRESSED");
            //(e.element as TurnButton).EndTurn();
            return FlowState.RUNNING;
        }


        //if there's already a subFlow, pass input through that:
        if (subFlow != null)
		{
			Debug.Log("... subflow on maincontroller");

			var subFlowState = subFlow.HandleInput(e, this);

			switch (subFlowState)
			{
				case FlowState.YIELD:
					break;

				case FlowState.DONE:
					TransitionTo(null);
					return FlowState.RUNNING;

				case FlowState.RUNNING:
					subFlow.Update();
					return FlowState.RUNNING;
			}
		}

		//... if subflow didn't want input, or you've clicked a new one:
		if (e.element.flowController != null)
		{
            Debog.logGameflow("clicked element had a subflow : " + e.element.flowController.name);

			if(e.element.flowController is CellFlowController)
			{
				return FlowState.RUNNING;
			}

			//... clicking the element you're already in:
            if (e.element.flowController == subFlow)
			{
				Debog.logGameflow("clicked existing element");
				TransitionTo(null);

				//... go back to peeking it:
				peekedFlow = e.element.flowController;
				peekedFlow.HoverPeek();

				//... clear out "selected wanderer so that TurnFlow stops ticking it:
				//Globals.SelectedWanderer.Items.Clear();
			}
			else
			{
				//Debog.logGameflow("found flow, heading in");

				TransitionTo(e.element.flowController);

				//... fill out "selected wanderer" for TurnFlow to reference:
				//if (e.element.flowController is WandererFlowController)
				//{
				//	Debug.Log("... setting selected wanderer");

				//	Wanderer wanderer = (e.element.flowController as WandererFlowController).wanderer;
				//	Globals.SelectedWanderer.Add(wanderer);
				//}
			}
		}
		else
		{
			Debog.logGameflow("... element flow controller was null on maincontroller");
		}
	
		return FlowState.RUNNING;
	}
}
