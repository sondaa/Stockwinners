using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSite.Interfaces
{
    interface IEmailable
    {
        void Email(bool isPreview = false);
    }
}
