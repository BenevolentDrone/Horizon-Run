namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public static class SystemConstsUnity
	{
		//Core loop system builders

		public const string UPDATE_SYSTEM_BUILDER_ID = "Update";

		public const string FIXED_UPDATE_SYSTEM_BUILDER_ID = "FixedUpdate";

		public const string LATE_UPDATE_SYSTEM_BUILDER_ID = "LateUpdate";

		//Core loop root installers

		public const string UPDATE_ROOT_INSTALLER_ID = "Update.Root";

		public const string FIXED_UPDATE_ROOT_INSTALLER_ID = "FixedUpdate.Root";

		public const string LATE_UPDATE_ROOT_INSTALLER_ID = "LateUpdate.Root";

		//Core loop systems

		public const string UPDATE_SYSTEMS_ID = "Update systems";

		public const string FIXED_UPDATE_SYSTEMS_ID = "Fixed update systems";

		public const string LATE_UPDATE_SYSTEMS_ID = "Late update systems";

		//Fixed update subsystems

		//Update subsystems

		public const string MODEL_VALIDATION_SYSTEMS_ID = "Model validation systems";

		public const string GAME_LOGIC_SYSTEMS_ID = "Game logic systems";

		//Late update subsystems

		public const string VIEW_INPUT_SYSTEMS_ID = "View input systems";

		public const string VIEW_PRESENTER_SYSTEMS_ID = "View presenter systems";

		public const string VIEW_VISUAL_SYSTEMS_ID = "View visual systems";
	}
}