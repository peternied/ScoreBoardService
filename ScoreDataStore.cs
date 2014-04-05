﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace ScoreBoardService
{
    public interface IScoreDataStore
    {
        IEnumerable<Tuple<string, int>> GetGameScores(GameId gameId);

        void AddGameScore(GameId gameId, string playerName, int score);
    }

    internal class ScoreDataStore : IScoreDataStore
    {
        private ConcurrentDictionary<GameId, Dictionary<string, int>> allGameScores { get; set; }
        private object InternalLock { get; set; }
        private string FileStoreName = Path.Combine(Path.GetTempPath(), "scoredatastore.xml");
        private bool SeralizationEnabled = false;

        internal ScoreDataStore()
        {
            InternalLock = new object();
            allGameScores = new ConcurrentDictionary<GameId, Dictionary<string, int>>();

            if (SeralizationEnabled)
            {
                SerializeFromDisk();
            }
        }

        private void SerializeFromDisk()
        {
            lock (InternalLock)
            {
                if (!File.Exists(FileStoreName))
                {
                    return;
                }
                using (FileStream fs = new FileStream(FileStoreName, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<KeyValuePair<GameId, List<KeyValuePair<string, int>>>>));
                    try
                    {
                        var deserialized = serializer.Deserialize(fs) as List<KeyValuePair<GameId, List<KeyValuePair<string, int>>>>;
                        if (deserialized != null)
                        {
                            allGameScores.Clear();
                            foreach (var entry in deserialized)
                            {
                                allGameScores.TryAdd(entry.Key, entry.Value.ToDictionary(t => t.Key, t => t.Value));
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public IEnumerable<Tuple<string, int>> GetGameScores(GameId gameId)
        {
            Dictionary<string, int> gameScores = null;
            if (!this.allGameScores.TryGetValue(gameId, out gameScores))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (gameScores == null)
            {
                return Enumerable.Empty<Tuple<string, int>>();
            }

            return gameScores
                .OrderBy(t => t.Value)
                .Select(t => new Tuple<string, int>(t.Key, t.Value))                
                .Take(20);
        }

        public void AddGameScore(GameId gameId, string playerName, int score)
        {
            this.allGameScores.AddOrUpdate(gameId,
                (key) =>
                {
                    Dictionary<string, int> newDictionary = new Dictionary<string, int>();
                    newDictionary.Add(playerName, score);
                    return newDictionary;
                },
                (key, dictionary) =>
                {
                    dictionary[playerName] = Math.Max(score, dictionary[playerName]);
                    return dictionary;
                });

            if (SeralizationEnabled)
            {
                SerializeToDisk();
            }
        }

        private void SerializeToDisk()
        {
            lock (InternalLock)
            {
                using (FileStream fs = new FileStream(FileStoreName, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<KeyValuePair<GameId, List<KeyValuePair<string, int>>>>));
                    var serialized = new List<KeyValuePair<GameId, List<KeyValuePair<string, int>>>>();

                    foreach (var item in allGameScores)
                    {
                        serialized
                            .Add(new KeyValuePair<GameId, List<KeyValuePair<string, int>>>(item.Key,
                                item.Value.Select(t => new KeyValuePair<string, int>(t.Key, t.Value))
                            .ToList()));
                    }

                    serializer.Serialize(fs, serialized);
                }
            }
        }
    }
}