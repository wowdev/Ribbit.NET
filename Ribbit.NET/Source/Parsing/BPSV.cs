using System.Collections.Generic;

namespace Ribbit.Parsing
{
    public class BPSV
    {
        public int sequenceNumber;
        public string[] headers;
        public List<string[]> data = new List<string[]>();

        public BPSV(string content)
        {
            var splitContent = content.Split('\n');
            for (var i = 0; i < splitContent.Length; i++)
            {
                var line = splitContent[i];

                if (string.IsNullOrWhiteSpace(line)) continue;

                // Seqn
                if (line.StartsWith("## seqn"))
                {
                    sequenceNumber = int.Parse(line.Replace("## seqn = ", "").Trim());
                    continue;
                }

                var splitLine = line.Split('|');

                // Headers
                if (i == 0)
                {
                    var headerList = new List<string>();
                    foreach(var header in splitLine)
                    {
                        // TODO: Proper type checking/mapping/whatever, ignore that for now
                        headerList.Add(header.Remove(header.IndexOf('!')));
                    }
                    headers = headerList.ToArray();
                }
                else
                {
                    // Data
                    data.Add(splitLine);
                }
            }
        }
    }
}
