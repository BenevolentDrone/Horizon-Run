namespace HereticalSolutions.Templates.Universal.Unity
{
	public interface IPhysicsManager<THandle, TPhysicsBody>
	{
		bool HasPhysicsBody(THandle rigidBodyHandle);

		bool SpawnPhysicsBody(
			string prototypeID,
			out THandle physicsBodyHandle,
			out TPhysicsBody physicsBody);

		bool TryGetPhysicsBody(
			THandle physicsBodyHandle,
			out TPhysicsBody physicsBody);

		bool TryDestroyPhysicsBody(
			THandle physicsBodyHandle);
	}
}