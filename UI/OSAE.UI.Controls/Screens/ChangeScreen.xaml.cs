using System.Data;
using System.Windows.Controls;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for ChangeScreen control
    /// </summary>
    public partial class ChangeScreen : UserControl
    {
        /// <summary>
        /// OSAE API to interact with OSA DB
        /// </summary>
        private OSAE osae = new OSAE("OSAE.UI.Controls");

        /// <summary>
        /// Default constructor
        /// </summary>
        public ChangeScreen()
        {
            InitializeComponent();
            LoadScreens();
        }

        /// <summary>
        /// Load the screens into the list view from the DB
        /// </summary>
        private void LoadScreens()
        {
            DataSet dataSet = this.osae.RunSQL("SELECT object_name,property_value FROM osae_v_object_property WHERE base_type='SCREEN' AND property_name='Background Image' ORDER BY object_name");
            screensListView.ItemsSource = dataSet.Tables[0].DefaultView;
        }
    }
}
