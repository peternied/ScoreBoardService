using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ScoreBoardService
{
    [DataContract]
    public class ScoreBoardVersion
    {
        [DataMember]
        public int Version { get; set; }
    }
}
