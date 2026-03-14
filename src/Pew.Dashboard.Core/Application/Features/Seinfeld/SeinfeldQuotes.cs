namespace Pew.Dashboard.Application.Features.Seinfeld;

public sealed record SeinfeldQuote(string Quote, string Character, string Episode);

public static class SeinfeldQuotes
{
    public static readonly SeinfeldQuote[] Quotes =
    [
        new("No soup for you!", "The Soup Nazi", "The Soup Nazi (S7E6)"),
        new("These pretzels are making me thirsty.", "Kramer", "The Alternate Side (S3E11)"),
        new("I was in the pool! I was in the pool!", "George", "The Hamptons (S5E21)"),
        new("Serenity now!", "Frank Costanza", "The Serenity Now (S9E3)"),
        new("Not that there's anything wrong with that.", "Jerry", "The Outing (S4E17)"),
        new("Yada yada yada.", "Elaine", "The Yada Yada (S8E19)"),
        new("I don't wanna be a pirate!", "Jerry", "The Puffy Shirt (S5E2)"),
        new("The sea was angry that day, my friends.", "George", "The Marine Biologist (S5E14)"),
        new("It's not a lie if you believe it.", "George", "The Beard (S6E16)"),
        new("I'm out!", "Kramer", "The Contest (S4E11)"),
        new("Giddyup!", "Kramer", "Various episodes"),
        new("You're so good-looking.", "Jerry", "The Good Samaritan (S3E20)"),
        new("Hoochie mama!", "Frank Costanza", "The Serenity Now (S9E3)"),
        new("A George divided against itself cannot stand!", "George", "The Abstinence (S8E9)"),
        new("I'm like a commercial jingle. First it's a little irritating, then you hear it a few times, you hum it in the shower.", "George", "The Tape (S3E8)"),
        new("You know, I always wanted to pretend I was an architect.", "George", "The Stake Out (S1E2)"),
        new("The jerk store called, they're running out of you!", "George", "The Comeback (S8E13)"),
        new("I'm disturbed, I'm depressed, I'm inadequate. I've got it all!", "George", "The Old Man (S4E18)"),
        new("You want to talk about a waste of time? You ever hear of this thing, the Bermuda Triangle?", "Kramer", "The Butter Shave (S9E1)"),
        new("Maybe the dingo ate your baby.", "Elaine", "The Stranded (S3E10)"),
        new("I have hand!", "George", "The Pez Dispenser (S3E14)"),
        new("Was that wrong? Should I have not done that?", "George", "The Red Dot (S3E12)"),
        new("You're killing independent George!", "George", "The Pool Guy (S7E8)"),
        new("I mentioned the bisque.", "Elaine", "The Soup Nazi (S7E6)"),
        new("That's a shame.", "Jerry", "Various episodes"),
        new("Hello, Newman.", "Jerry", "Various episodes"),
        new("Am I crazy, or is that a lot of gum?", "Elaine", "The Gum (S7E10)"),
        new("I'm Victoria's Secret. I'll never tell.", "Kramer", "The Caddy (S7E12)"),
        new("People don't turn down money. It's what separates us from the animals.", "Jerry", "The Calzone (S7E20)"),
        new("We're living in a society!", "George", "The Chinese Restaurant (S2E11)"),
        new("I think you're in love with the K-Man!", "Kramer", "The Bizarro Jerry (S8E3)"),
    ];
}
