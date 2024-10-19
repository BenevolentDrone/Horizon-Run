using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Systems
{
	public abstract class ASystemBuilder<TSystem, TProcedure>
		: ISystemBuilder<TSystem, TProcedure>
	{
		protected readonly HashSet<IProcedureNode<TProcedure>> allProcedureNodes;

		protected readonly IRepository<string, IStageNode<TProcedure>> stageRepository;

		protected readonly IRepository<Type, IList<IProcedureNode<TProcedure>>> procedureListByTypeRepository;

		protected readonly ILogger logger;

		protected byte freeThreadIndex;

		public ASystemBuilder(
			HashSet<IProcedureNode<TProcedure>> allProcedureNodes,
			IRepository<string, IStageNode<TProcedure>> stageRepository,
			IRepository<Type, IList<IProcedureNode<TProcedure>>> procedureListByTypeRepository,

			IStageNode<TProcedure> startNode,
			IStageNode<TProcedure> finishNode,

			byte freeThreadIndex = 2,

			ILogger logger = null)
		{
			this.allProcedureNodes = allProcedureNodes;

			this.stageRepository = stageRepository;

			this.procedureListByTypeRepository = procedureListByTypeRepository;

			this.logger = logger;


			StartNode = startNode;

			FinishNode = finishNode;

			this.freeThreadIndex = freeThreadIndex;
		}

		#region ISystemBuilder

		public IStageNode<TProcedure> StartNode { get; protected set; }

		public IStageNode<TProcedure> FinishNode { get; protected set; }

		#region Has

		public bool HasStageNode(
			string stageID)
		{
			if (string.IsNullOrEmpty(stageID))
				return false;

			return stageRepository.Has(stageID);	
		}

		public bool HasAllStageNodes(
			IEnumerable<string> stageIDs)
		{
			foreach (var stageID in stageIDs)
				if (string.IsNullOrEmpty(stageID)
					|| !stageRepository.Has(stageID))
					return false;

			return true;
		}

		public bool HasProcedureNodes(
			Type procedureType)
		{
			if (procedureType == null)
				return false;

			return procedureListByTypeRepository.Has(procedureType);
		}

		#endregion

		#region Get

		public bool TryGetStageNode(
			string stageID,
			out IStageNode<TProcedure> node)
		{
			if (string.IsNullOrEmpty(stageID))
			{
				logger?.LogError(
					GetType(),
					$"INVALID STAGE ID");

				node = null;

				return false;
			}

			return stageRepository.TryGet(
				stageID,
				out node);
		}

		public bool TryGetProcedureNodes(
			Type procedureType,
			out IEnumerable<IReadOnlyProcedureNode<TProcedure>> nodes)
		{
			if (procedureType == null)
			{
				logger?.LogError(
					GetType(),
					$"INVALID PROCEDURE TYPE");

				nodes = null;

				return false;
			}

			var result = procedureListByTypeRepository.TryGet(
				procedureType,
				out var nodesList);

			nodes = nodesList;

			return result;
		}

		#endregion

		#region Add

		public bool TryAddBeforeStage(
			string stageID,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var procedureNode))
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
				procedureNode);

			return AddBefore(
				currentSuccessor,
				procedureNode,
				parallel);
		}

		public bool TryAddBeforeStages(
			IEnumerable<string> stageIDs,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var procedureNode))
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
				procedureNode);

			return AddBefore(
				currentSuccessor,
				procedureNode,
				parallel);
		}

		public bool TryAddAfterStage(
			string stageID,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var procedureNode))
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
				procedureNode);

			return AddAfter(
				currentPredecessor,
				procedureNode,
				parallel);
		}

		public bool TryAddAfterStages(
			IEnumerable<string> stageIDs,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var procedureNode))
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
				procedureNode);

			return AddAfter(
				currentPredecessor,
				procedureNode,
				parallel);
		}

		public bool TryAddBeforeNode(
			IReadOnlyProcedureNode<TProcedure> successor,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var procedureNode))
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
				procedureNode);

			return AddBefore(
				currentSuccessor,
				procedureNode,
				parallel);
		}

		public bool TryAddBeforeNodes(
			IEnumerable<IReadOnlyProcedureNode<TProcedure>> successors,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var procedureNode))
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
				procedureNode);

			return AddBefore(
				currentSuccessor,
				procedureNode,
				parallel);
		}

		public bool TryAddAfterNode(
			IReadOnlyProcedureNode<TProcedure> predecessor,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var procedureNode))
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
				procedureNode);

			return AddAfter(
				currentPredecessor,
				procedureNode,
				parallel);
		}

		public bool TryAddAfterNodes(
			IEnumerable<IReadOnlyProcedureNode<TProcedure>> predecessors,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false)
		{
			if (!ValidateNodeToAdd(
				node,
				out var procedureNode))
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
				procedureNode);

			return AddAfter(
				currentPredecessor,
				procedureNode,
				parallel);
		}

		#endregion

		#region Remove

		public bool TryRemoveStage(
			string stageID)
		{
			if (!TryGetAndValidateStageNode(
				stageID,
				out var procedureNode))
			{
				return false;
			}

			return Remove(procedureNode);
		}

		public bool TryRemoveNode(
			IReadOnlyProcedureNode<TProcedure> node)
		{
			return Remove(node as IProcedureNode<TProcedure>);
		}

		public bool TryRemoveNodes(
			IEnumerable<IReadOnlyProcedureNode<TProcedure>> nodes)
		{
			bool success = true;

			foreach (var node in nodes)
			{
				success &= Remove(node as IProcedureNode<TProcedure>);
			}

			return success;
		}

		#endregion

		#region Link

		public bool TryLinkNodes(
			IReadOnlyProcedureNode<TProcedure> predecessor,
			IReadOnlyProcedureNode<TProcedure> successor)
		{
			if (!ValidateNodeToAttachTo(
				predecessor,
				out var predecessorAsProcedureNode))
			{
				return false;
			}

			if (!ValidateNodeToAttachTo(
				successor,
				out var successorAsProcedureNode))
			{
				return false;
			}

			if (predecessorAsProcedureNode.ExpectedThread != successorAsProcedureNode.ExpectedThread)
			{
				var parallelPrevious = successorAsProcedureNode.ParallelPrevious as IList<IProcedureNode<TProcedure>>;

				if (parallelPrevious == null)
				{
					parallelPrevious = new List<IProcedureNode<TProcedure>>();

					successorAsProcedureNode.ParallelPrevious = parallelPrevious;
				}

				parallelPrevious.Add(predecessorAsProcedureNode);


				var parallelNext = predecessorAsProcedureNode.ParallelNext as IList<IProcedureNode<TProcedure>>;

				if (parallelNext == null)
				{
					parallelNext = new List<IProcedureNode<TProcedure>>();

					predecessorAsProcedureNode.ParallelNext = parallelNext;
				}

				parallelNext.Add(successorAsProcedureNode);


				return true;
			}
		

			if (predecessorAsProcedureNode.SequentialNext != null)
			{
				logger?.LogError(
					GetType(),
					$"PREDECESSOR NODE {predecessor} ALREADY LINKED TO {predecessorAsProcedureNode.SequentialNext}");

				return false;
			}

			if (successorAsProcedureNode.SequentialPrevious != null)
			{
				logger?.LogError(
					GetType(),
					$"SUCCESSOR NODE {successor} ALREADY LINKED TO {successorAsProcedureNode.SequentialPrevious}");

				return false;
			}

			predecessorAsProcedureNode.SequentialNext = successorAsProcedureNode;

			successorAsProcedureNode.SequentialPrevious = predecessorAsProcedureNode;


			return true;
		}

		#endregion

		#region Validate

		public bool ValidateSystem()
		{
			return
				ValidateAllNodesShouldHaveStartAndFinish()
				&& ValidateDAG()
				&& ValidateProcessFlowFromStartToFinish();
		}

		#endregion

		#region Build

		public abstract bool BuildSystem(
			out TSystem system);

		#endregion

		#endregion

		protected bool ValidateNodeToAdd(
			IReadOnlyProcedureNode<TProcedure> node,
			out IProcedureNode<TProcedure> procedureNode)
		{
			procedureNode = null;

			if (node == null)
			{
				logger?.LogError(
					GetType(),
					$"INVALID NODE");

				return false;
			}

			if (node is not IProcedureNode<TProcedure> result)
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

			if (allProcedureNodes.Contains(result))
			{
				logger?.LogError(
					GetType(),
					$"NODE {result} IS ALREADY PRESENT IN THE SYSTEM");

				return false;
			}

			procedureNode = result;

			return true;
		}

		protected bool TryGetAndValidateStageNode(
			string stageID,
			out IProcedureNode<TProcedure> stageNodeAsProcedureNode)
		{
			stageNodeAsProcedureNode = null;

			if (string.IsNullOrEmpty(stageID))
			{
				logger?.LogError(
					GetType(),
					$"INVALID STAGE ID");

				return false;
			}

			if (!stageRepository.TryGet(
				stageID,
				out var stageNode))
			{
				logger?.LogError(
					GetType(),
					$"STAGE {stageID} NOT FOUND");

				return false;
			}

			stageNodeAsProcedureNode = stageNode as IProcedureNode<TProcedure>;

			if (stageNodeAsProcedureNode == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CAST STAGE NODE {stageNode} TO PROCEDURE NODE");

				return false;
			}

			return true;
		}

		protected bool ValidateNodeToAttachTo(
			IReadOnlyProcedureNode<TProcedure> node,
			out IProcedureNode<TProcedure> procedureNode)
		{
			procedureNode = null;

			if (node == null)
			{
				logger?.LogError(
					GetType(),
					$"INVALID NODE");

				return false;
			}

			if (node is not IProcedureNode<TProcedure> result)
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

			if (!allProcedureNodes.Contains(result))
			{
				logger?.LogError(
					GetType(),
					$"NODE {result} IS NOT PRESENT IN THE SYSTEM");

				return false;
			}

			procedureNode = result;

			return true;
		}

		protected bool ValidateNodeToAttachTo(
			IProcedureNode<TProcedure> node)
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

			if (!allProcedureNodes.Contains(node))
			{
				logger?.LogError(
					GetType(),
					$"NODE {node} IS NOT PRESENT IN THE SYSTEM");

				return false;
			}

			return true;
		}

		protected bool ValidatePredecessor(
			IProcedureNode<TProcedure> predecessor)
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

			if (!allProcedureNodes.Contains(predecessor))
			{
				logger?.LogError(
					GetType(),
					$"NODE {predecessor} IS NOT PRESENT IN THE SYSTEM");

				return false;
			}

			return true;
		}

		protected void FindMatchingPredecessor(
			ref IProcedureNode<TProcedure> currentPredecessor,
			IProcedureNode<TProcedure> node,
			ushort maxDepth = ushort.MaxValue)
		{
			for (ushort i = 0; i < maxDepth; i++)
			{
				//Cannot insert nodes into void
				if (currentPredecessor.SequentialNext == null)
					return;

				var predecessorCandidate = currentPredecessor.SequentialNext;

				//Cannot insert nodes after invalid nodes
				if (predecessorCandidate is not IProcedureNode<TProcedure> candidateAsNode)
				{
					return;
				}

				if (!ValidatePredecessor(
					candidateAsNode))
				{
					return;
				}

				//Cannot insert nodes after new stages
				if (predecessorCandidate is IStageNode<TProcedure> stageNode)
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

		protected bool ValidateSuccessor(
			IProcedureNode<TProcedure> successor)
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

			if (!allProcedureNodes.Contains(successor))
			{
				logger?.LogError(
					GetType(),
					$"NODE {successor} IS NOT PRESENT IN THE SYSTEM");

				return false;
			}

			return true;
		}

		protected void FindMatchingSuccessor(
			ref IProcedureNode<TProcedure> currentSuccessor,
			IProcedureNode<TProcedure> node,
			ushort maxDepth = ushort.MaxValue)
		{
			for (ushort i = 0; i < maxDepth; i++)
			{
				//Cannot insert nodes into void
				if (currentSuccessor.SequentialPrevious == null)
					return;

				var successorCandidate = currentSuccessor.SequentialPrevious;

				//Cannot insert nodes before invalid nodes
				if (successorCandidate is not IProcedureNode<TProcedure> candidateAsNode)
				{
					return;
				}

				//Cannot insert nodes before invalid nodes
				if (!ValidateSuccessor(candidateAsNode))
				{
					return;
				}

				//Cannot insert nodes before new stages
				if (successorCandidate is IStageNode<TProcedure> stageNode)
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

		protected bool FindFirst(
			IEnumerable<string> stageIDs,
			out IProcedureNode<TProcedure> currentSuccessor)
		{
			List<IProcedureNode<TProcedure>> stageNodes = new List<IProcedureNode<TProcedure>>();

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

		protected bool FindFirst(
			IEnumerable<IReadOnlyProcedureNode<TProcedure>> successors,
			out IProcedureNode<TProcedure> currentSuccessor)
		{
			List<IProcedureNode<TProcedure>> procedureNodes = new List<IProcedureNode<TProcedure>>();

			foreach (var successor in successors)
			{
				procedureNodes.Add(successor as IProcedureNode<TProcedure>);
			}

			if (!FindFirst(
				procedureNodes,
				out currentSuccessor))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND THE FIRST NODE AMONG THE PROVIDED NOES");

				return false;
			}

			return true;
		}

		protected bool FindFirst(
			IList<IProcedureNode<TProcedure>> nodes,
			out IProcedureNode<TProcedure> first,
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

					if (!allProcedureNodes.Contains(node))
					{
						logger?.LogError(
							GetType(),
							$"NODE {node} IS NOT PRESENT IN THE SYSTEM");

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

				var previousNode = currentNode.SequentialPrevious as IProcedureNode<TProcedure>;

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

				if (!allProcedureNodes.Contains(previousNode))
				{
					logger?.LogError(
						GetType(),
						$"NODE {previousNode} IS NOT PRESENT IN THE SYSTEM");

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

		protected bool FindLast(
			IEnumerable<string> stageIDs,
			out IProcedureNode<TProcedure> currentPredecessor)
		{
			List<IProcedureNode<TProcedure>> stageNodes = new List<IProcedureNode<TProcedure>>();

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

		protected bool FindLast(
			IEnumerable<IReadOnlyProcedureNode<TProcedure>> predecessors,
			out IProcedureNode<TProcedure> currentPredecessor)
		{
			List<IProcedureNode<TProcedure>> procedureNodes = new List<IProcedureNode<TProcedure>>();

			foreach (var predecessor in predecessors)
			{
				procedureNodes.Add(predecessor as IProcedureNode<TProcedure>);
			}

			if (!FindLast(
				procedureNodes,
				out currentPredecessor))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND THE LAST NODE AMONG THE PROVIDED NOES");

				return false;
			}

			return true;
		}

		protected bool FindLast(
			IList<IProcedureNode<TProcedure>> nodes,
			out IProcedureNode<TProcedure> last,
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

					if (!allProcedureNodes.Contains(node))
					{
						logger?.LogError(
							GetType(),
							$"NODE {node} IS NOT PRESENT IN THE SYSTEM");

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

				var nextNode = currentNode.SequentialNext as IProcedureNode<TProcedure>;

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

				if (!allProcedureNodes.Contains(nextNode))
				{
					logger?.LogError(
						GetType(),
						$"NODE {nextNode} IS NOT PRESENT IN THE SYSTEM");

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

		protected bool AddBefore(
			IProcedureNode<TProcedure> successor,
			IProcedureNode<TProcedure> node,
			bool parallel)
		{
			if (node is IStageNode<TProcedure> stageNode)
			{
				if (stageRepository.Has(stageNode.Stage))
				{
					logger?.LogError(
						GetType(),
						$"STAGE {stageNode.Stage} ALREADY REGISTERED");

					return false;
				}

				stageRepository.TryAdd(
					stageNode.Stage,
					stageNode);
			}

			var procedureType = node.ProcedureType;

			if (procedureType != null)
			{
				if (procedureListByTypeRepository.Has(procedureType))
				{
					procedureListByTypeRepository.TryGet(
						procedureType,
						out var proceduresEnumerable);

					proceduresEnumerable.Add(node as IProcedureNode<TProcedure>);
				}
				else
				{
					var proceduresList = new List<IProcedureNode<TProcedure>>();

					proceduresList.Add(node as IProcedureNode<TProcedure>);

					procedureListByTypeRepository.TryAdd(
						procedureType,
						proceduresList);
				}
			}

			if (!parallel)
			{
				if (successor.SequentialPrevious != null)
				{
					var previous = successor.SequentialPrevious as IProcedureNode<TProcedure>;
	
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
				var parallelPrevious = successor.ParallelPrevious as IList<IProcedureNode<TProcedure>>;

				if (parallelPrevious == null)
				{
					parallelPrevious = new List<IProcedureNode<TProcedure>>();

					successor.ParallelPrevious = parallelPrevious;
				}

				parallelPrevious.Add(node);


				var parallelNext = node.ParallelNext as IList<IProcedureNode<TProcedure>>;

				if (parallelNext == null)
				{
					parallelNext = new List<IProcedureNode<TProcedure>>();
					
					node.ParallelNext = parallelNext;
				}

				parallelNext.Add(successor);


				node.ExpectedThread = freeThreadIndex;

				freeThreadIndex++;
			}

			allProcedureNodes.Add(node);

			node.IsDetached = false;

			return true;
		}

		protected bool AddAfter(
			IProcedureNode<TProcedure> predecessor,
			IProcedureNode<TProcedure> node,
			bool parallel)
		{
			if (node is IStageNode<TProcedure> stageNode)
			{
				if (stageRepository.Has(stageNode.Stage))
				{
					logger?.LogError(
						GetType(),
						$"STAGE {stageNode.Stage} ALREADY REGISTERED");

					return false;
				}

				stageRepository.TryAdd(
					stageNode.Stage,
					stageNode);
			}

			var procedureType = node.ProcedureType;

			if (procedureType != null)
			{
				if (procedureListByTypeRepository.Has(procedureType))
				{
					procedureListByTypeRepository.TryGet(
						procedureType,
						out var proceduresEnumerable);

					proceduresEnumerable.Add(node as IProcedureNode<TProcedure>);
				}
				else
				{
					var proceduresList = new List<IProcedureNode<TProcedure>>();

					proceduresList.Add(node as IProcedureNode<TProcedure>);

					procedureListByTypeRepository.TryAdd(
						procedureType,
						proceduresList);
				}
			}

			if (!parallel)
			{
				if (predecessor.SequentialNext != null)
				{
					var next = predecessor.SequentialNext as IProcedureNode<TProcedure>;

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
				var parallelNext = predecessor.ParallelNext as IList<IProcedureNode<TProcedure>>;

				if (parallelNext == null)
				{
					parallelNext = new List<IProcedureNode<TProcedure>>();

					predecessor.ParallelNext = parallelNext;
				}

				parallelNext.Add(node);


				var parallelPrevious = node.ParallelPrevious as IList<IProcedureNode<TProcedure>>;

				if (parallelPrevious == null)
				{
					parallelPrevious = new List<IProcedureNode<TProcedure>>();

					node.ParallelPrevious = parallelPrevious;
				}

				parallelPrevious.Add(predecessor);


				node.ExpectedThread = freeThreadIndex;

				freeThreadIndex++;
			}

			allProcedureNodes.Add(node);

			node.IsDetached = false;

			return true;
		}

		protected bool Remove(
			IProcedureNode<TProcedure> node)
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

			if (node is IStageNode<TProcedure> stageNode)
			{
				if (!stageRepository.Has(stageNode.Stage))
				{
					logger?.LogError(
						GetType(),
						$"STAGE {stageNode.Stage} NOT FOUND");

					return false;
				}

				stageRepository.TryRemove(
					stageNode.Stage);
			}


			var procedureType = node.ProcedureType;

			if (procedureType != null)
			{
				if (procedureListByTypeRepository.Has(procedureType))
				{
					procedureListByTypeRepository.TryGet(
						procedureType,
						out var proceduresEnumerable);

					proceduresEnumerable.Remove(node);

					if (proceduresEnumerable.Count == 0)
					{
						procedureListByTypeRepository.TryRemove(procedureType);
					}
				}
			}


			var sequentialPrevious = node.SequentialPrevious as IProcedureNode<TProcedure>;

			var sequentialNext = node.SequentialNext as IProcedureNode<TProcedure>;

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


			var parallelPrevious = node.ParallelPrevious as IList<IProcedureNode<TProcedure>>;

			if (parallelPrevious != null)
			{
				foreach (var previous in parallelPrevious)
				{
					var previousesParallelNext = previous.ParallelNext as IList<IProcedureNode<TProcedure>>;

					if (previousesParallelNext != null)
					{
						previousesParallelNext.Remove(node);
					}
				}
			}


			var parallelNext = node.ParallelNext as IList<IProcedureNode<TProcedure>>;

			if (parallelNext != null)
			{
				foreach (var next in parallelNext)
				{
					var nextsParallelPrevious = next.ParallelPrevious as IList<IProcedureNode<TProcedure>>;

					if (nextsParallelPrevious != null)
					{
						nextsParallelPrevious.Remove(node);
					}
				}
			}

			allProcedureNodes.Remove(node);

			node.IsDetached = true;

			node.ExpectedThread = 0;

			return true;
		}

		protected bool ValidateProcessFlowFromStartToFinish()
		{
			var currentNode = StartNode as IProcedureNode<TProcedure>;

			if (currentNode == null)
			{
				logger?.LogError(
					GetType(),
					$"INVALID START NODE");

				return false;
			}

			while (currentNode != FinishNode)
			{
				if (currentNode.SequentialNext == null)
				{
					logger?.LogError(
						GetType(),
						$"NODE {currentNode} HAS NO SUCCESSOR");

					return false;
				}

				var nextNode = currentNode.SequentialNext as IProcedureNode<TProcedure>;

				if (nextNode == null)
				{
					logger?.LogError(
						GetType(),
						$"NODE {currentNode} HAS INVALID SUCCESSOR");

					return false;
				}

				if (nextNode.SequentialPrevious != currentNode)
				{
					logger?.LogError(
						GetType(),
						$"NODE {nextNode} HAS INVALID PREVIOUS NODE LINK");

					return false;
				}

				if (nextNode.ExpectedThread != currentNode.ExpectedThread)
				{
					logger?.LogError(
						GetType(),
						$"INVALID THREAD EXPECTATION: {nextNode.ExpectedThread} != {currentNode.ExpectedThread}");

					return false;
				}

				currentNode = nextNode;
			}

			return true;
		}

		protected bool ValidateAllNodesShouldHaveStartAndFinish()
		{
			foreach (var procedureNode in allProcedureNodes)
			{
				if (procedureNode == StartNode)
				{
					continue;
				}

				if (procedureNode == FinishNode)
				{
					continue;
				}

				bool hasPredecessor = false;

				if (procedureNode.SequentialPrevious != null)
				{
					hasPredecessor = true;
				}
				else
				{
					var parallelPrevious = procedureNode.ParallelPrevious as IList<IProcedureNode<TProcedure>>;

					if (parallelPrevious == null)
					{
						hasPredecessor = false;
					}
					else
					{
						hasPredecessor = parallelPrevious.Count > 0;
					}
				}

				if (!hasPredecessor)
				{
					logger?.LogError(
						GetType(),
						$"PROCEDURE NODE {procedureNode} HAS NO PREDECESSOR");

					return false;
				}

				bool hasSuccessor = false;

				if (procedureNode.SequentialNext != null)
				{
					hasSuccessor = true;
				}
				else
				{
					var parallelNext = procedureNode.ParallelNext as IList<IProcedureNode<TProcedure>>;

					if (parallelNext == null)
					{
						hasSuccessor = false;
					}
					else
					{
						hasSuccessor = parallelNext.Count > 0;
					}
				}

				if (!hasSuccessor)
				{
					logger?.LogError(
						GetType(),
						$"PROCEDURE NODE {procedureNode} HAS NO SUCCESSOR");

					return false;
				}
			}

			return true;
		}

		//Courtesy of https://stackoverflow.com/questions/583876/how-do-i-check-if-a-directed-graph-is-acyclic
		//Courtesy of https://stackoverflow.com/questions/4168/graph-serialization/4577#4577
		//Courtesy of https://en.wikipedia.org/wiki/Topological_sorting
		protected bool ValidateDAG()
		{
			var permanentMarks = new HashSet<IProcedureNode<TProcedure>>();

			var temporaryMarks = new HashSet<IProcedureNode<TProcedure>>();

			if (!VisitNodeDepthFirst(
				StartNode as IProcedureNode<TProcedure>,
				permanentMarks,
				temporaryMarks))
			{
				return false;
			}

			foreach (var procedureNode in allProcedureNodes)
			{
				if (!permanentMarks.Contains(procedureNode))
				{
					logger?.LogError(
						GetType(),
						$"NODE {procedureNode} NOT REACHABLE FROM START NODE");

					return false;
				}
			}

			return true;
		}

		protected bool VisitNodeDepthFirst(
			IProcedureNode<TProcedure> node,
			HashSet<IProcedureNode<TProcedure>> permanentMarks,
			HashSet<IProcedureNode<TProcedure>> temporaryMarks)
		{
			if (!ValidateNodeToAttachTo(
				node))
			{
				return false;
			}

			if (temporaryMarks.Contains(node))
			{
				logger?.LogError(
					GetType(),
					$"CYCLE DETECTED AT NODE: {node}");

				return false;
			}

			if (permanentMarks.Contains(node))
			{
				return true;
			}

			temporaryMarks.Add(node);

			if (node.SequentialNext != null)
			{
				var next = node.SequentialNext as IProcedureNode<TProcedure>;

				if (next != null)
				{
					if (!VisitNodeDepthFirst(
						next,
						permanentMarks,
						temporaryMarks))
					{
						return false;
					}
				}
			}

			if (node.ParallelNext != null)
			{
				var parallelNext = node.ParallelNext as IList<IProcedureNode<TProcedure>>;

				if (parallelNext != null)
				{
					foreach (var next in parallelNext)
					{
						if (!VisitNodeDepthFirst(
							next,
							permanentMarks,
							temporaryMarks))
						{
							return false;
						}
					}
				}
			}

			temporaryMarks.Remove(node);

			permanentMarks.Add(node);

			return true;
		}
	}
}