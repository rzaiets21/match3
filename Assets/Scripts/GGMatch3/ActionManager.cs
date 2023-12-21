using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class ActionManager
	{
		public LinkedList<BoardAction> actions = new LinkedList<BoardAction>();

		public int ActionCount => actions.Count;

		public void AddAction(BoardAction action)
		{
			AddActionAndRun(action);
		}

		public void OnUpdate(float deltaTime)
		{
			LinkedListNode<BoardAction> next;
			for (LinkedListNode<BoardAction> linkedListNode = actions.First; linkedListNode != null; linkedListNode = next)
			{
				next = linkedListNode.Next;
				BoardAction value = linkedListNode.Value;
				if (!value.isStarted)
				{
					value.OnStart(this);
					next = linkedListNode.Next;
				}
				if (value.isAlive)
				{
					value.OnUpdate(deltaTime);
					next = linkedListNode.Next;
				}
				if (!value.isAlive)
				{
					actions.Remove(linkedListNode);
					OnActionRemoved(value);
				}
			}
		}

		private void AddActionAndRun(BoardAction action)
		{
			actions.AddLast(action);
			if (!action.isStarted)
			{
				action.OnStart(this);
			}
			if (action.isAlive)
			{
				action.OnUpdate(0f);
			}
			if (!action.isAlive)
			{
				actions.Remove(action);
				OnActionRemoved(action);
			}
		}

		private void OnActionRemoved(BoardAction action)
		{
			action.lockContainer.UnlockAll();
		}
	}
}
