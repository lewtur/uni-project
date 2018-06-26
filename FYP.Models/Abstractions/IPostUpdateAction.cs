using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FYP.Models.Abstractions
{
    public interface IPostUpdateAction
    {
        string GetName();
        Task Act(int artistId);
    }
}
