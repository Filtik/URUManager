using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using URUManager.Models;

namespace URUManager.Services
{
    public class ShardStorageService
    {
        private readonly string _filePath;

        public ShardStorageService()
        {
            _filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shards.json");
        }

        public List<Shard> Load()
        {
            if (!File.Exists(_filePath))
                return new List<Shard>();

            try
            {
                var serializer = new DataContractJsonSerializer(typeof(List<Shard>));
                using (var stream = File.OpenRead(_filePath))
                    return (List<Shard>)serializer.ReadObject(stream);
            }
            catch
            {
                return new List<Shard>();
            }
        }

        public void Save(List<Shard> shards)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<Shard>));
            using (var stream = File.Create(_filePath))
                serializer.WriteObject(stream, shards);
        }
    }
}
