namespace HereticalSolutions.StanleyScript
{
	public static class StanleyConsts
	{
		//Courtesy of https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
		//Courtesy of https://stackoverflow.com/questions/4780728/regex-split-string-preserving-quotes/4780801#4780801
		public const string REGEX_SPLIT_LINE_BY_WHITESPACE_UNLESS_WITHIN_QUOTES = "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

		public const string LINE_COMMENT_PREFIX = "//";

		public const string SCOPE_OBJECT_NAME = "SCOPE";

		public const string SCOPE_HANDLE_VARIABLE_NAME = "HANDLE";

		public const string SCOPE_RETURN_PC_VARIABLE_NAME = "RETURN_PC";

		public const string SCOPE_RETURN_SCOPE_VARIABLE_NAME = "RETURN_SCOPE";

		public const string SCOPE_EVENT_LIST_VARIABLE_NAME = "EVENT_LIST";

		public const string SCOPE_JUMP_TABLE_VARIABLE_NAME = "JUMP_TABLE";

		public const string SCOPE_COUNT_VARIABLE_NAME = "COUNT";

		public const string SCOPE_COUNTER_VARIABLE_NAME = "COUNTER";
		
		public const string PROGRAM_HANDLE_VARIABLE_NAME = "HANDLE";
		
		public const string PROGRAM_STARTED_VARIABLE_NAME = "started";
		
		public const string PROGRAM_PAUSED_VARIABLE_NAME = "paused";
		
		public const string PROGRAM_RESUMED_VARIABLE_NAME = "resumed";
		
		public const string PROGRAM_STOPPED_VARIABLE_NAME = "stopped";
		
		public const string PROGRAM_STEPPED_VARIABLE_NAME = "stepped";
		
		public const string PROGRAM_DISCARDED_VARIABLE_NAME = "discarded";

		public const string RVALUE_VARIABLE_NAME = "RVALUE";

		//public const string TEMPORARY_VARIABLE_NAME_PREFIX = "TEMPVAR_";

		public const string LABEL = "LABEL";
	}
}