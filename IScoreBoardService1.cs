using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ScoreBoardService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IScoreBoardService1
    {
        [OperationContract]
        ScoreBoardVersion ServiceVersion();

        [OperationContract]
        GameId GameId(string game, int version);

        [OperationContract]
        void SubmitScore(ScoreBoardVersion version, GameId gameid, string username, int score);

        [OperationContract]
        IEnumerable<Tuple<string, int>> RetrieveScores(ScoreBoardVersion verion, GameId gameid);
    }
}
