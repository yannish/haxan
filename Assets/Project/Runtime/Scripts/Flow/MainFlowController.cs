using BOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *... why put turnflow control right in here w/ mainflow...
 * 
 * when you've got a command chain in from whichever character (wanderer or enemy)
 * that's it's own flow, so makes sense to inject here?
 */

public class MainFlowController : FlowController
{
	protected override void Awake()
	{
		SetPhase(TeamPhase.PLAYER);
		base.Awake();

		dummyTurn = ScriptableObject.CreateInstance<DummyTurn>();
	}

	#region TURN PROCESSING:

	public ITurnProcessor turnProcessor;
	//public List<GameObject> turnProcessors = new List<GameObject>();


	[ReadOnly]
	public TeamPhase phase = TeamPhase.PLAYER;
    public void SetPhase(TeamPhase newPhase)
    {
        switch(newPhase)
        {
            case TeamPhase.PLAYER:
                ClearPlayerTurns();
                break;

            case TeamPhase.ENEMY:
                PopulateEnemyTurns();
                break;
        }

        phase = newPhase;
    }


	//[ReadOnly] 
    public Character currCharacter;

	//... the turn waiting to be processed:
	public Turn currInputTurn;
	public DummyTurn dummyTurn;
	//public Queue<CharacterCommand> currCommandStack;

	//... the command currently being ticked along:
	public CharacterCommand activeCommand;
	public Queue<CharacterCommand> commandHistory = new Queue<CharacterCommand>();


    public List<Turn> playerTurns;
    void ClearPlayerTurns()
    {
        playerTurns.Clear();
        //foreach(Wanderer wanderer in Globals.ActiveWanderers.Items)
        //{
        //    if(!wanderer.isStunned)
        //    {
        //        playerTurns.Add(new Turn() { owner = wanderer });
        //    }
        //}
    }

    public List<Turn> enemyTurns;
    void PopulateEnemyTurns()
    {
        enemyTurns.Clear();
        foreach(Enemy enemy in Globals.ActiveEnemies.Items)
        {
            if(!enemy.isStunned)
            {
                //enemyTurns.Add(new Turn() { owner = enemy });
            }
        }
    }



	void BankCommand()
	{
		currInputTurn = null;
		commandHistory = null;
	}

	void ProcessCommand()
	{
        //if(currInputTurn.IsNullOrEmpty())
        //{
        //    Debug.LogWarning("No more commands to process!");
        //    return;
        //}

        //activeCommand = currInputTurn.Dequeue();
		//activeCommand.Execute();
		//commandHistory.Push(activeCommand);

        //Events.instance.Raise(new ForwardTimeStep(activeCommand.character));

        //...shouldn't need to validate here, should be building a legit chain to begin with
        //if (currCommandChain.Peek().IsValid())
        //{
        //}
    }

    void RewindCommand()
	{
		if (!commandHistory.IsNullOrEmpty())
		{
			//var poppedCommand = commandHistory.Pop();
			//poppedCommand.Undo();
			//currCommandStack.Push(poppedCommand);

			//Events.instance.Raise(new BackwardTimeStep(poppedCommand.character));
		}
		else
		{
			Debug.LogWarning("No command history to undo!");
		}
	}

	#endregion


	public bool doBreak;

	private bool wasProcessingLastFrame;
	private void LateUpdate()
	{
		if (doBreak)
		{
			doBreak = false;
		}

		if (turnProcessor == null)
			return;

		turnProcessor.ProcessTurns();

		if (turnProcessor.IsProcessing)
		{
			wasProcessingLastFrame = true;
			return;
		}

		if (wasProcessingLastFrame && lastSubFlow != null)
		{
			wasProcessingLastFrame = false;
			TransitionTo(lastSubFlow, true);
		}


		if(currInputTurn == null)
		{
			if (subFlow != null && subFlow is WandererFlowController)
			{
				//Debug.LogWarning("... trying to get input from wanderer");

				var currWandererFlow = subFlow as WandererFlowController;
				if(currWandererFlow.TryGetInputTurn(ref currInputTurn))
				{
					if (logDebug)
					{
						string log = string.Format(
							"new turn from {0}, length {1}",
							currWandererFlow.name,
							currInputTurn.commands.Count
							);

						Debog.logGameflow(log);
					}

					TransitionTo(null, true);

					turnProcessor.RecordTurn(currInputTurn);
					currInputTurn = null;
				}
			}
		}

	}

	#region FLOW PROCESSING:

	public override bool HandleHover(ElementHoveredEvent e)
	{
		if ((turnProcessor as Component) && turnProcessor.IsProcessing)
			return false;

		if (subFlow != null && subFlow.HandleHover(e))
			return true;

		if (peekedFlow != null)
        {
            peekedFlow.HoverUnpeek();
            peekedFlow = null;
        }

		//... 
		if (e.element == null || e.element.flowController == null)
			return false;

		peekedFlow = e.element.flowController;
		peekedFlow.HoverPeek();

		return false;
	}


	public override FlowState HandleInput(ElementClickedEvent e, FlowController owner)
	{
		// TODO: how to nullcheck reference to interface..
		if ((turnProcessor as Component) && turnProcessor.IsProcessing)
			return FlowState.RUNNING;

		if(logDebug)
			Debog.logInput("Handling input on maincontroller");

		//if (currInputTurn != null)
		//{
		//	if (logDebug)
		//		Debog.logGameflow("... curr command stack still being processed");
		//	return FlowState.RUNNING;
		//}

		if (e.element is TurnButton)
        {
			if (logDebug)
				Debog.logInput("TURN BUTTON PRESSED");

            return FlowState.RUNNING;
        }


        //if there's already a subFlow, pass input through that:
        if (subFlow != null)
		{
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

		if (logDebug)
			Debog.logGameflow("subflow passed on input in main");

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
				if(logDebug)
					Debog.logGameflow("found flow, heading in");

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

	#endregion
}
