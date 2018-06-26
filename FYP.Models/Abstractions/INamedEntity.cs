using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models.Abstractions
{
    public interface INamedEntity
    {
        int Id { get; set; }
        string Name { get; set; }
    }
}
