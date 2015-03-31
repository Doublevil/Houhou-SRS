using Kanji.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.Models
{
    class VocabVariant
    {
        public ExtendedVocab Parent { get; set; }

        public VocabEntity Variant { get; set; }

        public VocabVariant(ExtendedVocab parent, VocabEntity variant)
        {
            Parent = parent;
            Variant = variant;
        }
    }
}
