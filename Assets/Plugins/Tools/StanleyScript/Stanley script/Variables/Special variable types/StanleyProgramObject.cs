namespace HereticalSolutions.StanleyScript
{
    public class StanleyProgramObject
        : StanleyObject
    {
        public StanleyProgramObject(
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
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable Start
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable Pause
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.PROGRAM_PAUSED_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable Resume
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.PROGRAM_RESUMED_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable Stop
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.PROGRAM_STOPPED_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable Step
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.PROGRAM_STEPPED_VARIABLE_NAME,
                        out var result))
                {
                    return result;
                }

                return null;
            }
        }
        
        public IStanleyVariable Discard
        {
            get
            {
                if (properties.TryGetVariable(
                        StanleyConsts.PROGRAM_DISCARDED_VARIABLE_NAME,
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

            return StanleyFactory.BuildStanleyProgramObject(
                (ISymbolTable)propertiesAsClonable.Clone());
        }

        #endregion
    }
}