using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.union.wechat
{
    public class Group : SDK 
    {
        public Group(String _Accecc_Token) 
        {
            base.Access_Token = _Accecc_Token;
        }
    }
}
