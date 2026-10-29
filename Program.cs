var yol = "/home/qardin/.local/share/Steam/steamapps/common/Counter-Strike Global Offensive/game/csgo/pak01_dir.vpk";

// FileStream OkunanNesne = File.OpenRead(yol);
// var OlusanNesne = new BinaryReader(OkunanNesne);

// var olusanNesne2 = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"Olusan Nesne1 ={olusanNesne2}");
// var olusanNesne3 = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"Olusan Nesne2 ={olusanNesne3}");
// var olusanNesne4 = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"Olusan Nesne3 ={olusanNesne4}");
// var olusanNesne5 = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"Olusan Nesne4 ={olusanNesne5}");
// var olusanNesne6 = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"Olusan Nesne5 ={olusanNesne6}");
// var olusanNesne7 = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"Olusan Nesne6 ={olusanNesne7}");
// var olusanNesne8 = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"Olusan Nesne7 ={olusanNesne8}");
// // var olusanNesne9 = OlusanNesne.ReadUInt32();
// // System.Console.WriteLine($"Olusan Nesne8 ={olusanNesne9}");



// Okuyucu a = new Okuyucu();
// var olusanAtaNesne = a.BinaryOkuyucu(OlusanNesne);
// System.Console.WriteLine("Burasi Gorunuyor mu?" + olusanAtaNesne);

// var olusanAtaNesne1 = a.BinaryOkuyucu(OlusanNesne);
// // System.Console.WriteLine(olusanAtaNesne1);
// var olusanAtaNesne2 = a.BinaryOkuyucu(OlusanNesne);
// // System.Console.WriteLine(olusanAtaNesne2);
// // var olusanAtaNesne3 = a.BinaryOkuyucu(OlusanNesne);
// // System.Console.WriteLine(olusanAtaNesne3);

// var crc = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"{crc} Sen dosyanin verisisin.");
// var PreloadBytes = OlusanNesne.ReadUInt16();
// System.Console.WriteLine($"{PreloadBytes} sen onceden yuklenen byte larsin anladigim kadari ile");
// var ArchiveIndex = OlusanNesne.ReadUInt16();
// System.Console.WriteLine($"{ArchiveIndex} arsivlenen indexssin sen anladigim kadari ile.");
// var EntryOffset = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"{EntryOffset} SEni bilmiyrum sen girisin bir seyisinde bilmiyorum.");
// var EntryLength = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"{EntryLength}");
// var Terrminator = OlusanNesne.ReadUInt16();
// System.Console.WriteLine($"{Terrminator}");

// var olusanNesne10 = OlusanNesne.ReadUInt32();
// System.Console.WriteLine($"Olusan Nesne10 ={olusanNesne10}");

Okuyucu okuyucuNesnesi = new Okuyucu();
FileStream okunanNesne = File.OpenRead(yol);
var okunanNesneTutucu = new BinaryReader(okunanNesne);
okunanNesneTutucu.ReadBytes(28);
okuyucuNesnesi.BinaryOkuyucuDongulu(okunanNesneTutucu);
public class Okuyucu()
{
    public string BinaryOkuyucu(BinaryReader binary)
    {

        string kontrolMetini = "";
        while (true)
        {
            byte b = binary.ReadByte();
            if (b == 0)
                break;
            char c = (char)b;
            kontrolMetini += c;
        }
        return kontrolMetini;

    }
    public void BinaryOkuyucuDongulu(BinaryReader binary)
    {
        while (true)
        {
            ulong tutucu = 0;
            var raflar = BinaryOkuyucu(binary);
            if (raflar == "")
            {
                break;
            }
            while (true)
            {
                var kutular = BinaryOkuyucu(binary);
                if (kutular == "") { break; }

                while (true)
                {

                    var iceridekiKagitlar = BinaryOkuyucu(binary);
                    if (iceridekiKagitlar == "") { break; }
                    var crc = binary.ReadUInt32();

                    var preloadBytes = binary.ReadUInt16();

                    var archiveIndex = binary.ReadUInt16();

                    var entryOffset = binary.ReadUInt32();

                    var entryLength = binary.ReadUInt32();

                    var terminator = binary.ReadUInt16();
                    // System.Console.WriteLine($"{Raflar}/{kutular}/{IceridekiKagitlar}");
                    // System.Console.WriteLine($"Crc:{crc} ArchiveIndex:{ArchiveIndex} - PreloadBytes:{PreloadBytes} - EntryOffset:{EntryOffset} - EntryLength:{EntryLength} - Terrminator:{Terrminator} ");
                    // System.Console.WriteLine("---------------------------");
                    if (archiveIndex == 141)
                    {

                        tutucu += entryLength;
                    }
                    if (terminator != 0xffff)
                    {
                        System.Console.WriteLine("------------------KAYMA VAR --------------------------");
                        break;
                    }
                    // tutucu += EntryLength;
                }
            }
            System.Console.WriteLine($"su an tutucu nerede:{raflar}:  {tutucu / 1024.0 / 1024:f2}Mb");

        }
    }
}