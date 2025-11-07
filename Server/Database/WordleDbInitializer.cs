namespace Server.Database {
    public static class WordleDbInitializer {
        public static void Seed(WordleDbContext dbContext) {
            dbContext.Database.EnsureCreated();

            if (!dbContext.WordCategories.Any() || !dbContext.Words.Any()) {
                var defaultCategories = new List<Models.WordCategory>
                {
                    new Models.WordCategory
                    {
                        Name = "Nature",
                        Description = "Covers animals, plants, weather, and landscapes."
                    },
                    new Models.WordCategory
                    {
                        Name = "Adjectives",
                        Description = "Describes people, things, or concepts, yielding descriptive and common words."
                    },
                    new Models.WordCategory
                    {
                        Name = "People",
                        Description = "Focuses on titles, relationships, or occupations."
                    },
                    new Models.WordCategory
                    {
                        Name = "Geography",
                        Description = "Focuses on places, movement, and features found around the world. Offers many unique letter combinations."
                    },
                    new Models.WordCategory
                    {
                        Name = "Food & Drink",
                        Description = "Includes common food items, beverages, and culinary terms."
                    },
                };
                dbContext.WordCategories.AddRange(defaultCategories);
                dbContext.SaveChanges();

                var category = defaultCategories.ToDictionary(c => c.Name, c => c);


                var defaultWords = new List<Models.Word>
                {
                    // nature
                    new Models.Word { Text = "RIVER", Category = category["Nature"] },
                    new Models.Word { Text = "BEACH", Category = category["Nature"]  },
                    new Models.Word { Text = "SHORE", Category = category["Nature"] },
                    new Models.Word { Text = "FOGGY", Category = category["Nature"] },
                    new Models.Word { Text = "WOODS", Category = category["Nature"] },
                    new Models.Word { Text = "PLANT", Category = category["Nature"] },
                    new Models.Word { Text = "EARTH", Category = category["Nature"] },
                    new Models.Word { Text = "BLOSS", Category = category["Nature"] },
                    new Models.Word { Text = "CAVEA", Category = category["Nature"] },
                    new Models.Word { Text = "SWAMP", Category = category["Nature"] },

                    // Adjectives
                    new Models.Word { Text = "HEAVY", Category = category["Adjectives"] },
                    new Models.Word { Text = "WEIRD", Category = category["Adjectives"] },
                    new Models.Word { Text = "SUREL", Category = category["Adjectives"] },
                    new Models.Word { Text = "TIDAL", Category = category["Adjectives"] },
                    new Models.Word { Text = "WORSE", Category = category["Adjectives"] },
                    new Models.Word { Text = "READY", Category = category["Adjectives"] },
                    new Models.Word { Text = "FRESH", Category = category["Adjectives"] },
                    new Models.Word { Text = "GRAND", Category = category["Adjectives"] },
                    new Models.Word { Text = "JOLLY", Category = category["Adjectives"] },
                    new Models.Word { Text = "SWEET", Category = category["Adjectives"] },

                    // People
                    new Models.Word { Text = "NURSE", Category = category["People"] },
                    new Models.Word { Text = "PILOT", Category = category["People"] },
                    new Models.Word { Text = "JUDGE", Category = category["People"] },
                    new Models.Word { Text = "YOUTH", Category = category["People"] },
                    new Models.Word { Text = "CHILD", Category = category["People"] },
                    new Models.Word { Text = "WIVES", Category = category["People"] },
                    new Models.Word { Text = "CROWD", Category = category["People"] },
                    new Models.Word { Text = "LEADS", Category = category["People"] },
                    new Models.Word { Text = "SQUAD", Category = category["People"] },

                    // Geography
                    new Models.Word { Text = "EGYPT", Category = category["Geography"] },
                    new Models.Word { Text = "MALTA", Category = category["Geography"] },
                    new Models.Word { Text = "BENIN", Category = category["Geography"] },
                    new Models.Word { Text = "GABON", Category = category["Geography"] },
                    new Models.Word { Text = "CHILE", Category = category["Geography"] },
                    new Models.Word { Text = "CHINA", Category = category["Geography"] },
                    new Models.Word { Text = "GHANA", Category = category["Geography"] },
                    new Models.Word { Text = "ITALY", Category = category["Geography"] },
                    new Models.Word { Text = "JAPAN", Category = category["Geography"] },
                    new Models.Word { Text = "IDNIA", Category = category["Geography"] },
                
                    // Food & drink
                    new Models.Word { Text = "APPLE", Category = category["Food & Drink"] },
                    new Models.Word { Text = "BACON", Category = category["Food & Drink"] },
                    new Models.Word { Text = "BEANS", Category = category["Food & Drink"] },
                    new Models.Word { Text = "BAGEL", Category = category["Food & Drink"] },
                    new Models.Word { Text = "BERRY", Category = category["Food & Drink"] },
                    new Models.Word { Text = "COCOA", Category = category["Food & Drink"] },
                    new Models.Word { Text = "FRIES", Category = category["Food & Drink"] },
                    new Models.Word { Text = "DOUGH", Category = category["Food & Drink"] },
                    new Models.Word { Text = "CREAM", Category = category["Food & Drink"] },
                    new Models.Word { Text = "BREAD", Category = category["Food & Drink"] },

                };
                dbContext.Words.AddRange(defaultWords);
                dbContext.SaveChanges();
            }
        }
    }
}
