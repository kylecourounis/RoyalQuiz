# RoyalQuiz
RoyalQuiz is a Discord Quiz bot created by [Incredible](https://github.com/Incr3dible) and Kyle for use in a Discord where things like Supercell Private Servers and the reverse engineering of Supercell's mobile games is discussed. The codestyle is more how [Incredible](https://github.com/Incr3dible) writes C# than the way I do.

Since the bot is no longer in use in our Discord, I figured I would open-source it so that others can learn from it.

## Why Did We Create This?
We grew tired of people who had very primitive knowledge of the inner workings of Supercell games cluttering up general chat in our Discord. So we created an entrance channel where you could attempt a quiz that would automatically give you the Member role so you could access the other text channels. By the time people were able to pass, they could hold a proper conversation in the main channels.

## How It Works
When a user joined the server, they would be in to the entrance channel. When the user would type `!quiz` in the chat, the bot would send them a DM where the user will answer the questions by reacting with the 1,2,3,4 Emojis. If they fail the quiz, they must wait 24 hours before trying again. If they pass, they are automatically granted the Member role and can access other the channels.

## License
Since there is nothing truly proprietary about this, I have licensed this project under the MIT License. You are free to use any of the code in any way you wish.
