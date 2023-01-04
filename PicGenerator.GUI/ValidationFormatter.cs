#region

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Formatters.Abstractions;

#endregion

namespace PicGenerator.GUI;

public class ValidationFormatter : IValidationTextFormatter<string>
{
    /// <inheritdoc />
    public string Format(IValidationText validationText)
    {
        return validationText.Count != 0 ? $"{validationText[0]}, please~" : "";
    }
}
