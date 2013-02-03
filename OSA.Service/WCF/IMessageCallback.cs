namespace WCF
{
    using System;
    using System.ServiceModel;

    interface IMessageCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnMessageReceived(string msgType, string message, string from, DateTime timestamp);
    }
}
