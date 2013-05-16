namespace WCF
{
    using System;
    using System.ServiceModel;
    using OSAE;

    public interface IMessageCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnMessageReceived(OSAEWCFMessage message);

        //[OperationContract(IsOneWay = true)]
        //void EnablePlugin(Plugin plugin);
    }
}
