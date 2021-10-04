using MTCG.Models;
using MTCG.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Controller
{
    public class BattleController : Singleton<BattleController>
    {
        public const int MaxPlayedRounds = 100;

        //public List<Deck> MatchQueue { get; }

        //public BattleController()
        //{
        //    MatchQueue = new();
        //}

        public BattleResult Battle(Deck deck1, Deck deck2)
        {
            User playerOne = UserController.Instance.Select(deck1.UserID);
            User playerTwo = UserController.Instance.Select(deck2.UserID);

            // Create card lists from card instances of the decks
            List<Card> playerOneDeck = CardController.Instance.GetCards(deck1.Cards);
            List<Card> playerTwoDeck = CardController.Instance.GetCards(deck2.Cards);

            Card cardPlayerOne;
            Card cardPlayerTwo;

            // Defines the winner (0 = Tie, 1 = PlayerOne, 2 = PlayerTwo)
            int winner = 0;

            // Stream of chars where the log will be written into
            CharStream log = new();
            // Start log with welcome message
            log.WriteLine("Battle begins!");
            log.WriteLine($"Player One: {playerOne.Username} (ELO {playerOne.ELO})");
            PrintDeck(playerOneDeck, log);
            log.WriteLine($"Player Two: {playerTwo.Username} (ELO {playerTwo.ELO})");
            PrintDeck(playerTwoDeck, log);

            // Init battle
            for (int i = 0; i < MaxPlayedRounds; i++)
            {
                // Check if battle is over
                if (playerTwoDeck.Count == 0)
                {
                    winner = 1;
                    break;
                }
                else if (playerOneDeck.Count == 0)
                {
                    winner = 2;
                    break;
                }

                log.WriteLine($"Round {i}:");

                // Choose random cards
                cardPlayerOne = GetRandomCard(playerOneDeck);
                cardPlayerTwo = GetRandomCard(playerTwoDeck);

                log.WriteLine($"{playerOne.Username} playes {cardPlayerOne.Name}");
                log.WriteLine($"{playerTwo.Username} playes {cardPlayerTwo.Name}");

                // Setup base fight varaibles
                bool p1Lose = false;
                bool p2Lose = false;
                float playerOneDamage = cardPlayerOne.Damage;
                float playerTwoDamage = cardPlayerTwo.Damage;

                // Monster fight
                if ((cardPlayerOne is MonsterCard monsterPlayerOne) && (cardPlayerTwo is MonsterCard monsterPlayerTwo))
                {
                    Race p1Race = monsterPlayerOne.Race;
                    Race p2Race = monsterPlayerTwo.Race;

                    // Get effects
                    p1Lose = LoseDueToSpecialty(p1Race, p2Race, log);
                    p2Lose = LoseDueToSpecialty(p2Race, p1Race, log);
                }
                // Mixed fight / Spell fight
                else
                {
                    // Check modifiers
                    p1Lose = LoseDueToSpecialty(cardPlayerOne, cardPlayerTwo, log);
                    p2Lose = LoseDueToSpecialty(cardPlayerTwo, cardPlayerOne, log);

                    // Check if lost due to modifier
                    if (!p1Lose && !p2Lose)
                    {
                        // Element modifier
                        playerOneDamage = GetElementMod(cardPlayerOne, cardPlayerTwo, log);
                        playerTwoDamage = GetElementMod(cardPlayerTwo, cardPlayerOne, log);
                    }
                }

                // Normal round, calculate damage
                if (!p1Lose && !p2Lose)
                {
                    // Check for draw
                    if (playerOneDamage != playerTwoDamage)
                    {
                        log.WriteLine($"{cardPlayerOne.Name} does {playerOneDamage} Damage");
                        log.WriteLine($"{cardPlayerTwo.Name} does {playerTwoDamage} Damage");
                        p1Lose = playerOneDamage < playerTwoDamage;
                        p2Lose = !p1Lose;
                    }
                }

                // Redistribute decks
                if (p1Lose)
                {
                    log.WriteLine("Player one loses this round");
                    playerOneDeck.Remove(cardPlayerOne);
                    playerTwoDeck.Add(cardPlayerOne);
                }
                else if (p2Lose)
                {
                    log.WriteLine("Player two loses this round");
                    playerTwoDeck.Remove(cardPlayerTwo);
                    playerOneDeck.Add(cardPlayerTwo);
                }
                else
                {
                    log.WriteLine("This round is a draw");
                }
            }

            // Reward elo
            if(winner == 1)
            {
                return PlayerWins(playerOne, playerTwo, log);
            }
            else if (winner == 2)
            {
                return PlayerWins(playerTwo, playerOne, log);
            }
            else
            {
                return Tie(playerOne, playerTwo, log);
            }
        }
        
        private BattleResult Tie(User u1, User u2, CharStream log)
        {
            log.WriteLine("Maximum rounds reached! The battle ends in a tie");
            
            u1.PlayedGames++;
            u2.PlayedGames++;
            UserController.Instance.UpdatePlayedRounds(u1);
            UserController.Instance.UpdatePlayedRounds(u2);

            return new BattleResult(u1.ID, u2.ID, log.ToString(), true);
        }

        private BattleResult PlayerWins(User winner, User loser, CharStream log)
        {
            log.WriteLine($"{loser.Username} has no cards left in his deck...");
            log.WriteLine($"{winner.Username} wins the battle!");
            // Update stats
            winner.ELO += 3;
            winner.PlayedGames++;
            winner.Coins += 1;

            loser.ELO -= 5;
            loser.PlayedGames++;

            // Update stats in database
            UserController.Instance.UpdateUserCoins(winner);
            UserController.Instance.UpdateUserELO(winner);
            UserController.Instance.UpdateUserELO(loser);
            UserController.Instance.UpdatePlayedRounds(winner);
            UserController.Instance.UpdatePlayedRounds(loser);

            return new BattleResult(winner.ID, loser.ID, log.ToString(), false);
        }
        
        private bool IsMonsterType(Card card, Race race)
        {
            return card is MonsterCard m && m.Race == race;
        }

        private void PrintDeck(List<Card> cards, CharStream log)
        {
            log.WriteLine("Deck:");
            for (int i = 0; i < cards.Count; i++)
            {
                log.WriteLine($"Card {i}:");
                log.WriteLine($"{cards[i].Name}");
                log.WriteLine($"{cards[i].CardType}");
                log.WriteLine($"{cards[i].Damage} Damage");
                log.WriteLine($"{cards[i].Element}");
                if (cards[i] is MonsterCard m)
                {
                    log.WriteLine($"{m.Race}");
                }
            }
        }

        /// <summary>
        /// Returns the damage of the attacker with the element modifiers applied<br></br><br></br>
        /// Effectiveness:<br></br>
        ///  • water->fire<br></br>
        ///  • fire->normal<br></br>
        ///  • normal->water
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        float GetElementMod(Card attacker, Card defender, CharStream log)
        {
            int dmg;
            Element ae = attacker.Element;
            Element de = defender.Element;

            // No modifier
            if (ae == de)
                return attacker.Damage;

            switch (ae)
            {
                case Element.Normal:
                    switch (de)
                    {
                        // Damage is halved
                        case Element.Fire:
                            return attacker.Damage * 0.5f;
                        // Damage is doubled
                        case Element.Water:
                            return attacker.Damage * 2f;
                    }
                    break;
                case Element.Fire:
                    switch (de)
                    {
                        // Damage is doubled
                        case Element.Normal:
                            return attacker.Damage * 2f;
                        // Damage is halved
                        case Element.Water:
                            return attacker.Damage * 0.5f;
                    }
                    break;
                case Element.Water:
                    switch (de)
                    {
                        // Damage is doubled
                        case Element.Fire:
                            return attacker.Damage * 2f;
                        // Damage is halved
                        case Element.Normal:
                            return attacker.Damage * 0.5f;
                    }
                    break;
            }

            // Should not be possible
            return attacker.Damage;
        }

        public Deck FindMatch(Deck searcher)
        {
            Npgsql.NpgsqlCommand cmd = new("SELECT deck_id FROM user_decks WHERE deck_id!=@searcher_id AND main_deck=True ORDER BY RANDOM() LIMIT 1");
            cmd.Parameters.AddWithValue("searcher_id", searcher.ID);
            var row = Database.Instance.SelectSingle(cmd);
            return DeckController.Instance.Select((Guid)row["deck_id"]);
        }

        /// <summary>
        /// Returns wheter the attacker loses due to a monster modifer or not<br></br>
        /// • Goblins are too afraid of Dragons to attack.<br></br>
        /// • Wizzard can control Orks so they are not able to damage them.<br></br> 
        /// • The FireElves know Dragons since they were little and can evade their attacks.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        bool LoseDueToSpecialty(Race attacker, Race defender, CharStream log)
        {
            if (attacker == Race.Goblin && defender == Race.Draconid)
            {
                // Attacker loses
                log.WriteLine("The goblin is too afraid to attack the dragon");
                return true;
            }
            else if (attacker == Race.Orc && defender == Race.Mage)
            {
                // Attacker loses
                log.WriteLine("The orc is under the mages control and cannot attack");
                return true;
            }
            else if (attacker == Race.Draconid && defender == Race.FireElf)
            {
                // Attacker loses
                log.WriteLine("The fire elf evades the dragons attack");
                return true;
            }
            return false;
        }

        // • The armor of Knights is so heavy that WaterSpells make them drown them instantly.
        // • The Kraken is immune against spells.
        bool LoseDueToSpecialty(Card attacker, Card defender, CharStream log)
        {
            // Attacker loses due to kraken
            if(IsMonsterType(defender, Race.Kraken) && attacker.CardType == CardType.Spell)
            {
                log.WriteLine("The kraken is immune against spells");
                return true;
            }
            // Attacker loses due to the knight drowning in water
            else if(IsMonsterType(attacker, Race.Knight) && defender.CardType == CardType.Spell &&
                defender.Element == Element.Water)
            {
                log.WriteLine("The armor of the knight is so heavy that he instantly drowns against the water spell");
                return true;
            }
            return false;
        }
        private Card GetRandomCard(List<Card> cards)
        {
            return cards[new Random().Next(0, cards.Count)];
        }
    }

    public class BattleResult
    {
        public Guid Winner { get; }
        public Guid Loser { get; }
        public string Log { get; }
        public bool Tie { get; }

        public BattleResult(Guid winner, Guid loser, string log, bool tie)
        {
            Winner = winner;
            Loser = loser;
            Log = log;
            Tie = tie;
        }
    }
}
