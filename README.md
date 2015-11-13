# Simple Bot
Custom chat bot for Twitch TV

This is an open-source project that will benefit anyone who wants to have a foundation of making their own Twitch bot. This is primarly written in C#/SQL Server using an Azure database from Microsoft. Currently, this is not end-user friendly because I am concentrating on the logic of the bot first.

This is a console application that requires an Azure database login. Please change the credentials from what is currently implemented to your database's credentials. This can be done through the `app.config` file. The password is manually entered every time the application runs to prevent compromise of the user's credentials. The `oauth` and `clientID` are being stored into the database as well.

After entering the password, the program will look for a local Spotify client. If there is one available, it will attempt to grab the song that is currently playing and post it onto the chat. This chat bot can control some of Spotify's music player like `!spotifyplay`, `!spotifypause`, `!spotifyskip`, and `!spotifyprev`.

You can also request songs and check out what songs are requested using `!requestsong [artist] - [song]` and `!requestlist`.

The bot itself is an account on Twitch that I have made in order to have a custom bot name.

This is an ongoing document. Please wait for more info...
