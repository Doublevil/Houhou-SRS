using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Kanji.Database.Entities;
using Kanji.Database.Extensions;
using Kanji.Database.Helpers;

namespace Kanji.Database.EntityBuilders
{
    class RadicalBuilder : EntityBuilder<RadicalEntity>
    {
        public override RadicalEntity BuildEntity(NameValueCollection row, string prefix)
        {
            RadicalEntity radical = new RadicalEntity();
            radical.ID = row.ReadLong(GetField(prefix, SqlHelper.Field_Radical_Id)).Value;
            radical.Character = row.Get(GetField(prefix, SqlHelper.Field_Radical_Character));

            return radical;
        }
    }
}
