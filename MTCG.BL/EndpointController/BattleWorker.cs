using MTCG.BL.Http;
using MTCG.DAL.Repositories;
using MTCG.Models;
using MTCG.Serialization;
using Newtonsoft.Json;
using System.Net.Mime;

namespace MTCG.BL.EndpointController
{
    public class BattleWorker : Worker
    {
        public const int MaxPlayedRounds = 100;

        private readonly BattleController _controller;
        private readonly UserRepository _userRepository;
        private static readonly object _lock = new();

        public BattleWorker(
            Server server,
            UserRepository userRepository,
            BattleController controller,
            ILog log,
            int id) : base(server, log, id)
        {
            _userRepository = userRepository;
            _controller = controller;
        }

        protected override void Run()
        {
            while (_running)
            {
                if (_controller.MatchQueue.Count == 2)
                {
                    lock (_lock)
                    {
                        if (_controller.MatchQueue.TryDequeue(out Tuple<Deck, HttpRequest>? t1) &&
                           _controller.MatchQueue.TryDequeue(out Tuple<Deck, HttpRequest>? t2))
                        {
                            BattleResult result = Battle(t1.Item1, t2.Item1);

                            HttpResponse response = new(
                                JsonConvert.SerializeObject(result),
                                HttpStatusCode.OK,
                                MediaTypeNames.Application.Json);

                            _server.HttpClient.SendHttpResponse(response, t1.Item2);
                            _server.HttpClient.SendHttpResponse(response, t2.Item2);
                        }
                    }
                }
            }
        }

        private BattleResult Battle(Deck deck1, Deck deck2)
        {
            User? playerOne = _userRepository.GetById(deck1.UserID);
            User? playerTwo = _userRepository.GetById(deck2.UserID);

            if (playerOne == null || playerTwo == null)
                throw new Exception("Fatal error in battle!");

            CardInstance cardPlayerOne;
            CardInstance cardPlayerTwo;

            // Defines the winner (0 = Tie, 1 = PlayerOne, 2 = PlayerTwo)
            int winner = 0;

            // Stream of chars where the log will be written into
            CharStream battleLog = new();
            // Start log with welcome message
            battleLog.WriteLine("Battle begins!");
            battleLog.WriteLine($"Player One: {playerOne.Username} (ELO {playerOne.ELO})");
            PrintDeck(deck2.Cards, battleLog);
            battleLog.WriteLine($"Player Two: {playerTwo.Username} (ELO {playerTwo.ELO})");
            PrintDeck(deck2.Cards, battleLog);

            // Init battle
            for (int i = 0; i < MaxPlayedRounds; i++)
            {
                // Check if battle is over
                if (deck1.Cards.Count == 0)
                {
                    winner = 1;
                    break;
                }
                else if (deck2.Cards.Count == 0)
                {
                    winner = 2;
                    break;
                }

                battleLog.WriteLine($"Round {i}:");

                // Choose random cards
                cardPlayerOne = GetRandomCard(deck1.Cards);
                cardPlayerTwo = GetRandomCard(deck1.Cards);

                battleLog.WriteLine($"{playerOne.Username} playes {cardPlayerOne.Name}");
                battleLog.WriteLine($"{playerTwo.Username} playes {cardPlayerTwo.Name}");

                // Setup base fight varaibles
                bool p1Lose = false;
                bool p2Lose = false;
                float playerOneDamage = cardPlayerOne.Damage;
                float playerTwoDamage = cardPlayerTwo.Damage;

                // Monster fight
                if ((cardPlayerOne.CardType == CardType.Monster) && (cardPlayerTwo.CardType == CardType.Monster))
                {
                    Race p1Race = cardPlayerOne.Race;
                    Race p2Race = cardPlayerTwo.Race;

                    // Get effects
                    p1Lose = LoseDueToSpecialty(p1Race, p2Race, battleLog);
                    p2Lose = LoseDueToSpecialty(p2Race, p1Race, battleLog);
                }
                // Mixed fight / Spell fight
                else
                {
                    // Check modifiers
                    p1Lose = LoseDueToSpecialty(cardPlayerOne, cardPlayerTwo, battleLog);
                    p2Lose = LoseDueToSpecialty(cardPlayerTwo, cardPlayerOne, battleLog);

                    // Check if lost due to modifier
                    if (!p1Lose && !p2Lose)
                    {
                        // Element modifier
                        playerOneDamage = GetElementMod(cardPlayerOne, cardPlayerTwo, battleLog);
                        playerTwoDamage = GetElementMod(cardPlayerTwo, cardPlayerOne, battleLog);
                    }
                }

                // Normal round, calculate damage
                if (!p1Lose && !p2Lose)
                {
                    // Check for draw
                    if (playerOneDamage != playerTwoDamage)
                    {
                        battleLog.WriteLine($"{cardPlayerOne.Name} does {playerOneDamage} Damage");
                        battleLog.WriteLine($"{cardPlayerTwo.Name} does {playerTwoDamage} Damage");
                        p1Lose = playerOneDamage < playerTwoDamage;
                        p2Lose = !p1Lose;
                    }
                }

                // Redistribute decks
                if (p1Lose)
                {
                    battleLog.WriteLine("Player one loses this round");
                    deck1.Cards.Remove(cardPlayerOne);
                    deck2.Cards.Add(cardPlayerOne);
                }
                else if (p2Lose)
                {
                    battleLog.WriteLine("Player two loses this round");
                    deck2.Cards.Remove(cardPlayerTwo);
                    deck1.Cards.Add(cardPlayerTwo);
                }
                else
                {
                    battleLog.WriteLine("This round is a draw");
                }
            }

            // Reward elo
            if (winner == 1)
            {
                return PlayerWins(playerOne, playerTwo, battleLog);
            }
            else if (winner == 2)
            {
                return PlayerWins(playerTwo, playerOne, battleLog);
            }
            else
            {
                return Tie(playerOne, playerTwo, battleLog);
            }
        }

        private BattleResult Tie(User u1, User u2, CharStream log)
        {
            log.WriteLine("Maximum rounds reached! The battle ends in a tie");

            u1.PlayedGames++;
            u2.PlayedGames++;
            _userRepository.Update(u1);
            _userRepository.Update(u2);

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
            _userRepository.Update(winner);
            _userRepository.Update(loser);

            return new BattleResult(winner.ID, loser.ID, log.ToString(), false);
        }

        private void PrintDeck(List<CardInstance> cards, CharStream log)
        {
            log.WriteLine("Deck:");
            for (int i = 0; i < cards.Count; i++)
            {
                log.WriteLine($"Card {i}:");
                log.WriteLine($"{cards[i].Name}");
                log.WriteLine($"{cards[i].CardType}");
                log.WriteLine($"{cards[i].Damage} Damage");
                log.WriteLine($"{cards[i].Element}");
                log.WriteLine($"{cards[i].Race}");
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
        public bool LoseDueToSpecialty(Race attacker, Race defender, CharStream log)
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
            if (defender.Race == Race.Kraken && attacker.CardType == CardType.Spell)
            {
                log.WriteLine("The kraken is immune against spells");
                return true;
            }
            // Attacker loses due to the knight drowning in water
            else if (attacker.Race == Race.Knight && defender.CardType == CardType.Spell &&
                defender.Element == Element.Water)
            {
                log.WriteLine("The armor of the knight is so heavy that he instantly drowns against the water spell");
                return true;
            }
            return false;
        }

        private CardInstance GetRandomCard(List<CardInstance> cards)
        {
            return cards[new Random().Next(0, cards.Count)];
        }
    }

    internal class BattleResult
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
