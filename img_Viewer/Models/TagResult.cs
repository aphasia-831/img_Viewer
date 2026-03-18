using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace img_Viewer.Models
{
    public class TagItem
    {
        public string tag { get; set; }
        public float score { get; set; }
    }
    public class TagResult
        {
        public string file_path { get; set; }

        public List<TagItem> general { get; set; }
        public List<TagItem> character { get; set; }
        public List<TagItem> copyright { get; set; }
        public List<TagItem> rating { get; set; }
    }


    
}
