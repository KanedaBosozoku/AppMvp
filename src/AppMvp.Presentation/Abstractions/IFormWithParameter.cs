using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.Abstractions
{
    public interface IFormWithParameter
    {
        void ReceiveParameter(object parameter);
    }
}
