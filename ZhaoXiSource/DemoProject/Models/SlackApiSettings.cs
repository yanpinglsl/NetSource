using ExtendLib.StartupExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoProject.Models
{
    public class SlackApiSettings : IValidatable
    {
        public string WebhookUrl { get; set; }
        public string DisplayName { get; set; }
        public bool ShouldNotify { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(WebhookUrl))
            {
                throw new Exception("SlackApiSettings.WebhookUrl must not be null or empty");
            }

            if (string.IsNullOrEmpty(DisplayName))
            {
                throw new Exception("SlackApiSettings.WebhookUrl must not be null or empty");
            }

            // 如果不是合法的Url，就会抛出异常
            var uri = new Uri(WebhookUrl);
        }
    }

    //还可以使用DataAnnotationsAttribute来实现上述验证
    //public class SlackApiSettings : IValidatable
    //{
    //    [Required, Url]
    //    public string WebhookUrl { get; set; }
    //    [Required]
    //    public string DisplayName { get; set; }
    //    public bool ShouldNotify { get; set; }

    //    public void Validate()
    //    {
    //        Validator.ValidateObject(this,
    //            new ValidationContext(this),
    //            validateAllProperties: true);
    //    }
    //}
}
