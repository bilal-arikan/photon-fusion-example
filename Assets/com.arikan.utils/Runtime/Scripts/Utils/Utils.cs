using UnityEngine;

namespace Arikan
{
    public static class Utils
    {
        public static void OpenURL(string url)
        {
#if UNITY_WEBGL
            Application.ExternalEval($"window.open({url});");
#else
            Application.OpenURL(url);
#endif
        }

        // [Button]
        // public void OpenPage(WebPage page)
        // {
        //     if (page == WebPage.WebSite)
        //         Core.OpenURL(C.website_url.StringValue);
        //     else if (page == WebPage.GoogleMarket)
        //         Core.OpenURL(C.market_url.StringValue);
        //     else if (page == WebPage.AppStore)
        //         Core.OpenURL(C.applestore_url.StringValue);
        //     else if (page == WebPage.Facebook)
        //         Core.OpenURL(C.facebook_page.StringValue);
        //     else if (page == WebPage.Twitter)
        //         Core.OpenURL(C.twitter_page.StringValue);
        //     else if (page == WebPage.GooglePlus)
        //         Core.OpenURL(C.googleplus_page.StringValue);
        //     else if (page == WebPage.Instagram)
        //         Core.OpenURL(C.instagram_page.StringValue);
        //     else if (page == WebPage.GameJolt)
        //         Core.OpenURL(C.gamejolt_page.StringValue);
        //     else if (page == WebPage.TermsConditions)
        //         Core.OpenURL(C.termsconditions_page.StringValue);
        //     else if (page == WebPage.PrivacyPolicy)
        //         Core.OpenURL(C.privacypolicy_page.StringValue);
        // }
        // public void OpenMarketPage() => OpenPage(WebPage.GoogleMarket);
        // public void Open_termsconditions_page() => OpenPage(WebPage.TermsConditions);
        // public void Open_privacypolicy_page() => OpenPage(WebPage.PrivacyPolicy);

        // public enum WebPage
        // {
        //     WebSite,
        //     AppStore,
        //     GoogleMarket,
        //     Facebook,
        //     Twitter,
        //     GooglePlus,
        //     Instagram,
        //     GameJolt,
        //     TermsConditions,
        //     PrivacyPolicy
        // }
    }
}
