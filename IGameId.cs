using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ScoreBoardService
{
    [DataContract]
    public class GameId
    {
        [DataMember]
        public string GameName { get; set; }

        [DataMember]
        public int VersionId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GameId)
            {
                GameId other = obj as GameId;
                return this.GameName.Equals(other.GameName) && this.VersionId.Equals(other.VersionId);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.GameName.GetHashCode() | this.VersionId.GetHashCode();
        }

    }
}
