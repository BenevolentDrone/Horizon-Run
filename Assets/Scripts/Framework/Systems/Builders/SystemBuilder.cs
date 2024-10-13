using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Systems
{
	public class SystemBuilder<TSystem>
		: ISystemBuilder<TSystem>
	{
		private readonly IRepository<string, IStageNode<TSystem>> stages;

		private readonly IRepository<Type, IList<ISystemNode<TSystem>>> systems;

		private readonly ILogger logger;

		private byte freeThreadIndex;

		public SystemBuilder(
			IRepository<string, IStageNode<TSystem>> stages,
			IRepository<Type, IList<ISystemNode<TSystem>>> systems,

			IStageNode<TSystem> startNode,
			IStageNode<TSystem> finishNode,

			byte freeThreadIndex = 2,

			ILogger logger = null)
		{
			this.stages = stages;

			this.systems = systems;

			this.logger = logger;


			StartNode = startNode;

			FinishNode = finishNode;

			this.freeThreadIndex = freeThreadIndex;
		}

		#region ISystemBuilder

		public IStageNode<TSystem> StartNode { get; private set; }

		public IStageNode<TSystem> FinishNode { get; private set; }

		#region Has

		public bool HasStageNode(
			string stageID)
		{
			return stages.Has(stageID);	
		}

		public bool HasSystemNodes(
			Type systemType)
		{
			return systems.Has(systemType);
		}

		#endregion

		#region Get

		public bool TryGetStageNode(
			string stageID,
			out IStageNode<TSystem> node)
		{
			return stages.TryGet(
				stageID,
				out node);
		}

		public bool TryGetSystemNodes(
			Type systemType,
			out IEnumerable<IReadOnlySystemNode<TSystem>> nodes)
		{
			var result = systems.TryGet(
				systemType,
				out var nodesList);

			nodes = nodesList;

			return result;
		}

		#endregion

		#region Add

		public bool TryAddBeforeStage(
			string stageID,
			IReadOnlySystemNode<TSystem> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var systemNode))
			{
				return false;
			}

			if (!TryGetAndValidateStageNode(
				stageID,
				out var currentSuccessor))
			{
				return false;
			}

			if (!ValidateSuccessor(currentSuccessor))
			{
				return false;
			}

			FindMatchingSuccessor(
				ref currentSuccessor,
				systemNode);

			return AddBefore(
				currentSuccessor,
				systemNode,
				parallel);
		}

		public bool TryAddBeforeStages(
			IEnumerable<string> stageIDs,
			IReadOnlySystemNode<TSystem> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var systemNode))
			{
				return false;
			}

			if (!FindFirst(
				stageIDs,
				out var currentSuccessor))
			{
				return false;
			}

			if (!ValidateSuccessor(currentSuccessor))
			{
				return false;
			}

			FindMatchingSuccessor(
				ref currentSuccessor,
				systemNode);

			return AddBefore(
				currentSuccessor,
				systemNode,
				parallel);
		}

		public bool TryAddAfterStage(
			string stageID,
			IReadOnlySystemNode<TSystem> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var systemNode))
			{
				return false;
			}

			if (!TryGetAndValidateStageNode(
				stageID,
				out var currentPredecessor))
			{
				return false;
			}

			if (!ValidatePredecessor(currentPredecessor))
			{
				return false;
			}

			FindMatchingPredecessor(
				ref currentPredecessor,
				systemNode);

			return AddAfter(
				currentPredecessor,
				systemNode,
				parallel);
		}

		public bool TryAddAfterStages(
			IEnumerable<string> stageIDs,
			IReadOnlySystemNode<TSystem> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var systemNode))
			{
				return false;
			}

			if (!FindLast(
				stageIDs,
				out var currentPredecessor))
			{
				return false;
			}

			if (!ValidatePredecessor(currentPredecessor))
			{
				return false;
			}

			FindMatchingPredecessor(
				ref currentPredecessor,
				systemNode);

			return AddAfter(
				currentPredecessor,
				systemNode,
				parallel);
		}

		public bool TryAddBeforeNode(
			IReadOnlySystemNode<TSystem> successor,
			IReadOnlySystemNode<TSystem> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var systemNode))
			{
				return false;
			}

			if (!ValidateNodeToAttachTo(
				successor,
				out var currentSuccessor))
			{
				return false;
			}

			if (!ValidateSuccessor(currentSuccessor))
			{
				return false;
			}

			FindMatchingSuccessor(
				ref currentSuccessor,
				systemNode);

			return AddBefore(
				currentSuccessor,
				systemNode,
				parallel);
		}

		public bool TryAddBeforeNodes(
			IEnumerable<IReadOnlySystemNode<TSystem>> successors,
			IReadOnlySystemNode<TSystem> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var systemNode))
			{
				return false;
			}

			if (!FindFirst(
				successors,
				out var currentSuccessor))
			{
				return false;
			}

			if (!ValidateSuccessor(currentSuccessor))
			{
				return false;
			}

			FindMatchingSuccessor(
				ref currentSuccessor,
				systemNode);

			return AddBefore(
				currentSuccessor,
				systemNode,
				parallel);
		}

		public bool TryAddAfterNode(
			IReadOnlySystemNode<TSystem> predecessor,
			IReadOnlySystemNode<TSystem> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var systemNode))
			{
				return false;
			}

			if (!ValidateNodeToAttachTo(
				predecessor,
				out var currentPredecessor))
			{
				return false;
			}

			if (!ValidatePredecessor(currentPredecessor))
			{
				return false;
			}

			FindMatchingPredecessor(
				ref currentPredecessor,
				systemNode);

			return AddAfter(
				currentPredecessor,
				systemNode,
				parallel);
		}

		public bool TryAddAfterNodes(
			IEnumerable<IReadOnlySystemNode<TSystem>> predecessors,
			IReadOnlySystemNode<TSystem> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var systemNode))
			{
				return false;
			}

			if (!FindLast(
				predecessors,
				out var currentPredecessor))
			{
				return false;
			}

			if (!ValidatePredecessor(currentPredecessor))
			{
				return false;
			}

			FindMatchingPredecessor(
				ref currentPredecessor,
				systemNode);

			return AddAfter(
				currentPredecessor,
				systemNode,
				parallel);
		}

		#endregion

		#region Remove

		public bool TryRemoveStage(
			string stageID)
		{
			if (!TryGetAndValidateStageNode(
				stageID,
				out var systemNode))
			{
				return false;
			}

			return Remove(systemNode);
		}

		public bool TryRemoveNode(
			IReadOnlySystemNode<TSystem> node)
		{
			return Remove(node as ISystemNode<TSystem>);
		}

		public bool TryRemoveNodes(
			IEnumerable<IReadOnlySystemNode<TSystem>> nodes)
		{
			bool success = true;

			foreach (var node in nodes)
			{
				success &= Remove(node as ISystemNode<TSystem>);
			}

			return success;
		}

		#endregion

		#endregion

		private bool ValidateNodeToAdd(
			IReadOnlySystemNode<TSystem> node,
			out ISystemNode<TSystem> systemNode)
		{
			systemNode = null;

			if (node == null)
			{
				logger?.LogError(
					GetType(),
					$"INVALID NODE");

				return false;
			}

			if (node is not ISystemNode<TSystem> result)
			{
				logger?.LogError(
					GetType(),
					$"INVALID NODE");

				return false;
			}

			if (!node.IsDetached)
			{
				logger?.LogError(
					GetType(),
					$"NODE {node} IS ALREADY ATTACHED");

				return false;
			}

			systemNode = result;

			return true;
		}

		private bool TryGetAndValidateStageNode(
			string stageID,
			out ISystemNode<TSystem> stageNodeAsSystemNode)
		{
			stageNodeAsSystemNode = null;

			if (string.IsNullOrEmpty(stageID))
			{
				logger?.LogError(
					GetType(),
					$"INVALID STAGE ID");

				return false;
			}

			if (!stages.TryGet(
				stageID,
				out var stageNode))
			{
				logger?.LogError(
					GetType(),
					$"STAGE {stageID} NOT FOUND");

				return false;
			}

			stageNodeAsSystemNode = stageNode as ISystemNode<TSystem>;

			if (stageNodeAsSystemNode == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CAST STAGE NODE {stageNode} TO SYSTEM NODE");

				return false;
			}

			return true;
		}

		private bool ValidateNodeToAttachTo(
			IReadOnlySystemNode<TSystem> node,
			out ISystemNode<TSystem> systemNode)
		{
			systemNode = null;

			if (node == null)
			{
				logger?.LogError(
					GetType(),
					$"INVALID NODE");

				return false;
			}

			if (node is not ISystemNode<TSystem> result)
			{
				logger?.LogError(
					GetType(),
					$"INVALID NODE");

				return false;
			}

			if (node.IsDetached)
			{
				logger?.LogError(
					GetType(),
					$"NODE {node} IS DETACHED");

				return false;
			}

			systemNode = result;

			return true;
		}

		private bool ValidateNodeToAttachTo(
			ISystemNode<TSystem> node)
		{
			if (node == null)
			{
				logger?.LogError(
					GetType(),
					$"INVALID NODE");

				return false;
			}

			if (node.IsDetached)
			{
				logger?.LogError(
					GetType(),
					$"NODE {node} IS DETACHED");

				return false;
			}

			return true;
		}

		private bool ValidatePredecessor(
			ISystemNode<TSystem> predecessor)
		{
			//Cannot insert nodes after the last node
			if (predecessor == FinishNode)
				return false;

			//Cannot insert nodes after invalid nodes
			if (predecessor.IsDetached)
			{
				logger?.LogError(
					GetType(),
					$"NODE {predecessor} IS DETACHED");

				return false;
			}

			return true;
		}

		private void FindMatchingPredecessor(
			ref ISystemNode<TSystem> currentPredecessor,
			ISystemNode<TSystem> node,
			ushort maxDepth = ushort.MaxValue)
		{
			for (ushort i = 0; i < maxDepth; i++)
			{
				//Cannot insert nodes into void
				if (currentPredecessor.SequentialNext == null)
					return;

				var predecessorCandidate = currentPredecessor.SequentialNext;

				//Cannot insert nodes after invalid nodes
				if (predecessorCandidate is not ISystemNode<TSystem> candidateAsNode)
				{
					return;
				}

				if (!ValidatePredecessor(
					candidateAsNode))
				{
					return;
				}

				//Cannot insert nodes after new stages
				if (predecessorCandidate is IStageNode<TSystem> stageNode)
				{
					return;
				}

				//Cannot insert nodes after invalid nodes
				if (predecessorCandidate.SequentialPrevious != currentPredecessor)
				{
					logger?.LogError(
						GetType(),
						$"NODE {predecessorCandidate} HAS INVALID PREVIOUS NODE LINK");

					return;
				}

				//Cannot insert nodes after invalid nodes
				if (predecessorCandidate.ExpectedThread != currentPredecessor.ExpectedThread)
				{
					logger?.LogError(
						GetType(),
						$"INVALID THREAD EXPECTATION: {predecessorCandidate.ExpectedThread} != {currentPredecessor.ExpectedThread}");

					return;
				}

				//Cannot insert nodes after nodes with lower priority
				if (predecessorCandidate.Priority <= node.Priority)
				{
					return;
				}

				currentPredecessor = candidateAsNode;
			}

			logger?.LogError(
				GetType(),
				$"MAX NODE DEPTH REACHED");
		}

		private bool ValidateSuccessor(
			ISystemNode<TSystem> successor)
		{
			//Cannot insert nodes before the first node
			if (successor == StartNode)
			{
				return false;
			}

			//Cannot insert nodes bedore invalid nodes
			if (successor.IsDetached)
			{
				logger?.LogError(
					GetType(),
					$"NODE {successor} IS DETACHED");

				return false;
			}

			return true;
		}

		private void FindMatchingSuccessor(
			ref ISystemNode<TSystem> currentSuccessor,
			ISystemNode<TSystem> node,
			ushort maxDepth = ushort.MaxValue)
		{
			for (ushort i = 0; i < maxDepth; i++)
			{
				//Cannot insert nodes into void
				if (currentSuccessor.SequentialPrevious == null)
					return;

				var successorCandidate = currentSuccessor.SequentialPrevious;

				//Cannot insert nodes before invalid nodes
				if (successorCandidate is not ISystemNode<TSystem> candidateAsNode)
				{
					return;
				}

				//Cannot insert nodes before invalid nodes
				if (!ValidateSuccessor(candidateAsNode))
				{
					return;
				}

				//Cannot insert nodes before new stages
				if (successorCandidate is IStageNode<TSystem> stageNode)
				{
					return;
				}

				//Cannot insert nodes before invalid nodes
				if (successorCandidate.SequentialPrevious != currentSuccessor)
				{
					logger?.LogError(
						GetType(),
						$"NODE {successorCandidate} HAS INVALID PREVIOUS NODE LINK");

					return;
				}

				//Cannot insert nodes after invalid nodes
				if (successorCandidate.ExpectedThread != currentSuccessor.ExpectedThread)
				{
					logger?.LogError(
						GetType(),
						$"INVALID THREAD EXPECTATION: {successorCandidate.ExpectedThread} != {currentSuccessor.ExpectedThread}");

					return;
				}

				//Cannot insert nodes before nodes with higher priority
				if (successorCandidate.Priority >= node.Priority)
				{
					return;
				}

				currentSuccessor = candidateAsNode;
			}

			logger?.LogError(
				GetType(),
				$"MAX NODE DEPTH REACHED");
		}

		private bool FindFirst(
			IEnumerable<string> stageIDs,
			out ISystemNode<TSystem> currentSuccessor)
		{
			List<ISystemNode<TSystem>> stageNodes = new List<ISystemNode<TSystem>>();

			foreach (var stageID in stageIDs)
			{
				if (TryGetAndValidateStageNode(
					stageID,
					out var stageNode))
				{
					stageNodes.Add(stageNode);
				}
			}

			if (!FindFirst(
				stageNodes,
				out currentSuccessor))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND THE FIRST STAGE AMONG THE PROVIDED STAGES");

				return false;
			}

			return true;
		}

		private bool FindFirst(
			IEnumerable<IReadOnlySystemNode<TSystem>> successors,
			out ISystemNode<TSystem> currentSuccessor)
		{
			List<ISystemNode<TSystem>> systemNodes = new List<ISystemNode<TSystem>>();

			foreach (var successor in successors)
			{
				systemNodes.Add(successor as ISystemNode<TSystem>);
			}

			if (!FindFirst(
				systemNodes,
				out currentSuccessor))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND THE FIRST NODE AMONG THE PROVIDED NOES");

				return false;
			}

			return true;
		}

		private bool FindFirst(
			IList<ISystemNode<TSystem>> nodes,
			out ISystemNode<TSystem> first,
			ushort maxDepth = ushort.MaxValue)
		{
			first = null;

			byte expectedThread = 0;

			if (nodes == null)
			{
				logger?.LogError(
					GetType(),
					$"INVALID NODES LIST");

				return false;
			}

			if (nodes.Count == 0)
			{
				logger?.LogError(
					GetType(),
					$"NODES LIST EMPTY");

				return false;
			}

			//foreach (var node in nodes)
			for (int i = nodes.Count - 1; i >= 0; i--)
			{
				var node = nodes[i];

				if (!ValidateNodeToAttachTo(node))
				{
					nodes.RemoveAt(i);

					continue;
				}

				if (first == null)
				{
					first = node;

					expectedThread = first.ExpectedThread;

					nodes.RemoveAt(i);
				}
				else
				{
					if (node.ExpectedThread != expectedThread)
					{
						logger?.LogError(
							GetType(),
							$"INCOSISTENT THREAD EXPECTATION: {node.ExpectedThread} != {expectedThread}");

						return false;
					}

					if (node.IsDetached)
					{
						logger?.LogError(
							GetType(),
							$"NODE {node} IS DETACHED");

						return false;
					}
				}
			}

			if (first == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT INITIALIZE FIRST NODE");

				return false;
			}

			var currentNode = first;

			for (ushort i = 0; i < maxDepth; i++)
			{
				if (currentNode.SequentialPrevious == null)
				{
					return true;
				}

				var previousNode = currentNode.SequentialPrevious as ISystemNode<TSystem>;

				if (previousNode == null)
				{
					return true;
				}

				if (previousNode.IsDetached)
				{
					logger?.LogError(
						GetType(),
						$"NODE {previousNode} IS DETACHED");

					return false;
				}

				if (previousNode.ExpectedThread != expectedThread)
				{
					logger?.LogError(
						GetType(),
						$"INVALID THREAD EXPECTATION: {previousNode.ExpectedThread} != {expectedThread}");

					return false;
				}

				if (nodes.Contains(previousNode))
				{
					first = previousNode;

					nodes.Remove(previousNode);

					if (nodes.Count == 0)
					{
						return true;
					}
				}

				currentNode = previousNode;
			}

			logger?.LogError(
				GetType(),
				$"MAX NODE DEPTH REACHED");

			return false;
		}

		private bool FindLast(
			IEnumerable<string> stageIDs,
			out ISystemNode<TSystem> currentPredecessor)
		{
			List<ISystemNode<TSystem>> stageNodes = new List<ISystemNode<TSystem>>();

			foreach (var stageID in stageIDs)
			{
				if (TryGetAndValidateStageNode(
					stageID,
					out var stageNode))
				{
					stageNodes.Add(stageNode);
				}
			}

			if (!FindLast(
				stageNodes,
				out currentPredecessor))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND THE LAST STAGE AMONG THE PROVIDED STAGES");

				return false;
			}

			return true;
		}

		private bool FindLast(
			IEnumerable<IReadOnlySystemNode<TSystem>> predecessors,
			out ISystemNode<TSystem> currentPredecessor)
		{
			List<ISystemNode<TSystem>> systemNodes = new List<ISystemNode<TSystem>>();

			foreach (var predecessor in predecessors)
			{
				systemNodes.Add(predecessor as ISystemNode<TSystem>);
			}

			if (!FindLast(
				systemNodes,
				out currentPredecessor))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND THE LAST NODE AMONG THE PROVIDED NOES");

				return false;
			}

			return true;
		}

		private bool FindLast(
			IList<ISystemNode<TSystem>> nodes,
			out ISystemNode<TSystem> last,
			ushort maxDepth = ushort.MaxValue)
		{
			last = null;

			byte expectedThread = 0;

			if (nodes == null)
			{
				logger?.LogError(
					GetType(),
					$"INVALID NODES LIST");

				return false;
			}

			if (nodes.Count == 0)
			{
				logger?.LogError(
					GetType(),
					$"NODES LIST EMPTY");

				return false;
			}

			//foreach (var node in nodes)
			for (int i = nodes.Count - 1; i >= 0; i--)
			{
				var node = nodes[i];

				if (!ValidateNodeToAttachTo(node))
				{
					nodes.RemoveAt(i);

					continue;
				}

				if (last == null)
				{
					last = node;

					expectedThread = last.ExpectedThread;

					nodes.RemoveAt(i);
				}
				else
				{
					if (node.ExpectedThread != expectedThread)
					{
						logger?.LogError(
							GetType(),
							$"INCOSISTENT THREAD EXPECTATION: {node.ExpectedThread} != {expectedThread}");
	
						return false;
					}

					if (node.IsDetached)
					{
						logger?.LogError(
							GetType(),
							$"NODE {node} IS DETACHED");

						return false;
					}
				}
			}

			if (last == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT INITIALIZE LAST NODE");

				return false;
			}

			var currentNode = last;

			for (ushort i = 0; i < maxDepth; i++)
			{
				if (currentNode.SequentialNext == null)
				{
					return true;
				}

				var nextNode = currentNode.SequentialNext as ISystemNode<TSystem>;

				if (nextNode == null)
				{
					return true;
				}

				if (nextNode.IsDetached)
				{
					logger?.LogError(
						GetType(),
						$"NODE {nextNode} IS DETACHED");

					return false;
				}

				if (nextNode.ExpectedThread != expectedThread)
				{
					logger?.LogError(
						GetType(),
						$"INVALID THREAD EXPECTATION: {nextNode.ExpectedThread} != {expectedThread}");

					return false;
				}
				
				if (nodes.Contains(nextNode))
				{
					last = nextNode;

					nodes.Remove(nextNode);

					if (nodes.Count == 0)
					{
						return true;
					}
				}

				currentNode = nextNode;
			}

			logger?.LogError(
				GetType(),
				$"MAX NODE DEPTH REACHED");

			return false;
		}

		private bool AddBefore(
			ISystemNode<TSystem> successor,
			ISystemNode<TSystem> node,
			bool parallel)
		{
			if (node is IStageNode<TSystem> stageNode)
			{
				if (stages.Has(stageNode.Stage))
				{
					logger?.LogError(
						GetType(),
						$"STAGE {stageNode.Stage} ALREADY REGISTERED");

					return false;
				}

				stages.TryAdd(
					stageNode.Stage,
					stageNode);
			}

			var systemType = node.SystemType;

			if (systemType != null)
			{
				if (systems.Has(systemType))
				{
					systems.TryGet(
						systemType,
						out var systemsEnumerable);

					systemsEnumerable.Add(node as ISystemNode<TSystem>);
				}
				else
				{
					var systemsList = new List<ISystemNode<TSystem>>();

					systemsList.Add(node as ISystemNode<TSystem>);

					systems.TryAdd(
						systemType,
						systemsList);
				}
			}

			if (!parallel)
			{
				if (successor.SequentialPrevious != null)
				{
					var previous = successor.SequentialPrevious as ISystemNode<TSystem>;
	
					if (previous != null)
					{
						previous.SequentialNext = node;
	
						node.SequentialPrevious = previous;
					}
				}
	
				successor.SequentialPrevious = node;
	
				node.SequentialNext = successor;


				node.ExpectedThread = successor.ExpectedThread;
			}
			else
			{
				var parallelPrevious = successor.ParallelPrevious as IList<ISystemNode<TSystem>>;

				if (parallelPrevious == null)
				{
					parallelPrevious = new List<ISystemNode<TSystem>>();

					successor.ParallelPrevious = parallelPrevious;
				}

				parallelPrevious.Add(node);


				var parallelNext = node.ParallelNext as IList<ISystemNode<TSystem>>;

				if (parallelNext == null)
				{
					parallelNext = new List<ISystemNode<TSystem>>();
					
					node.ParallelNext = parallelNext;
				}

				parallelNext.Add(successor);


				node.ExpectedThread = freeThreadIndex;

				freeThreadIndex++;
			}

			node.IsDetached = false;

			return true;
		}

		private bool AddAfter(
			ISystemNode<TSystem> predecessor,
			ISystemNode<TSystem> node,
			bool parallel)
		{
			if (node is IStageNode<TSystem> stageNode)
			{
				if (stages.Has(stageNode.Stage))
				{
					logger?.LogError(
						GetType(),
						$"STAGE {stageNode.Stage} ALREADY REGISTERED");

					return false;
				}

				stages.TryAdd(
					stageNode.Stage,
					stageNode);
			}

			var systemType = node.SystemType;

			if (systemType != null)
			{
				if (systems.Has(systemType))
				{
					systems.TryGet(
						systemType,
						out var systemsEnumerable);

					systemsEnumerable.Add(node as ISystemNode<TSystem>);
				}
				else
				{
					var systemsList = new List<ISystemNode<TSystem>>();

					systemsList.Add(node as ISystemNode<TSystem>);

					systems.TryAdd(
						systemType,
						systemsList);
				}
			}

			if (!parallel)
			{
				if (predecessor.SequentialNext != null)
				{
					var next = predecessor.SequentialNext as ISystemNode<TSystem>;

					if (next != null)
					{
						next.SequentialPrevious = node;

						node.SequentialNext = next;
					}
				}

				predecessor.SequentialNext = node;

				node.SequentialPrevious = predecessor;


				node.ExpectedThread = predecessor.ExpectedThread;
			}
			else
			{
				var parallelNext = predecessor.ParallelNext as IList<ISystemNode<TSystem>>;

				if (parallelNext == null)
				{
					parallelNext = new List<ISystemNode<TSystem>>();

					predecessor.ParallelNext = parallelNext;
				}

				parallelNext.Add(node);


				var parallelPrevious = node.ParallelPrevious as IList<ISystemNode<TSystem>>;

				if (parallelPrevious == null)
				{
					parallelPrevious = new List<ISystemNode<TSystem>>();

					node.ParallelPrevious = parallelPrevious;
				}

				parallelPrevious.Add(predecessor);


				node.ExpectedThread = freeThreadIndex;

				freeThreadIndex++;
			}

			node.IsDetached = false;

			return true;
		}

		private bool Remove(
			ISystemNode<TSystem> node)
		{
			if (!ValidateNodeToAttachTo(node))
			{
				return false;
			}

			if (node == StartNode)
			{
				logger?.LogError(
					GetType(),
					$"CANNOT REMOVE START NODE");

				return false;
			}

			if (node == FinishNode)
			{
				logger?.LogError(
					GetType(),
					$"CANNOT REMOVE FINISH NODE");

				return false;
			}

			if (node is IStageNode<TSystem> stageNode)
			{
				if (!stages.Has(stageNode.Stage))
				{
					logger?.LogError(
						GetType(),
						$"STAGE {stageNode.Stage} NOT FOUND");

					return false;
				}

				stages.TryRemove(
					stageNode.Stage);
			}


			var systemType = node.SystemType;

			if (systemType != null)
			{
				if (systems.Has(systemType))
				{
					systems.TryGet(
						systemType,
						out var systemsEnumerable);

					systemsEnumerable.Remove(node);

					if (systemsEnumerable.Count == 0)
					{
						systems.TryRemove(systemType);
					}
				}
			}


			var sequentialPrevious = node.SequentialPrevious as ISystemNode<TSystem>;

			var sequentialNext = node.SequentialNext as ISystemNode<TSystem>;

			if (sequentialPrevious != null && sequentialNext != null)
			{
				sequentialPrevious.SequentialNext = sequentialNext;

				sequentialNext.SequentialPrevious = sequentialPrevious;
			}
			else if (sequentialPrevious != null)
			{
				sequentialPrevious.SequentialNext = null;
			}
			else if (sequentialNext != null)
			{
				sequentialNext.SequentialPrevious = null;
			}


			var parallelPrevious = node.ParallelPrevious as IList<ISystemNode<TSystem>>;

			if (parallelPrevious != null)
			{
				foreach (var previous in parallelPrevious)
				{
					var previousesParallelNext = previous.ParallelNext as IList<ISystemNode<TSystem>>;

					if (previousesParallelNext != null)
					{
						previousesParallelNext.Remove(node);
					}
				}
			}


			var parallelNext = node.ParallelNext as IList<ISystemNode<TSystem>>;

			if (parallelNext != null)
			{
				foreach (var next in parallelNext)
				{
					var nextsParallelPrevious = next.ParallelPrevious as IList<ISystemNode<TSystem>>;

					if (nextsParallelPrevious != null)
					{
						nextsParallelPrevious.Remove(node);
					}
				}
			}

			node.IsDetached = true;

			node.ExpectedThread = 0;

			return true;
		}
	}
}