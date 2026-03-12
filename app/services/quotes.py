from datetime import date

QUOTES = [
    # Lord of the Rings
    {"quote": "All we have to decide is what to do with the time that is given us.", "source": "Gandalf — The Fellowship of the Ring"},
    {"quote": "Even the smallest person can change the course of the future.", "source": "Galadriel — The Fellowship of the Ring"},
    {"quote": "There is some good in this world, and it's worth fighting for.", "source": "Samwise Gamgee — The Two Towers"},
    {"quote": "Not all those who wander are not lost.", "source": "Bilbo Baggins — The Fellowship of the Ring"},
    {"quote": "Deeds will not be less valiant because they are unpraised.", "source": "Aragorn — The Return of the King"},
    {"quote": "It is useless to meet revenge with revenge: it will heal nothing.", "source": "Frodo — The Return of the King"},
    {"quote": "The world is indeed full of peril, and in it there are many dark places; but still there is much that is fair.", "source": "Haldir — The Fellowship of the Ring"},
    {"quote": "Faithless is he that says farewell when the road darkens.", "source": "Gimli — The Fellowship of the Ring"},
    {"quote": "I would rather share one lifetime with you than face all the ages of this world alone.", "source": "Arwen — The Fellowship of the Ring"},
    {"quote": "A wizard is never late. He arrives precisely when he means to.", "source": "Gandalf — The Fellowship of the Ring"},

    # Star Wars
    {"quote": "Do. Or do not. There is no try.", "source": "Yoda — The Empire Strikes Back"},
    {"quote": "The fear of loss is a path to the dark side.", "source": "Yoda — Revenge of the Sith"},
    {"quote": "In my experience there is no such thing as luck.", "source": "Obi-Wan Kenobi — A New Hope"},
    {"quote": "The ability to speak does not make you intelligent.", "source": "Qui-Gon Jinn — The Phantom Menace"},
    {"quote": "Your focus determines your reality.", "source": "Qui-Gon Jinn — The Phantom Menace"},
    {"quote": "Who's the more foolish, the fool or the fool who follows him?", "source": "Obi-Wan Kenobi — A New Hope"},
    {"quote": "The greatest teacher, failure is.", "source": "Yoda — The Last Jedi"},
    {"quote": "Never tell me the odds.", "source": "Han Solo — The Empire Strikes Back"},
    {"quote": "We are what they grow beyond. That is the true burden of all masters.", "source": "Yoda — The Last Jedi"},
    {"quote": "Hope is like the sun. If you only believe in it when you can see it, you'll never make it through the night.", "source": "Leia Organa — The Last Jedi"},

    # Clever quotes
    {"quote": "We suffer more often in imagination than in reality.", "source": "Seneca"},
    {"quote": "The obstacle is the way.", "source": "Marcus Aurelius — Meditations"},
    {"quote": "He who has a why to live can bear almost any how.", "source": "Friedrich Nietzsche"},
    {"quote": "The only true wisdom is in knowing you know nothing.", "source": "Socrates"},
    {"quote": "It is not that we have a short time to live, but that we waste a great deal of it.", "source": "Seneca — On the Shortness of Life"},
    {"quote": "Man is condemned to be free; because once thrown into the world, he is responsible for everything he does.", "source": "Jean-Paul Sartre"},
    {"quote": "In the middle of difficulty lies opportunity.", "source": "Albert Einstein"},
    {"quote": "The best time to plant a tree was 20 years ago. The second best time is now.", "source": "Chinese proverb"},
    {"quote": "Knowing is not enough, we must apply. Willing is not enough, we must do.", "source": "Bruce Lee"},
    {"quote": "What we know is a drop, what we don't know is an ocean.", "source": "Isaac Newton"},
    {"quote": "Waste no more time arguing about what a good man should be. Be one.", "source": "Marcus Aurelius — Meditations"},
    {"quote": "If you want to go fast, go alone. If you want to go far, go together.", "source": "African proverb"},
    {"quote": "A ship in harbor is safe, but that is not what ships are built for.", "source": "John A. Shedd"},
    {"quote": "The man who moves a mountain begins by carrying away small stones.", "source": "Confucius"},
    {"quote": "It is not the strongest of the species that survives, nor the most intelligent, but the one most responsive to change.", "source": "Charles Darwin"},
    {"quote": "Everything we hear is an opinion, not a fact. Everything we see is a perspective, not the truth.", "source": "Marcus Aurelius"},
    {"quote": "Think lightly of yourself and deeply of the world.", "source": "Miyamoto Musashi — The Book of Five Rings"},
    {"quote": "The mind is everything. What you think you become.", "source": "Buddha"},
    {"quote": "To live is the rarest thing in the world. Most people exist, that is all.", "source": "Oscar Wilde"},
    {"quote": "I am not what happened to me. I am what I choose to become.", "source": "Carl Jung"},
]


def fetch():
    today = date.today()
    idx = today.toordinal() % len(QUOTES)
    q = QUOTES[idx]
    return {
        "quote": q["quote"],
        "source": q["source"],
    }
