using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Poppin.Extensions
{
    public class TestingObjectTypeSerializer : IBsonSerializer
    {
        public Type ValueType { get; } = typeof(string);

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Int32) return GetNumberValue(context);
            if (context.Reader.CurrentBsonType == BsonType.Null) return GetNullValue(context);

            return context.Reader.ReadString();
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value != null)
												{
                context.Writer.WriteString(value as string);
												}
            else
												{
                context.Writer.WriteString(string.Empty);
												}
        }

        private static object GetNumberValue(BsonDeserializationContext context)
        {
            var value = context.Reader.ReadInt32();
            return value.ToString();
        }

        private static object GetNullValue(BsonDeserializationContext context)
        {
            context.Reader.ReadNull();
            return null;
        }
    }
}
