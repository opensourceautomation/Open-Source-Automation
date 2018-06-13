namespace OSAE
{

    using System;
    
    /// <summary>
    /// Creates a static instance of Custom User Controls and other required global program stuff
    /// </summary>
    public class GlobalUserControls
    {
        public GlobalUserControls() { } //Constructor
	
        public static OSAE.UserControlServices OSAEUserControls = new UserControlServices();

    }
}
