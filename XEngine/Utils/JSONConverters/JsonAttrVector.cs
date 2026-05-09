using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XEngine.Core.Utils.JSONConverters
{
    public class JsonVector3 : JsonConverterAttribute
    {
        public override JsonConverter? CreateConverter(Type typeToConvert)
        {
            return new OTKVector3Converter();
        }
    }

    public class JsonVector2 : JsonConverterAttribute
    {
        public override JsonConverter? CreateConverter(Type typeToConvert)
        {
            return new OTKVector2Converter();
        }
    }
}
