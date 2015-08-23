using System.Collections.Generic;

namespace Gamelogic
{
	/// <summary>
	/// This class is a state machine that has the ability to remember previous states
	/// and transition back to them (FIFO).
	/// </summary>
	/// <typeparam name="TLabel">The type of state labels.</typeparam>
	[Version(1, 4)]
	public class PushdownAutomaton<TLabel> : StateMachine<TLabel>
	{
		private readonly Stack<TLabel> stateHistory;

		public PushdownAutomaton()
		{
			stateHistory = new Stack<TLabel>();
		}

		/// <summary>
		/// Pushes the current state onto the stack, and transitions to the next state.
		/// </summary>
		/// <param name="nextState"></param>
		public void Push(TLabel nextState)
		{
			stateHistory.Push(CurrentState);
			
			CurrentState = nextState;
		}

		/// <summary>
		/// Pops a state from the stack and switches to it.
		/// </summary>
		public void Pop()
		{
			if (stateHistory.Count > 0)
			{
				CurrentState = stateHistory.Pop();
			}
		}
	}
}