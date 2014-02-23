using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Interface.Models;

namespace Kanji.Interface.Extensions
{
    static class StartPageEnumExtensions
    {
        public static NavigationPageEnum ToNavigationPage(this StartPageEnum v)
        {
            switch (v)
            {
                case StartPageEnum.Srs:
                    return NavigationPageEnum.Srs;
                case StartPageEnum.Kanji:
                    return NavigationPageEnum.Kanji;
                case StartPageEnum.Vocab:
                    return NavigationPageEnum.Vocab;
                default:
                    return NavigationPageEnum.Home;
            }
        }
    }
}
