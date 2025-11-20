namespace Server.Database {
    public static class WordleDbInitializer {
        public static void Seed(WordleDbContext dbContext) {
            dbContext.Database.EnsureCreated();

            if (!dbContext.WordCategories.Any() || !dbContext.Words.Any()) {
                var defaultCategories = InitCategories(dbContext);
                var category = defaultCategories.ToDictionary(c => c.Name, c => c);
                InitWords(dbContext, category);
            }

            if (!dbContext.Achievements.Any()) {
                InitAchievements(dbContext);
            }

            if (!dbContext.WordsValidations.Any()) {
                InitWordsValidation(dbContext);
            }
        }

        private static void InitAchievements(WordleDbContext dbContext) {
            var defaultAchievements = new List<Models.Achievement>
            {
                new () {
                    Name = "First Solve!",
                    Description = "Solve a Wordle game.",
                },
                new() {
                    Name = "First Try!",
                    Description = "Solve a Wordle on your first attempt.",
                },
                new () {
                    Name = "Persistent Player",
                    Description = "Play Wordle for 7 consecutive days.",
                },
                new () {
                    Name = "Category Master",
                    Description = "Solve 5 Wordles from each category.",
                },
                new () {
                    Name = "No hints",
                    Description = "Solve wordle game without hints.",
                },
                new () {
                    Name = "Hard mode!",
                    Description = "Solve wordle hard mode game.",
                },
            };
            dbContext.Achievements.AddRange(defaultAchievements);
            dbContext.SaveChanges();
        }

        private static void InitWords(WordleDbContext dbContext, Dictionary<string, Models.WordCategory> category) {
            var defaultWords = new List<Models.Word>
            {
                // nature
                new() { Text = "RIVER", Category = category["Nature"] },
                new() { Text = "BEACH", Category = category["Nature"]  },
                new() { Text = "SHORE", Category = category["Nature"] },
                new() { Text = "FOGGY", Category = category["Nature"] },
                new() { Text = "WOODS", Category = category["Nature"] },
                new() { Text = "PLANT", Category = category["Nature"] },
                new() { Text = "EARTH", Category = category["Nature"] },
                new() { Text = "ATOLL", Category = category["Nature"] },
                new() { Text = "CAVES", Category = category["Nature"] },
                new() { Text = "SWAMP", Category = category["Nature"] },
                new() { Text = "OASIS", Category = category["Nature"] },
                new() { Text = "DUNES", Category = category["Nature"] },
                new() { Text = "DELTA", Category = category["Nature"] },

                // Adjectives
                new() { Text = "HEAVY", Category = category["Adjectives"] },
                new() { Text = "WEIRD", Category = category["Adjectives"] },
                new() { Text = "LOCAL", Category = category["Adjectives"] },
                new() { Text = "TIDAL", Category = category["Adjectives"] },
                new() { Text = "WORSE", Category = category["Adjectives"] },
                new() { Text = "READY", Category = category["Adjectives"] },
                new() { Text = "FRESH", Category = category["Adjectives"] },
                new() { Text = "GRAND", Category = category["Adjectives"] },
                new() { Text = "JOLLY", Category = category["Adjectives"] },
                new() { Text = "SWEET", Category = category["Adjectives"] },

                // People
                new() { Text = "NURSE", Category = category["People"] },
                new() { Text = "PILOT", Category = category["People"] },
                new() { Text = "JUDGE", Category = category["People"] },
                new() { Text = "YOUTH", Category = category["People"] },
                new() { Text = "CHILD", Category = category["People"] },
                new() { Text = "WIVES", Category = category["People"] },
                new() { Text = "CROWD", Category = category["People"] },
                new() { Text = "LEADS", Category = category["People"] },
                new() { Text = "SQUAD", Category = category["People"] },

                // Geography
                new() { Text = "EGYPT", Category = category["Geography"] },
                new() { Text = "MALTA", Category = category["Geography"] },
                new() { Text = "BENIN", Category = category["Geography"] },
                new() { Text = "GABON", Category = category["Geography"] },
                new() { Text = "CHILE", Category = category["Geography"] },
                new() { Text = "CHINA", Category = category["Geography"] },
                new() { Text = "GHANA", Category = category["Geography"] },
                new() { Text = "ITALY", Category = category["Geography"] },
                new() { Text = "JAPAN", Category = category["Geography"] },
                new() { Text = "INDIA", Category = category["Geography"] },
                
                // Food & drink
                new() { Text = "APPLE", Category = category["Food & Drink"] },
                new() { Text = "BACON", Category = category["Food & Drink"] },
                new() { Text = "BEANS", Category = category["Food & Drink"] },
                new() { Text = "BAGEL", Category = category["Food & Drink"] },
                new() { Text = "BERRY", Category = category["Food & Drink"] },
                new() { Text = "COCOA", Category = category["Food & Drink"] },
                new() { Text = "FRIES", Category = category["Food & Drink"] },
                new() { Text = "DOUGH", Category = category["Food & Drink"] },
                new() { Text = "CREAM", Category = category["Food & Drink"] },
                new() { Text = "BREAD", Category = category["Food & Drink"] },

            };
            dbContext.Words.AddRange(defaultWords);
            dbContext.SaveChanges();
        }

        private static List<Models.WordCategory> InitCategories(WordleDbContext dbContext) {
            var defaultCategories = new List<Models.WordCategory>
            {
                new () {
                    Name = "Nature",
                    Description = "Covers animals, plants, weather, and landscapes."
                },
                new () {
                    Name = "Adjectives",
                    Description = "Describes people, things, or concepts, yielding descriptive and common words."
                },
                new () {
                    Name = "People",
                    Description = "Focuses on titles, relationships, or occupations."
                },
                new () {
                    Name = "Geography",
                    Description = "Focuses on places, movement, and features found around the world. Offers many unique letter combinations."
                },
                new () {
                    Name = "Food & Drink",
                    Description = "Includes common food items, beverages, and culinary terms."
                },
            };
            dbContext.WordCategories.AddRange(defaultCategories);
            dbContext.SaveChanges();
            return defaultCategories;
        }

        private static void InitWordsValidation(WordleDbContext context) {
            var filePath = "Database/WORDS.txt";
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException("Words validation file not found.", filePath);
            }

            var words = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => new Models.WordsValidation { Text = line.Trim().ToUpper() })
                .ToList();
            context.WordsValidations.AddRange(words);
            context.SaveChanges();
        }
    }
}
