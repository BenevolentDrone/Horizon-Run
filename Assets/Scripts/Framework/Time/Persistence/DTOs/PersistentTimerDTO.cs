#define JSON_OPT_IN_SUPPORT
#define CSV_SUPPORT
#define PROTOBUF_SUPPORT

#if JSON_OPT_IN_SUPPORT
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
    [Serializable]
#if JSON_OPT_IN_SUPPORT
    [JsonObject(MemberSerialization.OptIn)]
#endif
#if CSV_SUPPORT
    [Delimiter(HereticalSolutions.Persistence.PersistenceConsts.CSV_DELIMITER)]
#endif
#if PROTOBUF_SUPPORT
    [ProtoContract]
#endif
    /// <summary>
    /// Represents a persistent timer data transfer object (DTO)
    /// </summary>
    public class PersistentTimerDTO
    {
#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("ID")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(1)]
#endif
        /// <summary>
        /// Gets or sets the ID of the persistent timer
        /// </summary>
        public string ID { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("State")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(2)]
#endif
        /// <summary>
        /// Gets or sets the state of the persistent timer
        /// </summary>
        public ETimerState State { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("StartTime")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(3)]
#endif
        /// <summary>
        /// Gets or sets the start time of the persistent timer
        /// </summary>
        public DateTime StartTime { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("EstimatedFinishTime")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(4)]
#endif
        /// <summary>
        /// Gets or sets the estimated finish time of the persistent timer
        /// </summary>
        public DateTime EstimatedFinishTime { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("SavedProgress")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(5)]
#endif
        /// <summary>
        /// Gets or sets the saved progress of the persistent timer
        /// </summary>
        public TimeSpan SavedProgress { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("Accumulate")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(6)]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether the persistent timer should accumulate elapsed time or not
        /// </summary>
        public bool Accumulate { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("Repeat")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(7)]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether the persistent timer should repeat after reaching the finish time or not
        /// </summary>
        public bool Repeat { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("FlushTimeElapsedOnRepeat")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(8)]
#endif
        public bool FlushTimeElapsedOnRepeat { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("FireRepeatCallbackOnFinish")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(9)]
#endif
        public bool FireRepeatCallbackOnFinish { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("CurrentDurationSpan")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(10)]
#endif
        /// <summary>
        /// Gets or sets the current duration of the persistent timer
        /// </summary>
        public TimeSpan CurrentDurationSpan { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("DefaultDurationSpan")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(11)]
#endif
        /// <summary>
        /// Gets or sets the default duration of the persistent timer
        /// </summary>
        public TimeSpan DefaultDurationSpan { get; set; }
    }
}