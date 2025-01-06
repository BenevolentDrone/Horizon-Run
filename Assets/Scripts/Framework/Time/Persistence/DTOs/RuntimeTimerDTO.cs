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
    /// Represents a data transfer object for the runtime timer
    /// </summary>
    public class RuntimeTimerDTO
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
        /// Gets or sets the ID of the timer
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
        /// Gets or sets the current state of the timer
        /// </summary>
        public ETimerState State { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("CurrentTimeElapsed")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(3)]
#endif
        /// <summary>
        /// Gets or sets the current time elapsed for the timer
        /// </summary>
        public float CurrentTimeElapsed { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("Accumulate")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(4)]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether the timer should accumulate time
        /// </summary>
        public bool Accumulate { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("Repeat")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(5)]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether the timer should repeat
        /// </summary>
        public bool Repeat { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("FlushTimeElapsedOnRepeat")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(6)]
#endif
        public bool FlushTimeElapsedOnRepeat { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("FireRepeatCallbackOnFinish")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(7)]
#endif
        public bool FireRepeatCallbackOnFinish { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("CurrentDuration")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(8)]
#endif
        /// <summary>
        /// Gets or sets the current duration of the timer
        /// </summary>
        public float CurrentDuration { get; set; }

#if JSON_OPT_IN_SUPPORT
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("DefaultDuration")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(9)]
#endif
        /// <summary>
        /// Gets or sets the default duration of the timer
        /// </summary>
        public float DefaultDuration { get; set; }
    }
}