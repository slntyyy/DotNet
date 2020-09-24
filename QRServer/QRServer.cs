using Dicom.Network;

namespace QRServer
{
    public static class QRServer
    {
        private static IDicomServer _server;
        public static string AETitle { get; set; }
        public static void Start(int port, string aet)
        {
            AETitle = aet;
            _server = DicomServer.Create<QRService>(port);
        }
        public static void Stop()
        {
            _server.Dispose();
        }
    }
}
