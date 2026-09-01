using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.Commands
{
    public sealed class NavigateToFormCommand : IRequest
    {
        /// <summary>
        /// The WinForms Form type to open.
        /// Must be registered in DI.
        /// </summary>
        public Type FormType { get; }

        /// <summary>
        /// Optional parameter passed to the form.
        /// Only used if the form implements IFormWithParameter.
        /// </summary>
        public object? Parameter { get; }

        /// <summary>
        /// Whether the form should be shown modally.
        /// </summary>
        public bool Modal { get; }

        /// <summary>
        /// Optional callback invoked after the form closes (modal)
        /// or immediately after showing (non-modal).
        /// </summary>
        public Action<object?>? Callback { get; }

        public NavigateToFormCommand(
            Type formType,
            object? parameter = null,
            bool modal = false,
            Action<object?>? callback = null)
        {
            FormType = formType ?? throw new ArgumentNullException(nameof(formType));
            Parameter = parameter;
            Modal = modal;
            Callback = callback;
        }
    }
}
