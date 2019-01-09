using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackTwo
{
    class Bounds
    {
        private String[] Tags;

        //采集时间
        private DateTime capTimer;

        public Bounds(int sum) {
            Tags = new String[sum];
        }

        public DateTime CapTimer
        { get => capTimer; set => capTimer = value; }

        //正确添加
        public void AddTags(String[] tags) {
            Tags = (String[])tags.Clone();
        }

        public String[] getTags()
        {
            return (String[])Tags.Clone();
        }

    }
}
