using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;
using System.Globalization;

/**
 * 1. [1 punkt] Wczytaj dane z pliku favorite-tweets.jsonl Po wczytaniu poszczególne tweety powinny znajdować się w osobnych obiektach a obiekty na liście.
 * Możesz dowolnie modyfikować strukturę pliku, ale nie modyfikuj danych poszczególnych tweetów. 
2. [1 punkt] Napisz funkcję, który pozwoli na przekonwertowanie wczytanych w punkcie 1 danych do formatu XML. 
Funkcja ma pozwalać zarówno na zapis do pliku w formacie XML danych o tweetach jak i wczytanie tych danych z pliku.    
3. [1 punkt] Napisz funkcję sortującą tweety po nazwie użytkowników oraz funkcję sortującą użytkowników po dacie utworzenie tweetu.
4. [1 punkt] Wypisz najnowszy i najstarszy tweet znaleziony względem daty jego utworzenia.
*/

public class Tweet
    {
        public string? text {get; set;}
        public string? userName {get; set;}
        public string? linkToTweet {get; set;}
        public string? firstLinkUrl {get; set;}
        public string? createdAt {get; set;}
        public string? tweetEmbedCode {get; set;}
    }

public class JsonReader {
    
    public static void Main(string[] args) {
        
        List<Tweet> tweets = new List<Tweet>();
        using (StreamReader reader = new StreamReader("favorite-tweets.jsonl"))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                JsonDocument document = JsonDocument.Parse(line);
                JsonElement root = document.RootElement;
                Tweet tweet = new Tweet
                {
                    text = root.GetProperty("Text").GetString(),
                    userName = root.GetProperty("UserName").GetString(),
                    linkToTweet = root.GetProperty("LinkToTweet").GetString(),
                    firstLinkUrl = root.GetProperty("FirstLinkUrl").GetString(),
                    createdAt = root.GetProperty("CreatedAt").GetString(),
                    tweetEmbedCode = root.GetProperty("TweetEmbedCode").GetString()
                };
                tweets.Add(tweet);
            }
        }
        

        Dictionary<string, List<string>> userTweets = new Dictionary<string, List<string>>();
    }

    public static List<Tweet> sortViaName(List<Tweet> tweets) {
        int n = tweets.Count;
        for (int i = 1; i < n; ++i) {
            Tweet key = tweets[i];
            int j = i - 1;

            while (j >= 0 && String.Compare(tweets[j].userName, key.userName) > 0) {
                tweets[j + 1] = tweets[j];
                j = j - 1;
            }
            tweets[j + 1] = key;
        }
        return tweets;
    }
    

    public static List<Tweet> sortViaDate(List<Tweet> tweets) {
        
        tweets.Sort((tweet1, tweet2) => DateTime.ParseExact(tweet1.createdAt, "MMMM dd, yyyy 'at' hh:mmtt", CultureInfo.InvariantCulture)
            .CompareTo(DateTime.ParseExact(tweet2.createdAt, "MMMM dd, yyyy 'at' hh:mmtt", CultureInfo.InvariantCulture)));
        return tweets;
    }
    
    

    public static List<Tweet> xmlSerialize(List<Tweet> tweets) {
        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(tweets[0].GetType());
        using (StreamWriter writer = File.CreateText("tweets.xml"))
        {
            foreach(var t in tweets) {
                x.Serialize(writer, t);
            }
        }

        XmlSerializer serializer = new XmlSerializer(typeof(List<Tweet>));
        List<Tweet> tweets2;
        using (Stream reader = new FileStream("tweets.xml", FileMode.Open))
        {
            tweets2 = (List<Tweet>)serializer.Deserialize(reader);
        }
        return tweets2;
    }
}
