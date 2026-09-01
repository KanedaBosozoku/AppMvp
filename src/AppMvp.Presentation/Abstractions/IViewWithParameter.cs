using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Abstractions
{
    public interface IViewWithParameter
    {
        void ReceiveParameter(object parameter);
    }
}