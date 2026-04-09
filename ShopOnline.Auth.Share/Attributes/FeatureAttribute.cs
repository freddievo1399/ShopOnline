using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Share.Attributes;

public class FeatureAttribute : Attribute
{
    public string GroupName { get; set; }
    public string Name { get; set; }

    public FeatureAttribute(string groupName, string name)
    {
        this.GroupName = groupName;
        this.Name = name;
    }
}
