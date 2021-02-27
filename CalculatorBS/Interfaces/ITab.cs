using Microsoft.AspNetCore.Components;

namespace CalculatorBS.Interfaces
{
    public interface ITab
    {
        RenderFragment ChildContent { get; }
    }
}