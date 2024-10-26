using System.Net;

using UnityEngine;

namespace AIAssistant
{
    public static class OpenAIUtil
    {
        static string CreateChatRequestBody(string prompt)
        {
            var msg = new OpenAI.RequestMessage();
            msg.role = "user";
            msg.content = prompt;

            var req = new OpenAI.Request();
            req.model = "gpt-3.5-turbo";
            req.messages = new[] { msg };

            return JsonUtility.ToJson(req);
        }

        public static HttpWebRequest BuildRequest(
            string prompt,
            out byte[] data)
        {
            var settings = AIAssistantSettings.instance;
         
            data = new System.Text.UTF8Encoding().GetBytes(
                CreateChatRequestBody(
                    prompt));
            
            var request = (HttpWebRequest)WebRequest.Create(OpenAI.Api.Url);
            
            request.Method = "POST";
            
            WebHeaderCollection headers = new WebHeaderCollection();
            
            //headers.Add("Content-Type", "application/json");
            headers.Add("Authorization", "Bearer " + settings.apiKey);
            
            request.Headers = headers;

            request.ContentType = "application/json";
            
            request.ContentLength = data.Length;
            
            //request.Timeout = settings.timeout;

            return request;
        }
    }
}