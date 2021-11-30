﻿using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class StackRepository : IRepository<List<CardInstance>>
    {
        private readonly IDatabase _db;
        private readonly ILog _log;

        public StackRepository(IDatabase db, ILog log)
        {
            _db = db;
            _log = log;
        }

        public bool Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<CardInstance>? GetById(Guid userId)
        {
            try
            {
                // Get the instaces from the user
                NpgsqlCommand cmd = new(
                    @"SELECT card_instances.*, cards.name, cards.description,
                    cards.type, cards.damage, cards.element, cards.rarity,
                    cards.race FROM user_cards, card_instances, cards
                    WHERE user_id=@user_id
                    AND card_instance_id=card_instances.id
                    AND card_id=cards.id;");
                cmd.Parameters.AddWithValue("user_id", userId);
                OrderedDictionary[] rows = _db.Select(cmd);

                List<CardInstance> stack = new();
                for (int i = 0; i < rows.Length; i++)
                {
                    OrderedDictionary row = rows[i];
                    CardInstance? instance = CardInstanceRepository.ParseFromRow(row, _log);

                    if (instance != null)
                        stack.Add(instance);
                }
                return stack;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return null;
            }
        }

        public bool Insert(List<CardInstance> entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(List<CardInstance> entityOld, List<CardInstance> entityNew)
        {
            throw new NotImplementedException();
        }
    }
}
