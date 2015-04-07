using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    [Serializable]
    public enum AudioAutoplayModeEnum
    {
        Disabled = 0,
        OnFailure = 1,
        OnSuccess = 2,
        Always = 3
    }

    static class AudioAutoplayModeEnumExtensions
    {
        public static bool ShouldPlayOnFailure(this AudioAutoplayModeEnum v)
        {
            return (v == AudioAutoplayModeEnum.OnFailure || v == AudioAutoplayModeEnum.Always);
        }

        public static bool ShouldPlayOnSuccess(this AudioAutoplayModeEnum v)
        {
            return (v == AudioAutoplayModeEnum.OnSuccess || v == AudioAutoplayModeEnum.Always);
        }
    }
}
