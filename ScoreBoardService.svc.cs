using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ScoreBoardService
{
    public class ScoreBoardService : IScoreBoardService1
    {
        public ScoreBoardVersion ServiceVersion()
        {
            return new ScoreBoardVersion { Version = 1 };
        }

        public GameId GameId(string game, int version)
        {
            return new GameId { GameName = game, VersionId = version };
        }

        public void SubmitScore(ScoreBoardVersion version, GameId gameid, string username, int score)
        {
            switch (version.Version)
            {
                case 1:
                    ScoreDataStoreFactory.GetDataStore().AddGameScore(gameid, username, score);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerable<Tuple<string, int>> RetrieveScores(ScoreBoardVersion version, GameId gameid)
        {
            switch (version.Version)
            {
                case 1:
                    return ScoreDataStoreFactory.GetDataStore().GetGameScores(gameid);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
