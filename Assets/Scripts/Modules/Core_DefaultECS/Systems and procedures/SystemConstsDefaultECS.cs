namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public static class SystemConstsDefaultECS
	{
		//Simulation world system builders

		public const string SIMULATION_WORLD_INITIALIZATION_SYSTEM_BUILDER_ID = "Simulation.Init";

		public const string SIMULATION_WORLD_RESOLVE_SYSTEM_BUILDER_ID = "Simulation.Resolve";

		public const string SIMULATION_WORLD_DEINITIALIZATION_SYSTEM_BUILDER_ID = "Simulation.Deinit";

		//Simulation world root installers

		public const string SIMULATION_WORLD_INITIALIZATION_PACKAGE_INSTALLER_ID = "Simulation.Init.Root";

		public const string SIMULATION_WORLD_RESOLVE_PACKAGE_INSTALLER_ID = "Simulation.Resolve.Root";

		public const string SIMULATION_WORLD_DEINITIALIZATION_PACKAGE_INSTALLER_ID = "Simulation.Deinit.Root";

		//Simulation world systems

		public const string SIMULATION_WORLD_INITIALIZATION_SYSTEMS_ID = "Simulation world initialization systems";

		public const string SIMULATION_WORLD_RESOLVE_SYSTEMS_ID = "Simulation world resolve systems";

		public const string SIMULATION_WORLD_DEINITIALIZATION_SYSTEMS_ID = "Simulation world deinitialization systems";

		//View world system builders

		public const string VIEW_WORLD_INITIALIZATION_SYSTEM_BUILDER_ID = "View.Init.Root";

		public const string VIEW_WORLD_RESOLVE_SYSTEM_BUILDER_ID = "View.Resolve.Root";

		public const string VIEW_WORLD_DEINITIALIZATION_SYSTEM_BUILDER_ID = "View.Deinit.Root";

		//View world root installers

		public const string VIEW_WORLD_INITIALIZATION_PACKAGE_INSTALLER_ID = "View.Init.Root";

		public const string VIEW_WORLD_RESOLVE_PACKAGE_INSTALLER_ID = "View.Resolve.Root";

		public const string VIEW_WORLD_DEINITIALIZATION_PACKAGE_INSTALLER_ID = "View.Deinit.Root";

		//View world subsystems

		public const string PRESENTER_INITIALIZATION_SYSTEMS_ID = "Presenter initialization systems";

		//Simulation world systems

		public const string VIEW_WORLD_INITIALIZATION_SYSTEMS_ID = "View world initialization systems";

		public const string VIEW_WORLD_RESOLVE_SYSTEMS_ID = "View world resolve systems";

		public const string VIEW_WORLD_DEINITIALIZATION_SYSTEMS_ID = "View world deinitialization systems";

		//Event world system builders

		public const string EVENT_WORLD_SYSTEM_BUILDER_ID = "Event";

		//Event world root installers

		public const string EVENT_WORLD_PACKAGE_INSTALLER_ID = "Event.Root";

		//Event world systems

		public const string EVENT_WORLD_SYSTEMS_ID = "Event world systems";

		//Lifetime subsystems

		public const string SIMULATION_WORLD_LIFETIME_SYSTEMS_ID = "Simulation world lifetime systems";

		public const string VIEW_WORLD_LIFETIME_SYSTEMS_ID = "View world lifetime systems";

		public const string EVENT_WORLD_LIFETIME_SYSTEMS_ID = "Event world lifetime systems";
	}
}