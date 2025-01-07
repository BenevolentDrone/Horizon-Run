namespace HereticalSolutions.Pools
{
    public static class ArgumentExtensions
    {
        public static bool TryGetArgument<TArgument>(
            this IPoolPopArgument[] args,
            out TArgument value) where TArgument : IPoolPopArgument
        {
            value = default(TArgument);

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is TArgument)
                {
                    value = (TArgument)args[i];

                    return true;
                }
            }

            return false;
        }
    }
}