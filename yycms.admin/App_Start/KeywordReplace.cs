using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using yycms.entity;

namespace yycms.admin
{
    public class KeywordReplace
    {
        /// <summary>
        /// 替换内容
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="content"></param>
        /// <param name="ApplyType">0，全部，1，新闻，2，产品</param>
        /// <param name="TypeIDs">类目ID</param>
        /// <returns></returns>
        public static String Excute(DBConnection DB, String content,int ApplyType,String TypeIDs)
        {
            var doc = NSoup.NSoupClient.Parse(content);

            String content_Html = doc.OuterHtml();

            var keywords = DB.yy_Keywords.Where(x => x.IsShow == 1 && (x.ApplyType == ApplyType || x.ApplyType == 0)).ToList();

            foreach (var v in keywords)
            {
                #region 类目匹配
                if (v.ApplyType != 0 && !String.IsNullOrEmpty(v.TypeIDs))
                {
                    var IDs = v.TypeIDs.Split(new String[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    var ItemIDs = TypeIDs.Split(new String[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    var IncludType = false;

                    foreach (var id in ItemIDs)
                    {
                        if (IDs.Contains(id)) { IncludType = true; break; }
                    }

                    if (!IncludType) { continue; }
                }
                #endregion

                var r = new Regex(v.Title);

                if (v.ReplaceCount > 0)
                {
                    content_Html = r.Replace(content_Html, v.ReplaceContent, v.ReplaceCount);
                }
                else
                {
                    content_Html = r.Replace(content_Html, v.ReplaceContent);
                }

                Thread.Sleep(1);
            }

            var resultDoc = NSoup.NSoupClient.Parse(content_Html);

            String result = resultDoc.OuterHtml();

            return result;
        }
    }
}
