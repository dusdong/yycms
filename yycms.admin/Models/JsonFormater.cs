using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Web;

namespace yycms.admin
{
    public class JsonFormater : IMessageFormatter
    {
        public bool CanRead(Message message)
        {
            return message.BodyStream != null && message.BodyStream.Length > 0;
        }

        [ThreadStatic]
        private static byte[] mBuffer;

        public object Read(Message message)
        {
            if (mBuffer == null)
                mBuffer = new byte[14096];
            int count = (int)message.BodyStream.Length;
            message.BodyStream.Read(mBuffer, 0, count);
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(mBuffer));

        }

        [ThreadStatic]
        private static System.IO.MemoryStream mStream;

        public void Write(Message message, object obj)
        {
            if (mStream == null)
                mStream = new MemoryStream(14096);
            mStream.Position = 0;
            mStream.SetLength(14095);
            string value = JsonConvert.SerializeObject(obj);
            int count = Encoding.UTF8.GetBytes(value, 0, value.Length, mStream.GetBuffer(), 0);
            mStream.SetLength(count);
            message.BodyStream = mStream;
        }

        public object Clone()
        {
            return this;
        }
    }
}