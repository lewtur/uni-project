using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models.Abstractions
{
    public interface ITrend
    {
        DateTime GetDate();
        int GetScore();
    }
}
