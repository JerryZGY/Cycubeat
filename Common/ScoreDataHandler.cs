using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Cycubeat
{
    public class ScoreDataHandler
    {
        public ScoreDataHandler(string ID, string Name, string Depart, int Score)
        {
            var Times = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            JObject scoreData = new JObject();

            try
            {
                scoreData = JObject.Parse(File.ReadAllText(@"ScoreData.json"));
            }
            catch (FileNotFoundException) { }

            var studentData = scoreData[ID];

            if (studentData != null)
            {
                studentData["Score"] = Score;
                studentData["Times"] = Times;
            }
            else
            {
                scoreData.Add(ID, new JObject(
                    new JProperty("Name", Name),
                    new JProperty("Depart", Depart),
                    new JProperty("Score", Score),
                    new JProperty("Times", Times)
                ));
            }

            File.WriteAllText("ScoreData.json", scoreData.ToString());
        }
    }
}
