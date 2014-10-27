using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FolioHtmlGenerator
{
    public class HtmlContentGenerator
    {
        enum ParseStatus
        {
            General,
            FileDesc,
            FileTitle,
            FileCat
        }

        ParseStatus status = ParseStatus.General;

        ArrayList files = new ArrayList();

        HtmlFileRec currFile = null;

        public void Clear()
        {
            status = ParseStatus.General;
        }

        public void Add(string s)
        {
            if (s.Length == 0 || s.StartsWith("#"))
                return;

            switch(status)
            {
                case ParseStatus.General:
                    if (s.StartsWith("FILE|"))
                    {
                        string[] p = s.Split('|');
                        if (p.Length > 2)
                        {
                            HtmlFileRec fr = new HtmlFileRec();
                            currFile = fr;
                            files.Add(fr);
                            fr.fileName = p[2];
                            fr.fileTag = p[1];
                        }
                    }
                    else if (s.StartsWith("FILETITLE|"))
                    {
                        string[] p = s.Split('|');
                        if (p.Length > 1)
                        {
                            currFile = FindFile(p[1]);
                            status = ParseStatus.FileTitle;
                        }
                    }
                    else if (s.StartsWith("FILEDESC|"))
                    {
                        string[] p = s.Split('|');
                        if (p.Length > 1)
                        {
                            currFile = FindFile(p[1]);
                            status = ParseStatus.FileDesc;
                        }
                    }
                    else if (s.StartsWith("FILECAT|"))
                    {
                        string[] p = s.Split('|');
                        if (p.Length > 1)
                        {
                            currFile = FindFile(p[1]);
                            status = ParseStatus.FileCat;
                        }
                    }
                    break;
                case ParseStatus.FileDesc:
                    if (s == "FILEDESCEND")
                    {
                        status = ParseStatus.General;
                        break;
                    }
                    if (currFile != null)
                    {
                        currFile.fileDesc = currFile.fileDesc + " " + s;
                    }
                    break;
                case ParseStatus.FileTitle:
                    if (currFile != null)
                    {
                        currFile.fileTitle = s;
                    }
                    status = ParseStatus.General;
                    break;
                case ParseStatus.FileCat:
                    if (currFile != null)
                    {
                        currFile.category = s.Split('/');
                    }
                    status = ParseStatus.General;
                    break;
            }
        }

        public HtmlFileRec FindFile(string tag)
        {
            foreach (HtmlFileRec h in files)
            {
                if (h.fileTag == tag)
                    return h;
            }
            return null;
        }

        public HtmlFileRec FindFileTitle(string tag)
        {
            foreach (HtmlFileRec h in files)
            {
                if (h.fileTitle == tag)
                    return h;
            }
            return null;
        }

        public ArrayList SortedTitles()
        {
            ArrayList arr = new ArrayList();

            foreach (HtmlFileRec hf in files)
            {
                arr.Add(hf.fileTitle);
            }

            arr.Sort();

            return arr;
        }

        public string Generate()
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendLine("<html>");

            sb.AppendLine("<head>");
            sb.AppendLine("<title>");
            sb.AppendLine("</title>");
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("<!--");
            sb.AppendLine("function showStuff(id)");
            sb.AppendLine("{");
            sb.AppendLine("   document.getElementById('div1').style.display='none';");
            sb.AppendLine("   document.getElementById('div2').style.display='none';");
            sb.AppendLine("   document.getElementById(id).style.display = 'block';");
            sb.AppendLine("}");
            sb.AppendLine("-->");
            sb.AppendLine("</script>");
            sb.AppendLine("</head>");

            // body
            sb.AppendLine("<body>");

            // table of tabs
            sb.AppendLine("<table cellspacing=2>");
            sb.AppendLine("<tr>");
            sb.AppendLine("  <td style='background-color:blue;text-align:center;color:white;padding:8pt 12pt' onclick=\"showStuff('div1')\" >");
            sb.AppendLine("  <span style='cursor:arrow;'>BY CATEGORY</span><td>");
            sb.AppendLine("  <td style='background-color:blue;text-align:center;color:white;padding:8pt 12pt'  onclick=\"showStuff('div2')\">");
            sb.AppendLine("  <span style='cursor:arrow;'>BY ALPHABETIC ORDER</span><td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            // section of 
            sb.AppendLine("<div id='div1' style='display: block;'>");
            sb.AppendLine("</div>");

            // section of alphabetical order
            sb.AppendLine("<div id='div2' style='display: none;'>");
            ArrayList sorted = SortedTitles();
            char cprev = ' ';
            char c = ' ';
            int cnt = 0;
            foreach (string s in sorted)
            {
                c = s[0];
                if (cprev != c)
                {
                    if (cnt > 0)
                        sb.Append(" | ");
                    sb.AppendLine(string.Format("<a href='#' onclick=\"showStuff('divs-{0}')\">{0}</a>", c));
                    cnt++;
                }
                cprev = c;
            }
            cnt = 0;
            HtmlFileRec hfile = null;
            cprev = ' ';
            foreach (string s in sorted)
            {
                c = s[0];
                if (cprev != c)
                {
                    if (cnt > 0)
                    {
                        sb.AppendLine("</div>");
                    }
                    sb.AppendLine("<div id=\"divs-" + c + "\">");
                }
                hfile = FindFileTitle(s);
                if (hfile != null)
                {
                    sb.AppendLine(string.Format("<p><a href=\"{2}\">{0}</a><br>{1}</p>", s, hfile.fileDesc, hfile.fileName));
                }
                cnt++;
            }
            if (cnt > 0)
            {
                // ends section for last letter
                sb.AppendLine("</div>");
            }
            // ends section of alphabetical order
            sb.AppendLine("</div>");

            // end of body
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

    }

    public class HtmlFileRec
    {
        public string fileName = string.Empty;
        public string fileDesc = string.Empty;
        public string fileTag = string.Empty;
        public string fileTitle = string.Empty;
        public string[] category = null;
    }
}
