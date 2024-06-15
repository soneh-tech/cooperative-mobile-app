namespace CooperativeAppMobile.MAUI.Views;

public partial class ViewMember : ContentPage
{
	public ViewMember()
	{
		InitializeComponent();
	}

	private void OnFilterChanged()
	{
		if(dataGrid.View != null) {
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