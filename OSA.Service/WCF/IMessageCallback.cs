namespace WCF
{
    using System;
    using System.ServiceModel;
    using OSAE;

    interface IMessageCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnMessageReceived(string msgType, string message, string from, DateTime timestamp);

        [OperationContract(IsOneWay = true)]
        void EnablePlugin(Plugin plugin);
    }
}
