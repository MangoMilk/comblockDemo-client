using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    /// <summary>
    /// 资源数据对象，从持续化对象转化而来
    /// </summary>
    public class ResourceDataObject
    {
        public ResourceDataObject(string name, ResourceType type, string path)
        {
            this.Name = name;
            this.Type = type;
            this.Path = path;
        }
        public string Name { get; private set; }
        public ResourceType Type { get; private set; }
        public string Path { get; private set; }
    }
}
