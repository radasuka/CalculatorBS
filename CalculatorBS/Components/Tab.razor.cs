﻿using CalculatorBS.Interfaces;
using Microsoft.AspNetCore.Components;

namespace CalculatorBS.Components
{
    public partial class Tab : ITab
    {
        [CascadingParameter]
        public TabSet ContainerTabSet { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private string TitleCssClass
            => ContainerTabSet.ActiveTab == this ? "active" : null;

        protected override void OnInitialized()
            => ContainerTabSet.AddTab(this);

        private void ActivateTab()
            => ContainerTabSet.SetActiveTab(this);
    }
}