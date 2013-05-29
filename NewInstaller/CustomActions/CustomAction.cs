namespace OSAInstallCustomActions
{
    using Microsoft.Deployment.WindowsInstaller;
    using OSAE;

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CheckServerIp(Session session)
        {
            session.Log("Begin OSAInstallCustomActions CustomAction");

            ModifyRegistry registry = new ModifyRegistry();
            registry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";

            string db = registry.Read("DBCONNECTION");

            if (string.IsNullOrEmpty(db) || db == "default")
            {
                ServerDetails details = new ServerDetails(session);

                if (details.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    registry.Write("DBCONNECTION", details.ServerIP());
                    return ActionResult.Success;
                }
                else
                {
                    session.Log("OSAInstallCustomActions CustomAction - User exited");
                    return ActionResult.UserExit;
                }
            }
            else
            {
                return ActionResult.Success;
            }
        }

        [CustomAction]
        public static ActionResult DatabaseUpdate(Session session)
        {
            session.Log("Begin DatabaseUpdate CustomAction");

            DatabaseInstall databaseInstall = new DatabaseInstall("", "");


            if (databaseInstall.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {                
                return ActionResult.Success;
            }
            else
            {
                session.Log("OSAInstallCustomActions CustomAction - User exited");
                return ActionResult.UserExit;
            }
        }
    }
}
