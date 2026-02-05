using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace SkyWatch.Controllers;

public class CultureController : Controller
{
    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        //Adds cookie for language choose
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1), 
                IsEssential = true 
            }
        );

        return LocalRedirect(returnUrl);
    }
}