namespace CooperativeAppMobile.MAUI.Views;

public partial class ViewMemberWithdrawal : ContentPage
{
	public ViewMemberWithdrawal()
	{
		InitializeComponent();
	}
    private void OnFilterChanged()
    {
        if (dataGrid.View != null)
        {
            dataGrid.View.Filter = FilterRecords;
            dataGrid.View.RefreshFilter();
        }
    }

    private void Accountype_Selected(object sender, EventArgs e)
    {
        dataGrid.View.Filter = FilterRecords;
        dataGrid.View.RefreshFilter();

    }
    private bool FilterRecords(object record)
    {
        return true;
    }
}