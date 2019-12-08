using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;

namespace MATNodes
{
    public static class MNTools
    {
        static IPAddress mLocalAddress;
        static IPAddress mGlobalAddress;
        static List<IPAddress> mAddresses = null;

        static public string GetResponse(WebRequest request)
        {
            string response = "";

            try
            {
                WebResponse webResponse = request.GetResponse();
                Stream stream = webResponse.GetResponseStream();

                byte[] bytes = new byte[2048];

                for (; ; )
                {
                    int count = stream.Read(bytes, 0, bytes.Length);
                    if (count > 0) response += Encoding.ASCII.GetString(bytes, 0, count);
                    else break;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return response;
        }

        static public bool IsValidAddress(IPAddress address)
        {
            if (address.AddressFamily != AddressFamily.InterNetwork) return false;
            if (address.Equals(IPAddress.Loopback)) return false;
            if (address.Equals(IPAddress.None)) return false;
            if (address.Equals(IPAddress.Any)) return false;
            if (address.ToString().StartsWith("169.")) return false;
            return true;
        }

#if !UNITY_WEBPLAYER && !UNITY_WINRT
        static List<NetworkInterface> mInterfaces = null;

        /// <summary>
        /// Return the list of operational network interfaces.
        /// </summary>

        static public List<NetworkInterface> networkInterfaces
        {
            get
            {
                if (mInterfaces == null)
                {
                    mInterfaces = new List<NetworkInterface>();
                    NetworkInterface[] list = NetworkInterface.GetAllNetworkInterfaces();

                    foreach (NetworkInterface ni in list)
                    {
                        if (ni.Supports(NetworkInterfaceComponent.IPv4) &&
                            (ni.OperationalStatus == OperationalStatus.Up ||
                            ni.OperationalStatus == OperationalStatus.Unknown))
                            mInterfaces.Add(ni);
                    }
                }
                return mInterfaces;
            }
        }
#endif

        static public List<IPAddress> localAddresses
        {
            get
            {
                if (mAddresses == null)
                {
                    mAddresses = new List<IPAddress>();
#if !UNITY_WEBPLAYER && !UNITY_WINRT
                    try
                    {
                        List<NetworkInterface> list = networkInterfaces;

                        for (int i = list.Count; i > 0;)
                        {
                            NetworkInterface ni = list[--i];
                            if (ni == null) continue;

                            IPInterfaceProperties props = ni.GetIPProperties();
                            if (props == null) continue;

                            //if (ni.NetworkInterfaceType == NetworkInterfaceType.Unknown) continue;

                            UnicastIPAddressInformationCollection uniAddresses = props.UnicastAddresses;
                            if (uniAddresses == null) continue;

                            foreach (UnicastIPAddressInformation uni in uniAddresses)
                            {
                                // BUG: Accessing 'uni.Address' crashes when executed in a stand-alone build in Unity,
                                // yet works perfectly fine when launched from within the Unity Editor or any other platform.
                                // The stack trace reads:
                                //
                                // Argument cannot be null. Parameter name: src
                                // at (wrapper managed-to-native) System.Runtime.InteropServices.Marshal:PtrToStructure (intptr,System.Type)
                                // at System.Net.NetworkInformation.Win32_SOCKET_ADDRESS.GetIPAddress () [0x00000] in <filename unknown>:0 
                                // at System.Net.NetworkInformation.Win32UnicastIPAddressInformation.get_Address () [0x00000] in <filename unknown>:0

                                if (IsValidAddress(uni.Address))
                                    mAddresses.Add(uni.Address);
                            }
                        }
                    }
                    catch (Exception) { }
#endif
#if !UNITY_IPHONE && !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX && !UNITY_WINRT
                    // Fallback method. This won't work on the iPhone, but seems to be needed on some platforms
                    // where GetIPProperties either fails, or Unicast.Addres access throws an exception.
                    string hn = Dns.GetHostName();

                    if (!string.IsNullOrEmpty(hn))
                    {
                        IPAddress[] ips = Dns.GetHostAddresses(hn);

                        if (ips != null)
                        {
                            foreach (IPAddress ad in ips)
                            {
                                if (IsValidAddress(ad) && !mAddresses.Contains(ad))
                                    mAddresses.Add(ad);
                            }
                        }
                    }
#endif
                    // If everything else fails, simply use the loopback address
                    if (mAddresses.Count == 0) mAddresses.Add(IPAddress.Loopback);
                }
                return mAddresses;
            }
        }

        static public IPAddress localAddress
        {
            get
            {
                if (mLocalAddress == null)
                {
                    mLocalAddress = IPAddress.Loopback;
                    List<IPAddress> list = localAddresses;

                    if (list.Count > 0)
                    {
                        mLocalAddress = mAddresses[0];

                        for (int i = 0; i < mAddresses.Count; ++i)
                        {
                            IPAddress addr = mAddresses[i];
                            string str = addr.ToString();

                            // Hamachi IPs begin with 25
                            if (str.StartsWith("25.")) continue;

                            // This is a valid address
                            mLocalAddress = addr;
                            break;
                        }
                    }
                }
                return mLocalAddress;
            }
            set
            {
                mLocalAddress = value;

                if (value != null)
                {
                    List<IPAddress> list = localAddresses;
                    for (int i = 0; i < list.Count; ++i)
                        if (list[i] == value)
                            return;
                }
            }
        }

        static public IPAddress globalAddress
        {
            get
            {
                if (mGlobalAddress == null)
                {
                    try
                    {
                        mGlobalAddress = GetGlobalAddress("http://api.ipify.org");
                    }
                    catch (Exception) { }
                }
                return mGlobalAddress;
            }
            set
            {
                mGlobalAddress = value;
            }
        }
        static public IPAddress GetGlobalAddress(string sourceUrl)
        {
            string globalIp = new WebClient().DownloadString(sourceUrl);
            return IPAddress.Parse(globalIp);
        }
    }
}
