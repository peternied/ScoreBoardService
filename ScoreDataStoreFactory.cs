using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScoreBoardService
{
    internal class ScoreDataStoreFactory
    {
        private static IScoreDataStore ScoreDataStore { get; set; }

        internal static IScoreDataStore GetDataStore()
        {
            if (ScoreDataStore == null)
            {
                ScoreDataStore = new ScoreDataStore();
            }
            return ScoreDataStore;
        }
    }
}