namespace PeakMVP.ViewModels.Base {

    public interface IBottomBarTab : IVisualFiguring {

        bool IsBudgeVisible { get; }

        int BudgeCount { get; }

        string TabIcon { get; }
    }
}
