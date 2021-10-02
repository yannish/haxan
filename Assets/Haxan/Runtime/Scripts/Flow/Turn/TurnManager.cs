using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	[ReadOnly]
	public int currCommandChainLength;

	//[ReadOnly]
	public WandererFlowController currWanderer
	{
		get
		{
			if (Globals.SelectedWanderer.Items.Count > 0)
				return (Globals.SelectedWanderer.Items[0] as Wanderer).flowController as WandererFlowController;

            return null;
		}
	}

	//[SerializeField]
	public Stack<CharacterCommand> currCommandChain;
	//private Queue<CharacterCommand> currCommandChain;

	//... REWIND:
	public Stack<CharacterCommand> commandHistory = new Stack<CharacterCommand>();
	//public Queue<CharacterCommand> commandHistory = new Queue<CharacterCommand>();


	private void Awake()
	{
		//Events.instance.AddListener<ElementClickedEvent>(HandleClick);
	}

	void HandleClick(ElementClickedEvent e)
	{
		//if (e.element.flowController is WandererFlowController)
		//	currWanderer = e.element.flowController as WandererFlowController;
		//else
		//{
		//	currWanderer = null;
		//	currCommandChain = null;
		//}
	}

	void LateUpdate()
    {
		if (currWanderer != null && currCommandChain == null)
		{
			if(currWanderer.TryGetCommandStack(ref currCommandChain))
			{
				if (currCommandChain != null)
					Debug.Log("new command chain from : " + currWanderer.name);
			}
		}

		if(Input.GetKeyDown(KeyCode.N))
		{
			ProcessCommand();
		}

		if(Input.GetKeyDown(KeyCode.R))
		{
			RewindCommand();
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			BankCommand();
		}
    }

	void BankCommand()
	{
		currCommandChain = null;
		commandHistory = null;
	}

	void ProcessCommand()
	{
		if(currCommandChain.Count > 0)
		{
			var poppedCommand = currCommandChain.Pop();
			poppedCommand.Execute();
			commandHistory.Push(poppedCommand);

			//...shouldn't need to validate here, should be building a legit chain to begin with
			//if (currCommandChain.Peek().IsValid())
			//{
				
			//}
		}
		else
		{
			Debug.LogWarning("No more commands to process!");
		}
	}

	void RewindCommand()
	{
		if(commandHistory.Count > 0)
		{
			var poppedCommand = commandHistory.Pop();
			poppedCommand.Undo();
			currCommandChain.Push(poppedCommand);
		}
		else
		{
			Debug.LogWarning("No command history to undo!");
		}
	}
}
