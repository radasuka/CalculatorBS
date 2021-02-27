using CalculatorBS.Interfaces;
using Microsoft.AspNetCore.Components;

namespace CalculatorBS.Components
{
    public partial class TabSet
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public ITab ActiveTab { get; private set; }

        public void AddTab(ITab tab)
        {
            if (ActiveTab == null)
                SetActiveTab(tab);
        }

        public void SetActiveTab(ITab tab)
        {
            if (ActiveTab != tab)
            {
                ActiveTab = tab;
                StateHasChanged();
            }
        }
    }
}