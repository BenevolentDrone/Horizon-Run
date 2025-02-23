namespace HereticalSolutions.StanleyScript
{
    public class StanleyScopeObject
        : StanleyObject
    {
        public StanleyScopeObject(
            ISymbolTable properties)
        : base (
            properties)
        {
        }

        public IStanleyVariable Handle
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.SCOPE_HANDLE_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable ReturnPC
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.SCOPE_RETURN_PC_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable ReturnScope
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.SCOPE_RETURN_SCOPE_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable EventList
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.SCOPE_EVENT_LIST_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable JumpTable
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.SCOPE_JUMP_TABLE_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        #region IClonable

        public override object Clone()
        {
            var propertiesAsClonable = properties as IClonable;

            return StanleyFactory.BuildStanleyScopeObject(
                (ISymbolTable)propertiesAsClonable.Clone());
        }

        #endregion
    }
}