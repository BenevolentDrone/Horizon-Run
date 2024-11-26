using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Persistence
{
    public interface ISerializer
    {
        IFormatSerializer FormatSerializer { get; }

        ISerializationStrategy SerializationStrategy { get; }

        IStronglyTypedMetadata Arguments { get; }

        #region Serialize

        bool Serialize<TValue>(
            TValue value);
        
        bool Serialize(
            Type valueType,
            object valueObject);

        #endregion

        #region Deserialize

        bool Deserialize<TValue>(
            out TValue value);

        bool Deserialize(
            Type valueType,
            out object valueObject);

        #endregion

        #region Populate

        bool Populate<TValue>(
            ref TValue value);

        bool Populate(
            Type valueType,
            ref object valueObject);

        #endregion
    }
}