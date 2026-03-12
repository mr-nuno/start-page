from datetime import date

# Memorable quotes from the Seinfeld TV show with character and episode reference
QUOTES = [
    {"quote": "No soup for you!", "character": "The Soup Nazi", "episode": "The Soup Nazi (S7E6)"},
    {"quote": "These pretzels are making me thirsty.", "character": "Kramer", "episode": "The Alternate Side (S3E11)"},
    {"quote": "I was in the pool! I was in the pool!", "character": "George", "episode": "The Hamptons (S5E21)"},
    {"quote": "Serenity now!", "character": "Frank Costanza", "episode": "The Serenity Now (S9E3)"},
    {"quote": "Not that there's anything wrong with that.", "character": "Jerry", "episode": "The Outing (S4E17)"},
    {"quote": "Yada yada yada.", "character": "Elaine", "episode": "The Yada Yada (S8E19)"},
    {"quote": "I don't wanna be a pirate!", "character": "Jerry", "episode": "The Puffy Shirt (S5E2)"},
    {"quote": "The sea was angry that day, my friends.", "character": "George", "episode": "The Marine Biologist (S5E14)"},
    {"quote": "It's not a lie if you believe it.", "character": "George", "episode": "The Beard (S6E16)"},
    {"quote": "I'm out!", "character": "Kramer", "episode": "The Contest (S4E11)"},
    {"quote": "Giddyup!", "character": "Kramer", "episode": "Various episodes"},
    {"quote": "You're so good-looking.", "character": "Jerry", "episode": "The Good Samaritan (S3E20)"},
    {"quote": "Hoochie mama!", "character": "Frank Costanza", "episode": "The Serenity Now (S9E3)"},
    {"quote": "A George divided against itself cannot stand!", "character": "George", "episode": "The Abstinence (S8E9)"},
    {"quote": "I'm like a commercial jingle. First it's a little irritating, then you hear it a few times, you hum it in the shower.", "character": "George", "episode": "The Tape (S3E8)"},
    {"quote": "You know, I always wanted to pretend I was an architect.", "character": "George", "episode": "The Stake Out (S1E2)"},
    {"quote": "The jerk store called, they're running out of you!", "character": "George", "episode": "The Comeback (S8E13)"},
    {"quote": "I'm disturbed, I'm depressed, I'm inadequate. I've got it all!", "character": "George", "episode": "The Old Man (S4E18)"},
    {"quote": "You want to talk about a waste of time? You ever hear of this thing, the Bermuda Triangle?", "character": "Kramer", "episode": "The Butter Shave (S9E1)"},
    {"quote": "Maybe the dingo ate your baby.", "character": "Elaine", "episode": "The Stranded (S3E10)"},
    {"quote": "I have hand!", "character": "George", "episode": "The Pez Dispenser (S3E14)"},
    {"quote": "Was that wrong? Should I have not done that?", "character": "George", "episode": "The Red Dot (S3E12)"},
    {"quote": "You're killing independent George!", "character": "George", "episode": "The Pool Guy (S7E8)"},
    {"quote": "I mentioned the bisque.", "character": "Elaine", "episode": "The Soup Nazi (S7E6)"},
    {"quote": "That's a shame.", "character": "Jerry", "episode": "Various episodes"},
    {"quote": "Hello, Newman.", "character": "Jerry", "episode": "Various episodes"},
    {"quote": "Am I crazy, or is that a lot of gum?", "character": "Elaine", "episode": "The Gum (S7E10)"},
    {"quote": "I'm Victoria's Secret. I'll never tell.", "character": "Kramer", "episode": "The Caddy (S7E12)"},
    {"quote": "People don't turn down money. It's what separates us from the animals.", "character": "Jerry", "episode": "The Calzone (S7E20)"},
    {"quote": "We're living in a society!", "character": "George", "episode": "The Chinese Restaurant (S2E11)"},
    {"quote": "I think you're in love with the K-Man!", "character": "Kramer", "episode": "The Bizarro Jerry (S8E3)"},
]


def fetch():
    today = date.today()
    idx = today.toordinal() % len(QUOTES)
    q = QUOTES[idx]
    return {
        "quote": q["quote"],
        "character": q["character"],
        "episode": q["episode"],
    }
