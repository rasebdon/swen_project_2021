using System.Net.Http.Headers;
using MTCG.Models;
using Newtonsoft.Json;

void PrintSuccess(object s)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(s);
    Console.ForegroundColor = ConsoleColor.White;
}

void PrintFail(object s)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(s);
    Console.ForegroundColor = ConsoleColor.White;
}

// Declare vars
Console.ForegroundColor = ConsoleColor.White;
var client = new HttpClient();
int errors = 0;

User? user1;
string? user1Token;

User? user2;
string? user2Token;

Card? deathWing;
Card? waterMage;
Card? fireMage;
Card? goblin;
Card? waterbolt;
Card? fireblast;

Console.WriteLine("MTCG Integration Test!");

try
{
    Console.WriteLine("\n[TestSection] User Tests");

    Console.WriteLine("\n[Test] Register User1");
    var request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/users"),
        Content = new StringContent("{\n\t\"username\": \"testUser1\",\n\t\"password\": \"1234\"\n}")
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        user1 = JsonConvert.DeserializeObject<User>(body);

        Console.WriteLine($"{response.StatusCode}");

        if (user1 != null)
        {
            PrintSuccess($"User1.ID: {user1.ID}");
        }
        else
        {
            throw new ArgumentNullException(nameof(user1));
        }
    }

    Console.WriteLine("\n[Test] Register User1 again (should fail)");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/users"),
        Content = new StringContent("{\n\t\"username\": \"testUser1\",\n\t\"password\": \"1234\"\n}")
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        var body = await response.Content.ReadAsStringAsync();
        
        if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            PrintSuccess($"{response.StatusCode}");
        }
        else
        {
            errors++;
            PrintFail($"Wrong http status code: {response.StatusCode}");
        }
    }

    Console.WriteLine("\n[Test] Login User1 wrong credentials (should fail)");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/session"),
        Headers =
    {
        { "Authorization", "Basic Og==" },
    },
        Content = new StringContent("{\n\t\"username\": \"testUser1\",\n\t\"password\": \"3214\"\n}")
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        var body = await response.Content.ReadAsStringAsync();

        if(response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
            response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            PrintSuccess("Access denied!");
        }
        else
        {
            errors++;
            PrintFail($"Wrong http status code: {response.StatusCode}");
        }
    }

    Console.WriteLine("\n[Test] Login User1 right credentials");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/session"),
        Headers =
    {
        { "Authorization", "Basic Og==" },
    },
        Content = new StringContent("{\n\t\"username\": \"testUser1\",\n\t\"password\": \"1234\"\n}")
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        user1Token = JsonConvert.DeserializeObject<Dictionary<string, string>>(body)?.FirstOrDefault().Value;

        if(user1Token != null)
        {
            PrintSuccess($"SessionToken: {user1Token}");
        }
        else
        {
            errors++;
            throw new ArgumentNullException(nameof(user1Token));
        }
    }

    Console.WriteLine("\n[Test] Update User1");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Put,
        RequestUri = new Uri("http://localhost:10001/users/testUser1"),
        Headers =
    {
        { "Authorization", "Bearer testUser1-mtcgToken" },
    },
        Content = new StringContent("{\n\t\"Username\": \"testUser1\",\n\t\"Bio\": \"Updated Bio\",\n\t\"Image\": \":-D\"\n}")
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        user1 = JsonConvert.DeserializeObject<User>(body);

        Console.WriteLine($"{response.StatusCode}");

        if (user1 != null)
        {
            PrintSuccess($"Update successful!");
        }
        else
        {
            throw new ArgumentNullException(nameof(user1));
        }
    }

    Console.WriteLine("\n[Test] Get User1 information via token");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri("http://localhost:10001/users"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        if(response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            PrintSuccess(response.StatusCode);
        }
        else
        {
            errors++;
            PrintFail($"Wrong http status code: {response.StatusCode}");
        }
    }

    Console.WriteLine("\n[Test] Register User2");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/users"),
        Content = new StringContent("{\n\t\"username\": \"testUser2\",\n\t\"password\": \"1234\"\n}")
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        user2 = JsonConvert.DeserializeObject<User>(body);

        Console.WriteLine($"{response.StatusCode}");

        if (user2 != null)
        {
            PrintSuccess($"User2.ID: {user2.ID}");
        }
        else
        {
            throw new ArgumentNullException(nameof(user2));
        }
    }

    Console.WriteLine("\n[Test] Login User2");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/session"),
        Headers =
    {
        { "Authorization", "Basic Og==" },
    },
        Content = new StringContent("{\n\t\"username\": \"testUser2\",\n\t\"password\": \"1234\"\n}")
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        user2Token = JsonConvert.DeserializeObject<Dictionary<string, string>>(body)?.FirstOrDefault().Value;

        if (user2Token != null)
        {
            PrintSuccess($"SessionToken: {user2Token}");
        }
        else
        {
            errors++;
            throw new ArgumentNullException(nameof(user2Token));
        }
    }

    Console.WriteLine("\n[TestSection] Cards Tests");

    Card? CreateCard(Card? card)
    {
        Console.WriteLine($"\n[Test] Create card {card?.Name}");

        request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("http://localhost:10001/cards"),
            Headers =
    {
        { "Authorization", "Bearer admin-mtcgToken" },
    },
            Content = new StringContent(JsonConvert.SerializeObject(card))
            {
                Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
            }
        };
        using var response = client?.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();
        var body = response.Content.ReadAsStringAsync().Result;

        card = JsonConvert.DeserializeObject<Card>(body);

        if(card == null || card.ID == Guid.Empty)
            throw new ArgumentNullException(nameof(card));

        PrintSuccess($"Insert success {card?.Name}");

        return card;
    }

    deathWing = new(Guid.Empty, "Deathwing", "All shall burn!", 8, CardType.Monster, Element.Fire, Race.Draconid, Rarity.Legendary);
    deathWing = CreateCard(deathWing);

    waterMage = new(Guid.Empty, "Water Mage", "A normal trained water mage!", 3, CardType.Monster, Element.Water, Race.Mage, Rarity.Common);
    waterMage = CreateCard(waterMage);

    fireMage = new(Guid.Empty, "Fire Mage", "A normal trained fire mage!", 3, CardType.Monster, Element.Fire, Race.Mage, Rarity.Common);
    fireMage = CreateCard(fireMage);

    goblin = new(Guid.Empty, "Small Goblin", "A weak goblin, ready to fight!", 3, CardType.Monster, Element.Normal, Race.Goblin, Rarity.Common);
    goblin = CreateCard(goblin);

    waterbolt = new(Guid.Empty, "Waterbolt", "A strong water fountain heading for the enemy!", 4, CardType.Spell, Element.Water, Race.None, Rarity.Rare);
    waterbolt = CreateCard(waterbolt);

    fireblast = new(Guid.Empty, "Fireblast", "This is a strong fire spell", 5, CardType.Spell, Element.Fire, Race.None, Rarity.Epic);
    fireblast = CreateCard(fireblast);

    Console.WriteLine("\n[TestSection] Packages Tests");

    Console.WriteLine("\n[Test] Create Package 1");
    Package? package1 = new(Guid.Empty, "Package Number 1", "This is a new package!", 5, new List<Card>()
    {
        deathWing,
        fireMage,
        fireblast,
    });
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/packages"),
        Headers =
    {
        { "Authorization", "Bearer admin-mtcgToken" },
    },
        Content = new StringContent(JsonConvert.SerializeObject(package1))
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("text/plain")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        
        package1 = JsonConvert.DeserializeObject<Package>(body);

        if(package1 == null)
            throw new ArgumentNullException(nameof(package1));

        PrintSuccess("Package1 insert success");
    }

    Console.WriteLine("\n[Test] Create Package 2");
    Package? package2 = new(Guid.Empty, "Package Number 2", "This is another new package!", 5, new List<Card>()
    {
        waterMage,
        goblin,
        waterbolt,
    });
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/packages"),
        Headers =
    {
        { "Authorization", "Bearer admin-mtcgToken" },
    },
        Content = new StringContent(JsonConvert.SerializeObject(package2))
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("text/plain")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        package2 = JsonConvert.DeserializeObject<Package>(body);

        if (package2 == null)
            throw new ArgumentNullException(nameof(package2));

        PrintSuccess("Package2 insert success");
    }

    Console.WriteLine("\n[Test] User1 buys package1");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri($"http://localhost:10001/transactions/packages/{package1.ID}"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess($"Package1 bought");
    }

    Console.WriteLine("\n[Test] User1 buys package2");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri($"http://localhost:10001/transactions/packages/{package2.ID}"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess($"Package2 bought");
    }

    Console.WriteLine("\n[Test] User2 buys package1");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri($"http://localhost:10001/transactions/packages/{package1.ID}"),
        Headers =
    {
        { "Authorization", $"Bearer {user2Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess($"Package1 bought");
    }

    Console.WriteLine("\n[Test] User2 buys package2");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri($"http://localhost:10001/transactions/packages/{package2.ID}"),
        Headers =
    {
        { "Authorization", $"Bearer {user2Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess($"Package2 bought");
    }

    Console.WriteLine("\n[TestSection] Stack tests");

    Console.WriteLine("\n[Test] Get user1 stack");
    List<CardInstance>? user1Cards = new();
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri("http://localhost:10001/cards"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        user1Cards = JsonConvert.DeserializeObject<List<CardInstance>>(body);

        PrintSuccess($"User1 has {user1Cards?.Count} cards");
    }

    Console.WriteLine("\n[Test] Get user2 stack");
    List<CardInstance>? user2Cards = new();
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri("http://localhost:10001/cards"),
        Headers =
    {
        { "Authorization", $"Bearer {user2Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        user2Cards = JsonConvert.DeserializeObject<List<CardInstance>>(body);

        PrintSuccess($"User2 has {user2Cards?.Count} cards");
    }

    Console.WriteLine("\n[TestSection] Deck tests");

    Console.WriteLine("\n[Test] Create deck user1");
    Deck? user1Deck = new(Guid.Empty, "My first deck!", user1.ID, true, new List<CardInstance>()
    {
        user1Cards[0],
        user1Cards[1],
        user1Cards[2],
        user1Cards[3]
    }.ToArray());
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/decks"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
        Content = new StringContent(JsonConvert.SerializeObject(user1Deck))
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        
        PrintSuccess("User1 deck created");
    }

    Console.WriteLine("\n[Test] Get deck user1");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri("http://localhost:10001/decks"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess("Get decks user1 successful");
    }

    Console.WriteLine("\n[Test] Create deck user2");
    Deck? user2Deck = new(Guid.Empty, "My first deck!!!", user2.ID, true, new List<CardInstance>()
    {
        user2Cards[0],
        user2Cards[1],
        user2Cards[2],
        user2Cards[3]
    }.ToArray());
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/decks"),
        Headers =
    {
        { "Authorization", $"Bearer {user2Token}" },
    },
        Content = new StringContent(JsonConvert.SerializeObject(user2Deck))
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess("User2 deck created");
    }

    Console.WriteLine("\n[Test] Get deck user2");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri("http://localhost:10001/decks"),
        Headers =
    {
        { "Authorization", $"Bearer {user2Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess("Get decks user2 successful");
    }

    Console.WriteLine("\n[TestSection] Battle tests");

    Console.WriteLine("\n[Test] Start search battle user1");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/battles"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
    };
    var responseSB1 = client.SendAsync(request);

    Console.WriteLine("\n[Test] Start search battle user2");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/battles"),
        Headers =
    {
        { "Authorization", $"Bearer {user2Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        var response2 = responseSB1.Result;

        response.EnsureSuccessStatusCode();
        response2.EnsureSuccessStatusCode();

        PrintSuccess(response.Content.ReadAsStringAsync().Result);
    }

    Thread.Sleep(2000);

    Console.WriteLine("\n[TestSection] Stat tests");

    Console.WriteLine("\n[Test] Get user1 stats");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri("http://localhost:10001/stats"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess(body);
    }

    Console.WriteLine("\n[Test] Get user2 stats");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri("http://localhost:10001/stats"),
        Headers =
    {
        { "Authorization", $"Bearer {user2Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess(body);
    }

    Console.WriteLine("\n[TestSection] Scoreboard tests");

    Console.WriteLine("\n[Test] Get scoreboard");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri("http://localhost:10001/score"),
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        
        PrintSuccess(body);
    }

    Console.WriteLine("\n[TestSection] Trades tests");

    Console.WriteLine("\n[Test] Create offer user1");
    TradeOffer? offer;
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("http://localhost:10001/trades"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
        Content = new StringContent(
            $"{{\n\t\"OfferedCardId\": \"{user1Cards[4].ID}\",\n\t\"WantedCardId\": \"{user2Cards[4].CardID}\"\n}}")
        {
            Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
        }
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        offer = JsonConvert.DeserializeObject<TradeOffer>(body);

        if(offer == null)
        {
            errors++;
            PrintFail("Offer could not be parsed from response body!");
            throw new ArgumentNullException(nameof(offer));
        }
        else
        {
            PrintSuccess("Offer created successfully");
        }
    }

    Console.WriteLine("\n[Test] List all offers");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri("http://localhost:10001/trades"),
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess(body);
    }

    Console.WriteLine("\n[Test] Accept offer as user1 (should fail)");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri($"http://localhost:10001/trades/{offer.ID}"),
        Headers =
    {
        { "Authorization", $"Bearer {user1Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        var body = await response.Content.ReadAsStringAsync();
        
        if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            PrintSuccess("Returned bad request!");
        }
        else
        {
            errors++;
            PrintFail($"Did not return the right status code: {response.StatusCode}");
        }
    }

    Console.WriteLine("\n[Test] Accept offer as user2");
    request = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri($"http://localhost:10001/trades/{offer.ID}"),
        Headers =
    {
        { "Authorization", $"Bearer {user2Token}" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        PrintSuccess(body);
    }

}
catch (Exception ex)
{
    errors++;
    PrintFail(ex.ToString());
}

Console.WriteLine($"[Conclusion] Test finished with {errors} errors!");
Console.ReadLine();