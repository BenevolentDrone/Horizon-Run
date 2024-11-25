using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence
{
    public interface ISerializer
    {
        IFormatSerializer FormatSerializer { get; }

        ISerializationStrategy SerializationStrategy { get; }

        IReadOnlyObjectRepository Arguments { get; }

        #region Serialize

        bool Serialize<TValue>(
            TValue value);
        
        bool Serialize(
            Type ValueType,
            object valueObject);

        #endregion

        #region Deserialize

        bool Deserialize<TValue>(
            out TValue value);

        bool Deserialize(
            Type ValueType,
            out object valueObject);

        #endregion

        #region Populate

        bool Populate<TValue>(
            ref TValue value);

        bool Populate(
            Type ValueType,
            ref object valueObject);

        #endregion
    }
}