using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Globalization;
using System.Text.Encodings.Web;
using iSarv.Data.CultureModels;

namespace RIMS.Data
{
    public static class Utilities
    {
        private static readonly string? WebRootPath = new HttpContextAccessor().HttpContext?.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .WebRootPath;

        public static async Task SaveToFileAsync(string destination, IFormFile file)
        {
            var path = Path.Combine(WebRootPath!, destination);
            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public static void DeleteFile(string destination)
        {
            var path = Path.Combine(WebRootPath!, destination);
            if (File.Exists(path))
                File.Delete(path);
        }

        public static string ToHtmlString(this IHtmlContent content)
        {
            if (content is HtmlString htmlString)
            {
                return htmlString.Value!;
            }

            using var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static string ToPersianDate(this DateTime d)
        {
            try
            {
                return d.ToString("dd MMMM yyyy", new CultureInfo("fa-IR"));
            }
            catch
            {
                return "01 فروردین 0000";
            }
        }

        public static string ToShortPersianDate(this DateTime d)
        {
            try
            {
                return d.ToString("yyyy/MM/dd", new CultureInfo("fa-IR"));
            }
            catch
            {
                return "0000/01/01";
            }
        }

        public static string ToPersianDateTime(this DateTime d)
        {
            try
            {
                return d.ToString("dd MMMM yyyy | ساعت HH:mm", new CultureInfo("fa-IR"));
            }
            catch
            {
                return "01 فروردین 0000";
            }
        }

        public static string ToShortPersianDateTime(this DateTime d)
        {
            try
            {
                return d.ToString("yyyy/MM/dd | HH:mm", new CultureInfo("fa-IR"));
            }
            catch
            {
                return "0000/01/01";
            }
        }

        public static string ToDescription(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val
                .GetType()
                .GetField(val.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false)!;
            return attributes.Length > 0 ? attributes[0].Description : val.ToString();
        }

        public static SelectList ToSelectList(this Enum e, CultureLocalizer localizer)
        {
            return new SelectList(e.GetType().GetEnumValues().Cast<Enum>().Select(item => new SelectListItem
            {
                Text = localizer.Text(item.ToDescription()),
                Value = item.GetHashCode().ToString()
            }).ToList(), "Value", "Text");
        }

        public static List<(string English, string Farsi)> ReadTextFileWithTranslations(string filePath)
        {
            var result = new List<(string English, string Farsi)>();
            try
            {
                var lines = System.IO.File.ReadAllLines(filePath).ToList();
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        result.Add((parts[0], parts[1]));
                    }
                }
            }
            catch (Exception e)
            {
                // Handle the exception, e.g., log it or display an error message
                Console.WriteLine("Error reading file: " + e.Message);
            }
            return result;
        }
    }

    public static class RegExpPatterns
    {
        public const string FarsiNameFamily = @"^[\u0600-\u0660\u0669-\u06F0\u06F9-\u06FF ‌ ¬\-,.']{3,1000}$";
        public const string EnNameFamily = @"^[a-zA-Z \-,.']{3,1000}$";

        public const string Instagram =
            @"(?:(?:http|https):\/\/)?(?:www.)?(?:instagram.com|instagr.am|instagr.com)\/(\w+)";

        public const string Telegram = @"(?:(?:http|https):\/\/)?(?:www.)?(?:telegram.com|t.me)\/(\w+)";
        public const string Facebook = @"(?:(?:http|https):\/\/)?(?:www.)?(?:facebook.com)\/(\w+)";
        public const string Twitter = @"(?:(?:http|https):\/\/)?(?:www.)?(?:twitter.com)\/(\w+)";
        public const string LinkedIn = @"(?:(?:http|https):\/\/)?(?:www.)?(?:linkedin.com)\/(\w+)";
        public const string FarsiPhrase = @"^[\u0600-\u0660\u0669-\u06F0\u06F9-\u06FF ‌ ¬\-,.']{3,1000}$";
        public const string EnglishPhrase = @"^[a-zA-Z \-,.']{3,1000}$";
    }

    public static class SuccessMessages
    {
        public const string OperationSuccessful = "The operation was completed successfully.";
    }

    public static class ErrorMessages
    {
        public const string RequiredErr = "{0} field is required.";
        public const string OperationFailed = "Error: The operation failed!";
        public const string FarsiNameFamilyErr = "لطفا حروف فارسی وارد کنید (حداقل ۳ حرف)";
        public const string EnNameFamilyErr = "Enter English characters (at-least 3)";
        public const string InstagramErr = "Please enter valid Instagram Url.";
        public const string TelegramErr = "Please enter valid Telegram Url.";
        public const string FacebookErr = "Please enter valid Facebook Url.";
        public const string TwitterErr = "Please enter valid Twitter Url.";
        public const string LinkedInErr = "Please enter valid LinkedIn Url.";
        public const string FarsiPhraseErr = "لطفا حروف فارسی وارد کنید (حداقل ۳ حرف)";
        public const string EnglishPhraseErr = "Enter English characters (at-least 3)";
    }
}
