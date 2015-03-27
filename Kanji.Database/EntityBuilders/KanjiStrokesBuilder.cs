using Kanji.Database.Entities;
using Kanji.Database.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Kanji.Database.Extensions;

namespace Kanji.Database.EntityBuilders
{
    class KanjiStrokesBuilder : EntityBuilder<KanjiStrokes>
    {
        public override KanjiStrokes BuildEntity(NameValueCollection row, string prefix)
        {
            KanjiStrokes strokes = new KanjiStrokes();
            strokes.ID = row.ReadLong(GetField(prefix, SqlHelper.Field_KanjiStrokes_Id)).Value;
            strokes.FramesSvg = row.ReadBinary(GetField(prefix, SqlHelper.Field_KanjiStrokes_FramesSvg));

            return strokes;
        }
    }
}
