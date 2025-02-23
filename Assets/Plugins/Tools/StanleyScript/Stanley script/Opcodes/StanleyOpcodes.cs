namespace HereticalSolutions.StanleyScript
{
    public static class StanleyOpcodes
    {
	    // Push literals
        public const string OP_PUSH_INT = "OP_PUSH_INT";
		public const string OP_PUSH_FLOAT = "OP_PUSH_FLOAT";
		public const string OP_PUSH_BOOL = "OP_PUSH_BOOL";
		public const string OP_PUSH_STRING = "OP_PUSH_STRING";
		
		// Types that require conversion from other types
		public const string OP_PUSH_TYPE = "OP_PUSH_TYPE";
		public const string OP_PUSH_HANDLE = "OP_PUSH_HANDLE";
		
		// Copying
		public const string OP_COPY = "OP_COPY";
		public const string OP_REVERSE_COPY = "OP_REVERSE_COPY";
		 
		// Casting
		public const string OP_CAST = "OP_CAST";
		public const string OP_TYPE_GET = "OP_TYPE_GET";
		
		// Scope
		public const string OP_PUSH_SCOPE = "OP_PUSH_SCOPE";
		public const string OP_POP_SCOPE = "OP_POP_SCOPE";
		public const string OP_SCOPE_SWITCH = "OP_SCOPE_SWITCH";
		
		// Variables
		public const string OP_ALLOC_VARIABLE = "OP_ALLOC_VARIABLE";
		public const string OP_FREE_VARIABLE = "OP_FREE_VARIABLE";
		public const string OP_PUSH_VARIABLE = "OP_PUSH_VARIABLE";
		public const string OP_PUSH_VARIABLE_GLOBAL = "OP_PUSH_VARIABLE_GLOBAL";
		
		// Objects
		public const string OP_PUSH_OBJECT = "OP_PUSH_OBJECT";
		public const string OP_ALLOC_PROPERTY = "OP_ALLOC_PROPERTY";
		public const string OP_FREE_PROPERTY = "OP_FREE_PROPERTY";
		public const string OP_PUSH_PROPERTY = "OP_PUSH_PROPERTY";
		
		// Delegates
		public const string OP_PUSH_DELEGATE = "OP_PUSH_DELEGATE";
		public const string OP_DELEGATE_CALL = "OP_DELEGATE_CALL";
		
		// Events
		public const string OP_PUSH_EVENT = "OP_PUSH_EVENT";
		public const string OP_EVENT_RAISE = "OP_EVENT_RAISE";
		public const string OP_EVENT_RESET = "OP_EVENT_RESET";
		public const string OP_EVENT_GET_LABEL = "OP_EVENT_GET_LABEL";
		public const string OP_EVENT_SET_LABEL = "OP_EVENT_SET_LABEL";
		
		// Programs
		public const string OP_PUSH_SUBPROGRAM = "OP_PUSH_SUBPROGRAM";
		public const string OP_POP_SUBPROGRAM = "OP_POP_SUBPROGRAM";
		public const string OP_SUBPROGRAM_START = "OP_SUBPROGRAM_START";
		public const string OP_SUBPROGRAM_PAUSE = "OP_SUBPROGRAM_PAUSE";
		public const string OP_SUBPROGRAM_STOP = "OP_SUBPROGRAM_STOP";
		public const string OP_SUBPROGRAM_STEP = "OP_SUBPROGRAM_STEP";
		
		// Math operations
		public const string OP_ADD = "OP_ADD";
		public const string OP_SUBSTRACT = "OP_SUBSTRACT";
		public const string OP_MULTIPLY = "OP_MULTIPLY";
		public const string OP_DIVIDE = "OP_DIVIDE";
		public const string OP_DIVIDE_INT = "OP_DIVIDE_INT";
		public const string OP_MODULO = "OP_MODULO";
		public const string OP_POWER = "OP_POWER";
		public const string OP_NEGATIVE = "OP_NEGATIVE";
		public const string OP_CEILING = "OP_CEILING";
		public const string OP_FLOOR = "OP_FLOOR";
		public const string OP_ROUND = "OP_ROUND";
		public const string OP_INC = "OP_INC";
		public const string OP_DEC = "OP_DEC";
		
		// Boolean operations
		public const string OP_NOT = "OP_NOT";
		public const string OP_AND = "OP_AND";
		public const string OP_OR = "OP_OR";
		public const string OP_XOR = "OP_XOR";
		public const string OP_EQ = "OP_EQ";
		public const string OP_NEQ = "OP_NEQ";
		public const string OP_LESS = "OP_LESS";
		public const string OP_LEQ = "OP_LEQ";
		public const string OP_MORE = "OP_MORE";
		public const string OP_MEQ = "OP_MEQ";
		
		// Collections
		public const string OP_PUSH_LIST = "OP_PUSH_LIST";
		public const string OP_LIST_COUNT = "OP_LIST_COUNT";
		public const string OP_LIST_FIRSTVAL = "OP_LIST_FIRSTVAL";
		public const string OP_LIST_LASTVAL = "OP_LIST_LASTVAL";
		public const string OP_LIST_VALATINDEX = "OP_LIST_VALATINDEX";
		public const string OP_LIST_PUSH = "OP_LIST_PUSH";
		public const string OP_LIST_POP = "OP_LIST_POP";
		public const string OP_LIST_INSERTAT = "OP_LIST_INSERTAT";
		public const string OP_LIST_REMOVEAT = "OP_LIST_REMOVEAT";
		public const string OP_LIST_CONCAT = "OP_LIST_CONCAT";
		public const string OP_LIST_ENUMERATE = "OP_LIST_ENUMERATE";
		public const string OP_LIST_CLEAR = "OP_LIST_CLEAR";
		
		// Waiting
		public const string OP_WAIT_MS = "OP_WAIT_MS";
		public const string OP_WAIT_EVENT = "OP_WAIT_EVENT";
		
		// Stack manipulation
		public const string OP_STACK_PUSH_PC = "OP_STACK_PUSH_PC";
		public const string OP_STACK_PUSH_LC = "OP_STACK_PUSH_LC";
		public const string OP_STACK_SET_LC = "OP_STACK_SET_LC";
		public const string OP_STACK_PUSH_SIZE = "OP_STACK_PUSH_SIZE";
		public const string OP_STACK_PEEK = "OP_STACK_PEEK";
		public const string OP_STACK_PEEK_FROM_TOP = "OP_STACK_PEEK_FROM_TOP";
		public const string OP_STACK_PEEK_FROM_BOTTOM = "OP_STACK_PEEK_FROM_BOTTOM";
		public const string OP_STACK_DUPE = "OP_STACK_DUPE";
		public const string OP_STACK_DUPE_FROM_TOP = "OP_STACK_DUPE_FROM_TOP";
		public const string OP_STACK_DUPE_FROM_BOTTOM = "OP_STACK_DUPE_FROM_BOTTOM";
		public const string OP_STACK_POP = "OP_STACK_POP";
		public const string OP_STACK_POP_FROM_TOP = "OP_STACK_POP_FROM_TOP";
		public const string OP_STACK_POP_FROM_BOTTOM = "OP_STACK_POP_FROM_BOTTOM";
		public const string OP_STACK_PUSH_TO_TOP = "OP_STACK_PUSH_TO_TOP";
		public const string OP_STACK_PUSH_TO_BOTTOM = "OP_STACK_PUSH_TO_BOTTOM";
		public const string OP_STACK_RVALUE = "OP_STACK_RVALUE";
		
		// Flow control
		public const string OP_ALLOC_LABEL = "OP_ALLOC_LABEL";
		public const string OP_FREE_LABEL = "OP_FREE_LABEL";
		public const string OP_JUMP = "OP_JUMP";
		public const string OP_JUMP_LABEL = "OP_JUMP_LABEL";
		public const string OP_JUMP_CONDITIONAL = "OP_JUMP_CONDITIONAL";
		public const string OP_JUMP_LABEL_CONDITIONAL = "OP_JUMP_LABEL_CONDITIONAL";
		public const string OP_RETURN = "OP_RETURN";
		public const string LABEL = "LABEL";
		
		// Logging
		public const string OP_PRINT = "OP_PRINT";
		
		// Assertions
		public const string OP_ASSERT = "OP_ASSERT";
    }
}