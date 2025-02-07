#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
using Newtonsoft.Json;
#endif

#if CSV_SUPPORT
using CsvHelper.Configuration.Attributes;
#endif

#if PROTOBUF_SUPPORT
using ProtoBuf;
#endif

using System;

namespace HereticalSolutions.Time
{
#region Serialization attributes        
    [Serializable]
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
    [JsonObject(MemberSerialization.OptIn)]
#endif
#if CSV_SUPPORT
    [Delimiter(HereticalSolutions.Persistence.PersistenceConsts.CSV_DELIMITER)]
#endif
#if PROTOBUF_SUPPORT
    [ProtoContract]
#endif
#endregion
    public class PersistentTimerDTO
    {
#region Serialization attributes     
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("ID")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(1)]
#endif
#endregion
        public string ID { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("State")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(2)]
#endif
#endregion
        public ETimerState State { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("StartTime")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(3)]
#endif
#endregion
        public DateTime StartTime { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("EstimatedFinishTime")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(4)]
#endif
#endregion
        public DateTime EstimatedFinishTime { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("SavedProgress")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(5)]
#endif
#endregion
        public TimeSpan SavedProgress { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("Accumulate")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(6)]
#endif
#endregion
        public bool Accumulate { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("Repeat")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(7)]
#endif
#endregion
        public bool Repeat { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("FlushTimeElapsedOnRepeat")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(8)]
#endif
#endregion
        public bool FlushTimeElapsedOnRepeat { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("FireRepeatCallbackOnFinish")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(9)]
#endif
#endregion
        public bool FireRepeatCallbackOnFinish { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("CurrentDurationSpan")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(10)]
#endif
#endregion
        public TimeSpan CurrentDurationSpan { get; set; }

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("DefaultDurationSpan")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(11)]
#endif
#endregion
        public TimeSpan DefaultDurationSpan { get; set; }
    }
}