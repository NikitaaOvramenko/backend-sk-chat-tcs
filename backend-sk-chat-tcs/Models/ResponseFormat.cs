using System.ComponentModel;
using System.Text.Json.Serialization;

namespace backend_sk_chat_tcs.Models
{


    public class ResponseFormat
    {

        [Description("Return Message for user")]
        public string Message { get; set; }


        [Description("Return URL of Edited Image for User")]
        public string Url { get; set; }


       
            
    }


    

    

}
