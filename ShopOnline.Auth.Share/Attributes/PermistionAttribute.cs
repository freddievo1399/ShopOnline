using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Share.Attributes;

public class PermistionAttribute : Attribute
{
    public string Name { get; set; }

    public PermistionAttribute(string name)
    {
        this.Name = name;
    }
}
