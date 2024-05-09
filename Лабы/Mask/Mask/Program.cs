using System.Net;
using System.Text.RegularExpressions;

enum IPAddressClass
{
    A,
    B,
    C,
    D,
    E
}

class NetworkInfo
{
    public IPAddress NetworkId { get; }
    public IPAddress HostId { get; }

    public NetworkInfo(IPAddress networkId, IPAddress hostId)
    {
        NetworkId = networkId;
        HostId = hostId;
    }
}

class NetworkAnalyzer
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Введите IP-адрес (в формате x.x.x.x): ");
        string? ipAddressString = Console.ReadLine();

        Console.WriteLine("Введите маску подсети (в формате x.x.x.x): ");
        string? subnetMaskString = Console.ReadLine();

        if (CheckIpAndMask(ipAddressString, subnetMaskString))
        {
            IPAddress ipAddress = IPAddress.Parse(ipAddressString);
            IPAddress subnetMask = IPAddress.Parse(subnetMaskString);

            NetworkInfo networkInfo = CalculateNetworkInfo(ipAddress, subnetMask);
            IPAddress broadcastAddress = CalculateBroadcastAddress(networkInfo.NetworkId, subnetMask);
            IPAddressClass ipAddressClass = GetIPAddressClass(ipAddress);

            Console.WriteLine($"Network ID: {networkInfo.NetworkId}");
            Console.WriteLine($"Host ID: {networkInfo.HostId}");
            Console.WriteLine($"Broadcast Address: {broadcastAddress}");
            Console.WriteLine($"IP Address Class: {ipAddressClass}");
        }
        else
        {
            Console.WriteLine("Неверный IP-адрес или маска подсети.");
        }
    }

    public static bool CheckIpAndMask(string ip, string mask)
    {
        Regex regex = new Regex(@"^\d{1,3}(\.\d{1,3}){3}$");

        if (!regex.IsMatch(ip) || !regex.IsMatch(mask))
        {
            return false;
        }

        return CheckNumbersInIpAndMask(ip, mask);
    }

    private static bool CheckNumbersInIpAndMask(string ip, string mask)
    {
        var numsIp = ip.Split(".");
        var numsMask = mask.Split(".");

        if (!CheckNumbers(numsIp, numsMask))
        {
            return false;
        }

        if (!CheckMask(numsMask))
        {
            return false;
        }

        return true;
    }

    private static bool CheckNumbers(string[] numsIp, string[] numsMask)
    {
        for (int i = 0; i < numsIp.Length; i++)
        {
            if (!int.TryParse(numsIp[i], out int numIp) || !int.TryParse(numsMask[i], out int numMask))
            {
                return false;
            }

            if (numIp < 0 || numIp > 255 || numMask < 0 || numMask > 255)
            {
                return false;
            }
        }

        return true;
    }

    private static bool CheckMask(string[] numsMask)
    {
        string binMask = string.Join("", numsMask);
        if (binMask.Contains("01"))
        {
            return false;
        }

        return true;
    }

    static NetworkInfo CalculateNetworkInfo(IPAddress ipAddress, IPAddress subnetMask)
    {
        byte[] ipAddressBytes = ipAddress.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        byte[] networkIdBytes = new byte[4];
        byte[] hostIdBytes = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            networkIdBytes[i] = (byte)(ipAddressBytes[i] & subnetMaskBytes[i]);
            hostIdBytes[i] = (byte)(ipAddressBytes[i] & ~subnetMaskBytes[i]);
        }

        IPAddress networkId = new IPAddress(networkIdBytes);
        IPAddress hostId = new IPAddress(hostIdBytes);

        return new NetworkInfo(networkId, hostId);
    }

    //Вычислить широковещательный адрес
    static IPAddress CalculateBroadcastAddress(IPAddress networkId, IPAddress subnetMask)
    {
        byte[] networkIdBytes = networkId.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        byte[] broadcastAddressBytes = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            broadcastAddressBytes[i] = (byte)(networkIdBytes[i] | ~subnetMaskBytes[i]);
        }

        return new IPAddress(broadcastAddressBytes);
    }

    static IPAddressClass GetIPAddressClass(IPAddress ipAddress)
    {
        byte[] ipAddressBytes = ipAddress.GetAddressBytes();
        byte firstByte = ipAddressBytes[0];

        if ((firstByte & 0x80) == 0)
        {
            return IPAddressClass.A;
        }
        else if ((firstByte & 0xC0) == 0x80)
        {
            return IPAddressClass.B;
        }
        else if ((firstByte & 0xE0) == 0xC0)
        {
            return IPAddressClass.C;
        }
        else if ((firstByte & 0xF0) == 0xE0)
        {
            return IPAddressClass.D;
        }
        else
        {
            return IPAddressClass.E;
        }
    }
}