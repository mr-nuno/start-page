namespace Pew.Dashboard.Application.Features.Quote;

public sealed record QuoteDefinition(string Quote, string Source);

public static class QuoteLibrary
{
    public static readonly QuoteDefinition[] Quotes =
    [
        // Lord of the Rings
        new("All we have to decide is what to do with the time that is given us.", "Gandalf \u2014 The Fellowship of the Ring"),
        new("Even the smallest person can change the course of the future.", "Galadriel \u2014 The Fellowship of the Ring"),
        new("There is some good in this world, and it's worth fighting for.", "Samwise Gamgee \u2014 The Two Towers"),
        new("Not all those who wander are not lost.", "Bilbo Baggins \u2014 The Fellowship of the Ring"),
        new("Deeds will not be less valiant because they are unpraised.", "Aragorn \u2014 The Return of the King"),
        new("It is useless to meet revenge with revenge: it will heal nothing.", "Frodo \u2014 The Return of the King"),
        new("The world is indeed full of peril, and in it there are many dark places; but still there is much that is fair.", "Haldir \u2014 The Fellowship of the Ring"),
        new("Faithless is he that says farewell when the road darkens.", "Gimli \u2014 The Fellowship of the Ring"),
        new("I would rather share one lifetime with you than face all the ages of this world alone.", "Arwen \u2014 The Fellowship of the Ring"),
        new("A wizard is never late. He arrives precisely when he means to.", "Gandalf \u2014 The Fellowship of the Ring"),

        // Star Wars
        new("Do. Or do not. There is no try.", "Yoda \u2014 The Empire Strikes Back"),
        new("The fear of loss is a path to the dark side.", "Yoda \u2014 Revenge of the Sith"),
        new("In my experience there is no such thing as luck.", "Obi-Wan Kenobi \u2014 A New Hope"),
        new("The ability to speak does not make you intelligent.", "Qui-Gon Jinn \u2014 The Phantom Menace"),
        new("Your focus determines your reality.", "Qui-Gon Jinn \u2014 The Phantom Menace"),
        new("Who's the more foolish, the fool or the fool who follows him?", "Obi-Wan Kenobi \u2014 A New Hope"),
        new("The greatest teacher, failure is.", "Yoda \u2014 The Last Jedi"),
        new("Never tell me the odds.", "Han Solo \u2014 The Empire Strikes Back"),
        new("We are what they grow beyond. That is the true burden of all masters.", "Yoda \u2014 The Last Jedi"),
        new("Hope is like the sun. If you only believe in it when you can see it, you'll never make it through the night.", "Leia Organa \u2014 The Last Jedi"),

        // Philosophy
        new("We suffer more often in imagination than in reality.", "Seneca"),
        new("The obstacle is the way.", "Marcus Aurelius \u2014 Meditations"),
        new("He who has a why to live can bear almost any how.", "Friedrich Nietzsche"),
        new("The only true wisdom is in knowing you know nothing.", "Socrates"),
        new("It is not that we have a short time to live, but that we waste a great deal of it.", "Seneca \u2014 On the Shortness of Life"),
        new("Man is condemned to be free; because once thrown into the world, he is responsible for everything he does.", "Jean-Paul Sartre"),
        new("In the middle of difficulty lies opportunity.", "Albert Einstein"),
        new("The best time to plant a tree was 20 years ago. The second best time is now.", "Chinese proverb"),
        new("Knowing is not enough, we must apply. Willing is not enough, we must do.", "Bruce Lee"),
        new("What we know is a drop, what we don't know is an ocean.", "Isaac Newton"),
        new("Waste no more time arguing about what a good man should be. Be one.", "Marcus Aurelius \u2014 Meditations"),
        new("If you want to go fast, go alone. If you want to go far, go together.", "African proverb"),
        new("A ship in harbor is safe, but that is not what ships are built for.", "John A. Shedd"),
        new("The man who moves a mountain begins by carrying away small stones.", "Confucius"),
        new("It is not the strongest of the species that survives, nor the most intelligent, but the one most responsive to change.", "Charles Darwin"),
        new("Everything we hear is an opinion, not a fact. Everything we see is a perspective, not the truth.", "Marcus Aurelius"),
        new("Think lightly of yourself and deeply of the world.", "Miyamoto Musashi \u2014 The Book of Five Rings"),
        new("The mind is everything. What you think you become.", "Buddha"),
        new("To live is the rarest thing in the world. Most people exist, that is all.", "Oscar Wilde"),
        new("I am not what happened to me. I am what I choose to become.", "Carl Jung"),
    ];
}
