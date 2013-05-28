using Microsoft.Deployment.WindowsInstaller;
using OSAE;

namespace RegKeys
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CheckServerIp(Session session)
        {
            session.Log("Begin RegKeys CustomAction");

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
                    session.Log("RegKeys CustomAction - User exited");
                    return ActionResult.UserExit;
                }
            }
            else
            {
                return ActionResult.Success;
            }
        }

        [CustomAction]
        public static ActionResult MySQLLoginDetails(Session session)
        {
            session.Log("Begin RegKeys CustomAction");

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
                    session.Log("RegKeys CustomAction - User exited");
                    return ActionResult.UserExit;
                }
            }
            else
            {
                return ActionResult.Success;
            }
        }
    }
}
