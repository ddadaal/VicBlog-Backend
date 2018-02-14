using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.Models
{
    public interface ISingleKey<T> where T: IEquatable<T>
    {
        T Id { get; }
    }
}
